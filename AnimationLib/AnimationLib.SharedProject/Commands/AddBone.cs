using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddBone : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string BoneName { get; set; }

		string ParentBoneName { get; set; }

		#endregion //Properties

		#region Methods

		public AddBone(AnimationContainer animationContainer, string boneName, string parentBoneName)
		{
			AnimationContainer = animationContainer;
			BoneName = boneName;
			ParentBoneName = parentBoneName;
		}

		public bool Execute()
		{
			//Find the parent bone
			var parent = AnimationContainer.Skeleton.GetBone(ParentBoneName);
			if (parent == null)
			{
				throw new Exception($"Couldn't find parent bone with name {ParentBoneName}");
			}

			//make sure it is a unique bone & joint name
			var bone = AnimationContainer.Skeleton.GetBone(BoneName);
			var joint = AnimationContainer.Skeleton.GetJoint(BoneName);
			if (null != bone || null != joint)
			{
				throw new Exception($"Couldn't add bone with duplicate name {BoneName}");
			}

			//add a matching joint to the parent
			joint = parent.AddJoint(BoneName, false);

			//create the new bone & add it to the parent
			bone = new Bone()
			{
				Name = BoneName
			};
			parent.Bones.Add(bone);

			//update the anchor joint
			bone.SetAnchorJoint(parent);

			//Add the bone to each animation
			foreach (var animation in AnimationContainer.Animations)
			{
				//find the parent keybone
				var parentKeyBone = animation.GetKeyBone(ParentBoneName);
				if (null == parentKeyBone)
				{
					throw new Exception($"Couldn't find parentKeyBone with name {ParentBoneName}");
				}

				//create the new keybone
				var keybone = new KeyBone(bone);

				//add to the parent keybone
				parentKeyBone.Bones.Add(keybone);
			}

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
