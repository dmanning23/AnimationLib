using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Xml;

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
		private GarmentAnimationContainer _animationContainer;

		/// <summary>
		/// flag for whether this garment has been added to the model or not
		/// </summary>
		private bool _isAddedToModel;

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
		public GarmentBone(GarmentAnimationContainer owner)
			: base()
		{
			Debug.Assert(null != owner);
			_animationContainer = owner;
			ParentBone = null;
			BoneType = EBoneType.Garment;
			_isAddedToModel = false;
		}

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		override public void Update(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			float scale,
			bool ignoreRagdoll)
		{
			Debug.Assert(null != _animationContainer);

			//update the animation container, which will update the Bone base class 
			_animationContainer.Update(time, position, parentFlip, scale, parentRotation, ignoreRagdoll, parentLayer, ParentBone);
		}

		public void UpdateBaseBone(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			float scale,
			bool ignoreRagdoll)
		{
			base.Update(time, position, keyBone, parentRotation, parentFlip, parentLayer, scale, ignoreRagdoll);
		}

		/// <summary>
		/// add to model
		/// </summary>
		public void AddToModel()
		{
			Debug.Assert(null != ParentBone);

			if (!_isAddedToModel)
			{
				ParentBone.AddGarment(this);
				_isAddedToModel = true;
			}
		}

		/// <summary>
		/// remove from model
		/// </summary>
		public void RemoveFromModel()
		{
			Debug.Assert(null != ParentBone);

			if (_isAddedToModel)
			{
				ParentBone.RemoveGarment(GarmentName);
				_isAddedToModel = false;
			}
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Parse a child node of this BoneXML
		/// </summary>
		/// <param name="childNode">teh node to parse</param>
		/// <param name="rRenderer"></param>
		/// <returns></returns>
		protected override bool ParseChildXmlNode(XmlNode childNode)
		{
			//what is in this node?
			var name = childNode.Name;
			var value = childNode.InnerText;

			if (name == "parentBone")
			{
				//set teh parent bone of this dude
				ParentBoneName = value;
			}
			else
			{
				//Let the base class parse the rest of the xml
				return base.ParseChildXmlNode(childNode);
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		/// <param name="startElement">whether to tag element as "Asset" or "Item"</param>
		/// <param name="scale"></param>
		public override void WriteXmlFormat(XmlTextWriter xmlWriter, bool startElement, float scale)
		{
			//add the xml node
			if (startElement)
			{
				xmlWriter.WriteStartElement("XnaContent");
				xmlWriter.WriteStartElement("Asset");
			}
			else
			{
				xmlWriter.WriteStartElement("Item");
			}
			xmlWriter.WriteAttributeString("Type", "AnimationLib.GarmentBoneXML");

			WriteChildXmlNode(xmlWriter, scale);

			if (startElement)
			{
				//write out extra end element for XnaContent
				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();
		}

		public override void WriteChildXmlNode(XmlTextWriter xmlWriter, float scale)
		{
			//write out all the base class xml stuff
			base.WriteChildXmlNode(xmlWriter, scale);

			//add the name attribute
			xmlWriter.WriteStartElement("parentBone");
			xmlWriter.WriteString(ParentBoneName);
			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}