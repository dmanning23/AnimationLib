using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// this object moves the location of a joint
	/// </summary>
	public class SetJointCoords : IStackableCommand
	{
		#region Fields

		Bone Bone;

		/// <summary>
		/// teh image that has that the joint data to set
		/// </summary>
		Image Image;

		/// <summary>
		/// the index of the joint data to set
		/// </summary>
		int JointIndex;

		/// <summary>
		/// The previous joint data
		/// </summary>
		JointData PrevData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		JointData NextData;

		#endregion //Fields

		#region Methods

		public SetJointCoords(Bone bone, Image image, int jointIndex, JointData nextData)
		{
			if (bone == null)
			{
				throw new ArgumentNullException("bone");
			}

			if (image == null)
			{
				throw new ArgumentNullException("image");
			}

			if (0 > jointIndex)
			{
				throw new Exception("jointIndex must be >= 0");
			}

			if (image.JointCoords.Count <= jointIndex)
			{
				throw new Exception("jointIndex must be < image.JointCoords.Count");
			}

			Bone = bone;
			Image = image;
			JointIndex = jointIndex;
			NextData = nextData;
			PrevData = Image.GetJointLocation(JointIndex);
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			Image.SetJointCoords(JointIndex, NextData);
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			Image.SetJointCoords(JointIndex, PrevData);
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetJointCoords;
			return ((next != null) &&
				(Bone.Name == next.Bone.Name) &&
				(Image.ImageFile.File == next.Image.ImageFile.File) &&
				(JointIndex == next.JointIndex));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetJointCoords;
			if (next != null)
			{
				NextData = next.NextData;
			}
		}

		#endregion //Methods
	}
}