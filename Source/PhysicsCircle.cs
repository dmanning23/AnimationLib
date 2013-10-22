using System.Xml;
using Microsoft.Xna.Framework;
using CollisionBuddy;
using MatrixExtensions;
using DrawListBuddy;
using BasicPrimitiveBuddy;
using System;
using Vector2Extensions;
using RenderBuddy;
using System.Diagnostics;

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
			LocalPosition = new Vector2(0.0f);
			LocalRadius = 0.0f;
		}

		public void Reset(Vector2 Position)
		{
			Pos = Position;
			OldPos = Position;
			Radius = Radius;
		}

		public void Update(Image rOwner, Vector2 BonePosition, float fRotation, bool bFlip, float fScale)
		{
			//set to local coord
			Vector2 WorldPosition = LocalPosition * fScale;

			//is it flipped?
			if (bFlip && (null != rOwner))
			{
				//flip from the edge of the image
				WorldPosition.X = (rOwner.Width * fScale) - WorldPosition.X;
			}

			//rotate correctly
			WorldPosition = MatrixExt.Orientation(fRotation).Mutliply(WorldPosition);

			//move to the correct position
			WorldPosition = BonePosition + WorldPosition;
			Pos = WorldPosition;

			//set teh world radius
			Radius = LocalRadius * fScale;
		}

		public void Render(IRenderer rRenderer, Color rColor)
		{
			rRenderer.Primitive.Circle(Pos, Radius, rColor);
		}

		public float DistanceToPoint(int iScreenX, int iScreenY)
		{
			return DistanceToPoint(new Vector2(iScreenX, iScreenY));
		}

		/// <summary>
		/// Copy another circle's data into this one
		/// </summary>
		/// <param name="rInst">The circle to copy</param>
		public void Copy(PhysicsCircle rInst)
		{
			LocalPosition = rInst.LocalPosition;
			LocalRadius = rInst.LocalRadius;
			_Position = rInst._Position;
			OldPos = rInst.OldPos;
			Radius = rInst.Radius;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode)
		{
			if ("Item" != rXMLNode.Name)
			{
				Debug.Assert(false);
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
					if ("AnimationLib.CircleXML" != strValue)
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

					if (strName == "center")
					{
						LocalPosition = strValue.ToVector2();
					}
					else if (strName == "radius")
					{
						LocalRadius = Convert.ToSingle(strValue);
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
		public void WriteXMLFormat(XmlTextWriter rXMLFile)
		{
			//write out the item tag
			rXMLFile.WriteStartElement("Item");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.CircleXML");

			//write out joint offset
			rXMLFile.WriteStartElement("center");
			rXMLFile.WriteString(LocalPosition.X.ToString() + " " +
				LocalPosition.Y.ToString());
			rXMLFile.WriteEndElement();

			//write first limit 
			rXMLFile.WriteStartElement("radius");
			rXMLFile.WriteString(LocalRadius.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// read this asshole from a serialized xml file object
		/// </summary>
		/// <param name="myCircle">the object to get data from</param>
		public bool ReadSerializedFormat(CircleXML myCircle)
		{
			LocalPosition = myCircle.center;
			LocalRadius = myCircle.radius;
			return true;
		}

		#endregion //File IO
	}
}