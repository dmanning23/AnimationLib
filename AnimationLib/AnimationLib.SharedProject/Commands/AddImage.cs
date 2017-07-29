using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddImage : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string ParentBoneName { get; set; }

		#endregion //Properties

		#region Methods

		public AddImage(AnimationContainer animationContainer, string parentBoneName)
		{
			AnimationContainer = animationContainer;
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

			//create the image
			var image = new Image();

			//set the bone & joint data
			image.SetAnchorJoint(parent);

			//add to the bone
			parent.Images.Add(image);

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
