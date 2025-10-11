using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElementFlip : BaseSetKeyElement, ICommand
	{
		#region Methods

		public SetKeyElementFlip(Animation animation, Bone bone, int time, bool flip) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.Flip = flip;
		}

		#endregion //Methods
	}
}
