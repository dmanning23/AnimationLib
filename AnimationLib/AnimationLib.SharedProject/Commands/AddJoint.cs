using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddJoint : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string JointName { get; set; }

		string ParentBoneName { get; set; }

		bool InsertBeginning { get; set; }

		#endregion //Properties

		#region Methods

		public AddJoint(AnimationContainer animationContainer, string jointName, string parentBoneName, bool insertBeginning)
		{
			AnimationContainer = animationContainer;
			JointName = jointName;
			ParentBoneName = parentBoneName;
			InsertBeginning = insertBeginning;
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
			var joint = AnimationContainer.Skeleton.GetJoint(JointName);
			if (null != joint)
			{
				throw new Exception($"Couldn't add joint with duplicate name {JointName}");
			}

			//add a matching joint to the parent
			parent.AddJoint(JointName, InsertBeginning);

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
