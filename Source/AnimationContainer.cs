using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using GameTimer;
using FilenameBuddy;
using DrawListBuddy;
using RenderBuddy;
#if OUYA
using Ouya.Console.Api;
#endif
using Vector2Extensions;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class AnimationContainer
	{
		#region Member Variables

		/// <summary>
		/// The way the current animation is being played
		/// </summary>
		private EPlayback m_ePlayback;

		/// <summary>
		/// Index of the current animation being played
		/// </summary>
		private int m_iAnimationIndex;

		/// <summary>
		/// The amount of gravity to use for ragdoll stuff.
		/// Start out with the default, but allow the user to change it per game.
		/// </summary>
		private static Vector2 ragdollGravity = new Vector2(0.0f, Helper.RagdollGravity());

		#endregion

		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public Bone Model { get; protected set; }

		/// <summary>
		/// The current animation being played
		/// </summary>
		/// <value>The current animation.</value>
		public Animation CurrentAnimation { get; protected set; }

		/// <summary>
		/// Index of the current animation being played
		/// </summary>
		/// <value>The index of the current animation.</value>
		public int CurrentAnimationIndex 
		{
			get { return m_iAnimationIndex; }
			set
			{
				m_iAnimationIndex = value;

				//If there is an animation at that index, use it.
				if ((0 <= value) && (Animations.Count > value))
				{
					CurrentAnimation = Animations[m_iAnimationIndex];
				}
				else
				{
					CurrentAnimation = null;
				}
			}
		}

		/// 			<summary>
		/// the list of animations 
		/// </summary>
		/// <value>The animations.</value>
		public List<Animation> Animations { get; protected set; }

		/// <summary>
		/// Timer for timing the animations, both backwards and forwards
		/// </summary>
		/// <value>The stop watch.</value>
		public GameClock StopWatch { get; protected set; }

		public Filename AnimationFile { get; set; }

		public Filename ModelFile { get; set; }

		/// <summary>
		/// This flag gets set when the animation is changed, the ragdoll needs to be reset after the first update
		/// </summary>
		protected bool ResetRagdoll { get; set; }

		/// <summary>
		/// change the ragdoll gravity.
		/// </summary>
		public static float RagdollGravity
		{
			get
			{
				return ragdollGravity.Y;
			}
			set
			{
				ragdollGravity = new Vector2(0.0f, value);
			}
		}

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainer()
		{
			Model = null;
			Animations = new List<Animation>();
			CurrentAnimationIndex = -1;
			StopWatch = new GameClock();
			m_ePlayback = EPlayback.Forwards;
			ModelFile = new Filename();
			AnimationFile = new Filename();
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
			if ((null == Animations) || (null == CurrentAnimation))
			{
				return;
			}

			//update the animation clock
			StopWatch.Update(myClock);

			//apply the animation
			ApplyAnimation(GetAnimationTime(), myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
		}

		/// <summary>
		/// Get the current time of the animation.
		/// </summary>
		/// <returns>The animation time, in frames.  Will be between 0 and the length of the animation</returns>
		public int GetAnimationTime()
		{
			//This will hold the time of the animation
			float fTime = 0.0f;

			//apply the animation
			switch (m_ePlayback)
			{
				case EPlayback.Backwards:
				{
					//apply the eggtimer to the animationiterator
					fTime = CurrentAnimation.Length - StopWatch.CurrentTime;
					if (fTime < 0.0f)
					{
						fTime = 0.0f;
					}
				}
				break;

				case EPlayback.Forwards:
				{
					//apply the stop watch to the aniiterator
					fTime = StopWatch.CurrentTime;
					if (fTime > CurrentAnimation.Length)
					{
						fTime = CurrentAnimation.Length;
					}
				}
				break;

				case EPlayback.Loop:
				{
					//apply the stop watch to the aniiterator
					int iNumTimes = (int)(StopWatch.CurrentTime / CurrentAnimation.Length);
					float fTimeDiff = (CurrentAnimation.Length * (float)iNumTimes);
					fTime = (StopWatch.CurrentTime - fTimeDiff);
				}
				break;

				case EPlayback.LoopBackwards:
				{
					//apply the eggtimer to the animationiterator
					int iNumTimes = (int)(StopWatch.CurrentTime / CurrentAnimation.Length);
					float fTimeDiff = (CurrentAnimation.Length * (float)iNumTimes);
					fTime = CurrentAnimation.Length - (StopWatch.CurrentTime - fTimeDiff);
				}
				break;
			}

			return fTime.ToFrames();
		}

		/// <summary>
		/// Apply the animation at a certain time
		/// </summary>
		/// <param name="time"></param>
		/// <param name="position"></param>
		/// <param name="flip"></param>
		/// <param name="scale"></param>
		/// <param name="rotation"></param>
		/// <param name="ignoreRagdoll"></param>
		protected virtual void ApplyAnimation(
			int time,
			Vector2 position,
			bool flip,
			float scale,
			float rotation,
			bool ignoreRagdoll)
		{
			Debug.Assert(null != Model);
			Debug.Assert(null != CurrentAnimation);

			//Apply teh current animation to the bones and stuff
			Model.AnchorJoint.Position = position;
			Model.Update(time,
				position,
				CurrentAnimation.KeyBone,
				rotation,
				flip,
				0,
				scale,
				ignoreRagdoll || ResetRagdoll);

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
			Model.AccumulateForces(ragdollGravity, Helper.RagdollSpring(), fScale);

			//run the integrator
			float fTimeDelta = StopWatch.TimeDelta;
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
			//set teh stuff
			m_ePlayback = ePlaybackMode;
			CurrentAnimationIndex = iIndex;

			RestartAnimation();
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
		public void RestartAnimation()
		{
			StopWatch.Start();
			ResetRagdoll = true;
		}

		/// <summary>
		/// check if the animation is done
		/// </summary>
		public bool IsAnimationDone()
		{
			if (null != CurrentAnimation)
			{
				//check which type of playbakc we are doing
				switch (m_ePlayback)
				{
					case EPlayback.Backwards:
						{
							//check if the eggtimer has finished up
							return (CurrentAnimation.Length < StopWatch.CurrentTime);
						}

					case EPlayback.Forwards:
						{
							//check if the time delta since starting is greater than the animation length
							//that mean the animation is done playing, idjit
							return (CurrentAnimation.Length < StopWatch.CurrentTime);
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
			for (int i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].Name == strAnimationName)
				{
					return Animations[i];
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
			for (int i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].Name == strAnimationName)
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

		#region Model File IO

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="strResource">filename of the resource to load</param>
		/// <param name="rRenderer">renderer to use to load bitmap images</param>
		public bool ReadXMLModelFormat(Filename strResource, IRenderer rRenderer)
		{
			CreateBone();

			//gonna have to do this the HARD way

			//Open the file.
			#if ANDROID
			Stream stream = Game.Activity.Assets.Open(strResource.File);
			#else
			FileStream stream = File.Open(strResource.File, FileMode.Open, FileAccess.Read);
			#endif
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;

#if DEBUG
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					stream.Close();
					return false;
				}
#endif

				//make sure to read from the the next node
				if (!Model.ReadXMLFormat(rootNode.FirstChild, null, rRenderer))
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
			ModelFile.File = strResource.File;
			return true;
		}

		/// <summary>
		/// Open an xml file and dump model data to it
		/// </summary>
		/// <param name="strFileName">name of the file to dump to</param>
		public virtual void WriteModelXMLFormat(Filename strFileName, float fEnbiggify)
		{
			Debug.Assert(null != Model);

			//first rename all the joints so they are correct
			Model.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			XmlTextWriter rFile = new XmlTextWriter(strFileName.File, null);
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

		#endregion //Model File IO

		#region Animation File IO

		/// <summary>
		/// read in a list of animations from a serialized xml format file
		/// </summary>
		/// <param name="strFileName">filename of the animations to load</param>
		public virtual bool ReadXMLAnimationFormat(Filename strFileName)
		{
			Debug.Assert(null != Model); //need a model to read in animations
			Animations.Clear();

			//gonna have to do this the HARD way

			//Open the file.
			#if ANDROID
			Stream stream = Game.Activity.Assets.Open(strFileName.File);
			#else
			FileStream stream = File.Open(strFileName.File, FileMode.Open, FileAccess.Read);
			#endif
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;

#if DEBUG
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
				if (!rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//next node is "Asset"
				XmlNode AssetNode = rootNode.FirstChild;

#if DEBUG
				if (null == AssetNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!AssetNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//next node is "Animations"
				XmlNode AnimationsNode = AssetNode.FirstChild;

#if DEBUG
				if (null == AnimationsNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!AnimationsNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//the rest of the nodes are animations
				if (!ReadAnimationsNode(AnimationsNode))
				{
					Debug.Assert(false);
					return false;
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
				return false;
			}

			// Close the file.
			stream.Close();

			//grab teh filename
			AnimationFile = strFileName;

			m_ePlayback = EPlayback.Forwards;
			CurrentAnimation = null;
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
				if (!myAnimation.ReadXMLFormat(childNode, Model))
				{
					Debug.Assert(false);
					return false;
				}
				Animations.Add(myAnimation);
			}

			return true;
		}

		public void WriteXMLFormat(Filename strFileName)
		{
			//first rename all the joints so they are correct
			Debug.Assert(null != Model);
			Model.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			XmlTextWriter rXMLFile = new XmlTextWriter(strFileName.File, null);
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
			for (int i = 0; i < Animations.Count; i++)
			{
				Debug.Assert(null != Model);
				Animations[i].WriteXMLFormat(rXMLFile, Model);
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
			foreach (Animation i in Animations)
			{
				i.MultiplyLayers(iMultiply);
			}
		}

		#endregion //Animation File IO
	}
}