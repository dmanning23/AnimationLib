using Microsoft.Xna.Framework;
using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// this object sets teh data for a collision circle
	/// </summary>
	public class SetCirclePosition : IStackableCommand
	{
		#region Fields

		/// <summary>
		/// The previous circle data
		/// </summary>
		PhysicsCircle _circle;

		Vector2 _prevPos;
		Vector2 _nextPos;

		#endregion //Members

		#region Methods

		public SetCirclePosition(PhysicsCircle circle, Vector2 nextPos)
		{
			if (circle == null)
			{
				throw new ArgumentNullException("circle");
			}

			_circle = circle;
			_nextPos = nextPos;
			_prevPos = _circle.LocalPosition;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_circle.LocalPosition = _nextPos;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_circle.LocalPosition = _prevPos;
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetCirclePosition;
			return ((next != null) &&
				(_circle == next._circle));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetCirclePosition;
			if (next != null)
			{
				_nextPos = next._nextPos;
			}
		}

		#endregion //Methods
	}
}