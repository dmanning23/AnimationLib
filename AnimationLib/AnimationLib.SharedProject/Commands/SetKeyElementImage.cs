using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElementImage : BaseSetKeyElement, ICommand
	{
		#region Methods

		public SetKeyElementImage(Animation animation, Bone bone, int time, int image) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.ImageIndex = image;
		}

		#endregion //Methods
	}
}
