using System.Diagnostics;
using UndoRedoBuddy;
using Vector2Extensions;

namespace AnimationLib
{
	/// <summary>
	/// This object is a single animation for the skeleton
	/// </summary>
	public class Animation
	{
		#region Properties

		/// <summary>
		/// name of the animation
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; private set; }
		
		/// <summary>
		/// length of this animation
		/// </summary>
		/// <value>The length.</value>
		public float Length { get; private set; }

		/// <summary>
		/// the root key series
		/// </summary>
		/// <value>The key bone.</value>
		public KeyBone KeyBone { get; private set; }

		#endregion

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Animation()
		{
			Length = 0.0f;
		}

		public Animation(Skeleton skeleton)
			: this()
		{
			Debug.Assert(null != skeleton);
			KeyBone = new KeyBone(skeleton.RootBone);
		}

		public Animation(Skeleton skeleton, AnimationModel animation)
			: this(skeleton)
		{
			Name = animation.Name;
			Length = animation.Length;

			//get the animation length in frames
			int iLengthFrames = Length.ToFrames();

			foreach (var keyModel in animation.KeyElements)
			{
				//read in the key element
				var key = new KeyElement(keyModel, skeleton);

				//is this keyelement worth keeping?
				if ((key.KeyFrame) && (iLengthFrames >= key.Time))
				{
					//add to the correct keyjoint
					var keyJoint = KeyBone.GetKeyJoint(key.JointName);
					if (null != keyJoint)
					{
						keyJoint.AddKeyElement(key);
					}
				}
			}
		}

		#endregion //Initialization

		#region Methods

		public bool AddKeyframe(KeyElement key)
		{
			//find the correct keyjoint
			KeyJoint myKeyJoint = KeyBone.GetKeyJoint(key.JointName);
			if (null != myKeyJoint)
			{
				//add the keyframe to the keyjoint
				myKeyJoint.AddKeyElement(key);
				return true;
			}
			return false;
		}

		public bool RemoveKeyframe(KeyElement key)
		{
			//find the correct keyjoint
			KeyJoint myKeyJoint = KeyBone.GetKeyJoint(key.JointName);
			if (null != myKeyJoint)
			{
				//remove all keyframes at that time
				return myKeyJoint.RemoveKeyElement(key.Time);
			}
			return false;
		}

		public bool RemoveKeyframe(CommandStack myAction, Bone copyBone, int time)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(copyBone.Name);
			if (null == myKeyBone)
			{
				return false;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(copyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.RemoveKeyElement(myAction, time, this);
			}

			//copy the rest of the thing
			myKeyBone.RemoveKeyElement(myAction, time, this);
			return true;
		}

		/// <summary>
		/// Copy the skeleton from one animation into another animation
		/// </summary>
		/// <param name="myAction">undo/redo action to put all the changes into</param>
		/// <param name="copyBone">bone that is being copied</param>
		/// <param name="myTargetAnimation">animation to paste that bone into</param>
		/// <param name="sourceTime">the time to copy from this animation</param>
		/// <param name="targetTime">the time to paste into the other animation</param>
		/// <param name="selectiveCopy">if this is true, it means only copy image, layer, ragdoll, flip</param>
		public void Copy(CommandStack myAction, 
		                 Bone copyBone, 
		                 Animation myTargetAnimation, 
		                 int sourceTime, 
		                 int targetTime,
		                 bool selectiveCopy)
		{
			//find that bone
			KeyBone myKeyBone = GetKeyBone(copyBone.Name);
			if (null == myKeyBone)
			{
				return;
			}

			//first copy the anchor joint of that dude!
			KeyJoint anchorKeyJoint = KeyBone.GetKeyJoint(copyBone.AnchorJoint.Name);
			if (null != anchorKeyJoint)
			{
				anchorKeyJoint.Copy(myAction, myTargetAnimation, sourceTime, targetTime, selectiveCopy);
			}

			//copy the rest of the thing
			myKeyBone.Copy(myAction, myTargetAnimation, sourceTime, targetTime, selectiveCopy);
		}

		public KeyBone GetKeyBone(string boneName)
		{
			return KeyBone.GetKeyBone(boneName);
		}

		/// <summary>
		/// Find a keyjoint by name recursively
		/// </summary>
		/// <param name="jointName">The name of the joint to get</param>
		public KeyJoint GetKeyJoint(string jointName)
		{
			return KeyBone.GetKeyJoint(jointName);
		}

		/// <summary>
		/// Change teh time for this animation and move all the keyframes to match
		/// </summary>
		/// <param name="time">the new time delta of this animation</param>
		public void SetTime(float time)
		{
			//set the time in the animation itself
			KeyBone.SetTime(Length.ToFrames(), time.ToFrames());

			//grab the time
			Length = time;
		}

		/// <summary>
		/// rename a joint in this animation.  rename all the keyjoint and fix name in keyelements
		/// </summary>
		/// <param name="oldName">the name of the joint to be renamed</param>
		/// <param name="newName">the new name for that joint.</param>
		public void RenameJoint(string oldName, string newName)
		{
			if (oldName == newName)
			{
				return;
			}

			KeyBone.RenameJoint(oldName, newName);
		}

		/// <summary>
		/// Multiply all the layers to spread out the model
		/// </summary>
		/// <param name="multiply"></param>
		public void MultiplyLayers(int multiply)
		{
			KeyBone.MultiplyLayers(multiply);
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods
	}
}