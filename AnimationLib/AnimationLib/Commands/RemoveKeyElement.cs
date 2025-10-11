using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class RemoveKeyElement : ICommand
	{
		#region Members

		//the animation the element is in
		private Animation _animation;

		//the old element 
		private KeyElement _oldKeyElement;

		#endregion // Members

		#region Methods

		/// <summary>
		/// hello, standard constructor!  Set all the data for this action
		/// </summary>
		public RemoveKeyElement(Animation animation, KeyElement oldElement)
		{
			_animation = animation;
			_oldKeyElement = new KeyElement();
			_oldKeyElement.Copy(oldElement);
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			return _animation.RemoveKeyframe(_oldKeyElement);
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			//replace the "new" key element with the "old" one
			return _animation.AddKeyframe(_oldKeyElement);
		}

		#endregion //Methods
	}
}