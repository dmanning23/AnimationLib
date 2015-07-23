using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	public class JointModel : XmlObject
	{
		#region Properties

		/// <summary>
		/// The name of this joint
		/// </summary>
		public string Name { get; set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public JointModel()
		{
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

			switch (name)
			{
				case "name":
				{
					//set the name of this joint
					Name = value;
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
			xmlWriter.WriteAttributeString("Type", "AnimationLib.JointXML");

			//write out upper left coords
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteString(Name);
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}