using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetLineData : ICommand
	{
		#region Members

		/// <summary>
		/// teh image that has that the circle to set
		/// </summary>
		Image m_Image;

		/// <summary>
		/// the index of the circle to set
		/// </summary>
		int m_iLineIndex;

		/// <summary>
		/// The previous circle data
		/// </summary>
		PhysicsLine m_OldData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		PhysicsLine m_NewData;

		#endregion //Members

		#region Methods

		public SetLineData(Image myImage, int iLineIndex, PhysicsLine myNewData)
		{
			m_Image = myImage;
			m_NewData = myNewData;
			m_iLineIndex = iLineIndex;
			m_OldData = myImage.Lines[m_iLineIndex];
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			m_Image.Lines[m_iLineIndex] = m_NewData;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			m_Image.Lines[m_iLineIndex] = m_OldData;
			return true;
		}

		#endregion //Methods
	}
}