using System.Collections.Generic;
using System.Diagnostics;

namespace AnimationLib
{
	public class KeyBone
	{
		#region Member Variables

		/// <summary>
		/// the list of keyjoints for this dude
		/// </summary>
		private KeyJoint m_KeyJoint;

		/// <summary>
		/// the list of child keybones for this dude
		/// </summary>
		private List<KeyBone> m_listChildren;

		/// <summary>
		/// name of the bone this dude represents
		/// </summary>
		private string m_strBoneName;

		#endregion

		#region Properties

		public string Name
		{
			get { return m_strBoneName; }
		}

		private List<KeyBone> Bones
		{
			get { return m_listChildren; }
		}

		public KeyJoint KeyJoint
		{
			get { return m_KeyJoint; }
		}

		#endregion //Properties

		#region Methods

		public KeyBone(Bone myBone)
		{
			//grab the name of this bone
			m_strBoneName = myBone.Name;

			//setup the child bones
			m_listChildren = new List<KeyBone>();
			for (int i = 0; i < myBone.Bones.Count; i++)
			{
				Bone rMyChildBone = myBone.Bones[i];
				KeyBone childKeyBone = new KeyBone(rMyChildBone);
				m_listChildren.Add(childKeyBone);
			}

			//setup the key joint
			m_KeyJoint = new KeyJoint(m_strBoneName);
		}

		/// <summary>
		/// recursively get a keyjoint from the bone structure
		/// </summary>
		/// <param name="strJointName">The name of the keyjoint to get</param>
		public KeyJoint GetKeyJoint(string strJointName)
		{
			//check my keyjoints
			if (m_KeyJoint.Name == strJointName)
			{
				return m_KeyJoint;
			}

			//check child key bones
			KeyJoint rFoundKeyJoint = null;
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				rFoundKeyJoint = m_listChildren[i].GetKeyJoint(strJointName);
				if (rFoundKeyJoint != null)
				{
					return rFoundKeyJoint;
				}
			}

			//didnt find a keyjoint with that name
			return null;
		}

		public KeyBone GetKeyBone(string strBoneName)
		{
			//is this the dude?
			if (Name == strBoneName)
			{
				return this;
			}

			//is the requested bone underneath this dude?
			for (int i = 0; i < Bones.Count; i++)
			{
				KeyBone myBone = Bones[i].GetKeyBone(strBoneName);
				if (null != myBone)
				{
					return myBone;
				}
			}

			//didnt find a joint with that name :(
			return null;
		}

		/// <summary>
		/// Get a child bone from this dude
		/// </summary>
		/// <param name="iIndex">the index of the child to get</param>
		public KeyBone GetChildBone(int iIndex)
		{
			Debug.Assert(iIndex >= 0);
			if (iIndex < m_listChildren.Count)
			{
				return m_listChildren[iIndex];
			}
			else
			{
				return null;
			}
		}

#if TOOLS

		public void Copy(CPasteAction myPasteAction,
			Animation myTargetAnimation, 
			int iSourceTime, 
			int iTargetTime,
			bool bSelectiveCopy)
		{
			//go through each joint
				m_KeyJoint.Copy(myPasteAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);

			//go through child bones
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].Copy(myPasteAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);
			}
		}

		public void RemoveKeyElement(CPasteAction rPasteAction, int iTime, Animation myAnimation)
		{
			//go through each joint
			m_KeyJoint.RemoveKeyElement(rPasteAction, iTime, myAnimation);

			//go through child bones
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].RemoveKeyElement(rPasteAction, iTime, myAnimation);
			}
		}

		public void MirrorRightToLeft(KeyBone RootBone, CPasteAction rPasteAction, int iTime, Animation myAnimation)
		{
			//mirror all the keyjoints
			m_KeyJoint.MirrorRightToLeft(RootBone, rPasteAction, iTime, myAnimation);

			//mirror all the child bones too
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].MirrorRightToLeft(RootBone, rPasteAction, iTime, myAnimation);
			}
		}

#endif

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="strOldName">the name of the joint to be renamed</param>
		/// <param name="strNewName">the new name for that joint.</param>
		public bool RenameJoint(string strOldName, string strNewName)
		{
			if (m_KeyJoint.RenameJoint(strOldName, strNewName))
			{
				return true;
			}

			for (int i = 0; i < m_listChildren.Count; i++)
			{
				if (m_listChildren[i].RenameJoint(strOldName, strNewName))
				{
					return true;
				}
			}

			//the joint wasnt found yet
			return false;
		}

		/// <summary>
		/// change the time for this animation and move all the key frames around
		/// </summary>
		/// <param name="iOldTime">teh previous time of this animation</param>
		/// <param name="iCurrentTime">the new time of this animation</param>
		public void SetTime(int iOldTime, int iCurrentTime)
		{
			//set the time for all the joints
			m_KeyJoint.SetTime(iOldTime, iCurrentTime);

			//set the time for all the child bones
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].SetTime(iOldTime, iCurrentTime);
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// write all this dude's stuff out to xml
		/// </summary>
		/// <param name="rXMLDude">the animtion object to add all the keyframes to</param>
		public void WriteXMLFormat(AnimationLib.AnimationXML rXMLDude, Bone rMyBone)
		{
			Debug.Assert(null != rMyBone);

			//write out all my joints
			m_KeyJoint.WriteXMLFormat(rXMLDude, rMyBone);
			
			//write out all the children's joints
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				//find the bone for this keybone
				Bone rChildBone = rMyBone.GetBone(m_listChildren[i].Name);
				Debug.Assert(null != rChildBone);
				m_listChildren[i].WriteXMLFormat(rXMLDude, rChildBone);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			m_KeyJoint.MultiplyLayers(iMultiply);

			foreach (KeyBone i in m_listChildren)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

		#endregion
	}
}