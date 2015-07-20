using Microsoft.Xna.Framework;

namespace AnimationLib
{
	public static class RagdollConstants
	{
		#region Ragdoll Constants

		/// <summary>
		/// Amount of gravity to apply to ragdoll physics in this game
		/// gravity is positive in this game since y axis is flipped
		/// </summary>
		private static Vector2 _gravity = new Vector2(0.0f, 9.8f * 150.0f);

		public static Vector2 Gravity
		{
			get
			{
				return _gravity;
			}
			set 
			{
				_gravity = value; 
			}
		}

		public static void SetGravity(float grav)
		{
			_gravity.Y = grav;
			_spring = new Vector2(0.0f, Gravity.Y * 1.5f);
		}

		/// <summary>
		/// Amount to spring floating ragdoll back to center.
		/// </summary>
		private static Vector2 _spring = new Vector2(0.0f, Gravity.Y * 1.5f);

		public static float Spring
		{
			get
			{
				return _spring.Y;
			}
			set
			{
				_spring.Y = value;
			}
		}

		#endregion //Ragdoll Constants
	}
}