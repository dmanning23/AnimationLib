using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetRagdollLimit : IStackableCommand
	{
		#region Fields

		/// <summary>
		/// The previous joint data
		/// </summary>
		JointData _jointData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		float _nextLimit;

		float _prevLimit;

		bool _limit1;

		#endregion //Fields

		#region Methods

		public SetRagdollLimit(JointData jointData, float nextLimit, bool limit1)
		{
			if (jointData == null)
			{
				throw new ArgumentNullException("bone");
			}

			_jointData = jointData;
			_nextLimit = nextLimit;
			_limit1 = limit1;
			_prevLimit = _limit1 ? _jointData.FirstLimit : _jointData.SecondLimit;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			if (_limit1)
			{
				_jointData.FirstLimit = _nextLimit;
			}
			else
			{
				_jointData.SecondLimit = _nextLimit;
			}

			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			if (_limit1)
			{
				_jointData.FirstLimit = _prevLimit;
			}
			else
			{
				_jointData.SecondLimit = _prevLimit;
			}
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollLimit;
			return ((next != null) &&
				(_jointData == next._jointData) &&
				(_limit1 == next._limit1));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollLimit;
			if (next != null)
			{
				_nextLimit = next._nextLimit;
			}
		}

		#endregion //Methods
	}
}
