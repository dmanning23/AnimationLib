using Microsoft.Xna.Framework;
using System.Diagnostics;
using System.Xml;

namespace AnimationLib
{
	/// <summary>
	/// This is a special type of bone, that is animated based on the parent bone, can be added and removed at runtime, etc.
	/// </summary>
	public class GarmentBone : Bone
	{
		#region Members

		/// <summary>
		/// reference to the animation container that owns this bone
		/// used to change animation when the parent bone changes images
		/// </summary>
		private GarmentAnimationContainer _animationContainer;

		/// <summary>
		/// flag for whether this garment has been added to the model or not
		/// </summary>
		private bool _isAddedToSkeleton;

		#endregion //Members

		#region Properties

		/// <summary>
		/// The peice of clothing that this bone is part of
		/// </summary>
		public string GarmentName { get; set; }

		/// <summary>
		/// Reference to the bone this guy attaches too.  Should have the same name as this guy
		/// </summary>
		public Bone ParentBone { get; set; }

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; protected set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBone(GarmentAnimationContainer owner)
			: base()
		{
			Debug.Assert(null != owner);
			_animationContainer = owner;
			ParentBone = null;
			BoneType = EBoneType.Garment;
			_isAddedToSkeleton = false;
		}

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		override public void Update(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			float scale,
			bool ignoreRagdoll)
		{
			Debug.Assert(null != _animationContainer);

			//update the animation container, which will update the Bone base class 
			_animationContainer.Update(time, position, parentFlip, scale, parentRotation, ignoreRagdoll, parentLayer, ParentBone);
		}

		public void UpdateBaseBone(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			float scale,
			bool ignoreRagdoll)
		{
			base.Update(time, position, keyBone, parentRotation, parentFlip, parentLayer, scale, ignoreRagdoll);
		}

		/// <summary>
		/// add to model
		/// </summary>
		public void AddToSkeleton()
		{
			Debug.Assert(null != ParentBone);

			if (!_isAddedToSkeleton)
			{
				ParentBone.AddGarment(this);
				_isAddedToSkeleton = true;
			}
		}

		/// <summary>
		/// remove from model
		/// </summary>
		public void RemoveFromSkeleton()
		{
			Debug.Assert(null != ParentBone);

			if (_isAddedToSkeleton)
			{
				ParentBone.RemoveGarment(GarmentName);
				_isAddedToSkeleton = false;
			}
		}

		#endregion //Methods
	}
}