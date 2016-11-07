using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// this object sets teh data for a collision circle
	/// </summary>
	public class SetCircleData : ICommand
	{
		#region Members

		/// <summary>
		/// teh image that has that the circle to set
		/// </summary>
		Image m_Image;

		/// <summary>
		/// the index of the circle to set
		/// </summary>
		int m_iCircleIndex;

		/// <summary>
		/// The previous circle data
		/// </summary>
		PhysicsCircle m_OldData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		PhysicsCircle m_NewData;

		#endregion //Members

		#region Methods

		public SetCircleData(Image myImage, int iCircleIndex, PhysicsCircle myNewData)
		{
			Debug.Assert(null != myImage);
			Debug.Assert(null != myNewData);
			Debug.Assert(iCircleIndex >= 0);
			Debug.Assert(iCircleIndex < myImage.Circles.Count);
			Debug.Assert(null != myImage.Circles[iCircleIndex]);

			m_Image = myImage;
			m_NewData = myNewData;
			m_iCircleIndex = iCircleIndex;
			m_OldData = myImage.Circles[iCircleIndex];
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			Debug.Assert(null != m_Image);
			Debug.Assert(null != m_NewData);
			Debug.Assert(0 <= m_iCircleIndex);
			Debug.Assert(m_iCircleIndex < m_Image.Circles.Count);

			m_Image.Circles[m_iCircleIndex] = m_NewData;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			Debug.Assert(null != m_Image);
			Debug.Assert(null != m_NewData);
			Debug.Assert(0 <= m_iCircleIndex);
			Debug.Assert(m_iCircleIndex < m_Image.Circles.Count);

			m_Image.Circles[m_iCircleIndex] = m_OldData;
			return true;
		}

		#endregion //Methods
	}
}