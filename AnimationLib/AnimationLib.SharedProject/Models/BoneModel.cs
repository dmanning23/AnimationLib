using System;
using System.Collections.Generic;
#if !BRIDGE
using System.Xml;
#endif
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the model for a bone as laoded from a file, data store, etc
	/// </summary>
	public class BoneModel : XmlObject
	{
		#region Properties

		public SkeletonModel SkeletonModel { get; private set; }

		/// <summary>
		/// the child bones of this guy
		/// </summary>
		public List<BoneModel> Bones { get; private set; }

		/// <summary>
		/// The joints in this bone
		/// </summary>
		public List<JointModel> Joints { get; private set; }

		/// <summary>
		/// The images in this bone
		/// </summary>
		public List<ImageModel> Images { get; private set; }

		/// <summary>
		/// The name of this bone
		/// </summary>
		public string Name { get; protected set; }

		/// <summary>
		/// Flag used to tell the difference between the bone types for collision purposes
		/// </summary>
		public EBoneType BoneType { get; set; }

		/// <summary>
		/// Whether or not this bone should be colored by the palette swap
		/// </summary>
		public bool Colorable { get; set; }

		public float RagdollWeightRatio { get; set; }

		private float Scale { get; set; }
		private float FragmentScale { get; set; }

		public string PrimaryColorTag { get; set; }
		public string SecondaryColorTag { get; set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public BoneModel(SkeletonModel skeleton, float scale, float fragmentScale)
		{
			Scale = scale;
			FragmentScale = fragmentScale;
			Bones = new List<BoneModel>();
			Joints = new List<JointModel>();
			Images = new List<ImageModel>();
			Name = "Root";
			Colorable = false;
			BoneType = EBoneType.Normal;
			RagdollWeightRatio = 0.5f;
			SkeletonModel = skeleton;
		}

		/// <summary>
		/// copy all the bone info into this dude
		/// </summary>
		/// <param name="bone"></param>
		public BoneModel(SkeletonModel skeleton, Bone bone)
			: this(skeleton, 1f, 1f)
		{
			Name = bone.Name;
			Colorable = bone.Colorable;
			BoneType = bone.BoneType;
			RagdollWeightRatio = bone.RagdollWeightRatio;
			PrimaryColorTag = bone.PrimaryColorTag;
			SecondaryColorTag = bone.SecondaryColorTag;

			foreach (var joint in bone.Joints)
			{
				Joints.Add(new JointModel(joint));
			}
			foreach (var image in bone.Images)
			{
				Images.Add(new ImageModel(SkeletonModel, image));
			}
			foreach (var childBone in bone.Bones)
			{
				//Don't add garment bones to the model
				if (!childBone.IsGarment)
				{
					Bones.Add(new BoneModel(SkeletonModel, childBone));
				}
			}
		}

		public override string ToString()
		{
			return Name;
		}

#if !BRIDGE

		/// <summary>
		/// Parse a child node of this BoneXML
		/// </summary>
		/// <param name="node">teh node to parse</param>
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
						//set the name of this bone
						Name = value;

						//set the 'foot' flag as appropriate
						if (Name == "Left Foot" || Name == "Right Foot")
						{
							BoneType = EBoneType.Foot;
						}
					}
					break;
				case "type":
					{
						switch (value)
						{
							case "Foot":
								{
									BoneType = EBoneType.Foot;
								}
								break;

							case "Weapon":
								{
									BoneType = EBoneType.Weapon;
								}
								break;
							case "Anchor":
								{
									BoneType = EBoneType.Anchor;
								}
								break;
							default:
								{
									BoneType = EBoneType.Normal;
								}
								break;
						}
					}
					break;
				case "RagdollWeightRatio":
					{
						RagdollWeightRatio = Convert.ToSingle(value);
					}
					break;
				case "colorable":
					{
						Colorable = Convert.ToBoolean(value);
					}
					break;
				case "primaryColorTag":
					{
						PrimaryColorTag = value;
					}
					break;
				case "secondaryColorTag":
					{
						SecondaryColorTag = value;
					}
					break;
				case "joints":
					{
						XmlFileBuddy.ReadChildNodes(node, ReadJoint);
					}
					break;
				case "images":
					{
						XmlFileBuddy.ReadChildNodes(node, ReadImage);
					}
					break;
				case "bones":
					{
						XmlFileBuddy.ReadChildNodes(node, ReadChildBone);
					}
					break;
				default:
					{
						base.ParseXmlNode(node);
					}
					break;
			}
		}

		public void ReadJoint(XmlNode node)
		{
			var joint = new JointModel();
			XmlFileBuddy.ReadChildNodes(node, joint.ParseXmlNode);
			Joints.Add(joint);
		}

		public void ReadImage(XmlNode node)
		{
			var image = new ImageModel(SkeletonModel, Scale, FragmentScale);
			XmlFileBuddy.ReadChildNodes(node, image.ParseXmlNode);
			Images.Add(image);
		}

		public void ReadChildBone(XmlNode node)
		{
			var bone = new BoneModel(SkeletonModel, Scale, FragmentScale);
			XmlFileBuddy.ReadChildNodes(node, bone.ParseXmlNode);
			Bones.Add(bone);
		}


		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("bone");

			WriteChildXmlNode(xmlWriter);

			xmlWriter.WriteEndElement();
		}

		public virtual void WriteChildXmlNode(XmlTextWriter xmlWriter)
		{
			//add the name attribute
			xmlWriter.WriteAttributeString("name", Name);

			//add the type attribute
			if (BoneType != EBoneType.Normal)
			{
				xmlWriter.WriteAttributeString("type", BoneType.ToString());
			}

			if (0.5f != RagdollWeightRatio)
			{
				xmlWriter.WriteAttributeString("RagdollWeightRatio", RagdollWeightRatio.ToString());
			}

			//add whether or not this bone ignores palette swap
			if (Colorable)
			{
				xmlWriter.WriteAttributeString("colorable", "true");
			}

			if (!string.IsNullOrEmpty(PrimaryColorTag))
			{
				xmlWriter.WriteAttributeString("primaryColorTag", PrimaryColorTag);
			}

			if (!string.IsNullOrEmpty(SecondaryColorTag))
			{
				xmlWriter.WriteAttributeString("secondaryColorTag", SecondaryColorTag);
			}

			//write out joints
			if (Joints.Count > 0)
			{
				xmlWriter.WriteStartElement("joints");
				foreach (var joint in Joints)
				{
					joint.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			//write out images
			if (Images.Count > 0)
			{
				xmlWriter.WriteStartElement("images");
				foreach (var image in Images)
				{
					image.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			//write out child bones
			if (Bones.Count > 0)
			{
				xmlWriter.WriteStartElement("bones");
				foreach (var bone in Bones)
				{
					bone.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}
		}
#endif

		#endregion //Methods
	}
}