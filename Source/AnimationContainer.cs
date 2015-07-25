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
		private EPlayback _playback;

		/// <summary>
		/// Index of the current animation being played
		/// </summary>
		private int _animationIndex;

		#endregion

		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public Skeleton Skeleton { get; protected set; }

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
			get { return _animationIndex; }
			set
			{
				_animationIndex = value;

				//If there is an animation at that index, use it.
				if ((0 <= value) && (Animations.Count > value))
				{
					CurrentAnimation = Animations[_animationIndex];
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

		/// <summary>
		/// This flag gets set when the animation is changed, the ragdoll needs to be reset after the first update
		/// </summary>
		protected bool ResetRagdoll { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainer()
		{
			Skeleton = null;
			Animations = new List<Animation>();
			CurrentAnimationIndex = -1;
			StopWatch = new GameClock();
			_playback = EPlayback.Forwards;
			AnimationFile = new Filename();
			ResetRagdoll = false;
		}

		/// <summary>
		/// Update the model with the current animation
		/// </summary>
		/// <param name="time">the clock to update this dude with</param>
		/// <param name="position">this dude's screen location</param>
		/// <param name="isFlipped">whether or not to flip this dude on the y axis</param>
		/// <param name="scale">how much to scale the animation</param>
		/// <param name="rotation">how much to rotate the animation</param>
		/// <param name="ignoreRagdoll">whether or not to apply the ragdoll physics</param>
		public void Update(GameClock time, Vector2 position, bool isFlipped, float scale, float rotation, bool ignoreRagdoll)
		{
			if ((null == Animations) || (null == CurrentAnimation))
			{
				return;
			}

			//update the animation clock
			StopWatch.Update(time);

			//apply the animation
			ApplyAnimation(GetAnimationTime(), position, isFlipped, scale, rotation, ignoreRagdoll);
		}

		/// <summary>
		/// Get the current time of the animation.
		/// </summary>
		/// <returns>The animation time, in frames.  Will be between 0 and the length of the animation</returns>
		public int GetAnimationTime()
		{
			//This will hold the time of the animation
			var time = 0.0f;

			//apply the animation
			switch (_playback)
			{
				case EPlayback.Backwards:
				{
					//apply the eggtimer to the animationiterator
					time = CurrentAnimation.Length - StopWatch.CurrentTime;
					if (time < 0.0f)
					{
						time = 0.0f;
					}
				}
				break;

				case EPlayback.Forwards:
				{
					//apply the stop watch to the aniiterator
					time = StopWatch.CurrentTime;
					if (time > CurrentAnimation.Length)
					{
						time = CurrentAnimation.Length;
					}
				}
				break;

				case EPlayback.Loop:
				{
					//apply the stop watch to the aniiterator
					int numTimes = (int)(StopWatch.CurrentTime / CurrentAnimation.Length);
					float timeDiff = (CurrentAnimation.Length * (float)numTimes);
					time = (StopWatch.CurrentTime - timeDiff);
				}
				break;

				case EPlayback.LoopBackwards:
				{
					//apply the eggtimer to the animationiterator
					int numTimes = (int)(StopWatch.CurrentTime / CurrentAnimation.Length);
					float timeDiff = (CurrentAnimation.Length * (float)numTimes);
					time = CurrentAnimation.Length - (StopWatch.CurrentTime - timeDiff);
				}
				break;
			}

			return time.ToFrames();
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
			Debug.Assert(null != Skeleton);
			Debug.Assert(null != CurrentAnimation);

			//Apply teh current animation to the bones and stuff
			Skeleton.RootBone.AnchorJoint.Position = position;
			Skeleton.RootBone.Update(time,
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
				Skeleton.RootBone.RestartAnimation();
				ResetRagdoll = false;
			}
		}

		/// <summary>
		/// this needs to get run after the velocity is added to the model
		/// </summary>
		/// <param name="ignoreRagdoll"></param>
		/// <param name="scale">how much to scale the animation</param>
		public void UpdateRagdoll(bool ignoreRagdoll, float scale)
		{
			if (ignoreRagdoll)
			{
				return;
			}

			//add gravity to the ragdoll physics
			Skeleton.RootBone.AddGravity(RagdollConstants.Gravity);

			//accumulate all the force
			Skeleton.RootBone.AccumulateForces(RagdollConstants.Spring, scale);

			//run the integrator
			float fTimeDelta = StopWatch.TimeDelta;
			if (fTimeDelta > 0.0f)
			{
				Skeleton.RootBone.RunVerlet(fTimeDelta);
			}

			Skeleton.RootBone.SolveLimits(0.0f);
			Skeleton.RootBone.SolveConstraints(false, scale);

			//solve all the constraints
			for (int i = 0; i < 2; i++)
			{
				Skeleton.RootBone.SolveConstraints(false, scale);
			}

			//run through the post update so the matrix is correct
			Skeleton.RootBone.PostUpdate(scale, false);
		}

		/// <summary>
		/// Set the current animation
		/// </summary>
		/// <param name="index">the index of the animation to set</param>
		/// <param name="playbackMode">the playback mode to use</param>
		public void SetAnimation(int index, EPlayback playbackMode)
		{
			//set teh stuff
			_playback = playbackMode;
			CurrentAnimationIndex = index;

			RestartAnimation();
		}

		/// <summary>
		/// Render the animation out to a drawlist
		/// </summary>
		/// <param name="drawList">the drawlist to render out to</param>
		/// <param name="paletteSwap">the color to use for the palette swap</param>
		public void Render(DrawList drawList, Color paletteSwap)
		{
			if (null != Skeleton)
			{
				//send teh model to teh draw list
				Skeleton.RootBone.Render(drawList, paletteSwap);
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
				switch (_playback)
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

		public Animation FindAnimation(string animationName)
		{
			for (var i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].Name == animationName)
				{
					return Animations[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Get teh index of an animation
		/// </summary>
		/// <param name="animationName">name of teh animation to find</param>
		/// <returns>teh index of teh animation with that name, -1 if none found</returns>
		public int FindAnimationIndex(string animationName)
		{
			for (var i = 0; i < Animations.Count; i++)
			{
				if (Animations[i].Name == animationName)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Overridden methoed to create the correct type of bone
		/// </summary>
		public virtual Bone CreateBone()
		{
			return new Bone();
		}

		#endregion //Methods

		#region Skeleton File IO

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="filename">filename of the resource to load</param>
		/// <param name="renderer">renderer to use to load bitmap images</param>
		public bool ReadSkeletonXml(Filename filename, IRenderer renderer)
		{
			Skeleton = new Skeleton(this, filename, renderer);
			Skeleton.ReadXmlFile();
		}

		/// <summary>
		/// Open an xml file and dump model data to it
		/// </summary>
		/// <param name="filename">name of the file to dump to</param>
		/// <param name="scale">How much to scale the model when writing it out</param>
		public virtual void WriteSkeletonXml(Filename filename, float scale)
		{
			Skeleton.Scale = scale;
			Skeleton.Filename = filename;
			Skeleton.WriteXml();
		}

		#endregion //Skeleton File IO

		#region Animation File IO

		/// <summary>
		/// read in a list of animations from a serialized xml format file
		/// </summary>
		/// <param name="filename">filename of the animations to load</param>
		public virtual bool ReadAnimationXml(Filename filename)
		{
			Debug.Assert(null != Skeleton); //need a model to read in animations
			Animations.Clear();

			//gonna have to do this the HARD way

			//Open the file.
			#if ANDROID
			Stream stream = Game.Activity.Assets.Open(filename.File);
#else
			FileStream stream = File.Open(filename.File, FileMode.Open, FileAccess.Read);
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
				XmlNode assetNode = rootNode.FirstChild;

#if DEBUG
				if (null == assetNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!assetNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//next node is "Animations"
				XmlNode animationsNode = assetNode.FirstChild;

#if DEBUG
				if (null == animationsNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!animationsNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//the rest of the nodes are animations
				if (!ReadAnimationsNode(animationsNode))
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
			AnimationFile = filename;

			_playback = EPlayback.Forwards;
			CurrentAnimation = null;
			RestartAnimation();

			return true;
		}

		/// <summary>
		/// This node reads in all the animations from an xml node
		/// </summary>
		/// <param name="animationsNode">the xml node with all the animations underneath it</param>
		/// <returns>bool: whether or not an error occurred.</returns>
		protected virtual bool ReadAnimationsNode(XmlNode animationsNode)
		{
			for (var childNode = animationsNode.FirstChild;
				null != childNode;
				childNode = childNode.NextSibling)
			{
				Animation myAnimation = new Animation(Skeleton);
				Debug.Assert(null != Skeleton);
				if (!myAnimation.ReadXmlFormat(childNode, Skeleton))
				{
					Debug.Assert(false);
					return false;
				}
				Animations.Add(myAnimation);
			}

			return true;
		}

		public void WriteAnimationXml(Filename fileName)
		{
			//first rename all the joints so they are correct
			Debug.Assert(null != Skeleton);
			Skeleton.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			var xmlWriter = new XmlTextWriter(fileName.File, null);
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.Indentation = 1;
			xmlWriter.IndentChar = '\t';

			xmlWriter.WriteStartDocument();

			//add the xml node
			xmlWriter.WriteStartElement("XnaContent");
			xmlWriter.WriteStartElement("Asset");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.AnimationContainerXML");

			//write out joints
			xmlWriter.WriteStartElement("animations");
			for (int i = 0; i < Animations.Count; i++)
			{
				Debug.Assert(null != Skeleton);
				Animations[i].WriteXmlFormat(xmlWriter, Skeleton);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndDocument();

			// Close the file.
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			foreach (var animation in Animations)
			{
				animation.MultiplyLayers(multiply);
			}
		}

		#endregion //Animation File IO
	}
}