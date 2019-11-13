using DrawListBuddy;
using FilenameBuddy;
using GameTimer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RandomExtensions;
using RenderBuddy;
using System;
using System.Collections.Generic;
using System.Linq;
using Vector2Extensions;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class AnimationContainer
	{
		#region Fields

		private static Random _random = new Random();

		/// <summary>
		/// The way the current animation is being played
		/// </summary>
		private EPlayback _playback;

		/// <summary>
		/// Index of the current animation being played
		/// </summary>
		private string _currentAnimationName;

		#endregion //Fields

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
		protected string CurrentAnimationName
		{
			get { return _currentAnimationName; }
			set
			{
				_currentAnimationName = value;

				//If there is an animation at that index, use it.
				if (Animations.ContainsKey(_currentAnimationName))
				{
					CurrentAnimation = Animations[_currentAnimationName];
				}
			}
		}

		/// 			<summary>
		/// the list of animations 
		/// </summary>
		/// <value>The animations.</value>
		public Dictionary<string, Animation> Animations { get; protected set; }

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

		/// <summary>
		/// Whether or not to reset the ragdoll when the animation changes.
		/// If the animations aren't changing images very often, might not be bad idea to set this to false.
		/// </summary>
		public bool ResetRagdollOnAnimationChange { get; set; }

		public float Scale { get; set; }

		public ColorRepository Colors { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainer(float scale = 1f)
		{
			Skeleton = new Skeleton(this);
			Animations = new Dictionary<string, Animation>();
			StopWatch = new GameClock();
			_playback = EPlayback.Forwards;
			AnimationFile = new Filename();
			ResetRagdoll = false;
			Scale = scale;
			Colors = new ColorRepository();
			ResetRagdollOnAnimationChange = true;
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
				Animations[animation.Name] = new Animation(Skeleton, animation);
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

				case EPlayback.LoopRandom:
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

		public virtual void AddRagdollGravity(Vector2 gravity)
		{
			Skeleton.RootBone.AddGravity(gravity);
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
			Skeleton.RootBone.SolveConstraints(false);

			//run through the post update so the matrix is correct
			Skeleton.RootBone.PostUpdate(false);
		}

		/// <summary>
		/// Set the current animation
		/// </summary>
		/// <param name="index">the index of the animation to set</param>
		/// <param name="playbackMode">the playback mode to use</param>
		public void SetAnimation(string animation, EPlayback playbackMode)
		{
			//set teh stuff
			_playback = playbackMode;
			CurrentAnimationName = animation;

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

			//Add a little bit of time for a randomloop animation
			if (_playback == EPlayback.LoopRandom)
			{
				StopWatch.CurrentTime = _random.NextFloat(0f, CurrentAnimation.Length);
			}

			if (ResetRagdollOnAnimationChange)
			{
				ResetRagdoll = true;
			}
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
			if (Animations.ContainsKey(animationName))
			{
				return Animations[animationName];
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			foreach (var animation in Animations)
			{
				animation.Value.MultiplyLayers(multiply);
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

		public void AddAnimations(AnimationContainer animations)
		{
			foreach (var animation in animations.Animations)
			{
				AddAnimation(animation.Value);
			}
		}

		public void AddAnimation(Animation animation)
		{
			Animations[animation.Name] = animation;
		}

		public void RemoveAnimations(AnimationContainer animations)
		{
			foreach (var animation in animations.Animations)
			{
				RemoveAnimation(animation.Key);
			}
		}

		public void RemoveAnimation(string animationName)
		{
			//find the animation and remove it
			Animations.Remove(animationName);
		}

		public void SetColor(string tag, Color color)
		{
			Colors.AddColor(tag, color);
			Skeleton.SetColor(tag, color);
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
		/// <param name="xmlContent">this is a content manager specifically created to load xml content</param>
		public virtual void ReadSkeletonXml(Filename filename, IRenderer renderer, ContentManager xmlContent = null)
		{
			SkeletonFile = filename;
			var skelModel = new SkeletonModel(filename, Scale, 1f);
			skelModel.ReadXmlFile(xmlContent);
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
		/// <param name="xmlContent">this is a content manager specifically created to load xml content</param>
		public virtual void ReadAnimationXml(Filename filename, ContentManager xmlContent = null)
		{
			AnimationFile = filename;

			//load up the animations from file
			using (var animations = new AnimationsModel(filename, Scale))
			{
				animations.ReadXmlFile(xmlContent);
				LoadAnimations(animations);
			}
		}

		private void LoadAnimations(AnimationsModel animations)
		{
			CreateAnimations(Skeleton, animations);

			_playback = EPlayback.Forwards;
			CurrentAnimation = null;
			RestartAnimation();
		}

		public void CreateAnimations(Skeleton skeleton, AnimationsModel animations)
		{
			//create each animation
			foreach (var animationModel in animations.Animations)
			{
				Animations[animationModel.Name] = new Animation(skeleton, animationModel);
			}
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