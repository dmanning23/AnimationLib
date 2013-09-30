using System;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;
using DrawListBuddy;
using RenderBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is a special type of bone, that is animated based on the parent bone, can be added and removed at runtime, etc.
	/// </summary>
	public class GarmentBone : Bone
	{
		#region Members

		/// <summary>
		/// reference to the animation container that owns this bone
		/// used to change animation when the parent bone changes images
		/// </summary>
		private GarmentAnimationContainer m_AnimationContainer;

		/// <summary>
		/// flag for whether this garment has been added to the model or not
		/// </summary>
		private bool m_bAddedToModel;

		#endregion //Members

		#region Properties

		/// <summary>
		/// The peice of clothing that this bone is part of
		/// </summary>
		public string GarmentName { get; set; }

		/// <summary>
		/// Reference to the bone this guy attaches too.  Should have the same name as this guy
		/// </summary>
		public Bone ParentBone { get; set; }

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; protected set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBone(GarmentAnimationContainer rOwner)
			: base()
		{
			Debug.Assert(null != rOwner);
			m_AnimationContainer = rOwner;
			ParentBone = null;
			BoneType = EBoneType.Garment;
			m_bAddedToModel = false;
		}

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		/// <param name="iTime"></param>
		/// <param name="myPosition"></param>
		/// <param name="myKeyBone"></param>
		/// <param name="fParentRotation"></param>
		/// <param name="bParentFlip"></param>
		override public void Update(int iTime,
			Vector2 myPosition,
			KeyBone myKeyBone,
			float fParentRotation,
			bool bParentFlip,
			int iParentLayer,
			float fScale,
			bool bIgnoreRagdoll)
		{
			Debug.Assert(null != m_AnimationContainer);

			//update the animation container, which will update the Bone base class 
			m_AnimationContainer.Update(iTime, myPosition, bParentFlip, fScale, fParentRotation, bIgnoreRagdoll, iParentLayer, ParentBone);
		}

		public void UpdateBaseBone(int iTime,
			Vector2 myPosition,
			KeyBone myKeyBone,
			float fParentRotation,
			bool bParentFlip,
			int iParentLayer,
			float fScale,
			bool bIgnoreRagdoll)
		{
			base.Update(iTime, myPosition, myKeyBone, fParentRotation, bParentFlip, iParentLayer, fScale, bIgnoreRagdoll);
		}

		/// <summary>
		/// add to model
		/// </summary>
		public void AddToModel()
		{
			Debug.Assert(null != ParentBone);

			if (!m_bAddedToModel)
			{
				ParentBone.AddGarment(this);
				m_bAddedToModel = true;
			}
		}

		/// <summary>
		/// remove from model
		/// </summary>
		public void RemoveFromModel()
		{
			Debug.Assert(null != ParentBone);

			if (m_bAddedToModel)
			{
				ParentBone.RemoveGarment(GarmentName);
				m_bAddedToModel = false;
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Parse a child node of this BoneXML
		/// </summary>
		/// <param name="childNode">teh node to parse</param>
		/// <param name="ParentBone"></param>
		/// <param name="rRenderer"></param>
		/// <returns></returns>
		protected override bool ParseChildXMLNode(XmlNode childNode, Bone ParentBone, IRenderer rRenderer)
		{
			//what is in this node?
			string strName = childNode.Name;
			string strValue = childNode.InnerText;

			if (strName == "parentBone")
			{
				//set teh parent bone of this dude
				ParentBoneName = strValue;
			}
			else
			{
				//Let the base class parse the rest of the xml
				return base.ParseChildXMLNode(childNode, ParentBone, rRenderer);
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="rXMLFile">the xml file to add this dude as a child of</param>
		/// <param name="bStartElement">whether to tag element as "Asset" or "Item"</param>
		public override void WriteXMLFormat(XmlTextWriter rXMLFile, bool bStartElement, float fEnbiggify)
		{
			//add the xml node
			if (bStartElement)
			{
				rXMLFile.WriteStartElement("XnaContent");
				rXMLFile.WriteStartElement("Asset");
			}
			else
			{
				rXMLFile.WriteStartElement("Item");
			}
			rXMLFile.WriteAttributeString("Type", "AnimationLib.GarmentBoneXML");

			WriteChildXMLNode(rXMLFile, fEnbiggify);

			if (bStartElement)
			{
				//write out extra end element for XnaContent
				rXMLFile.WriteEndElement();
			}
			rXMLFile.WriteEndElement();
		}

		public override void WriteChildXMLNode(XmlTextWriter rXMLFile, float fEnbiggify)
		{
			//write out all the base class xml stuff
			base.WriteChildXMLNode(rXMLFile, fEnbiggify);

			//add the name attribute
			rXMLFile.WriteStartElement("parentBone");
			rXMLFile.WriteString(ParentBoneName);
			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// Read in all the bone information from an object read in from a serialized XML file.
		/// </summary>
		/// <param name="rBone">the xml object to get data from</param>
		/// <param name="ParentBone">The parent bone for this dude.</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		public override bool ReadSerializedFormat(AnimationLib.BoneXML rBone, Bone ParentBone, IRenderer rRenderer)
		{
			AnimationLib.GarmentBoneXML myGarmentBoneXML = rBone as AnimationLib.GarmentBoneXML;
			if (null == myGarmentBoneXML)
			{
				return false;
			}

			//get the parent bone
			ParentBoneName = myGarmentBoneXML.parentBone;

			//let the base class parse the rest of that shit
			return base.ReadSerializedFormat(rBone, ParentBone, rRenderer);
		}

		#endregion //File IO
	}
}