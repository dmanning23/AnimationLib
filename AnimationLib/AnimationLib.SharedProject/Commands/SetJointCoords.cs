using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// this object moves the location of a joint
	/// </summary>
	public class SetJointCoords : ICommand
	{
		#region Members

		/// <summary>
		/// teh image that has that the joint data to set
		/// </summary>
		Image m_Image;

		/// <summary>
		/// the index of the joint data to set
		/// </summary>
		int m_iJointIndex;

		/// <summary>
		/// The previous joint data
		/// </summary>
		JointData m_OldData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		JointData m_NewData;

		#endregion //Members

		#region Methods

		public SetJointCoords(Image myImage, int iJointIndex, JointData myNewData)
		{
			Debug.Assert(null != myImage);
			Debug.Assert(null != myNewData);
			Debug.Assert(iJointIndex >= 0);
			Debug.Assert(iJointIndex < myImage.JointCoords.Count);

			m_Image = myImage;
			m_NewData = myNewData;
			m_iJointIndex = iJointIndex;
			m_OldData = myImage.GetJointLocation(iJointIndex);
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			m_Image.SetJointCoords(m_iJointIndex, m_NewData);
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			m_Image.SetJointCoords(m_iJointIndex, m_OldData);
			return true;
		}

		#endregion //Methods
	}
}