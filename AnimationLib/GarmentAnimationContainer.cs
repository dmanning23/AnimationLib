using GameTimer;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;

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

        /// <summary>
        /// The character skeleton this garment attaches to
        /// </summary>
        private Skeleton _characterSkeleton;

        /// <summary>
        /// The name of the bone this garment attaches to. Resolved when the garment is added to the skeleton.
        /// </summary>
        private string _parentBoneName;

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
            //Resolve the parent bone now, rather than at load time.
            //The parent may live inside another garment (e.g. a gun barrel attached to a gun stock),
            //and those bones are only reachable once that garment has been added to the skeleton.
            var parentBone = FindParentBone();
            if (null == parentBone)
            {
                throw new Exception($"Unable to find parent bone {_parentBoneName} for {GarmentName}. If it belongs to another garment, add that garment to the skeleton first.");
            }

            //If this garment is attached to a different bone (e.g. the parent garment was swapped out), move it
            if (!ReferenceEquals(parentBone, GarmentSkeleton.ParentBone))
            {
                GarmentSkeleton.RemoveFromSkeleton();
                BindParentBone(parentBone);
            }

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
            _characterSkeleton = characterSkeleton;
            _parentBoneName = parentBoneName;

            //Try to bind now. If the parent bone is inside another garment that hasn't been added yet,
            //this will fail and the bone gets bound later in AddToSkeleton.
            var parentBone = FindParentBone();
            if (null != parentBone)
            {
                BindParentBone(parentBone);
            }
        }

        private Bone FindParentBone()
        {
            return _characterSkeleton?.RootBone?.GetBone(_parentBoneName);
        }

        private void BindParentBone(Bone parentBone)
        {
            var myGarmentBone = GarmentSkeleton;
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