using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	/// <summary>
	/// this object sets teh data for a collision circle
	/// </summary>
	public class SetCircleRadius : IStackableCommand
	{
		#region Fields

		/// <summary>
		/// The previous circle data
		/// </summary>
		PhysicsCircle _circle;

		float _prevRadius;
		float _nextRadius;

		#endregion //Members

		#region Methods

		public SetCircleRadius(PhysicsCircle circle, float nextRadius)
		{
			if (circle == null)
			{
				throw new ArgumentNullException("circle");
			}

			if (0 > nextRadius)
			{
				throw new Exception("nextRadius must be >= 0");
			}
			
			_circle = circle;
			_nextRadius = nextRadius;
			_prevRadius = _circle.LocalRadius;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_circle.LocalRadius = _nextRadius;
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_circle.LocalRadius = _prevRadius;
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetCircleRadius;
			return ((next != null) &&
				(_circle == next._circle));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetCircleRadius;
			if (next != null)
			{
				_nextRadius = next._nextRadius;
			}
		}

		#endregion //Methods
	}
}