using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Xml;

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
		public int Index { get; private set; }

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

		#region Methods

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

			//simulate friction to add damping into the equation
			if (Data.Floating)
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
		public void SpringFloatingRagdoll(Joint joint, float springStrength, float desiredDistance, float scale)
		{
			if (Data.Floating)
			{
				//get the deisred float radius of this dude
				float fMyFloatRadius = Data.FloatRadius * scale;
				if (0.0f < fMyFloatRadius) //the float radius can't be 0 or negative
				{
					//find the current distance bewteen the two joints
					Vector2 deltaVector = Position - joint.Position; //swap this so it points from joint -> anchor
					float fCurDistance = deltaVector.Length();
					deltaVector /= fCurDistance; //normalize

					//float stretch = fCurDistance - (fMyFloatRadius * 0.2f);
					//Vector2 springForce = (deltaVector * stretch) * springStrength;

					//what is the ratio of the distance between the current position and desired position? 
					//0.0 = at same position
					//1.0 = fully extended
					float springRatio = fCurDistance / fMyFloatRadius;
					springRatio = Math.Min(Math.Max(0.0f, springRatio), 1.0f); //constrain the springratio

					//get the force, but point it back at the first joint
					Vector2 springForce = (deltaVector * springStrength) * springRatio;

					joint._acceleration += springForce;
				}
			}
		}

		/// <summary>
		/// rearrange the joints so they match the distance constraints
		/// </summary>
		/// <param name="joint">the joints to solve constraint with</param>
		/// <param name="desiredDistance">the distance from that joint in a perfect world</param>
		/// <param name="scale">the current scale of the model</param>
		/// <param name="movement">whether this joint should be moved</param>
		public void SolveConstraint(Joint joint, float desiredDistance, float scale, ERagdollMove movement)
		{
			//find the current distance bewteen the two joints
			Vector2 deltaVector = joint.Position - Position;
			float fCurDistance = deltaVector.Length();
			deltaVector /= fCurDistance; //normalize

			//Check if we are floating instead of rotating
			float fDiff = 0.0f;
			if (Data.Floating)
			{
				float fMyFloatRadius = Data.FloatRadius * scale;
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
				fDiff = (fCurDistance - (desiredDistance * scale));
			}

			if (0.0f != fDiff)
			{
				switch (movement)
				{
					case ERagdollMove.MoveAll:
					{
						//find the amount to move them by
						Vector2 halfVector = deltaVector*0.5f*fDiff;
						joint.Position = joint.Position - halfVector;
						_position = Position + halfVector;
					}
					break;
					case ERagdollMove.OnlyHim:
					{
						//only move the other dude
						Vector2 halfVector = deltaVector*fDiff;
						joint.Position = joint.Position - halfVector;
					}
					break;
					default:
					{
						//only move me
						Vector2 halfVector = deltaVector*fDiff;
						_position = Position + halfVector;
					}
					break;
				}
			}

			////meak sure the position stays in the board
			//if (m_Position.Y() < FLOOR)
			//{
			//    m_Position.Y(FLOOR);
			//}

			////meak sure the other dude's position stays in the board
			//if (rJoint.m_Position.Y() < FLOOR)
			//{
			//    rJoint.m_Position.Y(FLOOR);
			//}
		}

		#endregion //Ragdoll
	}
}