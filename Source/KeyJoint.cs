using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace SPFLib
{
	public class CKeyJoint
	{
		#region Member Variables

		//!Teh list of key elements for this dude
		private List<CKeyElement> m_listElements;

		//!this dude's name
		private string m_strName;

		#endregion

		#region Properties

		public string Name
		{
			get { return m_strName; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public CKeyJoint(string strName)
		{
			m_listElements = new List<CKeyElement>();
			m_strName = strName;
		}

		/// <summary>
		/// Get a key element at a certain tim from this dude
		/// </summary>
		/// <param name="iTime">The time of the key element to get</param>
		/// <param name="rKeyElement">The key element to output the stuff to</param>
		public bool GetKeyElement(int iTime, CKeyElement rKeyElement)
		{
			//if no elements, fuckin FALSE
			if (m_listElements.Count <= 0)
			{
				return false;
			}

			//if only one element, return it
			if (m_listElements.Count == 1)
			{
				rKeyElement.Copy(m_listElements[0]);
				rKeyElement.Time = iTime;
				return true;
			}

			//so there are 2 or more elements...
			int iPrevIndex = 0;
			int iNextIndex = iPrevIndex + 1;
			while (iNextIndex < m_listElements.Count)
			{
				//check if the time falls on or between the two keyframes
				if ((m_listElements[iPrevIndex].Time <= iTime) &&
					(m_listElements[iNextIndex].Time >= iTime))
				{
					CKeyElement rPrev = m_listElements[iPrevIndex];
					CKeyElement rNext = m_listElements[iNextIndex];

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
			rKeyElement.Copy(m_listElements[m_listElements.Count - 1]);
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
			while (i < m_listElements.Count)
			{
				if (m_listElements[i].Time == iTime)
				{
					m_listElements.RemoveAt(i);
					bFound = true;
				}
				else
				{
					i++;
				}
			}

			return bFound;
		}

		public void RemoveKeyElement(CPasteAction rPasteAction, int iTime, CAnimation myAnimation)
		{
			//get teh key element at that time
			CKeyElement CurrentKeyElement = new CKeyElement();
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
				CRemoveKeyElement myAction = new CRemoveKeyElement(myAnimation, CurrentKeyElement);
				rPasteAction.AddAction(myAction);
			}
		}

		public void MirrorRightToLeft(CKeyBone RootBone, CPasteAction rPasteAction, int iTime, CAnimation myAnimation)
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
					CKeyJoint MirrorJoint = RootBone.GetKeyJoint(strJointName);
					if (null != MirrorJoint)
					{
						//get the current keyframe of the mirror joint
						CKeyElement CurrentKeyElement = new CKeyElement();
						if (!MirrorJoint.GetKeyElement(iTime, CurrentKeyElement))
						{
							return;
						}
						CurrentKeyElement.JointName = strJointName;

						//okay, fake up a new keyframe
						CKeyElement ReplacementKeyElement = new CKeyElement();
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
						CSetKeyElement myAction = new CSetKeyElement(myAnimation, CurrentKeyElement, ReplacementKeyElement);
						rPasteAction.AddAction(myAction);
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
		public void AddKeyElement(CKeyElement rMyElement)
		{
			//Do any elements exist with that time?
			RemoveKeyElement(rMyElement.Time);

			//add the element to the end and sort the list
			m_listElements.Add(rMyElement);
			m_listElements.Sort(new KeyElementSort());
		}

		public void Copy(CPasteAction myPasteAction,
			CAnimation myTargetAnimation, 
			int iSourceTime, 
			int iTargetTime,
			bool bSelectiveCopy)
		{
			//get the keyelement to copy into the animation
			CKeyElement SourceKeyElement = new CKeyElement();
			GetKeyElement(iSourceTime, SourceKeyElement);
			SourceKeyElement.Time = iTargetTime;
			SourceKeyElement.JointName = Name;
			SourceKeyElement.KeyFrame = true;

			//get the current keyelement out of that animtion
			CKeyJoint TargetJoint = myTargetAnimation.GetKeyJoint(Name);
			if (null == TargetJoint)
			{
				return;
			}
			CKeyElement OldKeyElement = new CKeyElement();
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
			CSetKeyElement myAction = new CSetKeyElement(myTargetAnimation, OldKeyElement, SourceKeyElement);
			myPasteAction.AddAction(myAction);
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
				for (int i = 0; i < m_listElements.Count; i++)
				{
					Debug.Assert(m_listElements[i].JointName == strOldName);
					m_listElements[i].RenameJoint(strOldName, strNewName);
					Debug.Assert(m_listElements[i].JointName == strNewName);
				}

				//rename this dude
				m_strName = strNewName;

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
			for (int i = 0; i < m_listElements.Count; i++)
			{
				//get the updated time of this keyelemt
				int fMyOldTime = m_listElements[i].Time;
				m_listElements[i].Time = ((fMyOldTime * iCurrentTime) / iOldTime);

				//check if any other keyframes are already at this time, so frames dont overlap
				for (int j = 0; j < i; j++)
				{
					if (m_listElements[i].Time == m_listElements[j].Time)
					{
						m_listElements[i].Time++;
					}
				}
			}
		}

		#endregion //Methods

		#region File IO

#if WINDOWS

		/// <summary>
		/// write all this dude's stuff out to xml
		/// </summary>
		/// <param name="rXMLDude">the animtion object to add all the keyframes to</param>
		public void WriteXMLFormat(AnimationLib.AnimationXML rXMLDude, CBone rMyBone)
		{
			//add all the key elements to that dude
			for (int i = 0; i < m_listElements.Count; i++)
			{
				//don't write out fucked up shit?
				Debug.Assert(m_listElements[i].KeyFrame);

				//don't write out reduntant key elements
				if ((i > 0) && (i < (m_listElements.Count - 1)))
				{
					if (m_listElements[i].Compare(m_listElements[i - 1]) && m_listElements[i].Compare(m_listElements[i + 1]))
					{
						//dont write out if this matches the previous and next keys
						continue;
					}

					if  ((i == m_listElements.Count - 1) && m_listElements[i].Compare(m_listElements[i - 1]))
					{
						//dont write out if this is last key and matches prev
						continue;
					}
				}
				m_listElements[i].WriteXMLFormat(rXMLDude, rMyBone);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			foreach (CKeyElement i in m_listElements)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

#endif

		public CKeyElement ReadSerializedAnimationFormat(AnimationLib.KeyXML rKeyElement)
		{
			//create a new key element
			CKeyElement myElement = new CKeyElement();
			myElement.ReadSerializedFormat(rKeyElement);
			Debug.Assert(myElement.KeyFrame);
			m_listElements.Add(myElement);

			return myElement;
		}

		#endregion
	}
}