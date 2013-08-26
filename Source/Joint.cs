using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;

namespace AnimationLib
{
	public class Joint
	{
		#region Member Variables


		private string m_strJointName;

		/// <summary>
		/// The position for this dude
		/// </summary>
		private Vector2 m_JointPosition;

		/// <summary>
		/// The index of this joint in the bone who owns it
		/// </summary>
		private int m_iIndex;

		/// <summary>
		/// this is this dudes old position, stored in world coordinates
		/// </summary>
		private Vector2 m_OldPosition;

		/// <summary>
		/// this is the acceleration currently on this dude
		/// </summary>
		private Vector2 m_Acceleration;

		/// <summary>
		/// whether or not the bone that owns this joint is currently flipped
		/// used for ragdoll limit calculation
		/// </summary>
		bool m_bParentFlip;

		#endregion

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
			get { return m_JointPosition; }
			set { m_JointPosition = value; }
		}

		public Vector2 OldPosition
		{
			get { return m_OldPosition; }
			set { m_OldPosition = value; }
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
			get { return m_Acceleration; }
			set { m_Acceleration = value; }
		}

		public bool ParentFlip
		{
			get { return m_bParentFlip; }
			set { m_bParentFlip = value; }
		}

		public float FirstLimit
		{
			get 
			{
				if (m_bParentFlip)
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
				if (m_bParentFlip)
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
		public Joint(int iIndex)
		{
			m_iIndex = iIndex;
			m_JointPosition = new Vector2(0.0f);
			CurrentKeyElement = new KeyElement();
			Data = new JointData();
			m_OldPosition = new Vector2(0.0f);
			m_Acceleration = new Vector2(0.0f);
			m_strJointName = "";
			m_bParentFlip = false;
		}

		/// <summary>
		/// Update the joints stuff
		/// </summary>
		/// <param name="MyKeyJoint">the key joint for this dude</param>
		/// <param name="iTime">The current time of the animation</param>
		public void Update(KeyJoint MyKeyJoint, int iTime)
		{
			//get the key element for this dude
			MyKeyJoint.GetKeyElement(iTime, CurrentKeyElement);
		}

		public void RestartAnimation()
		{
			m_OldPosition = m_JointPosition;
		}

		#endregion

		#region Ragdoll

		public void RunVerlet(float fTimeDelta)
		{
			Vector2 tempCurrentPosition = m_JointPosition;
			m_JointPosition += ((tempCurrentPosition - m_OldPosition) + (m_Acceleration * (fTimeDelta * fTimeDelta)));
			m_OldPosition = tempCurrentPosition;

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
		/// rearrange the joints so they match the distance constraints
		/// </summary>
		/// <param name="rJoint">the joints to solve constraint with</param>
		/// <param name="fDesiredDistance">the distance from that joint in a perfect world</param>
		/// <param name="bMoveMe">whether this joint should be moved</param>
		public void SolveConstraint(Joint rJoint, float fDesiredDistance, float fScale, ERagdollMove bMovement)
		{
			//find the current distance bewteen the two joints
			Vector2 deltaVector = rJoint.Position - Position;
			float fCurDistance = deltaVector.Length();
			deltaVector /= fCurDistance; //normalize

			//Check if we are floating instead of rotating
			float fDiff = 0.0f;
			if (Data.Floating)
			{
				float fMyFloatRadius = Data.FloatRadius * fScale;
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
				fDiff = (fCurDistance - (fDesiredDistance * fScale));
			}

			if (0.0f != fDiff)
			{
				if (bMovement == ERagdollMove.MoveAll)
				{
					//find the amount to move them by
					Vector2 halfVector = deltaVector * 0.5f * fDiff;
					rJoint.Position = rJoint.Position - halfVector;
					m_JointPosition = Position + halfVector;
				}
				else if (bMovement == ERagdollMove.OnlyHim)
				{
					//only move the other dude
					Vector2 halfVector = deltaVector * fDiff;
					rJoint.Position = rJoint.Position - halfVector;
				}
				else
				{
					//only move me
					Vector2 halfVector = deltaVector * fDiff;
					m_JointPosition = Position + halfVector;
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

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadSerializedFormat(XmlNode rXMLNode)
		{
			if ("Item" != rXMLNode.Name)
			{
				return false;
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;
				if ("Type" == strName)
				{
					if ("AnimationLib.JointXML" != strValue)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			//Read in child nodes
			if (rXMLNode.HasChildNodes)
			{
				for (XmlNode childNode = rXMLNode.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					//what is in this node?
					string strName = childNode.Name;
					string strValue = childNode.InnerText;

					if (strName == "name")
					{
						//set the name of this joint
						m_strJointName = strValue;
					}
					else
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="rXMLFile">the xml file to add this dude as a child of</param>
		public void WriteXMLFormat(XmlTextWriter rXMLFile, float fEnbiggify)
		{
			//write out the item tag
			rXMLFile.WriteStartElement("Item");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.JointXML");

			//write out upper left coords
			rXMLFile.WriteStartElement("name");
			rXMLFile.WriteString(m_strJointName);
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// Read in all the bone information from an object read in from a serialized XML file.
		/// </summary>
		/// <param name="rJoint">the xml object to get data from</param>
		public bool ReadSerializedFormat(AnimationLib.JointXML rJoint)
		{
			m_strJointName = rJoint.name;
			return true;
		}

		#endregion //File IO
	}
}