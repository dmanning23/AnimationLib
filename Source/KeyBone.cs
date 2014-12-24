using System.Collections.Generic;
using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib
{
	/// <summary>
	/// There is one keybone for every keybone in every animation.
	/// </summary>
	public class KeyBone
	{
		#region Properties

		/// <summary>
		/// name of the bone this dude represents
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// the list of child keybones for this dude
		/// </summary>
		public List<KeyBone> Bones { get; private set; }

		/// <summary>
		/// This dude's keyjoint
		/// </summary>
		public KeyJoint KeyJoint { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// Constructor!
		/// </summary>
		/// <param name="myBone">the bone this dude represents</param>
		public KeyBone(Bone myBone)
		{
			//grab the name of this bone
			Name = myBone.Name;

			//setup the child bones
			Bones = new List<KeyBone>();
			for (int i = 0; i < myBone.Bones.Count; i++)
			{
				Bone rMyChildBone = myBone.Bones[i];
				KeyBone childKeyBone = new KeyBone(rMyChildBone);
				Bones.Add(childKeyBone);
			}

			//setup the key joint
			KeyJoint = new KeyJoint(Name);
		}

		/// <summary>
		/// recursively get a keyjoint from the bone structure
		/// </summary>
		/// <param name="strJointName">The name of the keyjoint to get</param>
		public KeyJoint GetKeyJoint(string strJointName)
		{
			//check my keyjoints
			if (KeyJoint.Name == strJointName)
			{
				return KeyJoint;
			}

			//check child key bones
			KeyJoint rFoundKeyJoint = null;
			for (int i = 0; i < Bones.Count; i++)
			{
				rFoundKeyJoint = Bones[i].GetKeyJoint(strJointName);
				if (rFoundKeyJoint != null)
				{
					return rFoundKeyJoint;
				}
			}

			//didnt find a keyjoint with that name
			return null;
		}

		/// <summary>
		/// Recursively find a keybone
		/// </summary>
		/// <param name="strBoneName">name of the bone to find</param>
		/// <returns>keybone with that name, or null if no bone found</returns>
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
			if (iIndex < Bones.Count)
			{
				return Bones[iIndex];
			}
			else
			{
				return null;
			}
		}

		public void Copy(Macro myPasteAction,
			Animation myTargetAnimation, 
			int iSourceTime, 
			int iTargetTime,
			bool bSelectiveCopy)
		{
			//go through each joint
				KeyJoint.Copy(myPasteAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);

			//go through child bones
				for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].Copy(myPasteAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);
			}
		}

		public void RemoveKeyElement(Macro rPasteAction, int iTime, Animation myAnimation)
		{
			//go through each joint
			KeyJoint.RemoveKeyElement(rPasteAction, iTime, myAnimation);

			//go through child bones
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].RemoveKeyElement(rPasteAction, iTime, myAnimation);
			}
		}

		public void MirrorRightToLeft(KeyBone RootBone, Macro rPasteAction, int iTime, Animation myAnimation)
		{
			//mirror all the keyjoints
			KeyJoint.MirrorRightToLeft(RootBone, rPasteAction, iTime, myAnimation);

			//mirror all the child bones too
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].MirrorRightToLeft(RootBone, rPasteAction, iTime, myAnimation);
			}
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="strOldName">the name of the joint to be renamed</param>
		/// <param name="strNewName">the new name for that joint.</param>
		public bool RenameJoint(string strOldName, string strNewName)
		{
			if (KeyJoint.RenameJoint(strOldName, strNewName))
			{
				return true;
			}

			for (int i = 0; i < Bones.Count; i++)
			{
				if (Bones[i].RenameJoint(strOldName, strNewName))
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
			KeyJoint.SetTime(iOldTime, iCurrentTime);

			//set the time for all the child bones
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].SetTime(iOldTime, iCurrentTime);
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
			Debug.Assert(null != rMyBone);

			//write out all my joints
			KeyJoint.WriteXMLFormat(rXMLDude, rMyBone);
			
			//write out all the children's joints
			for (int i = 0; i < Bones.Count; i++)
			{
				//find the bone for this keybone
				Bone rChildBone = rMyBone.GetBone(Bones[i].Name);
				Debug.Assert(null != rChildBone);
				Bones[i].WriteXMLFormat(rXMLDude, rChildBone);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			KeyJoint.MultiplyLayers(iMultiply);

			foreach (KeyBone i in Bones)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

		#endregion
	}
}