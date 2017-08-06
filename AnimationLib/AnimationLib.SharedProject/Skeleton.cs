using FilenameBuddy;
using Microsoft.Xna.Framework;
using RenderBuddy;
using UndoRedoBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class Skeleton
	{
		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public Bone RootBone { get; protected set; }

		protected AnimationContainer Animations { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Skeleton(AnimationContainer animations)
		{
			Animations = animations;
		}

		public Bone GetBone(string boneName)
		{
			return RootBone.GetBone(boneName);
		}

		public Joint GetJoint(string jointName)
		{
			return RootBone.GetJoint(jointName, true);
		}

		/// <summary>
		/// This method goes through the skeleton and makes sure the joints and bone names match
		/// </summary>
		/// <param name="animations"></param>
		public void Mirror(CommandStack command)
		{
			RootBone.MirrorRightToLeft(RootBone, command);
		}

		public void DrawPhysics(IRenderer renderer, Color color)
		{
			RootBone.DrawPhysics(renderer, true, color);
		}

		#endregion //Methods

		#region File IO

		protected virtual Bone CreateBone(SkeletonModel skeleton)
		{
			return new Bone(skeleton.RootBone);
		}

		public void Load(SkeletonModel skeleton, IRenderer renderer)
		{
			RootBone = CreateBone(skeleton);

			//Set the anchor joints of the whole model
			RootBone.SetAnchorJoint(null);

			//Load all the images
			RootBone.LoadImages(renderer);
		}

		public virtual void WriteXml(Filename filename, AnimationContainer animations)
		{
			var skeleton = new SkeletonModel(this, filename);
			skeleton.WriteXml();
		}

		#endregion //Model File IO
	}
}