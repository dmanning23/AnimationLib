using System.Xml;
using Microsoft.Xna.Framework;
using CollisionBuddy;
using MatrixExtensions;
using DrawListBuddy;
using BasicPrimitiveBuddy;

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

		public void Update(Image rOwner, Vector2 BonePosition, float fRotation, bool bFlip, float fScale)
		{
			//set to local coord
			Vector2 WorldStart = LocalStart * fScale;
			Vector2 WorldEnd = LocalEnd * fScale;

			//is it flipped?
			if (bFlip)
			{
				//flip from the edge of the image
				WorldStart.X = (rOwner.Width * fScale) - WorldStart.X;
				WorldEnd.X = (rOwner.Width * fScale) - WorldEnd.X;
			}

			//rotate correctly
			Matrix myRotation = MatrixExt.Orientation(fRotation);
			WorldStart = myRotation.Mutliply(WorldStart);
			WorldEnd = myRotation.Mutliply(WorldEnd);

			//move to the correct position
			WorldStart = BonePosition + WorldStart;
			WorldEnd = BonePosition + WorldEnd;

			//Set the start and end points that are stored in the line object
			Start = WorldStart;
			End = WorldEnd;
		}

		public void Render(Renderer rRenderer, Color rColor, BasicPrimitive primitive)
		{
			primitive.Line(Start, End, rColor, rRenderer.SpriteBatch);
		}

		/// <summary>
		/// Copy another line's data into this one
		/// </summary>
		/// <param name="rInst">The line to copy</param>
		public void Copy(PhysicsLine rInst)
		{
			LocalStart = rInst.LocalStart;
			LocalEnd = rInst.LocalEnd;
			_Start = rInst._Start;
			_End = rInst.End;
			OldStart = rInst.OldStart;
			OldEnd = rInst.OldEnd;
			Length = rInst.Length;
			Direction = rInst.Direction;
		}

		#endregion //Members

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
					if ("SPFSettings.LineXML" != strValue)
					{
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

					if (strName == "start")
					{
						LocalStart = CStringUtils.ReadVectorFromString(strValue);
					}
					else if (strName == "end")
					{
						LocalEnd = CStringUtils.ReadVectorFromString(strValue);
					}
					else
					{
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
			rXMLFile.WriteAttributeString("Type", "SPFSettings.LineXML");

			//write out joint offset
			rXMLFile.WriteStartElement("start");
			rXMLFile.WriteString(LocalStart.X.ToString() + " " +
				LocalStart.Y.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("end");
			rXMLFile.WriteString(LocalEnd.X.ToString() + " " +
				LocalEnd.Y.ToString());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// read this asshole from a serialized xml file object
		/// </summary>
		/// <param name="myCircle">the object to get data from</param>
		public bool ReadSerializedFormat(LineXML myLine)
		{
			LocalStart = myLine.start;
			LocalEnd = myLine.end;
			return true;
		}

		#endregion //File IO
	}
}