using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using UndoRedoBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This object is a single animation for the skeleton
	/// </summary>
	public class Animation
	{
		#region Properties
		
		/// <summary>
		/// length of this animation
		/// </summary>
		/// <value>The length.</value>
		public float Length { get; private set; }

		/// <summary>
		/// the root key series
		/// </summary>
		/// <value>The key bone.</value>
		public KeyBone KeyBone { get; private set; }

		/// <summary>
		/// name of the animation
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Animation()
		{
			Length = 0.0f;
		}

		public Animation(Bone boneStructure)
		{
			Debug.Assert(null != boneStructure);
			KeyBone = new KeyBone(boneStructure);
			Length = 0.0f;
		}

		public bool AddKeyframe(KeyElement key)
		{
			//find the correct keyjoint
			KeyJoint myKeyJoint = KeyBone.GetKeyJoint(key.JointName);
			if (null != myKeyJoint)
			{
				//add the keyframe to the keyjoint
				myKeyJoint.AddKeyElement(key);
				return true;
			}
			return false;
		}

		public bool RemoveKeyframe(KeyElement key)
		{
			//find the correct keyjoint
			KeyJoint myKeyJoint = KeyBone.GetKeyJoint(key.JointName);
			if (null != myKeyJoint)
			{
				//remove all keyframes at that time
				return myKeyJoint.RemoveKeyElement(key.Time);
			}
			return false;
		}

		public bool RemoveKeyframe(Macro myAction, Bone copyBone, int time)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(copyBone.Name);
			if (null == myKeyBone)
			{
				return false;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(copyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.RemoveKeyElement(myAction, time, this);
			}

			//copy the rest of the thing
			myKeyBone.RemoveKeyElement(myAction, time, this);
			return true;
		}

		/// <summary>
		/// Copy the skeleton from one animation into another animation
		/// </summary>
		/// <param name="myAction">undo/redo action to put all the changes into</param>
		/// <param name="copyBone">bone that is being copied</param>
		/// <param name="myTargetAnimation">animation to paste that bone into</param>
		/// <param name="sourceTime">the time to copy from this animation</param>
		/// <param name="targetTime">the time to paste into the other animation</param>
		/// <param name="selectiveCopy">if this is true, it means only copy image, layer, ragdoll, flip</param>
		public void Copy(Macro myAction, 
		                 Bone copyBone, 
		                 Animation myTargetAnimation, 
		                 int sourceTime, 
		                 int targetTime,
		                 bool selectiveCopy)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(copyBone.Name);
			if (null == myKeyBone)
			{
				return;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(copyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.Copy(myAction, myTargetAnimation, sourceTime, targetTime, selectiveCopy);
			}

			//copy the rest of the thing
			myKeyBone.Copy(myAction, myTargetAnimation, sourceTime, targetTime, selectiveCopy);
		}

		public KeyBone GetKeyBone(string boneName)
		{
			return KeyBone.GetKeyBone(boneName);
		}

		/// <summary>
		/// Find a keyjoint by name recursively
		/// </summary>
		/// <param name="jointName">The name of the joint to get</param>
		public KeyJoint GetKeyJoint(string jointName)
		{
			return KeyBone.GetKeyJoint(jointName);
		}

		/// <summary>
		/// Change teh time for this animation and move all the keyframes to match
		/// </summary>
		/// <param name="fTime">the new time delta of this animation</param>
		public void SetTime(float time)
		{
			//set the time in the animation itself
			KeyBone.SetTime(Length.ToFrames(), time.ToFrames());

			//grab the time
			Length = time;
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="oldName">the name of the joint to be renamed</param>
		/// <param name="newName">the new name for that joint.</param>
		public void RenameJoint(string oldName, string newName)
		{
			if (oldName == newName)
			{
				return;
			}

			KeyBone.RenameJoint(oldName, newName);
		}

		public override string ToString()
		{
			return Name;
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

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			KeyBone.MultiplyLayers(multiply);
		}

		#endregion
	}
}