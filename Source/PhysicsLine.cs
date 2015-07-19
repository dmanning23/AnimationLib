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

		#region Members

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

		#endregion //Members

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
					if ("AnimationLib.LineXML" != strValue)
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

					if (strName == "start")
					{
						LocalStart = strValue.ToVector2();
					}
					else if (strName == "end")
					{
						LocalEnd = strValue.ToVector2();
					}
					else
					{
						Debug.Assert(false);
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
			xmlWriter.WriteAttributeString("Type", "AnimationLib.LineXML");

			//write out joint offset
			xmlWriter.WriteStartElement("start");
			xmlWriter.WriteString(LocalStart.X + " " +
				LocalStart.Y);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("end");
			xmlWriter.WriteString(LocalEnd.X + " " +
				LocalEnd.Y);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}