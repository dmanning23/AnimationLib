using AnimationLib.Commands;
using Microsoft.Xna.Framework;
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

		private readonly Bone _bone;

		/// <summary>
		/// Teh list of key elements for this dude
		/// </summary>
		public List<KeyElement> Elements { get; private set; }

		/// <summary>
		/// this dude's name
		/// </summary>
		public string Name
		{
			get
			{
				return _bone.Name;
			}
		}

		public Bone Bone
		{
			get
			{
				return _bone;
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyJoint(Bone bone)
		{
			Elements = new List<KeyElement>();
			_bone = bone;
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
			int prevIndex = 0;
			int nextIndex = prevIndex + 1;
			while (nextIndex < Elements.Count)
			{
				//check if the time falls on or between the two keyframes
				if ((Elements[prevIndex].Time <= time) &&
					(Elements[nextIndex].Time >= time))
				{
					KeyElement prevElement = Elements[prevIndex];
					KeyElement nextElement = Elements[nextIndex];

					if (time == prevElement.Time)
					{
						//it is the previous element
						keyElement.Copy(prevElement);
						return true;
					}
					else if (time == nextElement.Time)
					{
						//it is the next element
						keyElement.Copy(nextElement);
						return true;
					}

					//okay, copy the first one and add the changes
					keyElement.Copy(prevElement);
					keyElement.Time = time;

					//find the ratio of rotation/time for the whole step
					float wholeTimeDelta = nextElement.Time - prevElement.Time;

					//so if it rotates x degrees in y seconds, it rotates z degrees in w seconds
					float stepTimeDelta = time - prevElement.Time;

					//if step is less than 1/60 second, use teh previous one
					if (wholeTimeDelta <= 1.0f)
					{
						return true;
					}

					keyElement.KeyFrame = false; //not a keyframe!

					//get the step 
					float wholeRotationDelta = nextElement.Rotation - prevElement.Rotation;
					float stepRotationDelta = ((wholeRotationDelta * stepTimeDelta) / wholeTimeDelta);
					keyElement.Rotation = prevElement.Rotation + stepRotationDelta;

					Vector2 wholeDelta = nextElement.Translation - prevElement.Translation;
					Vector2 stepDelta = ((wholeDelta * stepTimeDelta) / wholeTimeDelta);
					keyElement.Translation = prevElement.Translation + stepDelta;

					return true;
				}

				prevIndex++;
				nextIndex++;
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

		public void RemoveKeyElement(CommandStack pasteAction, int time, Animation myAnimation)
		{
			//get teh key element at that time
			KeyElement currentKeyElement = new KeyElement(_bone);
			if (!GetKeyElement(time, currentKeyElement))
			{
				//no key elements, so cant remove from this dude
				return;
			}

			//check if this is a key frame at this time
			if (currentKeyElement.KeyFrame)
			{
				//create the remove action
				RemoveKeyElement myAction = new RemoveKeyElement(myAnimation, currentKeyElement);
				pasteAction.Add(myAction);
			}
		}

		public void MirrorRightToLeft(KeyBone rootBone, CommandStack pasteAction, int time, Animation myAnimation)
		{
			//Check if this bone starts with the work "left"
			string[] nameTokens = Name.Split(new Char[] { ' ' });
			if (nameTokens.Length >= 2)
			{
				//check if this bone needs to be mirrored
				string jointName = "";
				bool copy = false;
				if (nameTokens[0] == "Left")
				{
					jointName = "Right";
					copy = true;
				}
				else if (nameTokens[0] == "Right")
				{
					jointName = "Left";
					copy = true;
				}
				else if (nameTokens[0] == "Middle")
				{
					jointName = "Right";
					copy = true;
				}

				if (copy)
				{
					for (int i = 1; i < nameTokens.Length; i++)
					{
						jointName += " ";
						jointName += nameTokens[i];
					}
					KeyJoint mirrorJoint = rootBone.GetKeyJoint(jointName);
					if (null != mirrorJoint)
					{
						//get the current keyframe of the mirror joint
						KeyElement currentKeyElement = new KeyElement(_bone);
						if (!mirrorJoint.GetKeyElement(time, currentKeyElement))
						{
							return;
						}

						//okay, fake up a new keyframe
						KeyElement replacementKeyElement = new KeyElement(_bone);
						if (!GetKeyElement(time, replacementKeyElement))
						{
							//no key elements, so cant finish operation
							return;
						}

						//set the keyframe up for the other dude
						currentKeyElement.Bone = mirrorJoint.Bone;
						replacementKeyElement.Bone = mirrorJoint.Bone;
						replacementKeyElement.KeyFrame = true;
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

		public void Copy(CommandStack pasteAction,
			Animation targetAnimation, 
			int sourceTime, 
			int targetTime,
			bool selectiveCopy)
		{
			//get the keyelement to copy into the animation
			var sourceKeyElement = new KeyElement(_bone);
			GetKeyElement(sourceTime, sourceKeyElement);
			sourceKeyElement.Time = targetTime;
			sourceKeyElement.KeyFrame = true;

			//get the current keyelement out of that animtion
			var targetJoint = targetAnimation.GetKeyJoint(Name);
			if (null == targetJoint)
			{
				return;
			}
			var oldKeyElement = new KeyElement(_bone);
			targetJoint.GetKeyElement(targetTime, oldKeyElement);

			//if this is a selective copy, set the rotation & translation to the old value
			if (selectiveCopy)
			{
				sourceKeyElement.Rotation = oldKeyElement.Rotation;
				sourceKeyElement.Translation = oldKeyElement.Translation;

				if (-1 != oldKeyElement.ImageIndex)
				{
					sourceKeyElement.ImageIndex = oldKeyElement.ImageIndex;
					sourceKeyElement.Flip = oldKeyElement.Flip;
				}
			}

			//add to the animation
			var myAction = new SetKeyElement(targetAnimation, oldKeyElement, sourceKeyElement);
			pasteAction.Add(myAction);
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

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods
	}
}