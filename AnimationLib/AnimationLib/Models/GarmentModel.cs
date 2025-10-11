using AnimationLib.Core.Json;
using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
#if !BRIDGE
using System.Xml;
#endif
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

		private float Scale { get; set; }

		public List<ColorTagModel> Colors { get; set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentModel(Filename filename, float scale)
			: base("garment", filename)
		{
			Scale = scale;
			Fragments = new List<GarmentFragmentModel>();
			Colors = new List<ColorTagModel>();
		}

		public GarmentModel(Filename filename, Garment garment)
			: this(filename, 1f)
		{
			Name = garment.Name;
			foreach (var fragment in garment.Fragments)
			{
				Fragments.Add(new GarmentFragmentModel(fragment, this));
			}

			foreach (var color in garment.Colors.Colors)
			{
				Colors.Add(new ColorTagModel(color.Key, color.Value));
			}
		}

		#endregion //Methods

		#region File IO

#if !BRIDGE
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
				case "colors":
					{
						XmlFileBuddy.ReadChildNodes(node, ReadColors);
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
			GarmentFragmentModel fragment = new GarmentFragmentModel(this, Scale, Content);
			XmlFileBuddy.ReadChildNodes(node, fragment.ParseXmlNode);
			Fragments.Add(fragment);
		}

		public void ReadColors(XmlNode node)
		{
			var colorModel = new ColorTagModel();
			XmlFileBuddy.ReadChildNodes(node, colorModel.ParseXmlNode);
			Colors.Add(colorModel);
		}

		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteAttributeString("name", Name);

			//write out the garment fragments
			xmlWriter.WriteStartElement("fragments");
			foreach (var fragment in Fragments)
			{
				fragment.WriteXmlNodes(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			if (Colors.Count > 0)
			{
				//write out the garment colors
				xmlWriter.WriteStartElement("colors");
				foreach (var color in Colors)
				{
					color.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}

		public override void WriteJson()
		{
			foreach (var fragment in Fragments)
			{
				fragment.WriteJson();
			}

			base.WriteJson();
		}
#endif

		public override void ReadJsonFile(ContentManager content = null)
		{
			using (var jsonModel = new GarmentJsonModel(this.ContentName, this.Filename))
			{
				//read the json file
				jsonModel.ReadJsonFile(content);

				//load from the json structure
				Name = jsonModel.Name;

				foreach (var fragment in jsonModel.Fragments)
				{
					Fragments.Add(new GarmentFragmentModel(this, fragment, Scale, content));
				}

				foreach (var color in jsonModel.Colors)
				{
					Colors.Add(new ColorTagModel(color.Tag, color.Color));
				}
			}
		}

		#endregion //File IO
	}
}