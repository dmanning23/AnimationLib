using CollisionBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;

namespace AnimationLib
{
	public class PhysicsCircle : Circle
	{
		#region Properties

		public Vector2 LocalPosition{ get; set; }

		public float LocalRadius{ get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public PhysicsCircle()
		{
			LocalPosition = Vector2.Zero;
		}

		public PhysicsCircle(PhysicsCircleModel circle)
			: this()
		{
			LocalPosition = circle.Center;
			LocalRadius = circle.Radius;
		}

		public void Reset(Vector2 position)
		{
			Pos = position;
			OldPos = position;
			Radius = Radius;
		}

		public void Update(Image owner, Vector2 bonePosition, float rotation, bool isFlipped)
		{
			//set to local coord
			Vector2 worldPosition = LocalPosition;

			//is it flipped?
			if (isFlipped && (null != owner))
			{
				//flip from the edge of the image
				worldPosition.X = (owner.Width) - worldPosition.X;
			}

			//rotate correctly
			worldPosition = MatrixExt.Orientation(rotation).Multiply(worldPosition);

			//move to the correct position
			worldPosition = bonePosition + worldPosition;

			Update(worldPosition);
		}

		/// <summary>
		/// update the position of this circle
		/// </summary>
		/// <param name="myPosition">the location of this dude in the world</param>
		/// <param name="scale">how much to scale the physics item</param>
		public void Update(Vector2 myPosition)
		{
			Pos = myPosition;

			//set teh world radius
			Radius = LocalRadius;
		}

		public void Render(IRenderer renderer, Color color)
		{
			renderer.Primitive.Circle(Pos, Radius, color);
		}

		/// <summary>
		/// Copy another circle's data into this one
		/// </summary>
		/// <param name="inst">The circle to copy</param>
		public void Copy(PhysicsCircle inst)
		{
			LocalPosition = inst.LocalPosition;
			LocalRadius = inst.LocalRadius;
			_position = inst._position;
			OldPos = inst.OldPos;
			Radius = inst.Radius;
		}

		/// <summary>
		/// Redo the whole scale of this model
		/// </summary>
		/// <param name="scale"></param>
		public void Rescale(float scale)
		{
			LocalPosition = LocalPosition * scale;
			LocalRadius *= scale;
		}

		#endregion //Methods
	}
}