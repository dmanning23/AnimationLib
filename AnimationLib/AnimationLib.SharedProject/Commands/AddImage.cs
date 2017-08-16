using System;
using System.Linq;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddImage : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string ParentBoneName { get; set; }

		string ImageName { get; set; }

		#endregion //Properties

		#region Methods

		public AddImage(AnimationContainer animationContainer, string parentBoneName, string imageName)
		{
			AnimationContainer = animationContainer;
			ParentBoneName = parentBoneName;
			ImageName = imageName;
		}

		public bool Execute()
		{
			return ExecuteAddImage(AnimationContainer, ParentBoneName, ImageName);
		}

		private static bool ExecuteAddImage(AnimationContainer animationContainer, string parentBoneName, string imageName)
		{
			//Find the parent bone
			var parent = animationContainer.Skeleton.GetBone(parentBoneName);
			if (parent == null)
			{
				throw new Exception($"Couldn't find parent bone with name {parentBoneName}");
			}

			//create the image
			var image = new Image()
			{
				Name = imageName
			};

			//set the bone & joint data
			image.SetAnchorJoint(parent);

			//add to the bone
			parent.Images.Add(image);

			//if there are any garment bones attached to the parent, add images to them as well
			var garmentBones = parent.Bones.Where(x => x.BoneType == EBoneType.Garment);
			foreach (var bone in garmentBones)
			{
				var garmentBone = bone as GarmentBone;
				AddAnimation.ExecuteAddAnimation(garmentBone.GarmentAnimationContainer, imageName, 0);
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
