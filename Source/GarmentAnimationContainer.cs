using GameTimer;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnimationLib
{
	public class GarmentAnimationContainer : AnimationContainer
	{
		#region Fields

		/// <summary>
		/// This is our own little timer, since we won't be getting one passed into the update function
		/// </summary>
		private readonly GameClock _animationTimer;

		/// <summary>
		/// The layer to draw this animation container
		/// </summary>
		int _currentLayer;
		
		#endregion //Fields

		#region Properties

		public string GarmentName
		{
			get
			{
				Debug.Assert(null != Skeleton);
				Debug.Assert(Skeleton.RootBone is GarmentBone);
				GarmentBone mySkeleton = Skeleton.RootBone as GarmentBone;
				return mySkeleton.GarmentName;
			}
			set 
			{
 				Debug.Assert(null != Skeleton);
				Debug.Assert(Skeleton.RootBone is GarmentBone);
				GarmentBone mySkeleton = Skeleton.RootBone as GarmentBone;
				mySkeleton.GarmentName = value;
			}
		}

		private GarmentBone GarmentSkeleton
		{
			get
			{
				Debug.Assert(null != Skeleton);
				Debug.Assert(Skeleton.RootBone is GarmentBone);
				return Skeleton.RootBone as GarmentBone;
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentAnimationContainer()
		{
			_animationTimer = new GameClock();
			_currentLayer = 0;
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
			float scale, 
			float rotation, 
			bool ignoreRagdoll, 
			int layer, 
			Bone attachedBone)
		{
			Debug.Assert(Animations.Count == attachedBone.Images.Count);

			//set teh layer to draw this dude at
			_currentLayer = layer;

			//update the timer
			_animationTimer.Update(time);

			//check if the animation has changed
			if ((attachedBone.ImageIndex != CurrentAnimationIndex) || (null == CurrentAnimation))
			{
				//the animation has changed!!!
				SetAnimation(attachedBone.ImageIndex, EPlayback.Loop);
			}

			//if there is no animation, the bone is invisible, hide the garment bone.
			if (null == CurrentAnimation)
			{
				Skeleton.Hide();
			}
			else
			{
				//call the base update
				Update(_animationTimer, position, isFlipped, scale, rotation, ignoreRagdoll);
			}
		}

		/// <summary>
		/// Apply the animation at a certain time
		/// </summary>
		protected override void ApplyAnimation(
			int time,
			Vector2 position,
			bool isFlipped,
			float scale,
			float rotation,
			bool ignoreRagdoll)
		{
			Debug.Assert(null != Skeleton);

			//Apply teh current animation to the bones and stuff
			var currentKeyBone = CurrentAnimation.KeyBone;
			GarmentSkeleton.UpdateBaseBone(time,
				position,
				currentKeyBone,
				rotation,
				isFlipped,
				_currentLayer,
				scale,
				ignoreRagdoll || ResetRagdoll);

			//is this the first update after an animation change?
			if (ResetRagdoll)
			{
				Skeleton.RestartAnimation();
				ResetRagdoll = false;
			}
		}

		/// <summary>
		/// Overridden methoed to create the correct type of bone
		/// </summary>
		public override Bone CreateBone()
		{
			Debug.Assert(null == Skeleton);
			return new GarmentBone(this);
		}

		public override string ToString()
		{
			return GarmentName + "-" + Skeleton.Name;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		/// <param name="model"></param>
		public void SetGarmentBones(Bone model)
		{
			//find a Bone with the same name as the garment bone
			var myGarmentBone = GarmentSkeleton;
			var myBone = model.GetBone(myGarmentBone.ParentBoneName);
			Debug.Assert(null != myBone);
			myGarmentBone.ParentBone = myBone;

			//Make sure this animation container has the correct number of animations
			Debug.Assert(Animations.Count == myBone.Images.Count);

			//ok, if we are in one of the tools, reorder the animations to match the image order.
			var myAnimations = new List<Animation>();
			foreach (Image myImage in myBone.Images)
			{
				//find an animation that has the same name as this image
				Animation foundAnimation = null;
				foreach (var curAnimation in Animations)
				{
					if (curAnimation.Name == myImage.ImageFile.GetFile())
					{
						foundAnimation = curAnimation;
						break;
					}
				}

				//add it to the list!
				Debug.Assert(null != foundAnimation);
				myAnimations.Add(foundAnimation);
			}

			//now hold onto the sorted list
			Animations = myAnimations;
		}

		#endregion //File IO
	}
}