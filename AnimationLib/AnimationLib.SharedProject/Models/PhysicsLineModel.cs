using Microsoft.Xna.Framework;
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
		public Vector2 Start { get; set; }

		/// <summary>
		/// the end of the line, local to the bone
		/// </summary>
		public Vector2 End { get; set; }

		#endregion //Members

		#region Members

		public PhysicsLineModel()
		{
			Start = Vector2.Zero;
			End = Vector2.Zero;
		}

		public PhysicsLineModel(PhysicsLine line)
			: this()
		{
			Start = line.LocalStart;
			End = line.LocalEnd;
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
				case "Type":
				{
					//throw these attributes out
				}
				break;
				case "start":
				{
					Start = value.ToVector2();
				}
				break;
				case "end":
				{
					End = value.ToVector2();
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
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("line");

			//write out joint offset
			xmlWriter.WriteStartElement("start");
			xmlWriter.WriteString(Start.StringFromVector());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("end");
			xmlWriter.WriteString(End.StringFromVector());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}