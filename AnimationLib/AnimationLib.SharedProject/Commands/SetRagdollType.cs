using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetRagdollType : ICommand
    {
		#region Fields

		/// <summary>
		/// The previous joint data
		/// </summary>
		JointData _jointData;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		RagdollType _nextType;

		RagdollType _prevType;

		#endregion //Fields

		#region Methods

		public SetRagdollType(JointData jointData, RagdollType nextType)
		{
			if (jointData == null)
			{
				throw new ArgumentNullException("bone");
			}

			_jointData = jointData;
			_nextType = nextType;
			_prevType = _jointData.RagdollType;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_jointData.RagdollType = _nextType;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_jointData.RagdollType = _prevType;
			return true;
		}

		#endregion //Methods
	}
}
