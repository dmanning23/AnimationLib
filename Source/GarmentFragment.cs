using FilenameBuddy;
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
		private readonly GarmentAnimationContainer _animationContainer;

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
				Debug.Assert(null != _animationContainer);
				return _animationContainer; 
			}
		}

		public string BoneName
		{
			get 
			{
				Debug.Assert(null != _animationContainer);
				Debug.Assert(null != _animationContainer.Model);
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
			_animationContainer = new GarmentAnimationContainer();
		}

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToModel()
		{
			//add all the garment bones to the bones they attach to
			_animationContainer.AddToModel();
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromModel()
		{
			//remove all the garment bones from the bones they attach to
			_animationContainer.RemoveFromModel();
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
		/// <param name="rootNode"></param>
		public void SetGarmentBones(Bone rootNode)
		{
			//set garment name in the bones
			AnimationContainer.GarmentName = GarmentName;

			//set the parent bone of all those root node garment bones

			AnimationContainer.SetGarmentBones(rootNode);
		}

		/// <summary>
		/// Read from XML!
		/// </summary>
		/// <param name="node">bone to attach garments to</param>
		/// <param name="renderer">renderer to use to load images</param>
		/// <returns>bool: whether or not it was able to read in the garment</returns>
		public bool ReadXmlFormat(XmlNode node, IRenderer renderer)
		{
#if DEBUG
			//make sure it is actually an xml node
			if (node.NodeType != XmlNodeType.Element)
			{
				Debug.Assert(false);
				return false;
			}

			if ("Item" != node.Name)
			{
				Debug.Assert(false);
				return false;
			}

			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = node.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				var name = mapAttributes.Item(i).Name;
				var value = mapAttributes.Item(i).Value;

				if ("Type" == name)
				{
					if ("AnimationLib.GarmentFragmentXML" != value)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}
#endif

			//Read in child nodes
			if (node.HasChildNodes)
			{
				for (XmlNode childNode = node.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					//what is in this node?
					var name = childNode.Name;
					var value = childNode.InnerText;

					if (name == "model")
					{
						//read in the model 
						var modelFile = new Filename(value);
						if (!AnimationContainer.ReadModelXml(modelFile, renderer))
						{
							Debug.Assert(false);
							return false;
						}
					}
					else if (name == "animation")
					{
						//read in the animations
						Filename strAnimationFile = new Filename(value);
						if (!AnimationContainer.ReadAnimationXml(strAnimationFile))
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
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		/// <param name="scale"></param>
		public void WriteXmlFormat(XmlTextWriter xmlWriter, float scale)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.GarmentFragmentXML");

			//write out model filename to use
			xmlWriter.WriteStartElement("model");
			xmlWriter.WriteString(AnimationContainer.ModelFile.GetRelFilename());
			xmlWriter.WriteEndElement();

			//write out animation filename to use
			xmlWriter.WriteStartElement("animation");
			xmlWriter.WriteString(AnimationContainer.AnimationFile.GetRelFilename());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();

			//write out the model file
			AnimationContainer.WriteModelXml(AnimationContainer.ModelFile, scale);

			//write out the animation file
			AnimationContainer.WriteAnimationXml(AnimationContainer.AnimationFile);
		}

		#endregion File IO
	}
}