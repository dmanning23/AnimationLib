using DrawListBuddy;
using FilenameBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UndoRedoBuddy;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the model for a bone as laoded from a file, data store, etc
	/// </summary>
	public class BoneModel : XmlObject
	{
		#region Properties

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

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public BoneModel()
		{
			Bones = new List<BoneModel>();
			Joints = new List<JointModel>();
			Images = new List<ImageModel>();
			Name = "Root";
			Colorable = false;
			BoneType = EBoneType.Normal;
		}

		public BoneModel(Bone bone)
		{
			//TODO: copy all the bone info into this dude

			Bones = new List<BoneModel>();
			Joints = new List<JointModel>();
			Images = new List<ImageModel>();
			Name = "Root";
			Colorable = false;
			BoneType = EBoneType.Normal;
		}

		public override string ToString()
		{
			return Name;
		}

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
						default:
						{
							BoneType = EBoneType.Normal;
						}
						break;

					}
				}
				break;
				case "colorable":
				{
					Colorable = Convert.ToBoolean(value);
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
			joint.ReadXmlFormat(node);
			Joints.Add(joint);
		}

		public void ReadImage(XmlNode node)
		{
			var image = new Image();
			image.ReadXmlFormat(node);
			Images.Add(image);
		}

		public void ReadChildBone(XmlNode node)
		{
			var childBone = CreateBone();
			childBone.ReadXmlFormat(node);
			Bones.Add(childBone);
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public virtual void WriteXmlNode(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("bone");

			WriteChildXmlNode(xmlWriter);

			xmlWriter.WriteEndElement();
		}

		public virtual void WriteChildXmlNode(XmlTextWriter xmlWriter)
		{
			//add the name attribute
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteString(Name);
			xmlWriter.WriteEndElement();

			//add the type attribute
			xmlWriter.WriteStartElement("type");
			xmlWriter.WriteString(BoneType.ToString());
			xmlWriter.WriteEndElement();

			//add whether or not this bone ignores palette swap
			xmlWriter.WriteStartElement("colorable");
			xmlWriter.WriteString(Colorable ? "true" : "false");
			xmlWriter.WriteEndElement();

			//write out joints
			xmlWriter.WriteStartElement("joints");
			for (var i = 0; i < Joints.Count; i++)
			{
				Joints[i].WriteXmlFormat(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			//write out images
			xmlWriter.WriteStartElement("images");
			for (var i = 0; i < Images.Count; i++)
			{
				Images[i].WriteXmlFormat(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			//write out child bones
			xmlWriter.WriteStartElement("bones");
			for (var i = 0; i < Bones.Count; i++)
			{
				//dont write out child garment bones
				if (Bones[i] is GarmentBone)
				{
					continue;
				}
				Bones[i].WriteXmlFormat(xmlWriter, scale);
			}
			xmlWriter.WriteEndElement();
		}

		#endregion //Methods
	}
}