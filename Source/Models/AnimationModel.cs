using System;
using System.Diagnostics;
using System.Xml;
using UndoRedoBuddy;
using Vector2Extensions;
using System.Collections.Generic;
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
		/// length of this animation
		/// </summary>
		/// <value>The length.</value>
		public float Length { get; set; }

		/// <summary>
		/// the root key series
		/// </summary>
		/// <value>The key bone.</value>
		public List<KeyJointModel> KeyJoints { get; private set; }

		/// <summary>
		/// name of the animation
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationModel()
		{
			KeyJoints = new List<KeyJointModel>();
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="node">The xml node to read from</param>
		/// <param name="model">the root node of the model that this dude animates</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXmlFormat(XmlNode node, Bone model)
		{
#if DEBUG
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
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;

				if ("Type" == strName)
				{
					if ("AnimationLib.AnimationXML" != strValue)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}
#endif

			//make sure to setup the bones model
			KeyBone = new KeyBone(model);

			//Read in child nodes
			if (node.HasChildNodes)
			{
				for (XmlNode childNode = node.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					//what is in this node?
					string strName = childNode.Name;
					string strValue = childNode.InnerText;

					switch (strName)
					{
						case "name":
						{
							Name = strValue;
						}
						break;

						case "length":
						{
							Length = Convert.ToSingle(strValue);
						}
						break;

						case "keys":
						{
							//get the animation length in frames
							int iLengthFrames = Length.ToFrames();

							if (childNode.HasChildNodes)
							{
								for (var keyNode = childNode.FirstChild;
									null != keyNode;
									keyNode = keyNode.NextSibling)
								{
									//read in the key element
									var myKey = new KeyElement();
									myKey.ReadXmlFormat(keyNode);

									//is this keyelement worth keeping?
									if ((myKey.KeyFrame) && (iLengthFrames >= myKey.Time))
									{
										//set the image index
										var rMyBone = model.GetBone(myKey.JointName);
										if (rMyBone != null)
										{
											myKey.ImageIndex = rMyBone.GetImageIndex(myKey.ImageName);
										}

										//add to the correct keyjoint
										var myKeyJoint = KeyBone.GetKeyJoint(myKey.JointName);
										if (null != myKeyJoint)
										{
											myKeyJoint.AddKeyElement(myKey);
										}
									}
								}
							}
						}
						break;
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		/// <param name="model">the root bone of the model this dude references</param>
		public void WriteXmlFormat(XmlTextWriter xmlWriter, Bone model)
		{
			Debug.Assert(null != model);

			//first get a list of all the keyframes
			var animationXml = new AnimationXml();
			KeyBone.WriteXmlFormat(animationXml, model);

			//now sort all those keyframes
			animationXml.keys.Sort(new KeyXmlSort());

			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.AnimationXML");

			//write out animation name
			xmlWriter.WriteStartElement("name");
			xmlWriter.WriteString(Name);
			xmlWriter.WriteEndElement();

			//write out animation length
			xmlWriter.WriteStartElement("length");
			xmlWriter.WriteString(Length.ToString());
			xmlWriter.WriteEndElement();

			//write out all the keyframes
			xmlWriter.WriteStartElement("keys");
			for (int i = 0; i < animationXml.keys.Count; i++)
			{
				xmlWriter.WriteStartElement("Item");
				xmlWriter.WriteAttributeString("Type", "AnimationLib.KeyXML");

				xmlWriter.WriteStartElement("time");
				xmlWriter.WriteString(animationXml.keys[i].time.ToString());
				xmlWriter.WriteEndElement();

				if (!animationXml.keys[i].SkipRotation)
				{
					xmlWriter.WriteStartElement("rotation");
					xmlWriter.WriteString(animationXml.keys[i].rotation.ToString());
					xmlWriter.WriteEndElement();
				}

				if (!animationXml.keys[i].SkipLayer)
				{
					xmlWriter.WriteStartElement("layer");
					xmlWriter.WriteString(animationXml.keys[i].layer.ToString());
					xmlWriter.WriteEndElement();
				}

				if (!animationXml.keys[i].SkipImage)
				{
					xmlWriter.WriteStartElement("image");
					xmlWriter.WriteString(animationXml.keys[i].image.ToString());
					xmlWriter.WriteEndElement();
				}

				if (!animationXml.keys[i].SkipFlip)
				{
					xmlWriter.WriteStartElement("flip");
					xmlWriter.WriteString(animationXml.keys[i].flip ? "true" : "false");
					xmlWriter.WriteEndElement();
				}

				if (!animationXml.keys[i].SkipTranslation)
				{
					xmlWriter.WriteStartElement("translation");
					xmlWriter.WriteString(animationXml.keys[i].translation.StringFromVector());
					xmlWriter.WriteEndElement();
				}

				if (!animationXml.keys[i].SkipRagDoll)
				{
					xmlWriter.WriteStartElement("ragdoll");
					xmlWriter.WriteString(animationXml.keys[i].ragdoll ? "true" : "false");
					xmlWriter.WriteEndElement();
				}

				xmlWriter.WriteStartElement("joint");
				xmlWriter.WriteString(animationXml.keys[i].joint.ToString());
				xmlWriter.WriteEndElement();

				xmlWriter.WriteEndElement();
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion
	}
}