using CollisionBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;

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

		public void Reset(Vector2 position)
		{
			Pos = position;
			OldPos = position;
			Radius = Radius;
		}

		public void Update(Image owner, Vector2 bonePosition, float rotation, bool isFlipped, float scale)
		{
			//set to local coord
			Vector2 worldPosition = LocalPosition * scale;

			//is it flipped?
			if (isFlipped && (null != owner))
			{
				//flip from the edge of the image
				worldPosition.X = (owner.Width * scale) - worldPosition.X;
			}

			//rotate correctly
			worldPosition = MatrixExt.Orientation(rotation).Multiply(worldPosition);

			//move to the correct position
			worldPosition = bonePosition + worldPosition;

			Update(worldPosition, scale);
		}

		/// <summary>
		/// update the position of this circle
		/// </summary>
		/// <param name="myPosition">the location of this dude in the world</param>
		/// <param name="scale">how much to scale the physics item</param>
		public void Update(Vector2 myPosition, float scale)
		{
			Pos = myPosition;

			//set teh world radius
			Radius = LocalRadius * scale;
		}

		public void Render(IRenderer renderer, Color color)
		{
			renderer.Primitive.Circle(Pos, Radius, color);
		}

		public float DistanceToPoint(int screenX, int screenY)
		{
			return DistanceToPoint(new Vector2(screenX, screenY));
		}

		/// <summary>
		/// Copy another circle's data into this one
		/// </summary>
		/// <param name="inst">The circle to copy</param>
		public void Copy(PhysicsCircle inst)
		{
			LocalPosition = inst.LocalPosition;
			LocalRadius = inst.LocalRadius;
			_Position = inst._Position;
			OldPos = inst.OldPos;
			Radius = inst.Radius;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="node">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode node)
		{
#if DEBUG
			if ("Item" != node.Name)
			{
				Debug.Assert(false);
				return false;
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = node.Attributes;
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
#endif

			//Read in child nodes
			if (node.HasChildNodes)
			{
				for (XmlNode childNode = node.FirstChild;
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
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public void WriteXMLFormat(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.CircleXML");

			//write out joint offset
			xmlWriter.WriteStartElement("center");
			xmlWriter.WriteString(LocalPosition.X + " " +
				LocalPosition.Y);
			xmlWriter.WriteEndElement();

			//write first limit 
			xmlWriter.WriteStartElement("radius");
			xmlWriter.WriteString(LocalRadius.ToString());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}