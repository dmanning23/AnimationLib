using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class RemoveKeyElement : ICommand
	{
		#region Members

		//the animation the element is in
		private Animation m_Animation;

		//the old element 
		private KeyElement m_OldKeyElement;

		#endregion // Members

		#region Methods

		/// <summary>
		/// hello, standard constructor!  Set all the data for this action
		/// </summary>
		public RemoveKeyElement(Animation myAnimation, KeyElement OldElement)
		{
			Debug.Assert(null != myAnimation);
			Debug.Assert(null != OldElement);
			m_Animation = myAnimation;
			m_OldKeyElement = new KeyElement();
			m_OldKeyElement.Copy(OldElement);
			m_OldKeyElement.JointName = OldElement.JointName;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			return m_Animation.RemoveKeyframe(m_OldKeyElement);
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			//replace the "new" key element with the "old" one
			return m_Animation.AddKeyframe(m_OldKeyElement);
		}

		#endregion //Methods

	}
}