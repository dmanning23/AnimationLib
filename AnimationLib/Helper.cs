using Microsoft.Xna.Framework;
using System;

namespace AnimationLib
{
	public static class Helper
	{
		/// <summary>
		/// This is the fastest speed for the player in the +y direction while falling
		/// </summary>
		/// <returns></returns>
		public static float MaxFallingSpeed()
		{
			return 2300.0f;
		}

		public static float atan2(Vector2 vect)
		{
			return atan2(vect.X, vect.Y);
		}

		public static float atan2(float fX, float fY)
		{
			fY *= -1.0f; //flip on y axis for coordinate system

			//get the angle to that vector
			var fAngle = (float)Math.Atan2(fY, fX);

			return fAngle;
		}

		public static float ClampAngle(float fAngle)
		{
			//keep the angle between -180 and 180
			while (-MathHelper.Pi > fAngle)
			{
				fAngle += MathHelper.TwoPi;
			}

			while (MathHelper.Pi < fAngle)
			{
				fAngle -= MathHelper.TwoPi;
			}

			return fAngle;
		}

		public static float Length(float fX, float fY)
		{
			var myVect = new Vector2(fX, fY);
			return myVect.Length();
		}
	}
}