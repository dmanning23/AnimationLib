using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElement : BaseSetKeyElement, ICommand
	{
		#region Methods

		/// <summary>
		/// hello, standard constructor!  Set all the data for this action
		/// </summary>
		public SetKeyElement(Animation animation, 
			KeyElement oldElement, 
			KeyElement newElement) : base(animation)
		{
			Animation = animation;
			PrevKeyElement = new KeyElement(oldElement);
			NextKeyElement = new KeyElement(newElement);
		}

		#endregion //Methods
	}
}