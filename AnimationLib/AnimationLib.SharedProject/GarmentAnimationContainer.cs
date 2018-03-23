using GameTimer;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Collections.Generic;

namespace AnimationLib
{
	public class GarmentAnimationContainer : AnimationContainer
	{
		#region Fields

		/// <summary>
		/// This is our own little timer, since we won't be getting one passed into the update function
		/// </summary>
		private GameClock AnimationTimer { get; set; }

		/// <summary>
		/// The layer to draw this animation container
		/// </summary>
		private int _currentLayer;

		#endregion //Fields

		#region Properties

		public override string Name
		{
			get
			{
				GarmentBone mySkeleton = Skeleton.RootBone as GarmentBone;
				return mySkeleton.Name;
			}
		}

		public string GarmentName
		{
			get
			{
				GarmentBone mySkeleton = Skeleton.RootBone as GarmentBone;
				return mySkeleton.GarmentName;
			}
			set
			{
				GarmentBone mySkeleton = Skeleton.RootBone as GarmentBone;
				mySkeleton.GarmentName = value;
			}
		}

		private GarmentBone GarmentSkeleton
		{
			get
			{
				return Skeleton.RootBone as GarmentBone;
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentAnimationContainer()
			: base()
		{
			Skeleton = new GarmentSkeleton(this);
			AnimationTimer = new GameClock();
			_currentLayer = 0;
		}

		public GarmentAnimationContainer(AnimationsModel animations, GarmentSkeletonModel skeleton, IRenderer renderer)
			: this()
		{
			Load(animations, skeleton, renderer);
		}

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToSkeleton()
		{
			//add all the garment bones to the bones they attach to
			GarmentSkeleton.AddToSkeleton();
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromSkeleton()
		{
			//remove all the garment bones from the bones they attach to
			GarmentSkeleton.RemoveFromSkeleton();
		}

		/// <summary>
		/// This gets called by the root garmentbone every frame to update the animation
		/// </summary>
		public void Update(int time, 
			Vector2 position, 
			bool isFlipped, 
			float rotation, 
			bool ignoreRagdoll, 
			int layer, 
			Bone attachedBone)
		{
			//set teh layer to draw this dude at
			_currentLayer = layer;

			//update the timer
			AnimationTimer.Update(time);

			//check if the animation has changed
			if ((attachedBone.ImageIndex != CurrentAnimationIndex) || (null == CurrentAnimation))
			{
				//the animation has changed!!!
				SetAnimation(attachedBone.ImageIndex, EPlayback.Loop);
			}

			//if there is no animation, the bone is invisible, hide the garment bone.
			if (null == CurrentAnimation)
			{
				Skeleton.RootBone.Hide();
			}
			else
			{
				//call the base update
				Update(AnimationTimer, position, isFlipped, rotation, ignoreRagdoll);
			}
		}

		/// <summary>
		/// Apply the animation at a certain time
		/// </summary>
		protected override void ApplyAnimation(
			int time,
			Vector2 position,
			bool isFlipped,
			float rotation,
			bool ignoreRagdoll)
		{
			//Apply teh current animation to the bones and stuff
			var currentKeyBone = CurrentAnimation.KeyBone;
			GarmentSkeleton.UpdateBaseBone(time,
				position,
				currentKeyBone,
				rotation,
				isFlipped,
				_currentLayer,
				ignoreRagdoll || ResetRagdoll);

			//is this the first update after an animation change?
			if (ResetRagdoll)
			{
				Skeleton.RootBone.RestartAnimation();
				ResetRagdoll = false;
			}
		}

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		public void SetGarmentBones(Skeleton characterSkeleton)
		{
			//find a Bone with the same name as the garment bone
			var myGarmentBone = GarmentSkeleton;
			var myBone = characterSkeleton.RootBone.GetBone(myGarmentBone.ParentBoneName);
			myGarmentBone.ParentBone = myBone;

			//ok, if we are in one of the tools, reorder the animations to match the image order.
			var myAnimations = new List<Animation>();
			foreach (Image myImage in myBone.Images)
			{
				//find an animation that has the same name as this image
				Animation foundAnimation = null;
				foreach (var curAnimation in Animations)
				{
					if (curAnimation.Name == myImage.Name)
					{
						foundAnimation = curAnimation;
						break;
					}
				}

				//add it to the list!
				myAnimations.Add(foundAnimation);
			}

			//now hold onto the sorted list
			Animations = myAnimations;
		}

		public override string ToString()
		{
			return GarmentName + "-" + Skeleton.RootBone.Name;
		}

		#endregion //Methods
	}
}