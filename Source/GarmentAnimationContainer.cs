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
		private GameClock _animationTimer;

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
				Debug.Assert(null != Model);
				Debug.Assert(Model is GarmentBone);
				GarmentBone myModel = Model as GarmentBone;
				return myModel.GarmentName;
			}
			set 
			{
 				Debug.Assert(null != Model);
				Debug.Assert(Model is GarmentBone);
				GarmentBone myModel = Model as GarmentBone;
				myModel.GarmentName = value;
			}
		}

		private GarmentBone GarmentModel
		{
			get
			{
				Debug.Assert(null != Model);
				Debug.Assert(Model is GarmentBone);
				return Model as GarmentBone;
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
		public void AddToModel()
		{
			//add all the garment bones to the bones they attach to
			GarmentModel.AddToModel();
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromModel()
		{
			//remove all the garment bones from the bones they attach to
			GarmentModel.RemoveFromModel();
		}

		/// <summary>
		/// This gets called by the root garmentbone every frame to update the animation
		/// </summary>
		/// <param name="myClock"></param>
		/// <param name="myPosition"></param>
		/// <param name="bFlip"></param>
		/// <param name="fScale"></param>
		/// <param name="fRotation"></param>
		/// <param name="bIgnoreRagdoll"></param>
		/// <param name="iLayer"></param>
		public void Update(int iTime, 
			Vector2 myPosition, 
			bool bFlip, 
			float fScale, 
			float fRotation, 
			bool bIgnoreRagdoll, 
			int iLayer, 
			Bone rAttachedBone)
		{
			Debug.Assert(Animations.Count == rAttachedBone.Images.Count);

			//set teh layer to draw this dude at
			_currentLayer = iLayer;

			//update the timer
			_animationTimer.Update(iTime);

			//check if the animation has changed
			if ((rAttachedBone.ImageIndex != CurrentAnimationIndex) || (null == CurrentAnimation))
			{
				//the animation has changed!!!
				SetAnimation(rAttachedBone.ImageIndex, EPlayback.Loop);
			}

			//if there is no animation, the bone is invisible, hide the garment bone.
			if (null == CurrentAnimation)
			{
				Model.Hide();
			}
			else
			{
				//call the base update
				Update(_animationTimer, myPosition, bFlip, fScale, fRotation, bIgnoreRagdoll);
			}
		}

		/// <summary>
		/// Apply the animation at a certain time
		/// </summary>
		/// <param name="iTime">the time of teh animation to set</param>
		/// <param name="rMatrix">the matrix to transform the model by</param>
		protected override void ApplyAnimation(
			int iTime,
			Vector2 myPosition,
			bool bFlip,
			float fScale,
			float fRotation,
			bool bIgnoreRagdoll)
		{
			Debug.Assert(null != Model);

			//Apply teh current animation to the bones and stuff
			KeyBone rCurrentKeyBone = CurrentAnimation.KeyBone;
			GarmentModel.UpdateBaseBone(iTime,
				myPosition,
				rCurrentKeyBone,
				fRotation,
				bFlip,
				_currentLayer,
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
		/// Overridden methoed to create the correct type of bone
		/// </summary>
		protected override void CreateBone()
		{
			Debug.Assert(null == Model);
			Model = new GarmentBone(this);
		}

		public override string ToString()
		{
			return GarmentName + "-" + Model.Name;
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
			var myGarmentBone = GarmentModel;
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