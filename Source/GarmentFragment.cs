using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace AnimationLib
{
	/// <summary>
	/// This is a peice of a garment that will get added to the skeleton
	/// </summary>
	public class GarmentFragment
	{
		#region Fields

		/// <summary>
		/// The animation container for the model to add to the skeleton
		/// </summary>
		private GarmentAnimationContainer m_AnimationContainer;

		#endregion //Fields

		#region Properties

		/// <summary>
		/// The name of the garment that this guy is a peice of.
		/// </summary>
		public string GarmentName { get; set; }

		public GarmentAnimationContainer AnimationContainer
		{
			get
			{
				Debug.Assert(null != m_AnimationContainer);
				return m_AnimationContainer; 
			}
		}

		public string BoneName
		{
			get 
			{
				Debug.Assert(null != m_AnimationContainer);
				Debug.Assert(null != m_AnimationContainer.Model);
				return AnimationContainer.Model.Name; 
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragment()
		{
			m_AnimationContainer = new GarmentAnimationContainer();
		}

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToModel()
		{
			//add all the garment bones to the bones they attach to
			m_AnimationContainer.AddToModel();
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromModel()
		{
			//remove all the garment bones from the bones they attach to
			m_AnimationContainer.RemoveFromModel();
		}

		#region Tools

		/// <summary>
		/// Get a list of all the weapon
		/// </summary>
		/// <param name="listWeapons"></param>
		public void GetAllWeaponBones(List<string> listWeapons)
		{
			AnimationContainer.Model.GetAllWeaponBones(listWeapons);
		}

		#endregion //Tools

		#endregion //Methods

		#region File IO

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		/// <param name="rRootNode"></param>
		public void SetGarmentBones(Bone rRootNode)
		{
			//set garment name in the bones
			AnimationContainer.GarmentName = GarmentName;

			//set the parent bone of all those root node garment bones
			AnimationContainer.SetGarmentBones(rRootNode);
		}

		/// <summary>
		/// Read from XML!
		/// </summary>
		/// <param name="strResource">xml filename to read from</param>
		/// <param name="rRenderer">renderer to use to load images</param>
		/// <param name="rRootNode">bone to attach garments to</param>
		/// <returns>bool: whether or not it was able to read in the garment</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode, IRenderer rRenderer)
		{
#if DEBUG
			//make sure it is actually an xml node
			if (rXMLNode.NodeType != XmlNodeType.Element)
			{
				Debug.Assert(false);
				return false;
			}

			if ("Item" != rXMLNode.Name)
			{
				Debug.Assert(false);
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
					if ("AnimationLib.GarmentFragmentXML" != strValue)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}
#endif

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

					if (strName == "model")
					{
						//read in the model
						Filename strModelFile = new Filename(strValue);
						if (!m_AnimationContainer.ReadXMLModelFormat(strModelFile, rRenderer))
						{
							Debug.Assert(false);
							return false;
						}
					}
					else if (strName == "animation")
					{
						//read in the animations
						Filename strAnimationFile = new Filename(strValue);
						if (!m_AnimationContainer.ReadXMLAnimationFormat(strAnimationFile))
						{
							Debug.Assert(false);
							return false;
						}
					}
					else
					{
						Debug.Assert(false);
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
		public void WriteXMLFormat(XmlTextWriter rXMLFile, float fEnbiggify)
		{
			//write out the item tag
			rXMLFile.WriteStartElement("Item");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.GarmentFragmentXML");

			//write out model filename to use
			rXMLFile.WriteStartElement("model");
			rXMLFile.WriteString(AnimationContainer.ModelFile.GetRelFilename());
			rXMLFile.WriteEndElement();

			//write out animation filename to use
			rXMLFile.WriteStartElement("animation");
			rXMLFile.WriteString(AnimationContainer.AnimationFile.GetRelFilename());
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();

			//write out the model file
			AnimationContainer.WriteModelXMLFormat(AnimationContainer.ModelFile, fEnbiggify);

			//write out the animation file
			AnimationContainer.WriteXMLFormat(AnimationContainer.AnimationFile);
		}

		#endregion File IO
	}
}