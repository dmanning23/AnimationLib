using UndoRedoBuddy;

namespace AnimationLib.Commands
{
    public class SetKeyElementLayer : BaseSetKeyElement, IStackableCommand
	{
		#region Methods

		public SetKeyElementLayer(Animation animation, Bone bone, int time, int layer) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.Layer = layer;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementLayer;
			return ((next != null) &&
				(Animation.Name == next.Animation.Name) &&
				(PrevKeyElement.BoneName == next.PrevKeyElement.BoneName) &&
				(PrevKeyElement.Time == next.PrevKeyElement.Time) &&
				(PrevKeyElement.Layer == next.PrevKeyElement.Layer));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetKeyElementLayer;
			if (next != null)
			{
				NextKeyElement.Layer = next.NextKeyElement.Layer;
			}
		}

		#endregion //Methods
	}
}
