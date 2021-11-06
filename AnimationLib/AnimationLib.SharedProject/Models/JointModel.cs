using AnimationLib.Core.Json;
#if !BRIDGE
using System.Xml;
#endif
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

		public JointModel(JointJsonModel joint)
		{
			Name = joint.Name;
		}

		public JointModel(Joint joint)
		{
			Name = joint.Name;
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods

		#region File IO

#if !BRIDGE
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
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("joint");
			xmlWriter.WriteAttributeString("name", Name);
			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //File IO
	}
}