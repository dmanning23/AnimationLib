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

		private readonly Bone _bone;

		/// <summary>
		/// name of the bone this dude represents
		/// </summary>
		public string Name
		{
			get
			{
				return _bone.Name;
			}
		}

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
		/// <param name="bone">the bone this dude represents</param>
		public KeyBone(Bone bone)
		{
			//grab the name of this bone
			_bone = bone;

			//setup the child bones
			Bones = new List<KeyBone>();
			for (var i = 0; i < bone.Bones.Count; i++)
			{
				Bone childBone = bone.Bones[i];
				KeyBone childKeyBone = new KeyBone(childBone);
				Bones.Add(childKeyBone);
			}

			//setup the key joint
			KeyJoint = new KeyJoint(_bone);
		}

		/// <summary>
		/// recursively get a keyjoint from the bone structure
		/// </summary>
		/// <param name="jointName">The name of the keyjoint to get</param>
		public KeyJoint GetKeyJoint(string jointName)
		{
			//check my keyjoints
			if (KeyJoint.Name == jointName)
			{
				return KeyJoint;
			}

			//check child key bones
			KeyJoint foundKeyJoint = null;
			for (var i = 0; i < Bones.Count; i++)
			{
				foundKeyJoint = Bones[i].GetKeyJoint(jointName);
				if (foundKeyJoint != null)
				{
					return foundKeyJoint;
				}
			}

			//didnt find a keyjoint with that name
			return null;
		}

		/// <summary>
		/// Recursively find a keybone
		/// </summary>
		/// <param name="boneName">name of the bone to find</param>
		/// <returns>keybone with that name, or null if no bone found</returns>
		public KeyBone GetKeyBone(string boneName)
		{
			//is this the dude?
			if (Name == boneName)
			{
				return this;
			}

			//is the requested bone underneath this dude?
			for (var i = 0; i < Bones.Count; i++)
			{
				KeyBone bone = Bones[i].GetKeyBone(boneName);
				if (null != bone)
				{
					return bone;
				}
			}

			//didnt find a joint with that name :(
			return null;
		}

		/// <summary>
		/// Get a child bone from this dude
		/// </summary>
		/// <param name="index">the index of the child to get</param>
		public KeyBone GetChildBone(int index)
		{
			Debug.Assert(index >= 0);
			if (index < Bones.Count)
			{
				return Bones[index];
			}
			else
			{
				return null;
			}
		}

		public void Copy(CommandStack pasteAction,
			Animation targetAnimation, 
			int sourceTime, 
			int targetTime,
			bool selectiveCopy)
		{
			//go through each joint
			KeyJoint.Copy(pasteAction, targetAnimation, sourceTime, targetTime, selectiveCopy);

			//go through child bones
				for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].Copy(pasteAction, targetAnimation, sourceTime, targetTime, selectiveCopy);
			}
		}

		public void RemoveKeyElement(CommandStack pasteAction, int time, Animation animation)
		{
			//go through each joint
			KeyJoint.RemoveKeyElement(pasteAction, time, animation);

			//go through child bones
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].RemoveKeyElement(pasteAction, time, animation);
			}
		}

		public void MirrorRightToLeft(KeyBone rootBone, CommandStack pasteAction, int time, Animation animation)
		{
			//mirror all the keyjoints
			KeyJoint.MirrorRightToLeft(rootBone, pasteAction, time, animation);

			//mirror all the child bones too
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].MirrorRightToLeft(rootBone, pasteAction, time, animation);
			}
		}

		/// <summary>
		/// change the time for this animation and move all the key frames around
		/// </summary>
		/// <param name="oldTime">teh previous time of this animation</param>
		/// <param name="currentTime">the new time of this animation</param>
		public void SetTime(int oldTime, int currentTime)
		{
			//set the time for all the joints
			KeyJoint.SetTime(oldTime, currentTime);

			//set the time for all the child bones
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].SetTime(oldTime, currentTime);
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			KeyJoint.MultiplyLayers(multiply);

			foreach (var bone in Bones)
			{
				bone.MultiplyLayers(multiply);
			}
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods
	}
}