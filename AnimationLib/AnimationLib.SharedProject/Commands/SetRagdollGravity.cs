using Microsoft.Xna.Framework;
using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class SetRagdollGravity : IStackableCommand
	{
		#region Fields

		/// <summary>
		/// The previous joint data
		/// </summary>
		Image _data;

		/// <summary>
		/// The data that is replacing the old stuff
		/// </summary>
		float _next;

		float _prev;

		#endregion //Fields

		#region Methods

		public SetRagdollGravity(Image data, float nextSpring)
		{
			if (data == null)
			{
				throw new ArgumentNullException("bone");
			}

			_data = data;
			_next = nextSpring;
			_prev = _data.RagdollGravity.Y;
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			_data.RagdollGravity = new Vector2(0f, _next);
			return true;
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			_data.RagdollGravity = new Vector2(0f, _prev);
			return true;
		}

		public bool CompareWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollGravity;
			return ((next != null) &&
				(_data == next._data));
		}

		public void StackWithNextCommand(IStackableCommand nextCommand)
		{
			var next = nextCommand as SetRagdollGravity;
			if (next != null)
			{
				_next = next._next;
			}
		}


		#endregion //Methods
	}
}
