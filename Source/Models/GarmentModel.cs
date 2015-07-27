using FilenameBuddy;
using System.Collections.Generic;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is a class that is used to put together a garment that can be added to a model skeleton
	/// </summary>
	public class GarmentModel : XmlFileBuddy
	{
		#region Properties

		/// <summary>
		/// The name of this garment.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// A list of all the garment fragments that get added to the skeleton
		/// </summary>
		public List<GarmentFragmentModel> Fragments { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentModel(Filename filename)
			: base("garment", filename)
		{
			Fragments = new List<GarmentFragmentModel>();
		}

		public GarmentModel(Filename filename, Garment garment)
			: this(filename)
		{
			Name = garment.Name;
			foreach (var fragment in garment.Fragments)
			{
				Fragments.Add(new GarmentFragmentModel(fragment));
			}
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			string name = node.Name;
			string value = node.InnerText;

			switch (name)
			{
				case "Asset":
				{
					//This is an old garment file that still has the XNA format.
					XmlFileBuddy.ReadChildNodes(node, ParseXmlNode);
				}
				break;
				case "Type":
				{
					//throw these attributes out
				}
				break;
				case "name":
				{
					Name = value;
				}
				break;
				case "fragments":
				{
					XmlFileBuddy.ReadChildNodes(node, ReadFragments);
				}
				break;
				default:
				{
					NodeError(node);
				}
				break;
			}
		}

		public void ReadFragments(XmlNode node)
		{
			var fragment = new GarmentFragmentModel();
			XmlFileBuddy.ReadChildNodes(node, fragment.ParseXmlNode);
			Fragments.Add(fragment);
		}

		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("garment");
			xmlWriter.WriteAttributeString("name", Name);

			//write out the garment fragments
			xmlWriter.WriteStartElement("fragments");
			foreach (var fragment in Fragments)
			{
				fragment.WriteXmlNode(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}