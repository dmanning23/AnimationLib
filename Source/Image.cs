using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;
using FilenameBuddy;
using CollisionBuddy;
using DrawListBuddy;
using BasicPrimitiveBuddy;
using Microsoft.Xna.Framework.Graphics;
using Vector2Extensions;
using RenderBuddy;
using UndoRedoBuddy;
using AnimationLib.Commands;

namespace AnimationLib
{
	public class Image
	{
		#region Member Variables

		//The UV coords of this frame
		//These are where on the bitmap to draw the texture from
		private Vector2 m_UpperLeftUV;
		private Vector2 m_LowerRightUV;

		//The anchor point for this frame
		//this is where the series connects to the joint
		private Vector2 m_AnchorCoord;

		//The bitmap for this frame
		private ITexture m_Image;

		#endregion

		#region Properties

		public Vector2 UpperLeft
		{
			get { return m_UpperLeftUV; }
			set { m_UpperLeftUV = value; }
		}

		public Vector2 LowerRight
		{
			get { return m_LowerRightUV; }
			set { m_LowerRightUV = value; }
		}

		public Filename ImageFile { get; set; }

		/// <summary>
		/// Get or set the m_AnchorCoord member variable
		/// </summary>
		public Vector2 AnchorCoord
		{
			get { return m_AnchorCoord; }
			set { m_AnchorCoord = value; }
		}

		public float Width
		{
			get { return LowerRight.X - UpperLeft.X; }
		}

		/// <summary>
		/// list of joint locations
		/// These are the coordinates of the joints for this frame
		/// </summary>
		/// <value>The JointCoords.</value>
		public List<JointData> JointCoords { get; protected set; }

		/// <summary>
		/// Physics JointCoords!
		/// </summary>
		/// <value>The circles.</value>
		public List<PhysicsCircle> Circles { get; private set; }

		/// <summary>
		/// physics JointCoords for level objects
		/// </summary>
		public List<PhysicsLine> Lines { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Image()
		{
			JointCoords = new List<JointData>();
			Circles = new List<PhysicsCircle>();
			Lines = new List<PhysicsLine>();
			UpperLeft = Vector2.Zero;
			LowerRight = Vector2.Zero;
			m_AnchorCoord = Vector2.Zero;
			m_Image = null;
			ImageFile = new Filename();
		}

		/// <summary>
		/// Render this image
		/// </summary>
		/// <param name="rMatrix">The matrix to rtansform by</param>
		/// <param name="rDrawList">the drawlist to put it in</param>
		/// <param name="iLayer">the layer to put the image at</param>
		public void Render(Vector2 myPosition, DrawList rDrawList, int iLayer, float fRotation, bool bFlip, Color myColor)
		{
			if (null != m_Image)
			{
				rDrawList.AddQuad(m_Image, myPosition, myColor, fRotation, bFlip, iLayer);
			}
		}

		/// <summary>
		/// Get a joint vector from this dude
		/// </summary>
		/// <param name="iIndex">teh index of the joint location to get</param>
		public JointData GetJointLocation(int iIndex)
		{
			Debug.Assert(iIndex >= 0);
			Debug.Assert(iIndex < JointCoords.Count);
			return JointCoords[iIndex];
		}

		public void AddJoint()
		{
			JointData myJointCoords = new JointData();
			JointCoords.Add(myJointCoords);
		}

		/// <summary>
		/// Gets the flipped anchor coordinate.
		/// </summary>
		/// <returns>The flipped anchor coordinate.</returns>
		/// <param name="bFlipped">Whether or not we want the anchor coord flipped</param>
		/// <param name="fScale">The scale to get the anchor coord</param>
		public Vector2 GetFlippedAnchorCoord(bool bFlipped, float fScale)
		{
			//Get the anchor coord
			Vector2 updatedAnchorCoord = new Vector2(0.0f);
			if (bFlipped)
			{
				//flip the x coord
				updatedAnchorCoord.X = Width - AnchorCoord.X;
				updatedAnchorCoord.Y = AnchorCoord.Y;
			}
			else
			{
				updatedAnchorCoord = AnchorCoord;
			}

			updatedAnchorCoord *= fScale;
			return updatedAnchorCoord;
		}

		/// <summary>
		/// Gets the flipped anchor coordinate.
		/// </summary>
		/// <returns>The flipped anchor coordinate.</returns>
		/// <param name="bFlipped">Whether or not we want the anchor coord flipped</param>
		/// <param name="fScale">The scale to get the anchor coord</param>
		public Vector2 GetFlippedJointCoord(int iJointIndex, bool bFlipped, float fScale)
		{
			Vector2 updatedJointCoord = GetJointLocation(iJointIndex).Location;
			if (bFlipped)
			{
				//flip the x coord
				updatedJointCoord.X = Width - updatedJointCoord.X;
			}

			updatedJointCoord *= fScale;
			return updatedJointCoord;
		}

		public void Copy(Image myInst, Macro ActionCollection)
		{
			//copy the anchor coord
			SetAnchorLocation myAnchorAction = new SetAnchorLocation(this, (int)myInst.AnchorCoord.X, (int)myInst.AnchorCoord.Y);
			ActionCollection.Add(myAnchorAction);

			//copy all the joint JointCoords
			for (int i = 0; ((i < JointCoords.Count) && (i < myInst.JointCoords.Count)); i++)
			{
				//create the new joint JointCoords
				JointData myNewJointCoords = new JointData();
				myNewJointCoords.Copy(myInst.JointCoords[i]);

				//create the action to set it
				SetJointCoords mySetJointJointCoordsAction = new SetJointCoords(this, i, myNewJointCoords);
				ActionCollection.Add(mySetJointJointCoordsAction);
			}

			//copy circles
			for (int i = 0; ((i < Circles.Count) && (i < myInst.Circles.Count)); i++)
			{
				PhysicsCircle myCircle = new PhysicsCircle();
				myCircle.Copy(myInst.Circles[i]);

				//create the action to set it
				SetCircleData mySetCircleAction = new SetCircleData(this, i, myCircle);
				ActionCollection.Add(mySetCircleAction);
			}

			//copy lines
			for (int i = 0; ((i < Lines.Count) && (i < myInst.Lines.Count)); i++)
			{
				PhysicsLine myLine = new PhysicsLine();
				myLine.Copy(myInst.Lines[i]);

				//create the action to set it
				SetLineData mySetCircleAction = new SetLineData(this, i, myLine);
				ActionCollection.Add(mySetCircleAction);
			}
		}

		public void SetJointCoords(int iIndex, JointData myNewJointCoords)
		{
			Debug.Assert(iIndex >= 0);
			Debug.Assert(iIndex < JointCoords.Count);
			JointCoords[iIndex] = myNewJointCoords;
		}

		public void Update(Vector2 BonePosition, float fRotation, bool bFlip, float fScale)
		{
			//update all the circles
			for (int i = 0; i < Circles.Count; i++)
			{
				Debug.Assert(null != Circles);
				Debug.Assert(null != Circles[i]);
				Circles[i].Update(this, BonePosition, fRotation, bFlip, fScale);
			}

			//update all the lines
			for (int i = 0; i < Lines.Count; i++)
			{
				Lines[i].Update(this, BonePosition, fRotation, bFlip, fScale);
			}
		}

		public void DrawPhysics(IRenderer rRenderer, Color rColor)
		{
			for (int i = 0; i < Circles.Count; i++)
			{
				Circles[i].Render(rRenderer, rColor);
			}

			for (int i = 0; i < Lines.Count; i++)
			{
				Lines[i].Render(rRenderer, rColor);
			}
		}

		/// <summary>
		/// Check if this image contains any physics JointCoords
		/// </summary>
		/// <returns>true if there is physics JointCoords, false if not</returns>
		public bool HasPhysics()
		{
			return ((Circles.Count > 0) || (Lines.Count > 0));
		}

		#endregion //methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXMLFormat(XmlNode rXMLNode, IRenderer rRenderer, Bone rParent)
		{
			Debug.Assert(null != rParent);

#if DEBUG
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
					if ("AnimationLib.ImageXML" != strValue)
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

					switch (strName)
					{
						case "upperleft":
						{
							//convert to the correct vector
							UpperLeft = strValue.ToVector2();
						}
						break;
						case "lowerright":
						{
							//convert to the correct vector
							LowerRight = strValue.ToVector2();
						}
						break;
						case "anchorcoord":
						{
							//convert to the correct vector
							m_AnchorCoord = strValue.ToVector2();
						}
						break;
						case "filename":
						{
							//get the correct path & filename
							ImageFile.SetRelFilename(strValue);

							//add the ability to have blank image, which means a skeletal structure that is not displayed
							if (ImageFile.GetFileExt().ToString().Length > 0)
							{
								//do we need to load the image?
								if (null != rRenderer)
								{
									m_Image = rRenderer.LoadImage(ImageFile.File);
									if (null == m_Image)
									{
										Debug.Assert(false);
										return false;
									}

									UpperLeft = Vector2.Zero;
									LowerRight = new Vector2(m_Image.Width, m_Image.Height);
								}
							}
						}
						break;
						case "joints":
						{
							//Read in all the joint JointCoords
							if (childNode.HasChildNodes)
							{
								for (XmlNode circleNode = childNode.FirstChild;
									null != circleNode;
									circleNode = circleNode.NextSibling)
								{
									JointData childJointJointCoords = new JointData();
									if (!childJointJointCoords.ReadXMLFormat(circleNode, this))
									{
										Debug.Assert(false);
										return false;
									}
									JointCoords.Add(childJointJointCoords);
								}
							}
						}
						break;
						case "circles":
						{
							//Read in all the circles
							if (childNode.HasChildNodes)
							{
								for (XmlNode jointNode = childNode.FirstChild;
									null != jointNode;
									jointNode = jointNode.NextSibling)
								{
									PhysicsCircle myCircle = new PhysicsCircle();
									if (!myCircle.ReadXMLFormat(jointNode))
									{
										Debug.Assert(false);
										return false;
									}
									Circles.Add(myCircle);
								}
							}
						}
						break;
						case "lines":
						{
							//Read in all the lines
							if (childNode.HasChildNodes)
							{
								for (XmlNode jointNode = childNode.FirstChild;
									null != jointNode;
									jointNode = jointNode.NextSibling)
								{
									PhysicsLine myCircle = new PhysicsLine();
									if (!myCircle.ReadXMLFormat(jointNode))
									{
										Debug.Assert(false);
										return false;
									}
									Lines.Add(myCircle);
								}
							}
						}
						break;
						default:
						{
							Debug.Assert(false);
							return false;
						}
					}
				}
			}

			//check if there are too many joint JointCoords objects
			while (JointCoords.Count > rParent.Joints.Count)
			{
				JointCoords.RemoveAt(0);
			}

			//okay if there are not enough joint JointCoords objects, add some blank ones
			while (JointCoords.Count < rParent.Joints.Count)
			{
				JointCoords.Add(new JointData());
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
			rXMLFile.WriteAttributeString("Type", "AnimationLib.ImageXML");

			//write out upper left coords
			rXMLFile.WriteStartElement("upperleft");
			rXMLFile.WriteString(UpperLeft.X.ToString() + " " +
				UpperLeft.Y.ToString());
			rXMLFile.WriteEndElement();

			//write out lower right coords
			rXMLFile.WriteStartElement("lowerright");
			rXMLFile.WriteString(LowerRight.X.ToString() + " " +
				LowerRight.Y.ToString());
			rXMLFile.WriteEndElement();

			//write out lower right coords
			rXMLFile.WriteStartElement("anchorcoord");
			rXMLFile.WriteString((m_AnchorCoord.X * fEnbiggify).ToString() + " " +
				(m_AnchorCoord.Y * fEnbiggify).ToString());
			rXMLFile.WriteEndElement();

			//write out filename to use
			rXMLFile.WriteStartElement("filename");
			rXMLFile.WriteString(ImageFile.GetRelFilename());
			rXMLFile.WriteEndElement();

			//write out joint locations
			rXMLFile.WriteStartElement("joints");
			for (int i = 0; i < JointCoords.Count; i++)
			{
				JointCoords[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			//write out polygon info
			rXMLFile.WriteStartElement("circles");
			for (int i = 0; i < Circles.Count; i++)
			{
				Circles[i].WriteXMLFormat(rXMLFile);
			}
			rXMLFile.WriteEndElement();

			rXMLFile.WriteStartElement("lines");
			for (int i = 0; i < Lines.Count; i++)
			{
				Lines[i].WriteXMLFormat(rXMLFile);
			}
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
		}

		#endregion //File IO
	}
}