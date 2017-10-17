using DrawListBuddy;
using System.Linq;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RenderBuddy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		/// The name of this animation container.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return "Base";
			}
		}

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
		protected int CurrentAnimationIndex 
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

		public Filename SkeletonFile { get; set; }

		public Filename AnimationFile { get; set; }

		/// <summary>
		/// This flag gets set when the animation is changed, the ragdoll needs to be reset after the first update
		/// </summary>
		protected bool ResetRagdoll { get; set; }

		public float Scale { get; set; }

		#endregion

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainer(float scale = 1f)
		{
			Skeleton = new Skeleton(this);
			Animations = new List<Animation>();
			CurrentAnimationIndex = -1;
			StopWatch = new GameClock();
			_playback = EPlayback.Forwards;
			AnimationFile = new Filename();
			ResetRagdoll = false;
			Scale = scale;
		}

		public AnimationContainer(AnimationsModel animations, SkeletonModel skeleton, IRenderer renderer)
			: this()
		{
			Load(animations, skeleton, renderer);
		}

		protected void Load(AnimationsModel animations, SkeletonModel skeleton, IRenderer renderer)
		{
			Skeleton.Load(skeleton, renderer);
			foreach (var animation in animations.Animations)
			{
				Animations.Add(new Animation(Skeleton, animation));
			}
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Update the model with the current animation
		/// </summary>
		/// <param name="time">the clock to update this dude with</param>
		/// <param name="position">this dude's screen location</param>
		/// <param name="isFlipped">whether or not to flip this dude on the y axis</param>
		/// <param name="rotation">how much to rotate the animation</param>
		/// <param name="ignoreRagdoll">whether or not to apply the ragdoll physics</param>
		public void Update(GameClock time, Vector2 position, bool isFlipped, float rotation, bool ignoreRagdoll)
		{
			if ((null == Animations) || (null == CurrentAnimation))
			{
				return;
			}

			//update the animation clock
			StopWatch.Update(time);

			//apply the animation
			ApplyAnimation(GetAnimationTime(), position, isFlipped, rotation, ignoreRagdoll);
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
		/// <param name="rotation"></param>
		/// <param name="ignoreRagdoll"></param>
		protected virtual void ApplyAnimation(
			int time,
			Vector2 position,
			bool flip,
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
		public void UpdateRagdoll()
		{
			//add gravity to the ragdoll physics
			Skeleton.RootBone.AddGravity();

			//accumulate all the force
			Skeleton.RootBone.AccumulateForces(0f);

			//run the integrator
			float fTimeDelta = StopWatch.TimeDelta;
			if (fTimeDelta > 0.0f)
			{
				Skeleton.RootBone.RunVerlet(fTimeDelta);
			}

			Skeleton.RootBone.SolveLimits(0.0f);
			Skeleton.RootBone.SolveConstraints(false);

			//solve all the constraints
			for (int i = 0; i < 2; i++)
			{
				Skeleton.RootBone.SolveConstraints(false);
			}

			//run through the post update so the matrix is correct
			Skeleton.RootBone.PostUpdate(false);
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
		public void Render(DrawList drawList)
		{
			if (null != Skeleton &&
				null != Skeleton.RootBone)
			{
				//set the scale of the drawlist to the current character's scale
				drawList.Scale = Scale;

				//send teh model to teh draw list
				Skeleton.RootBone.Render(drawList);
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

		public void EditImageName(string oldName, string newName)
		{
		}

		public void RemoveBone(string boneName)
		{
		}

		public void RemoveJoint(string jointName)
		{
		}

		public void RemoveImage(int imageIndex, string parentBoneName)
		{
		}

		public void RemoveAnimation(string animationName)
		{
			//find the animation and remove it
			var animation = Animations.Where(x => x.Name == animationName).FirstOrDefault();
			if (null != animation)
			{
				Animations.Remove(animation);
			}
		}

		#endregion //Methods

		#region Skeleton File IO

		public void Write()
		{
			WriteSkeletonXml();
			WriteAnimationXml();
		}

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="filename">filename of the resource to load</param>
		/// <param name="renderer">renderer to use to load bitmap images</param>
		public virtual void ReadSkeletonXml(Filename filename, IRenderer renderer, ContentManager content = null)
		{
			SkeletonFile = filename;
			var skelModel = new SkeletonModel(filename, Scale, 1f);
			skelModel.ReadXmlFile(content);
			Skeleton.Load(skelModel, renderer);
		}

		public void WriteSkeletonXml()
		{
			WriteSkeletonXml(SkeletonFile);
		}

		/// <summary>
		/// Open an xml file and dump model data to it
		/// </summary>
		/// <param name="filename">name of the file to dump to</param>
		/// <param name="scale">How much to scale the model when writing it out</param>
		public virtual void WriteSkeletonXml(Filename filename)
		{
			SkeletonFile = filename;

			var skelModel = new SkeletonModel(Skeleton, filename);
			skelModel.WriteXml();
		}

		#endregion //Skeleton File IO

		#region Animation File IO

		/// <summary>
		/// read in a list of animations from a serialized xml format file
		/// </summary>
		/// <param name="filename">filename of the animations to load</param>
		public virtual void ReadAnimationXml(Filename filename, ContentManager content = null)
		{
			AnimationFile = filename;

			//load up the animations from file
			Debug.Assert(null != Skeleton);
			var animations = new AnimationsModel(filename, Scale);
			animations.ReadXmlFile(content);
			LoadAnimations(animations);
		}

		private void LoadAnimations(AnimationsModel animations)
		{
			//create each animation
			foreach (var animationModel in animations.Animations)
			{
				Animations.Add(new Animation(Skeleton, animationModel));
			}

			_playback = EPlayback.Forwards;
			CurrentAnimation = null;
			RestartAnimation();
		}

		public void WriteAnimationXml()
		{
			WriteAnimationXml(AnimationFile);
		}

		public void WriteAnimationXml(Filename filename)
		{
			AnimationFile = filename;
			var animations = new AnimationsModel(filename, this);
			animations.WriteXml();
		}

		#endregion //Animation File IO
	}
}