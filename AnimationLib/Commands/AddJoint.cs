using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddJoint : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string JointName { get; set; }

		Bone ParentBone { get; set; }

		bool InsertBeginning { get; set; }

		#endregion //Properties

		#region Methods

		public AddJoint(AnimationContainer animationContainer, string jointName, Bone bone, bool insertBeginning)
		{
			AnimationContainer = animationContainer;
			JointName = jointName;
			ParentBone = bone;
			InsertBeginning = insertBeginning;
		}

		public bool Execute()
		{
			//make sure it is a unique bone & joint name
			var joint = AnimationContainer.Skeleton.GetJoint(JointName);
			if (null != joint)
			{
				throw new Exception($"Couldn't add joint with duplicate name {JointName}");
			}

			//add a matching joint to the parent
			ParentBone.AddJoint(JointName, InsertBeginning);

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
