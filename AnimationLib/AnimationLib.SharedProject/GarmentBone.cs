using Microsoft.Xna.Framework;

namespace AnimationLib
{
	/// <summary>
	/// This is a special type of bone, that is animated based on the parent bone, can be added and removed at runtime, etc.
	/// </summary>
	public class GarmentBone : Bone
	{
		#region Members

		/// <summary>
		/// flag for whether this garment has been added to the model or not
		/// </summary>
		private bool _isAddedToSkeleton;

		#endregion //Members

		#region Properties

		/// <summary>
		/// reference to the animation container that owns this bone
		/// used to change animation when the parent bone changes images
		/// </summary>
		public GarmentAnimationContainer GarmentAnimationContainer { get; private set; }

		/// <summary>
		/// The peice of clothing that this bone is part of
		/// </summary>
		public string GarmentName { get; set; }

		/// <summary>
		/// Reference to the bone this guy attaches too.  Should have the same name as this guy
		/// </summary>
		public Bone ParentBone { get; set; }

		private string _parentBoneName;

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName
		{
			get
			{
				return (null != ParentBone) ? ParentBone.Name : _parentBoneName;
			}
			protected set
			{
				_parentBoneName = value;
			}
		}

		public override bool IsGarment => true;

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBone(GarmentAnimationContainer owner)
			: base()
		{
			Setup(owner);
		}

		public GarmentBone(GarmentAnimationContainer owner, GarmentBoneModel bone)
			: base(bone)
		{
			Setup(owner);
			ParentBoneName = bone.ParentBoneName;
		}

		private void Setup(GarmentAnimationContainer owner)
		{
			GarmentAnimationContainer = owner;
			_isAddedToSkeleton = false;
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		override public void Update(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			bool ignoreRagdoll)
		{
			//update the animation container, which will update the Bone base class 
			GarmentAnimationContainer.Update(time, position, parentFlip, parentRotation, ignoreRagdoll, parentLayer, ParentBone);
		}

		public void UpdateBaseBone(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			bool ignoreRagdoll)
		{
			base.Update(time, position, keyBone, parentRotation, parentFlip, parentLayer, ignoreRagdoll);
		}

		/// <summary>
		/// add to model
		/// </summary>
		public void AddToSkeleton()
		{
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
			if (_isAddedToSkeleton)
			{
				ParentBone.RemoveGarment(GarmentName);
				_isAddedToSkeleton = false;
			}
		}

		#endregion //Methods
	}
}