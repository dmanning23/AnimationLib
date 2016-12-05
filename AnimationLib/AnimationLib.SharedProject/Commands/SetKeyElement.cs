using System.Diagnostics;
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
			Debug.Assert(null != animation);
			Debug.Assert(null != oldElement);
			Debug.Assert(null != newElement);
			Animation = animation;
			PrevKeyElement = new KeyElement();
			PrevKeyElement.Copy(oldElement);
			PrevKeyElement.JointName = oldElement.JointName;
			NextKeyElement = new KeyElement();
			NextKeyElement.Copy(newElement);
			NextKeyElement.JointName = newElement.JointName;
		}

		#endregion //Methods
	}
}