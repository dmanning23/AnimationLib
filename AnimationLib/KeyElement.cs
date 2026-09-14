using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnimationLib
{
	/// <summary>
	/// class for sorting the keyframes by time
	/// </summary>
	class KeyElementSort : IComparer<KeyElement>
	{
		public int Compare(KeyElement key1, KeyElement key2)
		{
			return key1.Time.CompareTo(key2.Time);
		}
	}

	public class KeyElement
	{
		#region Properties

		/// <summary>
		/// the time of this key element, stored in 1/60ths of a second
		/// </summary>
		public int Time { get; set; }

		/// <summary>
		/// the angle of rotation, stored in radians
		/// </summary>
		public float Rotation { get; set; }

		/// <summary>
		/// the offset from the parents layer to draw at
		/// </summary>
		public int Layer { get; set; }

		/// <summary>
		/// the index of teh image to draw
		/// </summary>
		public int ImageIndex { get; set; }

		/// <summary>
		/// whether it should be reversed on the x plane(drawn backwards)
		/// </summary>
		public bool Flip { get; set; }

		/// <summary>
		/// the bool flags the element as a keyframe
		/// this is needed for recalibrating the animation
		/// </summary>
		public bool KeyFrame { get; set; }

		/// <summary>
		/// the translation for this frame
		/// </summary>
		public Vector2 Translation { get; set; }

		/// <summary>
		/// whether or not to turn on ragdoll physics
		/// </summary>
		public bool Ragdoll { get; set; }

		/// <summary>
		/// The name of the joint that this keyframe describes
		/// </summary>
		public string BoneName
		{
			get
			{
				return Bone.Name;
			}
		}

		public Bone Bone { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyElement()
		{
			Translation = Vector2.Zero;
			Time = 0;
			Rotation = 0.0f;
			Layer = -1;
			ImageIndex = -1;
			Flip = false;
			KeyFrame = false;
			Ragdoll = false;
			Bone = null;
		}

		public KeyElement(Bone bone) : this()
		{
			Translation = Vector2.Zero;
			Time = 0;
			Rotation = 0.0f;
			Layer = -1;
			ImageIndex = -1;
			Flip = false;
			KeyFrame = false;
			Ragdoll = false;
			Bone = bone;
		}

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public KeyElement(KeyElement inst)
		{
			Copy(inst);
		}

		public KeyElement(KeyElementModel key, Skeleton skeleton)
			: this()
		{
			KeyFrame = true;
			Time = key.Time;
			Rotation = key.Rotation;
			Layer = key.Layer;
			Flip = key.Flip;
			Translation = key.Translation;
			Ragdoll = key.Ragdoll;

			//set the image index
			Bone = skeleton.RootBone.GetBone(key.Joint);
			if (Bone != null)
			{
				ImageIndex = Bone.GetImageIndex(key.Image);
			}
		}

		public void Copy(KeyElement inst)
		{
			Translation = inst.Translation;
			Time = inst.Time;
			Rotation = inst.Rotation;
			Layer = inst.Layer;
			ImageIndex = inst.ImageIndex;
			Flip = inst.Flip;
			KeyFrame = inst.KeyFrame;
			Ragdoll = inst.Ragdoll;
			if (inst.Bone != null)
			{
				Bone = inst.Bone;
			}
		}

		#endregion //Initialization

		#region Methods

		public bool Compare(KeyElement inst)
		{
			if (Rotation != inst.Rotation)
			{
				return false;
			}
			else if (Layer != inst.Layer)
			{
				return false;
			}
			else if (ImageIndex != inst.ImageIndex)
			{
				return false;
			}
			else if (Flip != inst.Flip)
			{
				return false;
			}
			else if (Translation.X != inst.Translation.X)
			{
				return false;
			}
			else if (Translation.Y != inst.Translation.Y)
			{
				return false;
			}
			else if (Ragdoll != inst.Ragdoll)
			{
				return false;
			}
			else if (BoneName != inst.BoneName)
			{
				return false;
			}
			else
			{
				return true;
			}
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			Layer *= multiply;
		}

		#endregion //Methods
	}
}