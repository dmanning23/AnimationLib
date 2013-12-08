using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework.Content;
using FilenameBuddy;
using DrawListBuddy;
using RenderBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is a class that is used to put together a garment that can be added to a model skeleton
	/// </summary>
	public class Garment
	{
		#region Fields

		/// <summary>
		/// The name of this garment.
		/// </summary>
		string m_strName;

		/// <summary>
		/// A list of all the garment fragments that get added to the skeleton
		/// </summary>
		private List<GarmentFragment> m_listFragments;

		private Filename m_strFilename;

		/// <summary>
		/// Whether or not the garment has any physics data in it.
		/// </summary>
		private bool m_bHasPhysics;

		#endregion //Fields

		#region Properties

		public string Name
		{
			get { return m_strName; }
			private set { m_strName = value; }
		}

		public List<GarmentFragment> Fragments
		{
			get { return m_listFragments; }
		}

		public Filename Filename
		{
			get { return m_strFilename; }
		}

		public bool HasPhysics
		{
			get { return m_bHasPhysics; }
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public Garment()
		{
			m_listFragments = new List<GarmentFragment>();
			m_strFilename = new Filename();
			m_bHasPhysics = false;
		}

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToModel()
		{
			//add all the garment bones to the bones they attach to
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				m_listFragments[i].AddToModel();
			}
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromModel()
		{
			//remove all the garment bones from the bones they attach to
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				m_listFragments[i].RemoveFromModel();
			}
		}

		#region Tools

		/// <summary>
		/// Get a list of all the weapon
		/// </summary>
		/// <param name="listWeapons"></param>
		public void GetAllWeaponBones(List<string> listWeapons)
		{
			foreach (GarmentFragment curFragment in Fragments)
			{
				curFragment.GetAllWeaponBones(listWeapons);
			}
		}

		#endregion //Tools

		#endregion //Methods

		#region File IO

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		/// <param name="rRootNode"></param>
		private void SetGarmentBones(Bone rRootNode)
		{
			//set garment name in all bones
			Debug.Assert(m_strName.Length > 0);
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				m_listFragments[i].GarmentName = Name;
			}

			//set the parent bone of all those root node garment bones
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				m_listFragments[i].SetGarmentBones(rRootNode);
			}

			//check whether or not there is any physics data in the garment
			m_bHasPhysics = false;
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				if (m_listFragments[i].AnimationContainer.Model.HasPhysicsData())
				{
					m_bHasPhysics = true;
					break;
				}
			}
		}

		/// <summary>
		/// Read from XML!
		/// </summary>
		/// <param name="strResource">xml filename to read from</param>
		/// <param name="rRenderer">renderer to use to load images</param>
		/// <param name="rRootNode">bone to attach garments to</param>
		/// <returns>bool: whether or not it was able to read in the garment</returns>
		public bool ReadXMLFormat(Filename strResource, IRenderer rRenderer, Bone rRootNode)
		{
			//Open the file.
			FileStream stream = File.Open(strResource.File, FileMode.Open, FileAccess.Read);
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
			if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
			{
				Debug.Assert(false);
				stream.Close();
				return false;
			}

			//next node is "Asset"
			XmlNode AssetNode = rootNode.FirstChild;
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
							if (!childFragment.ReadXMLFormat(fragmentNode, rRenderer))
							{
								Debug.Assert(false);
								return false;
							}
							m_listFragments.Add(childFragment);
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
			SetGarmentBones(rRootNode);

			//grab teh filename
			m_strFilename.File = strResource.File;

			return true;
		}

		public void WriteXMLFormat(Filename strFileName, float fEnbiggify)
		{
			//open the file, create it if it doesnt exist yet
			XmlTextWriter rXMLFile = new XmlTextWriter(strFileName.File, null);
			rXMLFile.Formatting = Formatting.Indented;
			rXMLFile.Indentation = 1;
			rXMLFile.IndentChar = '\t';

			rXMLFile.WriteStartDocument();

			//add the xml node
			rXMLFile.WriteStartElement("XnaContent");
			rXMLFile.WriteStartElement("Asset");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.GarmentXML");

			//write out the garment name
			rXMLFile.WriteStartElement("name");
			rXMLFile.WriteString(Name);
			rXMLFile.WriteEndElement();

			//write out the garment fragments
			rXMLFile.WriteStartElement("fragments");
			for (int i = 0; i < m_listFragments.Count; i++)
			{
				m_listFragments[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndDocument();

			// Close the file.
			rXMLFile.Flush();
			rXMLFile.Close();
		}

		/// <summary>
		/// read from XNA content
		/// </summary>
		/// <param name="rXmlContent">teh content loader</param>
		/// <param name="strResource">name of the garment resource to load</param>
		/// <param name="rRenderer">renderer to use to load images</param>
		/// <param name="rRootNode">teh root node of the model that uses this garment</param>
		/// <returns>bool: whether or not was able to load the garment</returns>
		public bool ReadSerializedFormat(ContentManager rXmlContent, Filename strResource, IRenderer rRenderer, Bone rRootNode)
		{
			//open file
			Debug.Assert(null != rXmlContent);
			AnimationLib.GarmentXML rGarmentXML = rXmlContent.Load<AnimationLib.GarmentXML>(strResource.GetRelPathFileNoExt());

			//set the name 
			Name = rGarmentXML.name;
			m_strFilename.File = strResource.File;

			//read in all the bones
			for (int i = 0; i < rGarmentXML.fragments.Count; i++)
			{
				GarmentFragment childFragment = new GarmentFragment();
				if (!childFragment.ReadXNAContent(rXmlContent, rGarmentXML.fragments[i], rRenderer))
				{
					Debug.Assert(false);
					return false;
				}
				m_listFragments.Add(childFragment);
			}

			//Setup all the data in the garment bones.
			SetGarmentBones(rRootNode);
			return true;
		}

		#endregion //File IO
	}
}