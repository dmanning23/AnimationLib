using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetRagdollSpring : IStackableCommand
    {
		#region Fields

		/// <summary>
		/// The previous joint data
		/// </summary>
		Image _data;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		float _nextSpring;

		float _prevSpring;

		#endregion //Fields

		#region Methods

		public SetRagdollSpring(Image data, float nextSpring)
		{
			if (data == null)
			{
				throw new ArgumentNullException("bone");
			}

			_data = data;
			_nextSpring = nextSpring;
			_prevSpring = _data.RagdollSpring;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_data.RagdollSpring = _nextSpring;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_data.RagdollSpring = _prevSpring;
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollSpring;
			return ((next != null) &&
				(_data == next._data));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollSpring;
			if (next != null)
			{
				_nextSpring = next._nextSpring;
			}
		}


		#endregion //Methods
	}
}
