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

		public Animation(Bone rBoneStructure)
		{
			Debug.Assert(null != rBoneStructure);
			KeyBone = new KeyBone(rBoneStructure);
			Length = 0.0f;
		}

		public bool AddKeyframe(KeyElement rKey)
		{
			//find the correct keyjoint
			KeyJoint MyKeyJoint = KeyBone.GetKeyJoint(rKey.JointName);
			if (null != MyKeyJoint)
			{
				//add the keyframe to the keyjoint
				MyKeyJoint.AddKeyElement(rKey);
				return true;
			}
			return false;
		}

		public bool RemoveKeyframe(KeyElement rKey)
		{
			//find the correct keyjoint
			KeyJoint MyKeyJoint = KeyBone.GetKeyJoint(rKey.JointName);
			if (null != MyKeyJoint)
			{
				//remove all keyframes at that time
				return MyKeyJoint.RemoveKeyElement(rKey.Time);
			}
			return false;
		}

		public bool RemoveKeyframe(Macro myAction, Bone CopyBone, int iTime)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(CopyBone.Name);
			if (null == myKeyBone)
			{
				return false;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(CopyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.RemoveKeyElement(myAction, iTime, this);
			}

			//copy the rest of the thing
			myKeyBone.RemoveKeyElement(myAction, iTime, this);
			return true;
		}

		/// <summary>
		/// Copy the skeleton from one animation into another animation
		/// </summary>
		/// <param name="myAction">undo/redo action to put all the changes into</param>
		/// <param name="CopyBone">bone that is being copied</param>
		/// <param name="myTargetAnimation">animation to paste that bone into</param>
		/// <param name="iSourceTime">the time to copy from this animation</param>
		/// <param name="iTargetTime">the time to paste into the other animation</param>
		/// <param name="bSelectiveCopy">if this is true, it means only copy image, layer, ragdoll, flip</param>
		public void Copy(Macro myAction, 
		                 Bone CopyBone, 
		                 Animation myTargetAnimation, 
		                 int iSourceTime, 
		                 int iTargetTime,
		                 bool bSelectiveCopy)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(CopyBone.Name);
			if (null == myKeyBone)
			{
				return;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(CopyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.Copy(myAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);
			}

			//copy the rest of the thing
			myKeyBone.Copy(myAction, myTargetAnimation, iSourceTime, iTargetTime, bSelectiveCopy);
		}

		public KeyBone GetKeyBone(string strBoneName)
		{
			return KeyBone.GetKeyBone(strBoneName);
		}

		/// <summary>
		/// Find a keyjoint by name recursively
		/// </summary>
		/// <param name="strJointName">The name of the joint to get</param>
		public KeyJoint GetKeyJoint(string strJointName)
		{
			return KeyBone.GetKeyJoint(strJointName);
		}

		/// <summary>
		/// Change teh time for this animation and move all the keyframes to match
		/// </summary>
		/// <param name="fTime">the new time delta of this animation</param>
		public void SetTime(float fTime)
		{
			//set the time in the animation itself
			KeyBone.SetTime(Helper.SecondsToFrames(Length), Helper.SecondsToFrames(fTime));

			//grab the time
			Length = fTime;
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="strOldName">the name of the joint to be renamed</param>
		/// <param name="strNewName">the new name for that joint.</param>
		public void RenameJoint(string strOldName, string strNewName)
		{
			if (strOldName == strNewName)
			{
				return;
			}

			KeyBone.RenameJoint(strOldName, strNewName);
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
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode, Bone rModel)
		{
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
					if ("AnimationLib.AnimationXML" != strValue)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			//make sure to setup the bones model
			KeyBone = new KeyBone(rModel);

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

					if (strName == "name")
					{
						Name = strValue;
					}
					else if (strName == "length")
					{
						Length = Convert.ToSingle(strValue);
					}
					else if (strName == "keys")
					{
						//get the animation length in frames
						int iLengthFrames = Helper.SecondsToFrames(Length);

						if (childNode.HasChildNodes)
						{
							for (XmlNode keyNode = childNode.FirstChild;
								null != keyNode;
								keyNode = keyNode.NextSibling)
							{
								//read in the key element
								KeyElement myKey = new KeyElement();
								if (!myKey.ReadXMLFormat(keyNode))
								{
									Debug.Assert(false);
									return false;
								}

								//is this keyelement worth keeping?
								if ((myKey.KeyFrame) && (iLengthFrames >= myKey.Time))
								{
									//set the image index
									Bone rMyBone = rModel.GetBone(myKey.JointName);
									if (rMyBone != null)
									{
										myKey.ImageIndex = rMyBone.GetImageIndex(myKey.ImageName);
									}

									//add to the correct keyjoint
									KeyJoint myKeyJoint = KeyBone.GetKeyJoint(myKey.JointName);
									if (null != myKeyJoint)
									{
										myKeyJoint.AddKeyElement(myKey);
									}
								}
							}
						}
					}
				}
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="rXMLFile">the xml file to add this dude as a child of</param>
		public void WriteXMLFormat(XmlTextWriter rXMLFile, Bone rModel)
		{
			Debug.Assert(null != rModel);

			//first get a list of all the keyframes
			AnimationLib.AnimationXML myAnimationXML = new AnimationLib.AnimationXML();
			KeyBone.WriteXMLFormat(myAnimationXML, rModel);

			//now sort all those keyframes
			myAnimationXML.keys.Sort(new KeyXMLSort());

			//write out the item tag
			rXMLFile.WriteStartElement("Item");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.AnimationXML");

			//write out animation name
			rXMLFile.WriteStartElement("name");
			rXMLFile.WriteString(Name);
			rXMLFile.WriteEndElement();

			//write out animation length
			rXMLFile.WriteStartElement("length");
			rXMLFile.WriteString(Length.ToString());
			rXMLFile.WriteEndElement();

			//write out all the keyframes
			rXMLFile.WriteStartElement("keys");
			for (int i = 0; i < myAnimationXML.keys.Count; i++)
			{
				rXMLFile.WriteStartElement("Item");
				rXMLFile.WriteAttributeString("Type", "AnimationLib.KeyXML");

				rXMLFile.WriteStartElement("time");
				rXMLFile.WriteString(myAnimationXML.keys[i].time.ToString());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("rotation");
				rXMLFile.WriteString(myAnimationXML.keys[i].rotation.ToString());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("layer");
				rXMLFile.WriteString(myAnimationXML.keys[i].layer.ToString());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("image");
				rXMLFile.WriteString(myAnimationXML.keys[i].image.ToString());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("flip");
				rXMLFile.WriteString(myAnimationXML.keys[i].flip ? "true" : "false");
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("translation");
				rXMLFile.WriteString(myAnimationXML.keys[i].translation.StringFromVector());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("ragdoll");
				rXMLFile.WriteString(myAnimationXML.keys[i].ragdoll ? "true" : "false");
				rXMLFile.WriteEndElement();

				rXMLFile.WriteStartElement("joint");
				rXMLFile.WriteString(myAnimationXML.keys[i].joint.ToString());
				rXMLFile.WriteEndElement();

				rXMLFile.WriteEndElement();
			}
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			KeyBone.MultiplyLayers(iMultiply);
		}

		public void ReadSerializedFormat(AnimationLib.AnimationXML myDude, Bone rModel)
		{
			Name = myDude.name;
			Length = myDude.length;
			KeyBone = new KeyBone(rModel);

			for (int i = 0; i < myDude.keys.Count; i++)
			{
				//find a joint that uses this key
				KeyJoint myKeyJoint = KeyBone.GetKeyJoint(myDude.keys[i].joint);
				if (null != myKeyJoint)
				{
					AnimationLib.KeyXML myKeyXML = myDude.keys[i];
					KeyElement myElement = myKeyJoint.ReadSerializedAnimationFormat(myKeyXML);

					//ok set teh image index after its been read in
					Bone rMyBone = rModel.GetBone(myElement.JointName);
					if (rMyBone != null)
					{
						myElement.ImageIndex = rMyBone.GetImageIndex(myElement.ImageName);
					}
				}
			}
		}

		#endregion
	}
}