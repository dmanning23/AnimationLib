using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElementRotation : BaseSetKeyElement, IStackableCommand
	{
		#region Methods

		public SetKeyElementRotation(Animation animation, Bone bone, int time, float rotation) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.Rotation = rotation;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementRotation;
			return ((next != null) &&
				(Animation.Name == next.Animation.Name) &&
				(PrevKeyElement.JointName == next.PrevKeyElement.JointName) &&
				(PrevKeyElement.Time == next.PrevKeyElement.Time));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementRotation;
			if (next != null)
			{
				NextKeyElement.Rotation = next.NextKeyElement.Rotation;
			}
		}

		#endregion //Methods
	}
}
