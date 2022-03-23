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
		public override Bone ParentBone
		{
			get => base.ParentBone;
			set
			{
				base.ParentBone = value;
				SetId();
			}
		}

		public override string Name
		{
			get => base.Name;
			set
			{
				base.Name = value;
				SetId();
			}
		}

		public override bool IsGarment => true;

		public override bool AddToTools { get; set; } = false;

		private string _id;
		public override string Id => _id;

		public Garment Garment { get; private set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBone(GarmentAnimationContainer owner, Garment garment)
			: base()
		{
			Setup(owner, garment);
		}

		public GarmentBone(GarmentAnimationContainer owner, GarmentBoneModel bone, Garment garment)
			: base(bone)
		{
			Setup(owner, garment);
			AddToTools = bone.AddToTools;
		}

		private void SetId()
		{
			if (null != ParentBone)
			{
				_id = $"{ParentBone.Name}_{Name}";
			}
		}

		private void Setup(GarmentAnimationContainer owner, Garment garment)
		{
			Garment = garment;
			GarmentAnimationContainer = owner;
			_isAddedToSkeleton = false;
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		public override void Update(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			bool ignoreRagdoll,
			EPlayback playback)
		{
			//update the animation container, which will update the Bone base class 
			GarmentAnimationContainer.Update(time, position, parentFlip, parentRotation, ignoreRagdoll, parentLayer, ParentBone, playback);
		}

		public void UpdateBaseBone(int time,
			Vector2 position,
			KeyBone keyBone,
			float parentRotation,
			bool parentFlip,
			int parentLayer,
			bool ignoreRagdoll,
			EPlayback playback)
		{
			base.Update(time, position, keyBone, parentRotation, parentFlip, parentLayer, ignoreRagdoll, playback);
		}

		/// <summary>
		/// add to model
		/// </summary>
		public void AddToSkeleton()
		{
			if (!_isAddedToSkeleton)
			{
				//first check if there is already a garment attached to this bone, if so remove it
				if (null != ParentBone.ChildGarmentBone)
				{
					ParentBone.ChildGarmentBone.Garment.RemoveFromSkeleton();
				}

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