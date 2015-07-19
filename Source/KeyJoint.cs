using AnimationLib.Commands;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib
{
	/// <summary>
	/// Every keybone is attached to a keyjoint, which holds all the animation data for the particular animation
	/// </summary>
	public class KeyJoint
	{
		#region Properties

		/// <summary>
		/// Teh list of key elements for this dude
		/// </summary>
		private List<KeyElement> Elements { get; set; }

		/// <summary>
		/// this dude's name
		/// </summary>
		public string Name { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyJoint(string name)
		{
			Elements = new List<KeyElement>();
			Name = name;
		}

		/// <summary>
		/// Get a key element at a certain tim from this dude
		/// </summary>
		/// <param name="time">The time of the key element to get</param>
		/// <param name="keyElement">The key element to output the stuff to</param>
		public bool GetKeyElement(int time, KeyElement keyElement)
		{
			//if no elements, fuckin FALSE
			if (Elements.Count <= 0)
			{
				return false;
			}

			//if only one element, return it
			if (Elements.Count == 1)
			{
				keyElement.Copy(Elements[0]);
				keyElement.Time = time;
				return true;
			}

			//so there are 2 or more elements...
			int iPrevIndex = 0;
			int iNextIndex = iPrevIndex + 1;
			while (iNextIndex < Elements.Count)
			{
				//check if the time falls on or between the two keyframes
				if ((Elements[iPrevIndex].Time <= time) &&
					(Elements[iNextIndex].Time >= time))
				{
					KeyElement rPrev = Elements[iPrevIndex];
					KeyElement rNext = Elements[iNextIndex];

					if (time == rPrev.Time)
					{
						//it is the previous element
						keyElement.Copy(rPrev);
						return true;
					}
					else if (time == rNext.Time)
					{
						//it is the next element
						keyElement.Copy(rNext);
						return true;
					}

					//okay, copy the first one and add the changes
					keyElement.Copy(rPrev);
					keyElement.Time = time;

					//find the ratio of rotation/time for the whole step
					float fWholeTimeDelta = rNext.Time - rPrev.Time;

					//so if it rotates x degrees in y seconds, it rotates z degrees in w seconds
					float fStepTimeDelta = time - rPrev.Time;

					//if step is less than 1/60 second, use teh previous one
					if (fWholeTimeDelta <= 1.0f)
					{
						return true;
					}

					keyElement.KeyFrame = false; //not a keyframe!

					//get the step 
					float fWholeRotationDelta = rNext.Rotation - rPrev.Rotation;
					float fStepRotationDelta = ((fWholeRotationDelta * fStepTimeDelta) / fWholeTimeDelta);
					keyElement.Rotation = rPrev.Rotation + fStepRotationDelta;

					float fWholeXDelta = rNext.Translation.X - rPrev.Translation.X;
					float fStepXDelta = ((fWholeXDelta * fStepTimeDelta) / fWholeTimeDelta);
					keyElement.TranslationX = rPrev.Translation.X + fStepXDelta;

					float fWholeYDelta = rNext.Translation.Y - rPrev.Translation.Y;
					float fStepYDelta = ((fWholeYDelta * fStepTimeDelta) / fWholeTimeDelta);
					keyElement.TranslationY = rPrev.Translation.Y + fStepYDelta;

					return true;
				}

				iPrevIndex++;
				iNextIndex++;
			}

			//if it gets here, it is past the animation, return the last element
			keyElement.Copy(Elements[Elements.Count - 1]);
			keyElement.Time = time;
			return true;
		}

		/// <summary>
		/// remove a key element from the animation
		/// </summary>
		/// <param name="time">time of the keyelement to remove</param>
		/// <returns>bool: whether or not there was a key element at that time</returns>
		public bool RemoveKeyElement(int time)
		{
			bool bFound = false;
			int i = 0;
			while (i < Elements.Count)
			{
				if (Elements[i].Time == time)
				{
					Elements.RemoveAt(i);
					bFound = true;
				}
				else
				{
					i++;
				}
			}

			return bFound;
		}

		public void RemoveKeyElement(Macro pasteAction, int time, Animation myAnimation)
		{
			//get teh key element at that time
			KeyElement currentKeyElement = new KeyElement();
			if (!GetKeyElement(time, currentKeyElement))
			{
				//no key elements, so cant remove from this dude
				return;
			}

			//check if this is a key frame at this time
			if (currentKeyElement.KeyFrame)
			{
				//set the joint name
				currentKeyElement.JointName = Name;

				//create the remove action
				RemoveKeyElement myAction = new RemoveKeyElement(myAnimation, currentKeyElement);
				pasteAction.Add(myAction);
			}
		}

		public void MirrorRightToLeft(KeyBone rootBone, Macro pasteAction, int time, Animation myAnimation)
		{
			//Check if this bone starts with the work "left"
			string[] nameTokens = Name.Split(new Char[] { ' ' });
			if (nameTokens.Length >= 2)
			{
				//check if this bone needs to be mirrored
				string strJointName = "";
				bool bCopy = false;
				if (nameTokens[0] == "Left")
				{
					strJointName = "Right";
					bCopy = true;
				}
				else if (nameTokens[0] == "Right")
				{
					strJointName = "Left";
					bCopy = true;
				}

				if (bCopy)
				{
					for (int i = 1; i < nameTokens.Length; i++)
					{
						strJointName += " ";
						strJointName += nameTokens[i];
					}
					KeyJoint mirrorJoint = rootBone.GetKeyJoint(strJointName);
					if (null != mirrorJoint)
					{
						//get the current keyframe of the mirror joint
						KeyElement currentKeyElement = new KeyElement();
						if (!mirrorJoint.GetKeyElement(time, currentKeyElement))
						{
							return;
						}
						currentKeyElement.JointName = strJointName;

						//okay, fake up a new keyframe
						KeyElement replacementKeyElement = new KeyElement();
						if (!GetKeyElement(time, replacementKeyElement))
						{
							//no key elements, so cant finish operation
							return;
						}

						//set the keyframe up for the other dude
						replacementKeyElement.KeyFrame = true;
						replacementKeyElement.JointName = strJointName;
						replacementKeyElement.Layer = currentKeyElement.Layer;

						//add to the pasteaction
						SetKeyElement myAction = new SetKeyElement(myAnimation, currentKeyElement, replacementKeyElement);
						pasteAction.Add(myAction);
					}
				}
			}
		}

		/// <summary>
		/// Add a key element to the animation.  
		/// If an element already exists at that time, replace it.
		/// Sort the list of keyframes after it has been added.
		/// </summary>
		/// <param name="myElement">the key frame to add to the animation</param>
		public void AddKeyElement(KeyElement myElement)
		{
			//Do any elements exist with that time?
			RemoveKeyElement(myElement.Time);

			//add the element to the end and sort the list
			Elements.Add(myElement);
			Elements.Sort(new KeyElementSort());
		}

		public void Copy(Macro pasteAction,
			Animation targetAnimation, 
			int sourceTime, 
			int targetTime,
			bool selectiveCopy)
		{
			//get the keyelement to copy into the animation
			var sourceKeyElement = new KeyElement();
			GetKeyElement(sourceTime, sourceKeyElement);
			sourceKeyElement.Time = targetTime;
			sourceKeyElement.JointName = Name;
			sourceKeyElement.KeyFrame = true;

			//get the current keyelement out of that animtion
			var targetJoint = targetAnimation.GetKeyJoint(Name);
			if (null == targetJoint)
			{
				return;
			}
			var oldKeyElement = new KeyElement();
			targetJoint.GetKeyElement(targetTime, oldKeyElement);
			oldKeyElement.JointName = targetJoint.Name;

			//if this is a selective copy, set the rotation & translation to the old value
			if (selectiveCopy)
			{
				sourceKeyElement.Rotation = oldKeyElement.Rotation;
				sourceKeyElement.Translation = oldKeyElement.Translation;

				if (-1 != oldKeyElement.ImageIndex)
				{
					sourceKeyElement.ImageIndex = oldKeyElement.ImageIndex;
					sourceKeyElement.Flip = oldKeyElement.Flip;
					sourceKeyElement.RagDoll = oldKeyElement.RagDoll;
				}
			}

			//add to the animation
			var myAction = new SetKeyElement(targetAnimation, oldKeyElement, sourceKeyElement);
			pasteAction.Add(myAction);
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="oldName">the name of the joint to be renamed</param>
		/// <param name="newName">the new name for that joint.</param>
		public bool RenameJoint(string oldName, string newName)
		{
			//is it this joint?
			if (Name == oldName)
			{
				//rename all the key elements
				for (int i = 0; i < Elements.Count; i++)
				{
					Debug.Assert(Elements[i].JointName == oldName);
					Elements[i].RenameJoint(oldName, newName);
					Debug.Assert(Elements[i].JointName == newName);
				}

				//rename this dude
				Name = newName;

				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// change the time for this animation and move all the key frames around
		/// </summary>
		/// <param name="oldTime">teh previous time of this animation</param>
		/// <param name="currentTime">the new time of this animation</param>
		public void SetTime(int oldTime, int currentTime)
		{
			for (int i = 0; i < Elements.Count; i++)
			{
				//get the updated time of this keyelemt
				int myOldTime = Elements[i].Time;
				Elements[i].Time = ((myOldTime * currentTime) / oldTime);

				//check if any other keyframes are already at this time, so frames dont overlap
				for (int j = 0; j < i; j++)
				{
					if (Elements[i].Time == Elements[j].Time)
					{
						Elements[i].Time++;
					}
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// write all this dude's stuff out to xml
		/// </summary>
		/// <param name="animationXml">the animtion object to add all the keyframes to</param>
		/// <param name="myBone">the bone this dude references</param>
		public void WriteXmlFormat(AnimationXml animationXml, Bone myBone)
		{
			//add all the key elements to that dude
			for (int i = 0; i < Elements.Count; i++)
			{
				//don't write out fucked up shit?
				Debug.Assert(Elements[i].KeyFrame);

				//don't write out reduntant key elements
				if ((i > 0) && (i < (Elements.Count - 1)))
				{
					if (Elements[i].Compare(Elements[i - 1]) && Elements[i].Compare(Elements[i + 1]))
					{
						//dont write out if this matches the previous and next keys
						continue;
					}

					if  ((i == Elements.Count - 1) && Elements[i].Compare(Elements[i - 1]))
					{
						//dont write out if this is last key and matches prev
						continue;
					}
				}
				Elements[i].WriteXmlFormat(animationXml, myBone);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			foreach (var element in Elements)
			{
				element.MultiplyLayers(multiply);
			}
		}

		#endregion
	}
}