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

namespace AnimationLib
{
	public class Image
	{
		#region Member Variables

		//list of joint locations
		//These are the coordinates of the joints for this frame
		private List<JointData> m_listJointCoords;

		//The UV coords of this frame
		//These are where on the bitmap to draw the texture from
		private Vector2 m_UpperLeftUV;
		private Vector2 m_LowerRightUV;

		//The anchor point for this frame
		//this is where the series connects to the joint
		private Vector2 m_AnchorCoord;

		//The bitmap for this frame
		private ITexture m_Image;
		private Filename m_strFileName;

		#endregion

		#region Properties

		public Vector2 LowerRight
		{
			get { return m_LowerRightUV; }
		}

		public Filename Filename
		{
			get { return m_strFileName; }
		}

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
			get { return m_LowerRightUV.X - m_UpperLeftUV.X; }
		}

		public List<JointData> Data
		{
			get { return m_listJointCoords; }
		}

		/// <summary>
		/// Physics Data!
		/// </summary>
		/// <value>The circles.</value>
		public List<PhysicsCircle> Circles { get; private set; }

		/// <summary>
		/// physics data for level objects
		/// </summary>
		public List<PhysicsLine> Lines { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Image()
		{
			m_listJointCoords = new List<JointData>();
			Circles = new List<PhysicsCircle>();
			Lines = new List<PhysicsLine>();
			m_UpperLeftUV = new Vector2(0.0f);
			m_LowerRightUV = new Vector2(0.0f);
			m_AnchorCoord = new Vector2(0.0f);
			m_Image = null;
			m_strFileName = new Filename();
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
			Debug.Assert(iIndex < m_listJointCoords.Count);
			return m_listJointCoords[iIndex];
		}

		public void AddJoint()
		{
			JointData myData = new JointData();
			m_listJointCoords.Add(myData);
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

#if TOOLS

		public void Copy(Image myInst, CPasteAction ActionCollection)
		{
			//copy the anchor coord
			CSetAnchorLocation myAnchorAction = new CSetAnchorLocation(this, (int)myInst.AnchorCoord.X, (int)myInst.AnchorCoord.Y);
			ActionCollection.AddAction(myAnchorAction);

			//copy all the joint data
			for (int i = 0; ((i < m_listJointCoords.Count) && (i < myInst.m_listJointCoords.Count)); i++)
			{
				//create the new joint data
				JointData myNewData = new JointData();
				myNewData.Copy(myInst.m_listJointCoords[i]);

				//create the action to set it
				CSetJointData mySetJointDataAction = new CSetJointData(this, i, myNewData);
				ActionCollection.AddAction(mySetJointDataAction);
			}

			//copy circles
			for (int i = 0; ((i < Circles.Count) && (i < myInst.Circles.Count)); i++)
			{
				CCircle myCircle = new CCircle();
				myCircle.Copy(myInst.Circles[i]);

				//create the action to set it
				CSetCircleData mySetCircleAction = new CSetCircleData(this, i, myCircle);
				ActionCollection.AddAction(mySetCircleAction);
			}

			//copy lines
			for (int i = 0; ((i < Lines.Count) && (i < myInst.Lines.Count)); i++)
			{
				CLine myLine = new CLine();
				myLine.Copy(myInst.Lines[i]);

				//create the action to set it
				CSetLineData mySetCircleAction = new CSetLineData(this, i, myLine);
				ActionCollection.AddAction(mySetCircleAction);
			}
		}

#endif

		public void SetJointData(int iIndex, JointData myNewData)
		{
			Debug.Assert(iIndex >= 0);
			Debug.Assert(iIndex < m_listJointCoords.Count);
			m_listJointCoords[iIndex] = myNewData;
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
		/// Check if this image contains any physics data
		/// </summary>
		/// <returns>true if there is physics data, false if not</returns>
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

					if (strName == "upperleft")
					{
						//convert to the correct vector
						m_UpperLeftUV = strValue.ToVector2();
					}
					else if (strName == "lowerright")
					{
						//convert to the correct vector
						m_LowerRightUV = strValue.ToVector2();
					}
					else if (strName == "anchorcoord")
					{
						//convert to the correct vector
						m_AnchorCoord = strValue.ToVector2();
					}
					else if (strName == "filename")
					{
						//get the correct path & filename
						m_strFileName.SetRelFilename(strValue);

						//add the ability to have blank image, which means a skeletal structure that is not displayed
						if (m_strFileName.GetFileExt().ToString().Length > 0)
						{
							//do we need to load the image?
							if (null != rRenderer)
							{
								m_Image = rRenderer.LoadImage(m_strFileName.File);
								if (null == m_Image)
								{
									Debug.Assert(false);
									return false;
								}

								m_UpperLeftUV = Vector2.Zero;
								m_LowerRightUV = new Vector2(m_Image.Width, m_Image.Height);
							}
						}
					}
					else if (strName == "joints")
					{
						//Read in all the joint data
						if (childNode.HasChildNodes)
						{
							for (XmlNode circleNode = childNode.FirstChild;
								null != circleNode;
								circleNode = circleNode.NextSibling)
							{
								JointData childJointData = new JointData();
								if (!childJointData.ReadXMLFormat(circleNode, this))
								{
									Debug.Assert(false);
									return false;
								}
								m_listJointCoords.Add(childJointData);
							}
						}
					}
					else if (strName == "circles")
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
					else if (strName == "lines")
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
					else
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			//check if there are too many joint data objects
			while (m_listJointCoords.Count > rParent.Joints.Count)
			{
				m_listJointCoords.RemoveAt(0);
			}

			//okay if there are not enough joint data objects, add some blank ones
			while (m_listJointCoords.Count < rParent.Joints.Count)
			{
				m_listJointCoords.Add(new JointData());
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
			rXMLFile.WriteString(m_UpperLeftUV.X.ToString() + " " +
				m_UpperLeftUV.Y.ToString());
			rXMLFile.WriteEndElement();

			//write out lower right coords
			rXMLFile.WriteStartElement("lowerright");
			rXMLFile.WriteString(m_LowerRightUV.X.ToString() + " " +
				m_LowerRightUV.Y.ToString());
			rXMLFile.WriteEndElement();

			//write out lower right coords
			rXMLFile.WriteStartElement("anchorcoord");
			rXMLFile.WriteString((m_AnchorCoord.X * fEnbiggify).ToString() + " " +
				(m_AnchorCoord.Y * fEnbiggify).ToString());
			rXMLFile.WriteEndElement();

			//write out filename to use
			rXMLFile.WriteStartElement("filename");
			rXMLFile.WriteString(m_strFileName.GetRelFilename());
			rXMLFile.WriteEndElement();

			//write out joint locations
			rXMLFile.WriteStartElement("joints");
			for (int i = 0; i < m_listJointCoords.Count; i++)
			{
				m_listJointCoords[i].WriteXMLFormat(rXMLFile, fEnbiggify);
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

		/// <summary>
		/// Read in all the bone information from an object read in from a serialized XML file.
		/// </summary>
		/// <param name="rImage">the xml object to get data from</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		public bool ReadSerializedFormat(AnimationLib.ImageXML rImage, IRenderer rRenderer)
		{
			//grab all that stuff
			m_UpperLeftUV = rImage.upperleft;
			m_LowerRightUV = rImage.lowerright;
			m_AnchorCoord = rImage.anchorcoord;

			//add the ability to have blank image, which means a skeletal structure that is not displayed
			m_strFileName.SetRelFilename(rImage.filename);
			if (m_strFileName.GetFileExt().Length > 0)
			{
				if (null != rRenderer)
				{
					m_Image = rRenderer.LoadImage(m_strFileName.File);
				}
			}

			//read in joint data
			for (int i = 0; i < rImage.joints.Count; i++)
			{
				JointData myJointData = new JointData();
				AnimationLib.JointDataXML myJointDataXML = rImage.joints[i];
				if (!myJointData.ReadSerializedFormat(myJointDataXML, this))
				{
					return false;
				}
				m_listJointCoords.Add(myJointData);
			}

			//read in physics data
			for (int i = 0; i < rImage.circles.Count; i++)
			{
				PhysicsCircle myCircle = new PhysicsCircle();
				if (!myCircle.ReadSerializedFormat(rImage.circles[i]))
				{
					return false;
				}
				Circles.Add(myCircle);
			}

			//read in line data
			for (int i = 0; i < rImage.lines.Count; i++)
			{
				PhysicsLine myLine = new PhysicsLine();
				if (!myLine.ReadSerializedFormat(rImage.lines[i]))
				{
					return false;
				}
				Lines.Add(myLine);
			}

			return true;
		}

		#endregion //File IO
	}
}