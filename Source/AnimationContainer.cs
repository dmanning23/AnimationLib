using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using GameTimer;
#if NETWORKING
using Microsoft.Xna.Framework.Net;
#endif
using FilenameBuddy;
using DrawListBuddy;
using RenderBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class AnimationContainer
	{
		#region Member Variables

		//the list of animations 
		private List<Animation> m_listAnimations;

		//The current animation being played
		private Animation m_CurrentAnimation;
		private int m_iCurrentAnimationIndex;

		//Timer for timing the animations, both backwards and forwards
		private GameClock m_StopWatch;

		//The way the current animation is being played
		private EPlayback m_ePlayback;

		private Filename m_strModelFile;

		private Filename m_strAnimationFile;

		/// <summary>
		/// This flag gets set when the animation is changed, the ragdoll needs to be reset after the first update
		/// </summary>
		protected bool ResetRagdoll { get; set; }

		#endregion

		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public Bone Model { get; protected set; }

		public Animation CurrentAnimation
		{
			get { return m_CurrentAnimation; }
		}

		protected int CurrentAnimationIndex
		{
			get { return m_iCurrentAnimationIndex; }
		}

		public List<Animation> Animations
		{
			get { return m_listAnimations; }
			protected set { m_listAnimations = value; }
		}

		public GameClock StopWatch
		{
			get { return m_StopWatch; }
		}

		public Filename AnimationFile
		{
			get { return m_strAnimationFile; }
			set { m_strAnimationFile = value; }
		}

		public Filename ModelFile
		{
			get { return m_strModelFile; }
			set { m_strModelFile = value; }
		}

		public GameClock Clock
		{
			get { return m_StopWatch; }
		}

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainer()
		{
			Model = null;
			m_listAnimations = new List<Animation>();
			m_CurrentAnimation = null;
			m_iCurrentAnimationIndex = -1;
			m_StopWatch = new GameClock();
			m_ePlayback = EPlayback.Forwards;
			m_strModelFile = new Filename();
			m_strAnimationFile = new Filename();
			ResetRagdoll = false;
		}

		/// <summary>
		/// Update the model with the current animation
		/// </summary>
		/// <param name="myClock">the clock to update this dude with</param>
		/// <param name="myPosition">this dude's screen location</param>
		/// <param name="bFlip">whether or not to flip this dude on the y axis</param>
		public void Update(GameClock myClock, Vector2 myPosition, bool bFlip, float fScale, float fRotation, bool bIgnoreRagdoll)
		{
			if ((null == m_listAnimations) ||
				(null == m_CurrentAnimation))
			{
				return;
			}

			//update the animation clock
			m_StopWatch.Update(myClock);

			//apply the animation
			switch (m_ePlayback)
			{
				case EPlayback.Backwards:
					{
						//apply the eggtimer to the animationiterator
						float fTime = m_CurrentAnimation.Length - m_StopWatch.CurrentTime;
						if (fTime < 0.0f)
						{
							fTime = 0.0f;
						}
						ApplyAnimation(Helper.SecondsToFrames(fTime), myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
					}
					break;

				case EPlayback.Forwards:
					{
						//apply the stop watch to the aniiterator
						float fTime = m_StopWatch.CurrentTime;
						if (fTime > m_CurrentAnimation.Length)
						{
							fTime = m_CurrentAnimation.Length;
						}
						ApplyAnimation(Helper.SecondsToFrames(fTime), myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
					}
					break;

				case EPlayback.Loop:
					{
						//apply the stop watch to the aniiterator
						int iNumTimes = (int)(m_StopWatch.CurrentTime / m_CurrentAnimation.Length);
						float fTimeDiff = (m_CurrentAnimation.Length * (float)iNumTimes);
						float fTime = (m_StopWatch.CurrentTime - fTimeDiff);
						ApplyAnimation(Helper.SecondsToFrames(fTime), myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
					}
					break;

				case EPlayback.LoopBackwards:
					{
						//apply the eggtimer to the animationiterator
						float fTime = m_CurrentAnimation.Length - m_StopWatch.CurrentTime;
						if (fTime < 0.0f)
						{
							fTime = 0.0f;
							m_StopWatch.Start();
						}
						ApplyAnimation(Helper.SecondsToFrames(fTime), myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
					}
					break;
			}
		}

		/// <summary>
		/// Apply the animation at a certain time
		/// </summary>
		/// <param name="iTime">the time of teh animation to set</param>
		/// <param name="rMatrix">the matrix to transform the model by</param>
		protected virtual void ApplyAnimation(
			int iTime,
			Vector2 myPosition,
			bool bFlip,
			float fScale,
			float fRotation,
			bool bIgnoreRagdoll)
		{
			Debug.Assert(null != Model);

			//Apply teh current animation to the bones and stuff
			Model.AnchorJoint.Position = myPosition;
			Model.Update(iTime,
				myPosition,
				m_CurrentAnimation.KeyBone,
				fRotation,
				bFlip,
				0,
				fScale,
				bIgnoreRagdoll || ResetRagdoll);

			//is this the first update after an animation change?
			if (ResetRagdoll)
			{
				Model.RestartAnimation();
				ResetRagdoll = false;
			}
		}

		/// <summary>
		/// this needs to get run after the velocity is added to the model
		/// </summary>
		/// <param name="bIgnoreRagdoll"></param>
		public void UpdateRagdoll(bool bIgnoreRagdoll, float fScale)
		{
			if (bIgnoreRagdoll)
			{
				return;
			}

			//accumulate all the force
			Vector2 gravity = Vector2.Zero;
			gravity.Y = Helper.RagdollGravity();
			Model.AccumulateForces(gravity);

			//run the integrator
			float fTimeDelta = m_StopWatch.TimeDelta;
			if (fTimeDelta > 0.0f)
			{
				Model.RunVerlet(fTimeDelta);
			}

			Model.SolveLimits(0.0f);
			Model.SolveConstraints(false, fScale);

			//solve all the constraints
			for (int i = 0; i < 2; i++)
			{
				Model.SolveConstraints(false, fScale);
			}

			//run through the post update so the matrix is correct
			Model.PostUpdate(fScale, false);
		}

		/// <summary>
		/// Set the current animation
		/// </summary>
		/// <param name="iIndex">the index of the animation to set</param>
		/// <param name="ePlaybackMode">the playback mode to use</param>
		public void SetAnimation(int iIndex, EPlayback ePlaybackMode)
		{
			if ((iIndex >= 0) && (iIndex < m_listAnimations.Count))
			{
				//set teh stuff
				m_ePlayback = ePlaybackMode;
				m_CurrentAnimation = m_listAnimations[iIndex];
				m_iCurrentAnimationIndex = iIndex;

				RestartAnimation();
			}
		}

		/// <summary>
		/// Render the animation out to a drawlist
		/// </summary>
		/// <param name="rDrawList">the drawlist to render out to</param>
		public void Render(DrawList rDrawList, Color PaletteSwap)
		{
			if (null != Model)
			{
				//send teh model to teh draw list
				Model.Render(rDrawList, PaletteSwap);
			}
		}

		/// <summary>
		/// Restart the animation
		/// </summary>
		private void RestartAnimation()
		{
			m_StopWatch.Start();
			ResetRagdoll = true;
		}

		/// <summary>
		/// check if the animation is done
		/// </summary>
		public bool IsAnimationDone()
		{
			if (null != m_CurrentAnimation)
			{
				//check which type of playbakc we are doing
				switch (m_ePlayback)
				{
					case EPlayback.Backwards:
						{
							//check if the eggtimer has finished up
							return (m_CurrentAnimation.Length < m_StopWatch.CurrentTime);
						}

					case EPlayback.Forwards:
						{
							//check if the time delta since starting is greater than the animation length
							//that mean the animation is done playing, idjit
							return (m_CurrentAnimation.Length < m_StopWatch.CurrentTime);
						}

					case EPlayback.Loop:
						{
							//if it is just looping, it means the thing is done playing
							return true;
						}

					case EPlayback.LoopBackwards:
						{
							//if it is just looping, it means the thing is done playing
							return true;
						}
				}
			}

			return true;
		}

		public Animation FindAnimation(string strAnimationName)
		{
			for (int i = 0; i < m_listAnimations.Count; i++)
			{
				if (m_listAnimations[i].Name == strAnimationName)
				{
					return m_listAnimations[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Get teh index of an animation
		/// </summary>
		/// <param name="strAnimationName">name of teh animation to find</param>
		/// <returns>teh index of teh animation with that name, -1 if none found</returns>
		public int FindAnimationIndex(string strAnimationName)
		{
			for (int i = 0; i < m_listAnimations.Count; i++)
			{
				if (m_listAnimations[i].Name == strAnimationName)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Overridden methoed to create the correct type of bone
		/// </summary>
		protected virtual void CreateBone()
		{
			Debug.Assert(null == Model);
			Model = new Bone();
		}

		public override string ToString()
		{
			return "Root";
		}

		#endregion //Methods

		#region Networking

#if NETWORKING

		/// <summary>
		/// Read this object from a network packet reader.
		/// </summary>
		public void ReadFromNetwork(PacketReader packetReader)
		{
			int iIndex = packetReader.ReadInt32();
			EPlayback ePlaybackMode = (EPlayback)packetReader.ReadInt32();

			if ((m_iCurrentAnimationIndex != iIndex) || (m_ePlayback != ePlaybackMode))
			{
				SetAnimation(iIndex, ePlaybackMode);
			}

			m_StopWatch.ReadFromNetwork(packetReader);
		}

		/// <summary>
		/// Write this object to a network packet reader.
		/// </summary>
		public void WriteToNetwork(PacketWriter packetWriter)
		{
			packetWriter.Write(m_iCurrentAnimationIndex);
			packetWriter.Write((int)m_ePlayback);
			m_StopWatch.WriteToNetwork(packetWriter);
		}

#endif

		#endregion //Networking

		#region Model File IO

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="strResource">filename of the resource to load</param>
		/// <param name="rRenderer">renderer to use to load bitmap images</param>
		public bool ReadSerializedModelFormat(string strResource, IRenderer rRenderer)
		{
			CreateBone();

			//gonna have to do this the HARD way

			//Open the file.
			FileStream stream = File.Open(strResource, FileMode.Open, FileAccess.Read);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					stream.Close();
					return false;
				}

				//make sure to read from the the next node
				if (!Model.ReadSerializedFormat(rootNode.FirstChild, null, rRenderer))
				{
					Debug.Assert(false);
					stream.Close();
					return false;
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
				stream.Close();
				return false;
			}

			// Close the file.
			stream.Close();

			//grab that filename 
			m_strModelFile.File = strResource;
			return true;
		}

		/// <summary>
		/// Open an xml file and dump model data to it
		/// </summary>
		/// <param name="strFileName">name of the file to dump to</param>
		public virtual void WriteModelXMLFormat(string strFileName, float fEnbiggify)
		{
			Debug.Assert(null != Model);

			//first rename all the joints so they are correct
			Model.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			XmlTextWriter rFile = new XmlTextWriter(strFileName, null);
			rFile.Formatting = Formatting.Indented;
			rFile.Indentation = 1;
			rFile.IndentChar = '\t';

			rFile.WriteStartDocument();
			Model.WriteXMLFormat(rFile, true, fEnbiggify);
			rFile.WriteEndDocument();

			// Close the file.
			rFile.Flush();
			rFile.Close();
		}

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="rContent">content loader to use</param>
		/// <param name="strResource">name of the resource to load</param>
		/// <param name="rRenderer">renderer to use to load bitmap images</param>
		public virtual bool ReadSerializedModelFormat(ContentManager rXmlContent, string strResource, IRenderer rRenderer)
		{
			CreateBone();

			Debug.Assert(null != rXmlContent);
			AnimationLib.BoneXML rBoneXML = rXmlContent.Load<AnimationLib.BoneXML>(strResource);
			if (!Model.ReadSerializedFormat(rBoneXML, null, rRenderer))
			{
				Debug.Assert(false);
				return false;
			}

			m_strModelFile.File = strResource;
			return true;
		}

		#endregion //Model File IO

		#region Animation File IO

		/// <summary>
		/// read in a list of animations from a serialized xml format file
		/// </summary>
		/// <param name="strFileName">filename of the animations to load</param>
		public virtual bool ReadSerializedAnimationFormat(string strFileName)
		{
			Debug.Assert(null != Model); //need a model to read in animations
			m_listAnimations.Clear();

			//gonna have to do this the HARD way

			//Open the file.
			FileStream stream = File.Open(strFileName, FileMode.Open, FileAccess.Read);
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					return false;
				}
				if (!rootNode.HasChildNodes)
				{
					return false;
				}

				//next node is "Asset"
				XmlNode AssetNode = rootNode.FirstChild;
				if (null == AssetNode)
				{
					return false;
				}
				if (!AssetNode.HasChildNodes)
				{
					return false;
				}

				//next node is "Animations"
				XmlNode AnimationsNode = AssetNode.FirstChild;
				if (null == AnimationsNode)
				{
					return false;
				}
				if (!AnimationsNode.HasChildNodes)
				{
					return false;
				}

				//the rest of the nodes are animations
				if (!ReadAnimationsNode(AnimationsNode))
				{
					return false;
				}
			}
			else
			{
				//should be an xml node!!!
				return false;
			}

			// Close the file.
			stream.Close();

			//grab teh filename
			m_strAnimationFile.File = strFileName;

			m_ePlayback = EPlayback.Forwards;
			m_CurrentAnimation = null;
			RestartAnimation();

			return true;
		}

		/// <summary>
		/// This node reads in all the animations from an xml node
		/// </summary>
		/// <param name="AnimationsNode">the xml node with all the animations underneath it</param>
		/// <returns>bool: whether or not an error occurred.</returns>
		protected virtual bool ReadAnimationsNode(XmlNode AnimationsNode)
		{
			for (XmlNode childNode = AnimationsNode.FirstChild;
				null != childNode;
				childNode = childNode.NextSibling)
			{
				Animation myAnimation = new Animation(Model);
				Debug.Assert(null != Model);
				if (!myAnimation.ReadSerializedFormat(childNode, Model))
				{
					return false;
				}
				m_listAnimations.Add(myAnimation);
			}

			return true;
		}

		public void WriteXMLFormat(string strFileName)
		{
			//first rename all the joints so they are correct
			Debug.Assert(null != Model);
			Model.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			XmlTextWriter rXMLFile = new XmlTextWriter(strFileName, null);
			rXMLFile.Formatting = Formatting.Indented;
			rXMLFile.Indentation = 1;
			rXMLFile.IndentChar = '\t';

			rXMLFile.WriteStartDocument();

			//add the xml node
			rXMLFile.WriteStartElement("XnaContent");
			rXMLFile.WriteStartElement("Asset");
			rXMLFile.WriteAttributeString("Type", "AnimationLib.AnimationContainerXML");

			//write out joints
			rXMLFile.WriteStartElement("animations");
			for (int i = 0; i < m_listAnimations.Count; i++)
			{
				Debug.Assert(null != Model);
				m_listAnimations[i].WriteXMLFormat(rXMLFile, Model);
			}
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndElement();
			rXMLFile.WriteEndElement();

			rXMLFile.WriteEndDocument();

			// Close the file.
			rXMLFile.Flush();
			rXMLFile.Close();
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="fMultiply"></param>
		public void MultiplyLayers(int iMultiply)
		{
			foreach (Animation i in m_listAnimations)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

		/// <summary>
		/// read in a list of animations from a resource
		/// </summary>
		/// <param name="rContent">content loader to use</param>
		/// <param name="strResource">name of the resource</param>
		public virtual void ReadSerializedAnimationFormat(ContentManager rXmlContent, string strResource)
		{
			m_listAnimations.Clear();

			//load the resource
			AnimationLib.AnimationContainerXML myDude = rXmlContent.Load<AnimationLib.AnimationContainerXML>(strResource);

			//set up all the animations
			for (int i = 0; i < myDude.animations.Count; i++)
			{
				Animation myAnimation = new Animation(Model);
				AnimationLib.AnimationXML myAnimationXML = myDude.animations[i];
				Debug.Assert(null != Model);
				myAnimation.ReadSerializedAnimationFormat(myAnimationXML, Model);
				m_listAnimations.Add(myAnimation);
			}

			m_strAnimationFile.File = strResource;
			m_ePlayback = EPlayback.Forwards;
			m_CurrentAnimation = null;
			RestartAnimation();
		}

		#endregion //Animation File IO
	}
}