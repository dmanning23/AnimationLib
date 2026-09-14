using Microsoft.Xna.Framework;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
    public class SetKeyElementTranslation : BaseSetKeyElement, IStackableCommand
	{
		#region Methods

		public SetKeyElementTranslation(Animation animation, Bone bone, int time, Vector2 translation) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.Translation = translation;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementTranslation;
			return ((next != null) &&
				(Animation.Name == next.Animation.Name) &&
				(PrevKeyElement.BoneName == next.PrevKeyElement.BoneName) &&
				(PrevKeyElement.Time == next.PrevKeyElement.Time));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementTranslation;
			if (next != null)
			{
				NextKeyElement.Translation = next.NextKeyElement.Translation;
			}
		}

		#endregion //Methods
	}
}
