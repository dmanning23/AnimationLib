using MatrixExtensions;
using Microsoft.Xna.Framework;
using System;

namespace AnimationLib
{
	public class Joint
	{
		#region Fields

		/// <summary>
		/// The position for this dude
		/// </summary>
		private Vector2 _position;

		/// <summary>
		/// this is this dudes old position, stored in world coordinates
		/// </summary>
		private Vector2 _oldPosition;

		/// <summary>
		/// this is the acceleration currently on this dude
		/// </summary>
		private Vector2 _acceleration;

		#endregion //Fields

		#region Properties

		/// <summary>
		/// The name of this joint
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The index of this joint in the bone who owns it
		/// </summary>
		public int Index { get; set; }

		/// <summary>
		/// Get or set the m_JointMatrix property
		/// </summary>
		public Vector2 Position
		{
			get
			{
				return _position;
			}
			set
			{
				_position = value;
			}
		}

		public Vector2 OldPosition
		{
			get
			{
				return _oldPosition;
			}
			set
			{
				_oldPosition = value;
			}
		}

		/// <summary>
		/// The current keyelement
		/// </summary>
		public KeyElement CurrentKeyElement { get; set; }

		/// <summary>
		/// the current joint data this dude is using
		/// </summary>
		public JointData Data { get; set; }

		public Vector2 Acceleration
		{
			get { return _acceleration; }
			set { _acceleration = value; }
		}

		/// <summary>
		/// whether or not the bone that owns this joint is currently flipped
		/// used for ragdoll limit calculation
		/// </summary>
		public bool ParentFlip { get; set; }

		public float FirstLimit
		{
			get
			{
				if (ParentFlip)
				{
					return Data.FirstLimitFlipped;
				}
				else
				{
					return Data.FirstLimit;
				}
			}
		}

		public float SecondLimit
		{
			get
			{
				if (ParentFlip)
				{
					return Data.SecondLimitFlipped;
				}
				else
				{
					return Data.SecondLimit;
				}
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Joint(int index)
		{
			Index = index;
			_position = Vector2.Zero;
			CurrentKeyElement = new KeyElement();
			Data = new JointData();
			_oldPosition = Vector2.Zero;
			_acceleration = Vector2.Zero;
			Name = "";
			ParentFlip = false;
		}

		public Joint(JointModel joint, int index)
			: this(index)
		{
			Name = joint.Name;
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Update the joints stuff
		/// </summary>
		/// <param name="myKeyJoint">the key joint for this dude</param>
		/// <param name="time">The current time of the animation</param>
		public void Update(KeyJoint myKeyJoint, int time)
		{
			//get the key element for this dude
			myKeyJoint.GetKeyElement(time, CurrentKeyElement);
		}

		public void RestartAnimation()
		{
			_oldPosition = _position;
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods

		#region Ragdoll

		public void RunVerlet(float timeDelta)
		{
			//Get the velocity we are going to apply
			Vector2 vel = ((_position - _oldPosition) + (_acceleration * (timeDelta * timeDelta)));

			//TODO: does this need to be different for each one?

			//simulate friction to add damping into the equation
			if (RagdollType.Float == Data.RagdollType)
			{
				vel *= .9f;
			}
			else
			{
				vel *= .98f;
			}

			Vector2 tempCurrentPosition = _position;
			_position += vel;
			_oldPosition = tempCurrentPosition;

			////meak sure the position stays in the board
			//if (m_Position.X() < LEFT_WALL)
			//{
			//    m_Position.X(LEFT_WALL);
			//}
			//else if (m_Position.X() > RIGHT_WALL)
			//{
			//    m_Position.X(RIGHT_WALL);
			//}
			//if (m_Position.Y() < FLOOR)
			//{
			//    m_Position.Y(FLOOR);
			//}
		}

		/// <summary>
		/// If a joint is floating ragdoll, bounce it back toward the anchor joint.
		/// Called from the anchor joint to spring child joints.
		/// </summary>
		/// <param name="joint">the child joint to bounce</param>
		/// <param name="springStrength"></param>
		/// <param name="desiredDistance"></param>
		/// <param name="scale"></param>
		public void SolveRagdollSpring(float parentRotation, Bone bone, Joint joint, float springStrength)
		{
			switch (Data.RagdollType)
			{
				case RagdollType.Float:
					{
						//get the deisred float radius of this dude
						float desiredDistance = Data.FloatRadius;
						if (0.0f < desiredDistance) //the float radius can't be 0 or negative
						{
							//find the current distance bewteen the two joints
							Vector2 deltaVector = Position - joint.Position; //swap this so it points from joint -> anchor
							float currentDistance = deltaVector.Length();
							deltaVector /= currentDistance; //normalize

							//what is the ratio of the distance between the current position and desired position? 
							//0.0 = at same position
							//1.0 = fully extended
							float springRatio = currentDistance / desiredDistance;
							springRatio = Math.Min(Math.Max(0.0f, springRatio), 1.0f); //constrain the springratio

							//get the total force to apply to the child joint
							Vector2 springForce = (deltaVector * springStrength) * springRatio;
							joint._acceleration += springForce;
						}
					}
					break;
				case RagdollType.Limit:
					{
						//GetLimitSpring1(parentRotation, bone, joint, springStrength);
						GetLimitSpring2(parentRotation, bone, joint, springStrength);
					}
					break;
			}
		}

		private void GetLimitSpring1(float parentRotation, Bone bone, Joint joint, float springStrength)
		{
			//get the current angle of the bone
			float firstLimit, secondLimit;
			bone.GetRotatedLimits(parentRotation, out firstLimit, out secondLimit);
			var currentAngle = bone.GetRagDollRotation();

			//what is the ratio of the current angle between the current position and desired position?
			var desiredAngle = (firstLimit + secondLimit) / 2f;
			float springRatio, springAngle;

			//get the direction to point the spring
			if (currentAngle < desiredAngle)
			{
				//if > desired angle, get the unit vector pointing +90 degrees
				springAngle = currentAngle + MathHelper.PiOver2;
				springRatio = ((currentAngle - desiredAngle) / (firstLimit - desiredAngle));
			}
			else if (currentAngle > desiredAngle)
			{
				//else is < desired angle, get the unit vector pointing -90 degrees
				springAngle = currentAngle - MathHelper.PiOver2;
				springRatio = ((currentAngle - desiredAngle) / (secondLimit - desiredAngle));
			}
			else
			{
				springAngle = 0f;
				springRatio = 0f;
			}

			Vector2 deltaVector = AngleToUnitVector(springAngle);

			//get the total force to apply to the child joint
			Vector2 springForce = (deltaVector * springStrength) * springRatio;
			joint._acceleration += springForce;
		}

		private void GetLimitSpring2(float parentRotation, Bone bone, Joint joint, float springStrength)
		{
			//Get the current rotation
			var rotation = bone.GetRagDollRotation();
			var actualRotation = Helper.ClampAngle(rotation - parentRotation);
			float firstLimit, secondLimit;
			bone.GetLimits(out firstLimit, out secondLimit);

			var desiredAngle = (firstLimit + secondLimit) / 2f;

			//get the direction to point the spring
			float springRatio, springAngle;
			if (actualRotation < desiredAngle)
			{
				//if > desired angle, get the unit vector pointing +90 degrees
				springAngle = rotation + MathHelper.PiOver2;
				springRatio = ((actualRotation - desiredAngle) / (firstLimit - desiredAngle));
			}
			else if (actualRotation > desiredAngle)
			{
				//else is < desired angle, get the unit vector pointing -90 degrees
				springAngle = rotation - MathHelper.PiOver2;
				springRatio = ((actualRotation - desiredAngle) / (secondLimit - desiredAngle));
			}
			else
			{
				springAngle = 0f;
				springRatio = 0f;
			}

			Vector2 deltaVector = AngleToUnitVector(springAngle);

			//get the total force to apply to the child joint
			Vector2 springForce = (deltaVector * springStrength) * springRatio;
			joint._acceleration += springForce;

			var TESTrotation = MathHelper.ToDegrees(rotation);
			var TESTactualRotation = MathHelper.ToDegrees(actualRotation);
			var TESTspringAngle = MathHelper.ToDegrees(springAngle);
		}

		public static Vector2 AngleToUnitVector(float springAngle)
		{
			return new Vector2((float)Math.Cos(springAngle), (float)Math.Sin(springAngle));
		}

		/// <summary>
		/// rearrange the joints so they match the distance constraints
		/// </summary>
		/// <param name="joint">the joints to solve constraint with</param>
		/// <param name="desiredDistance">the distance from that joint in a perfect world</param>
		/// <param name="scale">the current scale of the model</param>
		/// <param name="movement">whether this joint should be moved</param>
		/// <param name="weightRatio">if movement = is moveall, this is how to ratio the weight between this guy and the other</param>
		public void SolveConstraint(Joint joint, float desiredDistance, ERagdollMove movement, float weightRatio)
		{
			//find the current distance bewteen the two joints
			Vector2 deltaVector = joint.Position - Position;
			float fCurDistance = deltaVector.Length();
			deltaVector /= fCurDistance; //normalize

			//Check if we are floating instead of rotating
			float fDiff = 0.0f;
			if (RagdollType.Float == Data.RagdollType)
			{
				float fMyFloatRadius = Data.FloatRadius;
				if (fCurDistance < fMyFloatRadius)
				{
					//The distance is less that the amount of float, dont bother constraining
					return;
				}
				else
				{
					fDiff = (fCurDistance - fMyFloatRadius);
				}
			}
			else
			{
				//find the diff between the two
				fDiff = (fCurDistance - desiredDistance);
			}

			if (0.0f != fDiff)
			{
				switch (movement)
				{
					case ERagdollMove.MoveAll:
						{
							//Find the amount to move this guy by
							var halfVector = deltaVector * (1f - weightRatio) * fDiff;
							_position = Position + halfVector;

							//find the amount to move the other guy by
							halfVector = deltaVector * weightRatio * fDiff;
							joint.Position = joint.Position - halfVector;
						}
						break;
					case ERagdollMove.OnlyHim:
						{
							//only move the other dude
							Vector2 halfVector = deltaVector * fDiff;
							joint.Position = joint.Position - halfVector;
						}
						break;
					default:
						{
							//only move me
							Vector2 halfVector = deltaVector * fDiff;
							_position = Position + halfVector;
						}
						break;
				}
			}
		}

		#endregion //Ragdoll
	}
}