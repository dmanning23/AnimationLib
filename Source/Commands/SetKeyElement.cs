using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetKeyElement : ICommand
	{
		#region Members

		//the animation the element is in
		private Animation m_Animation;

		//the old element 
		private KeyElement m_OldKeyElement;

		//the current element to replace it
		private KeyElement m_NewKeyElement;

		#endregion // Members

		#region Methods

		/// <summary>
		/// hello, standard constructor!  Set all the data for this action
		/// </summary>
		public SetKeyElement(Animation myAnimation, 
			KeyElement OldElement, 
			KeyElement NewElement)
		{
			Debug.Assert(null != myAnimation);
			Debug.Assert(null != OldElement);
			Debug.Assert(null != NewElement);
			m_Animation = myAnimation;
			m_OldKeyElement = new KeyElement();
			m_OldKeyElement.Copy(OldElement);
			m_OldKeyElement.JointName = OldElement.JointName;
			m_NewKeyElement = new KeyElement();
			m_NewKeyElement.Copy(NewElement);
			m_NewKeyElement.JointName = NewElement.JointName;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			return ReplaceKeyElement(m_OldKeyElement, m_NewKeyElement);
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			//replace the "new" key element with the "old" one
			return ReplaceKeyElement(m_NewKeyElement, m_OldKeyElement);
		}

		private bool ReplaceKeyElement(KeyElement replacedKey, KeyElement replacementKey)
		{
			Debug.Assert(null != replacedKey);
			Debug.Assert(null != replacementKey);
			Debug.Assert(null != m_Animation);

			if (replacedKey.JointName != replacementKey.JointName)
			{
				return false;
			}

			//find the keyjoint we want 
			KeyJoint myKeyJoint = m_Animation.KeyBone.GetKeyJoint(replacementKey.JointName);
			if (null == myKeyJoint)
			{
				return false;
			}

			//set the keyelement
			if (replacementKey.KeyFrame)
			{
				myKeyJoint.AddKeyElement(replacementKey);
			}
			else
			{
				//just remove the 'replaced' keyframe
				myKeyJoint.RemoveKeyElement(replacedKey.Time);
			}

			return true;
		}

		#endregion //Methods
	}
}