using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElementRagdoll : BaseSetKeyElement, ICommand
	{
		#region Methods

		public SetKeyElementRagdoll(Animation animation, Bone bone, int time, bool ragdoll) : base(animation)
		{
			PopulateKeyElements(bone, time);

			NextKeyElement.KeyFrame = true;
			NextKeyElement.Ragdoll = ragdoll;
		}

		#endregion //Methods
	}
}
