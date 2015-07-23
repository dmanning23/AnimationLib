using CollisionBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	public class PhysicsLineModel : XmlObject
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

		public PhysicsLineModel()
		{
			LocalStart = Vector2.Zero;
			LocalEnd = Vector2.Zero;
		}

		#endregion //Members

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

			switch (name)
			{
				case "start":
				{
					LocalStart = value.ToVector2();
				}
				break;
				case "end":
				{
					LocalEnd = value.ToVector2();
				}
				break;
				default:
				{
					base.ParseXmlNode(node);
				}
				break;
			}
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public void WriteXmlFormat(XmlTextWriter xmlWriter)
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