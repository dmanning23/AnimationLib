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
		public KeyJoint(string strName)
		{
			Elements = new List<KeyElement>();
			Name = strName;
		}

		/// <summary>
		/// Get a key element at a certain tim from this dude
		/// </summary>
		/// <param name="iTime">The time of the key element to get</param>
		/// <param name="rKeyElement">The key element to output the stuff to</param>
		public bool GetKeyElement(int iTime, KeyElement rKeyElement)
		{
			//if no elements, fuckin FALSE
			if (Elements.Count <= 0)
			{
				return false;
			}

			//if only one element, return it
			if (Elements.Count == 1)
			{
				rKeyElement.Copy(Elements[0]);
				rKeyElement.Time = iTime;
				return true;
			}

			//so there are 2 or more elements...
			int iPrevIndex = 0;
			int iNextIndex = iPrevIndex + 1;
			while (iNextIndex < Elements.Count)
			{
				//check if the time falls on or between the two keyframes
				if ((Elements[iPrevIndex].Time <= iTime) &&
					(Elements[iNextIndex].Time >= iTime))
				{
					KeyElement rPrev = Elements[iPrevIndex];
					KeyElement rNext = Elements[iNextIndex];

					if (iTime == rPrev.Time)
					{
						//it is the previous element
						rKeyElement.Copy(rPrev);
						return true;
					}
					else if (iTime == rNext.Time)
					{
						//it is the next element
						rKeyElement.Copy(rNext);
						return true;
					}

					//okay, copy the first one and add the changes
					rKeyElement.Copy(rPrev);
					rKeyElement.Time = iTime;

					//find the ratio of rotation/time for the whole step
					float fWholeTimeDelta = rNext.Time - rPrev.Time;

					//so if it rotates x degrees in y seconds, it rotates z degrees in w seconds
					float fStepTimeDelta = iTime - rPrev.Time;

					//if step is less than 1/60 second, use teh previous one
					if (fWholeTimeDelta <= 1.0f)
					{
						return true;
					}

					rKeyElement.KeyFrame = false; //not a keyframe!

					//get the step 
					float fWholeRotationDelta = rNext.Rotation - rPrev.Rotation;
					float fStepRotationDelta = ((fWholeRotationDelta * fStepTimeDelta) / fWholeTimeDelta);
					rKeyElement.Rotation = rPrev.Rotation + fStepRotationDelta;

					float fWholeXDelta = rNext.Translation.X - rPrev.Translation.X;
					float fStepXDelta = ((fWholeXDelta * fStepTimeDelta) / fWholeTimeDelta);
					rKeyElement.TranslationX = rPrev.Translation.X + fStepXDelta;

					float fWholeYDelta = rNext.Translation.Y - rPrev.Translation.Y;
					float fStepYDelta = ((fWholeYDelta * fStepTimeDelta) / fWholeTimeDelta);
					rKeyElement.TranslationY = rPrev.Translation.Y + fStepYDelta;

					return true;
				}

				iPrevIndex++;
				iNextIndex++;
			}

			//if it gets here, it is past the animation, return the last element
			rKeyElement.Copy(Elements[Elements.Count - 1]);
			rKeyElement.Time = iTime;
			return true;
		}

		/// <summary>
		/// remove a key element from the animation
		/// </summary>
		/// <param name="iTime">time of the keyelement to remove</param>
		/// <returns>bool: whether or not there was a key element at that time</returns>
		public bool RemoveKeyElement(int iTime)
		{
			bool bFound = false;
			int i = 0;
			while (i < Elements.Count)
			{
				if (Elements[i].Time == iTime)
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

		public void RemoveKeyElement(Macro rPasteAction, int iTime, Animation myAnimation)
		{
			//get teh key element at that time
			KeyElement CurrentKeyElement = new KeyElement();
			if (!GetKeyElement(iTime, CurrentKeyElement))
			{
				//no key elements, so cant remove from this dude
				return;
			}

			//check if this is a key frame at this time
			if (CurrentKeyElement.KeyFrame)
			{
				//set the joint name
				CurrentKeyElement.JointName = Name;

				//create the remove action
				RemoveKeyElement myAction = new RemoveKeyElement(myAnimation, CurrentKeyElement);
				rPasteAction.Add(myAction);
			}
		}

		public void MirrorRightToLeft(KeyBone RootBone, Macro rPasteAction, int iTime, Animation myAnimation)
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
					KeyJoint MirrorJoint = RootBone.GetKeyJoint(strJointName);
					if (null != MirrorJoint)
					{
						//get the current keyframe of the mirror joint
						KeyElement CurrentKeyElement = new KeyElement();
						if (!MirrorJoint.GetKeyElement(iTime, CurrentKeyElement))
						{
							return;
						}
						CurrentKeyElement.JointName = strJointName;

						//okay, fake up a new keyframe
						KeyElement ReplacementKeyElement = new KeyElement();
						if (!GetKeyElement(iTime, ReplacementKeyElement))
						{
							//no key elements, so cant finish operation
							return;
						}

						//set the keyframe up for the other dude
						ReplacementKeyElement.KeyFrame = true;
						ReplacementKeyElement.JointName = strJointName;
						ReplacementKeyElement.Layer = CurrentKeyElement.Layer;

						//add to the pasteaction
						SetKeyElement myAction = new SetKeyElement(myAnimation, CurrentKeyElement, ReplacementKeyElement);
						rPasteAction.Add(myAction);
					}
				}
			}
		}

		/// <summary>
		/// Add a key element to the animation.  
		/// If an element already exists at that time, replace it.
		/// Sort the list of keyframes after it has been added.
		/// </summary>
		/// <param name="rMyElement">the key frame to add to the animation</param>
		public void AddKeyElement(KeyElement rMyElement)
		{
			//Do any elements exist with that time?
			RemoveKeyElement(rMyElement.Time);

			//add the element to the end and sort the list
			Elements.Add(rMyElement);
			Elements.Sort(new KeyElementSort());
		}

		public void Copy(Macro myPasteAction,
			Animation myTargetAnimation, 
			int iSourceTime, 
			int iTargetTime,
			bool bSelectiveCopy)
		{
			//get the keyelement to copy into the animation
			KeyElement SourceKeyElement = new KeyElement();
			GetKeyElement(iSourceTime, SourceKeyElement);
			SourceKeyElement.Time = iTargetTime;
			SourceKeyElement.JointName = Name;
			SourceKeyElement.KeyFrame = true;

			//get the current keyelement out of that animtion
			KeyJoint TargetJoint = myTargetAnimation.GetKeyJoint(Name);
			if (null == TargetJoint)
			{
				return;
			}
			KeyElement OldKeyElement = new KeyElement();
			TargetJoint.GetKeyElement(iTargetTime, OldKeyElement);
			OldKeyElement.JointName = TargetJoint.Name;

			//if this is a selective copy, set the rotation & translation to the old value
			if (bSelectiveCopy)
			{
				SourceKeyElement.Rotation = OldKeyElement.Rotation;
				SourceKeyElement.Translation = OldKeyElement.Translation;

				if (-1 != OldKeyElement.ImageIndex)
				{
					SourceKeyElement.ImageIndex = OldKeyElement.ImageIndex;
					SourceKeyElement.Flip = OldKeyElement.Flip;
					SourceKeyElement.RagDoll = OldKeyElement.RagDoll;
				}
			}

			//add to the animation
			SetKeyElement myAction = new SetKeyElement(myTargetAnimation, OldKeyElement, SourceKeyElement);
			myPasteAction.Add(myAction);
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="strOldName">the name of the joint to be renamed</param>
		/// <param name="strNewName">the new name for that joint.</param>
		public bool RenameJoint(string strOldName, string strNewName)
		{
			//is it this joint?
			if (Name == strOldName)
			{
				//rename all the key elements
				for (int i = 0; i < Elements.Count; i++)
				{
					Debug.Assert(Elements[i].JointName == strOldName);
					Elements[i].RenameJoint(strOldName, strNewName);
					Debug.Assert(Elements[i].JointName == strNewName);
				}

				//rename this dude
				Name = strNewName;

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
		/// <param name="iOldTime">teh previous time of this animation</param>
		/// <param name="iCurrentTime">the new time of this animation</param>
		public void SetTime(int iOldTime, int iCurrentTime)
		{
			for (int i = 0; i < Elements.Count; i++)
			{
				//get the updated time of this keyelemt
				int fMyOldTime = Elements[i].Time;
				Elements[i].Time = ((fMyOldTime * iCurrentTime) / iOldTime);

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
		/// <param name="rXMLDude">the animtion object to add all the keyframes to</param>
		public void WriteXMLFormat(AnimationLib.AnimationXML rXMLDude, Bone rMyBone)
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
				Elements[i].WriteXMLFormat(rXMLDude, rMyBone);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			foreach (KeyElement i in Elements)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

		#endregion
	}
}