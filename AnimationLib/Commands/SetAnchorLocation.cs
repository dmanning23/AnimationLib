using Microsoft.Xna.Framework;
using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// This object moves the anchor location of a bone
	/// </summary>
	public class SetAnchorLocation : IStackableCommand
	{
		#region Fields

		Bone Bone; 

		Image Image;

		Vector2 PrevPosition { get; set; }

		Vector2 NextPosition { get; set; }

		#endregion //Fields

		#region Methods

		public SetAnchorLocation(Bone bone, Image image, Vector2 nextPosition)
		{
			if (bone == null)
			{
				throw new ArgumentNullException("bone");
			}

			if (image == null)
			{
				throw new ArgumentNullException("image");
			}

			Bone = bone;
			Image = image;
			PrevPosition = image.AnchorCoord;
			NextPosition = nextPosition;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			//set the anchor location of that image
			Image.AnchorCoord = NextPosition;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			//set the anchor location of that image
			Image.AnchorCoord = PrevPosition;
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetAnchorLocation;
			return ((next != null) &&
				(Bone.Name == next.Bone.Name) &&
				(Image.ImageFile.File == next.Image.ImageFile.File) &&
				(Image.Name == next.Image.Name));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetAnchorLocation;
			if (next != null)
			{
				NextPosition = next.NextPosition;
			}
		}

		#endregion //Methods
	}
}