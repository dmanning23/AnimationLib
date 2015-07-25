using CollisionBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;

namespace AnimationLib
{
	public class PhysicsLine : Line
	{
		#region Properties

		/// <summary>
		/// the start of the line, local to the bone
		/// </summary>
		public Vector2 LocalStart { get; set; }

		/// <summary>
		/// the end of the line, local to the bone
		/// </summary>
		public Vector2 LocalEnd { get; set; }

		#endregion //Members

		#region Methods

		public PhysicsLine()
		{
			LocalStart = Vector2.Zero;
			LocalEnd = Vector2.Zero;
		}

		public void Update(Image owner, Vector2 bonePosition, float rotation, bool isFlipped, float scale)
		{
			//set to local coord
			Vector2 worldStart = LocalStart * scale;
			Vector2 worldEnd = LocalEnd * scale;

			//is it flipped?
			if (isFlipped)
			{
				//flip from the edge of the image
				worldStart.X = (owner.Width * scale) - worldStart.X;
				worldEnd.X = (owner.Width * scale) - worldEnd.X;
			}

			//rotate correctly
			Matrix myRotation = MatrixExt.Orientation(rotation);
			worldStart = myRotation.Multiply(worldStart);
			worldEnd = myRotation.Multiply(worldEnd);

			//move to the correct position
			worldStart = bonePosition + worldStart;
			worldEnd = bonePosition + worldEnd;

			//Set the start and end points that are stored in the line object
			Start = worldStart;
			End = worldEnd;
		}

		public void Render(IRenderer renderer, Color color)
		{
			renderer.Primitive.Line(Start, End, color);
		}

		/// <summary>
		/// Copy another line's data into this one
		/// </summary>
		/// <param name="inst">The line to copy</param>
		public void Copy(PhysicsLine inst)
		{
			LocalStart = inst.LocalStart;
			LocalEnd = inst.LocalEnd;
			_Start = inst._Start;
			_End = inst.End;
			OldStart = inst.OldStart;
			OldEnd = inst.OldEnd;
			Length = inst.Length;
			Direction = inst.Direction;
		}

		/// <summary>
		/// Redo the whole scale of this model
		/// </summary>
		/// <param name="scale"></param>
		public void Rescale(float scale)
		{
			LocalStart = LocalStart * scale;
			LocalEnd = LocalEnd * scale;
		}

		#endregion //Methods
	}
}