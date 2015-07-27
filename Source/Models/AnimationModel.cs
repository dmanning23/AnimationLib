using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This object is a single animation for the skeleton
	/// </summary>
	public class AnimationModel : XmlObject
	{
		#region Properties

		/// <summary>
		/// name of the animation
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }
		
		/// <summary>
		/// length of this animation
		/// </summary>
		/// <value>The length.</value>
		public float Length { get; set; }

		/// <summary>
		/// All the key elements for this animation
		/// </summary>
		public List<KeyElementModel> KeyElements { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationModel()
		{
			KeyElements = new List<KeyElementModel>();
		}

		public AnimationModel(Animation animation, Skeleton skeleton)
			: this()
		{
			KeyElements = new List<KeyElementModel>();
			GetKeys(animation.KeyBone, skeleton.RootBone);
			KeyElements.Sort(new KeyElementModelSort());
		}

		private void GetKeys(KeyBone keyBone, Bone bone)
		{
			//write out all my joints
			GetKeys(keyBone.KeyJoint, bone);

			//write out all the children's joints
			foreach (var childKeyBone in keyBone.Bones)
			{
				//find the bone for this keybone
				var childBone = bone.GetBone(childKeyBone.Name);
				Debug.Assert(null != childBone);
				GetKeys(childKeyBone, childBone);
			}
		}

		private void GetKeys(KeyJoint joint, Bone bone)
		{
			//add all the key elements to that dude
			for (int i = 0; i < joint.Elements.Count; i++)
			{
				var key = joint.Elements[i];

				//don't write out fucked up shit?
				Debug.Assert(key.KeyFrame);

				//don't write out reduntant key elements
				if ((i > 0) && (i < (joint.Elements.Count - 1)))
				{
					if (key.Compare(joint.Elements[i - 1]) && key.Compare(joint.Elements[i + 1]))
					{
						//dont write out if this matches the previous and next keys
						continue;
					}

					if ((i == joint.Elements.Count - 1) && key.Compare(joint.Elements[i - 1]))
					{
						//dont write out if this is last key and matches prev
						continue;
					}
				}

				KeyElements.Add(new KeyElementModel(key, bone));
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
				case "name":
				{
					Name = value;
				}
				break;

				case "length":
				{
					Length = Convert.ToSingle(value);
				}
				break;

				case "keys":
				{
					XmlFileBuddy.ReadChildNodes(node, ReadKeyElements);

					//now sort all those keyframes
					KeyElements.Sort(new KeyElementModelSort());
				}
				break;
				default:
				{
					base.ParseXmlNode(node);
				}
				break;
			}
		}

		public void ReadKeyElements(XmlNode node)
		{
			//read in the key element
			var key = new KeyElementModel();
			XmlFileBuddy.ReadChildNodes(node, key.ParseXmlNode);
			KeyElements.Add(key);
		}

		public override void WriteXmlNode(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("animation");

			xmlWriter.WriteAttributeString("name", Name);
			xmlWriter.WriteAttributeString("length", Length.ToString());

			xmlWriter.WriteStartElement("animation");

			//make sure thos dudes are sorted first
			KeyElements.Sort(new KeyElementModelSort());

			foreach (var key in KeyElements)
			{
				key.WriteXmlNode(xmlWriter);
			}

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}