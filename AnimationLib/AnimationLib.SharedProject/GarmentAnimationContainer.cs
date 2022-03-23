using GameTimer;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
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
				return mySkeleton.Id;
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
		public GarmentAnimationContainer(Garment garment)
			: base()
		{
			Skeleton = new GarmentSkeleton(this, garment);
			AnimationTimer = new GameClock();
			_currentLayer = 0;
		}

		public GarmentAnimationContainer(AnimationsModel animations, GarmentSkeletonModel skeleton, IRenderer renderer, Garment garment)
			: this(garment)
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
			Bone attachedBone,
			EPlayback playback)
		{
			//set teh layer to draw this dude at
			_currentLayer = layer;

			//update the timer
			AnimationTimer.Update(time);

			//check if the animation has changed
			var currentImage = attachedBone.GetCurrentImage();
			if ((null != currentImage) && ((currentImage.Name != CurrentAnimationName) || (null == CurrentAnimation)))
			{
				//the animation has changed!!!
				SetAnimation(currentImage.Name, playback);
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
			bool ignoreRagdoll,
			EPlayback playback)
		{
			//Apply teh current animation to the bones and stuff
			var currentKeyBone = CurrentAnimation.KeyBone;
			GarmentSkeleton.UpdateBaseBone(time,
				position,
				currentKeyBone,
				rotation,
				isFlipped,
				_currentLayer,
				ignoreRagdoll || ResetRagdoll,
				playback);

			//is this the first update after an animation change?
			if (ResetRagdoll)
			{
				RestartAnimation();
				Skeleton.RootBone.RestartAnimation();
				ResetRagdoll = false;
			}
		}

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		public void SetGarmentBones(Skeleton characterSkeleton, string parentBoneName)
		{
			//find a Bone with the same name as the garment bone
			var myGarmentBone = GarmentSkeleton;
			var parentBone = characterSkeleton.RootBone.GetBone(parentBoneName);

			if (null == parentBone)
			{
				throw new Exception($"Unable to find parent bone {parentBoneName} in {Name}");
			}

			myGarmentBone.ParentBone = parentBone;

			//Make sure the garment bone has the correct animations
			foreach (var image in parentBone.Images)
			{
				if (!Animations.ContainsKey(image.Name) && myGarmentBone.Images.Count > 0)
				{
					//create a new animation
					var animation = new Animation(Skeleton)
					{
						Name = image.Name,
						Length = 0.1f,
					};

					//create a keyframe at time 0
					var model = new KeyElementModel(Scale)
					{
						Time = 0,
						Image = myGarmentBone.Images[0].Name,
						Joint = myGarmentBone.Name,
					};
					var key = new KeyElement(model, Skeleton);
					animation.AddKeyframe(key);

					//create a keyframe at time 0.1 (6)
					model = new KeyElementModel(Scale)
					{
						Time = 6,
						Image = myGarmentBone.Images[0].Name,
						Joint = myGarmentBone.Name,
					};
					animation.AddKeyframe(new KeyElement(model, Skeleton));

					//store the animation
					Animations[image.Name] = animation;
				}
			}
		}

        public override void RestartAnimation()
        {
            base.RestartAnimation();

			AnimationTimer.Start();
        }

        public override string ToString()
		{
			return GarmentName + "-" + Skeleton.RootBone.Name;
		}

		#endregion //Methods
	}
}