using Microsoft.Xna.Framework;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// This object moves the anchor location of a bone
	/// </summary>
	public class SetAnchorLocation : ICommand
	{
		#region Members

		Image m_Image;

		Vector2 m_Old;

		Vector2 m_New;

		#endregion //Members

		#region Methods

		public SetAnchorLocation(Image myImage, int iNewX, int iNewY)
		{
			m_Image = myImage;
			m_Old = myImage.AnchorCoord;
			m_New =  new Vector2((float)iNewX, (float)iNewY);
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			if (null != m_Image)
			{
				//set the anchor location of that image
				m_Image.AnchorCoord = m_New;
			}

			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			if (null != m_Image)
			{
				//set the anchor location of that image
				m_Image.AnchorCoord = m_Old;
			}

			return true;
		}

		#endregion //Methods
	}
}