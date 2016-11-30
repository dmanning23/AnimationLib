using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetRagdollRadius : IStackableCommand
    {
		#region Fields

		/// <summary>
		/// The previous joint data
		/// </summary>
		JointData _jointData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		float _nextRadius;

		float _prevRadius;

		#endregion //Fields

		#region Methods

		public SetRagdollRadius(JointData jointData, float nextRadius)
		{
			if (jointData == null)
			{
				throw new ArgumentNullException("bone");
			}

			_jointData = jointData;
			_nextRadius = nextRadius;
			_prevRadius = _jointData.FloatRadius;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_jointData.FloatRadius = _nextRadius;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_jointData.FloatRadius = _prevRadius;
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollRadius;
			return ((next != null) &&
				(_jointData == next._jointData));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollRadius;
			if (next != null)
			{
				_nextRadius = next._nextRadius;
			}
		}

		#endregion //Methods
	}
}
