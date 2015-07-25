using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using FilenameBuddy;
using RenderBuddy;
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

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read from XML!
		/// </summary>
		/// <param name="filename">xml filename to read from</param>
		/// <param name="renderer">renderer to use to load images</param>
		/// <param name="rRootNode">bone to attach garments to</param>
		/// <returns>bool: whether or not it was able to read in the garment</returns>
		public bool ReadXmlFormat(Filename filename, IRenderer renderer, Bone rootBone)
		{
			//Open the file.
			#if ANDROID
			Stream stream = Game.Activity.Assets.Open(filename.File);
#else
			FileStream stream = File.Open(filename.File, FileMode.Open, FileAccess.Read);
			#endif
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType != XmlNodeType.Element)
			{
				Debug.Assert(false);
				stream.Close();
				return false;
			}

			//eat up the name of that xml node
			string strElementName = rootNode.Name;

#if DEBUG
			if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
			{
				Debug.Assert(false);
				stream.Close();
				return false;
			}
#endif

			//next node is "Asset"
			XmlNode AssetNode = rootNode.FirstChild;

#if DEBUG
			if (null == AssetNode)
			{
				Debug.Assert(false);
				return false;
			}
			if (!AssetNode.HasChildNodes)
			{
				Debug.Assert(false);
				return false;
			}
#endif

			//Read in child nodes
			for (XmlNode childNode = AssetNode.FirstChild;
				null != childNode;
				childNode = childNode.NextSibling)
			{
				//what is in this node?
				string strName = childNode.Name;
				string strValue = childNode.InnerText;

				if (strName == "name")
				{
					Name = strValue;
				}
				else if (strName == "fragments")
				{
					//read in all the child bones
					if (childNode.HasChildNodes)
					{
						for (XmlNode fragmentNode = childNode.FirstChild;
							null != fragmentNode;
							fragmentNode = fragmentNode.NextSibling)
						{
							GarmentFragment childFragment = new GarmentFragment();
							if (!childFragment.ReadXmlFormat(fragmentNode, renderer))
							{
								Debug.Assert(false);
								return false;
							}
							Fragments.Add(childFragment);
						}
					}
				}
				else
				{
					Debug.Assert(false);
					return false;
				}
			}
			
			// Close the file.
			stream.Close();

			//Setup all the data in the garment bones.
			SetGarmentBones(rootBone);

			//grab teh filename
			Filename.File = filename.File;

			return true;
		}

		public void WriteXmlFormat(Filename fileName, float scale)
		{
			//open the file, create it if it doesnt exist yet
			var xmlWriter = new XmlTextWriter(fileName.File, null);
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.Indentation = 1;
			xmlWriter.IndentChar = '\t';

			xmlWriter.WriteStartDocument();

			//add the xml node
			xmlWriter.WriteStartElement("XnaContent");
			xmlWriter.WriteStartElement("Asset");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.GarmentXML");

			//write out the garment name
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteString(Name);
			xmlWriter.WriteEndElement();

			//write out the garment fragments
			xmlWriter.WriteStartElement("fragments");
			for (var i = 0; i < Fragments.Count; i++)
			{
				Fragments[i].WriteXmlFormat(xmlWriter, scale);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndDocument();

			// Close the file.
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		#endregion //File IO

		public override void ParseXmlNode(XmlNode xmlNode)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteXmlNodes(XmlTextWriter xmlFile)
		{
			throw new System.NotImplementedException();
		}
	}
}