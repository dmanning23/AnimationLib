using DrawListBuddy;
using FilenameBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UndoRedoBuddy;
using Vector2Extensions;

namespace AnimationLib
{
	public class Bone
	{
		#region Properties

		/// <summary>
		/// the joint this guy attachs to
		/// </summary>
		public Joint AnchorJoint { get; protected set; }

		/// <summary>
		/// whether to flip this dude or not when rendering
		/// </summary>
		private bool _flipped;
		public bool Flipped
		{
			get { return _flipped; }
			set
			{
				_flipped = value;

				//set parent flip in all the joints
				for (var i = 0; i < Joints.Count; i++)
				{
					Joints[i].ParentFlip = _flipped;
				}
			}
		}

		/// <summary>
		/// the child bones of this guy
		/// </summary>
		public List<Bone> Bones { get; private set; }

		/// <summary>
		/// The joints in this bone
		/// </summary>
		public List<Joint> Joints { get; private set; }

		/// <summary>
		/// The images in this bone
		/// </summary>
		public List<Image> Images { get; private set; }

		/// <summary>
		/// The current position of this guys upper-left corner in screen cooridnates
		/// </summary>
		public Vector2 Position { get; set; }

		/// <summary>
		/// The position of this dude's anchor joint in screen coordinates
		/// </summary>
		public Vector2 AnchorPosition { get; private set; }

		/// <summary>
		/// The name of this bone
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// the rotation to render this guy at
		/// </summary>
		public float Rotation { get; set; }

		public bool IsFoot
		{
			get { return EBoneType.Foot == BoneType; }
		}

		public bool IsWeapon
		{
			get { return EBoneType.Weapon == BoneType; }
		}

		/// <summary>
		/// Flag used to tell the difference between the bone types for collision purposes
		/// </summary>
public EBoneType BoneType { get; set; }

		/// <summary>
		/// this guys current image to render
		/// </summary>
		public int ImageIndex { get; protected set; }

		/// <summary>
		/// this guys current layer to render at
		/// </summary>
		public int CurrentLayer { get; set; }

		/// <summary>
		/// Whether or not this bone should be colored by the palette swap
		/// </summary>
		public bool Colorable { get; set; }

		/// <summary>
		/// These are all the forces that will be applied when ragdoll is set
		/// </summary>
		public List<Vector2> RagdollForces { get; private set; }

		/// <summary>
		/// The color to draw this bone with. 
		/// Only used if Colorable flag is true
		/// </summary>
		private Color PrimaryColor { get; set; }

		/// <summary>
		/// The color to draw the secondary color mask with. 
		/// Only used if there is a ColorMask texture on the current Image
		/// </summary>
		private Color SecondaryColor { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Bone()
		{
			Images = new List<Image>();
			Joints = new List<Joint>();
			Bones = new List<Bone>();
			ImageIndex = -1;
			CurrentLayer = -1;
			AnchorJoint = new Joint(0);
			Name = "Root";
			Rotation = 0.0f;
			_flipped = false;
			Colorable = false;
			BoneType = EBoneType.Normal;
			RagdollForces = new List<Vector2>();
			PrimaryColor = Color.White;
			SecondaryColor = Color.White;
		}

		public Bone(BoneModel bone)
			: this()
		{
			Name = bone.Name;
			Colorable = bone.Colorable;
			BoneType = bone.BoneType;
			for (int i = 0; i < bone.Joints.Count; i++)
			{
				Joints.Add(new Joint(bone.Joints[i], i));
			}
			foreach (var image in bone.Images)
			{
				Images.Add(new Image(image));
			}
			foreach (var childBone in bone.Bones)
			{
				Bones.Add(new Bone(childBone));
			}
		}

		/// <summary>
		/// After the model has been read in from file, set all the anchor joints
		/// </summary>
		/// <param name="parentBone"></param>
		public void SetAnchorJoint(Bone parentBone)
		{
			//get the anchor joint from the parent bone
			if (null != parentBone)
			{
				AnchorJoint = parentBone.GetJoint(Name);
				if (null == AnchorJoint)
				{
					//add a joint to the parent
					AnchorJoint = new Joint(parentBone.Joints.Count);
					AnchorJoint.Name = Name;
					parentBone.Joints.Add(AnchorJoint);
				}
			}

			//update all the images
			for (var i = 0; i < Images.Count; i++)
			{
				Images[i].SetAnchorJoint(this);
			}

			//set the anchor joint in all child joints
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].SetAnchorJoint(this);
			}
		}

		/// <summary>
		/// Load all the images for the model
		/// </summary>
		/// <param name="renderer"></param>
		public void LoadImages(IRenderer renderer)
		{
			for (var i = 0; i < Images.Count; i++)
			{
				Images[i].LoadImage(renderer);
			}

			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].LoadImages(renderer);
			}
		}

		#endregion //Initialization

		#region Methods

		public void RestartAnimation()
		{
			//reset all my shit
			for (var i = 0; i < Joints.Count; i++)
			{
				Joints[i].RestartAnimation();
			}

			//reset child shit
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].RestartAnimation();
			}
		}

		/// <summary>
		/// Put a garment on this bone
		/// </summary>
		/// <param name="garmentBone">Add a garment over this bone</param>
		public void AddGarment(GarmentBone garmentBone)
		{
			//just add the bone to the end of the list
			Bones.Add(garmentBone);
		}

		/// <summary>
		/// Find and remove a garment from this bone
		/// </summary>
		/// <param name="garmentName">name of the garment to remove</param>
		public void RemoveGarment(string garmentName)
		{
			//find and remove the garment bone that matches the garment
			for (var i = 0; i < Bones.Count; i++)
			{
				var myBone = Bones[i] as GarmentBone;
				if ((null != myBone) && (garmentName == myBone.GarmentName))
				{
					Bones.RemoveAt(i);
					return;
				}
			}

			//if it gets here, for some reason that garment wasnt found
			Debug.Assert(false);
		}

		/// <summary>
		/// Get a specific joint from the list
		/// </summary>
		/// <param name="jointName">name index of the joint to get</param>
		public Joint GetJoint(string jointName)
		{
			for (var i = 0; i < Joints.Count; i++)
			{
				if (Joints[i].Name == jointName)
				{
					return Joints[i];
				}
			}

			//didnt find a joint with that name
			return null;
		}

		/// <summary>
		/// Get the index specific joint from the list
		/// </summary>
		/// <param name="jointName">name index of the joint to get</param>
		public int GetJointIndex(Joint joint)
		{
			if (null != joint)
			{
				for (var i = 0; i < Joints.Count; i++)
				{
					if (Joints[i].Name == joint.Name)
					{
						return i;
					}
				}
			}

			//didnt find a joint with that name
			return -1;
		}

		/// <summary>
		/// get a bone by name.  Recurse into teh tree until we find it!!!
		/// </summary>
		/// <param name="boneName">the name of teh bone we are looking for</param>
		/// <returns>The first bone we found with that name.</returns>
		public Bone GetBone(string boneName)
		{
			//is this the dude?
			if (Name == boneName)
			{
				return this;
			}

			//is the requested bone underneath this dude?
			for (var i = 0; i < Bones.Count; i++)
			{
				Bone myBone = Bones[i].GetBone(boneName);
				if (null != myBone)
				{
					return myBone;
				}
			}

			//didnt find a joint with that name :(
			return null;
		}

		/// <summary>
		/// get the bone that owns a particular bone
		/// </summary>
		/// <param name="boneName">name of the bone we want the parent of</param>
		/// <returns>Bone: bone that owns a bone with the requested name.  null if not found</returns>
		public Bone GetParentBone(string boneName)
		{
			//are any of my children the correct bone?
			for (var i = 0; i < Bones.Count; i++)
			{
				if (boneName == Bones[i].Name)
				{
					return this;
				}
			}

			//do any of my child have the correct bone?
			for (var i = 0; i < Bones.Count; i++)
			{
				Bone foundBone = Bones[i].GetParentBone(boneName);
				if (null != foundBone)
				{
					return foundBone;
				}
			}

			//couldn't find it
			return null;
		}

		/// <summary>
		/// Find and return an image that this bone uses (non-recursively)
		/// </summary>
		/// <param name="filename">filename of the image to find</param>
		/// <returns>the first instance of an image using that name, null if not found</returns>
		public Image GetImage(Filename filename)
		{
			for (var i = 0; i < Images.Count; i++)
			{
				if (Images[i].ImageFile.File == filename.File)
				{
					return Images[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Find and return the index of an image that this bone uses (non-recursively)
		/// </summary>
		/// <param name="filename">filename of the image to find (no path info!)</param>
		/// <returns>the index of the first instance of an image using that name, -1 if not found</returns>
		public int GetImageIndex(string filename)
		{
			//don't check for default value
			if ("" == filename)
			{
				return -1;
			}

			//check my images
			for (var i = 0; i < Images.Count; i++)
			{
				if (Images[i].ImageFile.GetFile() == filename)
				{
					return i;
				}
			}

			//check child bones images
			for (var i = 0; i < Bones.Count; i++)
			{
				int iResult = Bones[i].GetImageIndex(filename);
				if (-1 != iResult)
				{
					return iResult;
				}
			}

			return -1;
		}

		/// <summary>
		/// Get the index of an image, norecursively
		/// </summary>
		/// <param name="image"></param>
		/// <returns></returns>
		public int GetImageIndex(Image image)
		{
			//check my images
			for (var i = 0; i < Images.Count; i++)
			{
				if (Images[i].ImageFile.ToString() == image.ImageFile.ToString())
				{
					return i;
				}
			}
			
			return -1;
		}

		/// <summary>
		/// get the image currently being displayed by this bone
		/// </summary>
		/// <returns>Image: teh current image</returns>
		public Image GetCurrentImage()
		{
			if ((ImageIndex >= 0) && (ImageIndex < Images.Count))
			{
				return Images[ImageIndex];
			}

			return null;
		}

		/// <summary>
		/// Check if this bone contains any physics data
		/// </summary>
		/// <returns>true if there is physics data, false if not</returns>
		public bool HasPhysicsData()
		{
			for (var i = 0; i < Images.Count; i++)
			{
				if (Images[i].HasPhysics())
				{
					return true;
				}
			}

			return false;
		}

		public override string ToString()
		{
			return Name;
		}

		/// <summary>
		/// Gets the position.  Used in all the delegate methods
		/// </summary>
		/// <returns>The position.</returns>
		public Vector2 GetPosition()
		{
			return AnchorPosition;
		}

		/// <summary>
		/// Get the true current rotation of this bone, ignoring flip etc.
		/// </summary>
		/// <returns></returns>
		public Vector2 TrueRotation()
		{
			if ((null == GetCurrentImage()) || (Joints.Count <= 0))
			{
				return Vector2.UnitX;
			}

			//get the difference between the anchor position and the joint
			return Joints[0].Position - AnchorPosition;
		}

		/// <summary>
		/// Get the true current rotation of this bone, ignoring flip etc.
		/// </summary>
		/// <returns></returns>
		public float TrueRotationAngle()
		{
			return TrueRotation().Angle();
		}

		/// <summary>
		/// Dont display this bone or its children.
		/// Used to hide garments.
		/// </summary>
		public void Hide()
		{
			ImageIndex = -1;
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].Hide();
			}
		}

		#region Color Methods

		/// <summary>
		/// Set the primary color of this bone, if the colorable flag is true.
		/// Also recurses into all child bones
		/// </summary>
		/// <param name="color"></param>
		public void SetPrimaryColor(Color color)
		{
			//If this bone is colorable, change the color
			if (Colorable)
			{
				PrimaryColor = color;
			}

			//Set the color in all the child bones
			foreach (var bone in Bones)
			{
				bone.SetPrimaryColor(color);
			}
		}

		/// <summary>
		/// Set the secondary color of this bone.
		/// Also recurses into all child bones
		/// </summary>
		/// <param name="color"></param>
		public void SetSecondaryColor(Color color)
		{
			SecondaryColor = color;

			//Set the color in all the child bones
			foreach (var bone in Bones)
			{
				bone.SetSecondaryColor(color);
			}
		}

		#endregion //Color Methods

		#region Update Methods

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		/// <param name="time"></param>
		/// <param name="position"></param>
		/// <param name="keyBone"></param>
		/// <param name="parentRotation"></param>
		/// <param name="parentFlip"></param>
		/// <param name="parentLayer"></param>
		/// <param name="scale"></param>
		/// <param name="ignoreRagdoll"></param>
		virtual public void Update(int time,
								   Vector2 position,
								   KeyBone keyBone,
								   float parentRotation,
								   bool parentFlip,
								   int parentLayer,
								   float scale,
								   bool ignoreRagdoll)
		{
			Debug.Assert(null != AnchorJoint);

			//update my anchor joint
			UpdateAnchorJoint(time, keyBone);

			//update the image
			UpdateImage();

			//get the current layer
			UpdateLayer(parentLayer);

			//update the flip
			UpdateFlip(parentFlip);

			//update the rotation
			UpdateRotation(parentRotation, parentFlip, ignoreRagdoll);

			//update the translation
			UpdateTranslation(ref position, parentRotation, scale, ignoreRagdoll);

			//Update this bone and all its joint positions
			UpdateImageAndJoints(position, scale, ignoreRagdoll);

			UpdateChildren(time, keyBone, scale, ignoreRagdoll);
		}

		private void UpdateChildren(int time, KeyBone keyBone, float scale, bool ignoreRagdoll)
		{
			//this layer counter is incremented to layer garments on to pof each other
			var currentLayer = CurrentLayer;

			//update all the bones
			for (var i = 0; i < Bones.Count; i++)
			{
				//set the bones position to the anchor pos
				Vector2 childVector = Bones[i].AnchorJoint.Position;

				//if the anchor pos is not in this bone, set it to the bones pos
				if (i >= Joints.Count)
				{
					childVector = AnchorPosition;
				}

				//update the chlidren
				KeyBone childKeyBone = null;
				if (null != keyBone)
				{
					childKeyBone = keyBone.GetChildBone(i);
				}
				Bones[i].Update(time,
								childVector,
								childKeyBone,
								Rotation,
								Flipped,
								currentLayer,
								scale,
								ignoreRagdoll);

				//if that was a garment, increment the counter for the next garment
				if (EBoneType.Garment == Bones[i].BoneType)
				{
					currentLayer--; //subtract to put the next garment on top of this one
				}
			}
		}

		/// <summary>
		/// Updates the anchor joint.
		/// </summary>
		/// <param name="time">The current time of of the animation frames</param>
		/// <param name="keyBone">My key bone.</param>
		void UpdateAnchorJoint(int time, KeyBone keyBone)
		{
			//first update teh joint by the keyjoint
			if (null != keyBone)//if null == keybone, this is a hack update
			{
				KeyJoint rChildKeyJoint = keyBone.KeyJoint;
				AnchorJoint.Update(rChildKeyJoint, time);
			}
		}

		/// <summary>
		/// Update the current image
		/// </summary>
		private void UpdateImage()
		{
			ImageIndex = AnchorJoint.CurrentKeyElement.ImageIndex;
		}

		/// <summary>
		/// Update the current layer
		/// </summary>
		/// <param name="parentLayer">the parent's layer.</param>
		private void UpdateLayer(int parentLayer)
		{
			CurrentLayer = parentLayer + AnchorJoint.CurrentKeyElement.Layer;
		}

		/// <summary>
		/// Updates the flip.
		/// </summary>
		/// <param name="parentFlip">whether or not the parent is flipped</param>
		private void UpdateFlip(bool parentFlip)
		{
			if (parentFlip && AnchorJoint.CurrentKeyElement.Flip)
			{
				//they cancel each other
				Flipped = false;
			}
			else if (parentFlip && !AnchorJoint.CurrentKeyElement.Flip)
			{
				//the parent is flipped
				Flipped = true;
			}
			else if (!parentFlip && AnchorJoint.CurrentKeyElement.Flip)
			{
				//I am flipped
				Flipped = true;
			}
			else
			{
				//no one is flipped
				Flipped = false;
			}
		}

		public bool GetParentFlip()
		{
			if (Flipped && AnchorJoint.CurrentKeyElement.Flip)
			{
				//they cancel each other
				return false;
			}
			else if (Flipped && !AnchorJoint.CurrentKeyElement.Flip)
			{
				//the parent is flipped
				return true;
			}
			else if (!Flipped && AnchorJoint.CurrentKeyElement.Flip)
			{
				//I am flipped
				return true;
			}
			else
			{
				//no one is flipped
				return false;
			}
		}

		/// <summary>
		/// update the rotation of this bone
		/// </summary>
		/// <param name="parentRotation">the parent rotation.</param>
		/// <param name="parentFlip">whether or not the parent is flipped</param>
		/// <param name="ignoreRagdoll">If set to <c>true</c> b ignore ragdoll.</param>
		private void UpdateRotation(float parentRotation, bool parentFlip, bool ignoreRagdoll)
		{
			if (!AnchorJoint.CurrentKeyElement.Ragdoll ||
				ignoreRagdoll ||
				(0 == Joints.Count) ||
				(AnchorJoint.CurrentKeyElement.Ragdoll && AnchorJoint.Data.Floating))
			{
				//add my rotation to the parents rotation
				if (!parentFlip)
				{
					Rotation = parentRotation + AnchorJoint.CurrentKeyElement.Rotation;
				}
				else
				{
					Rotation = parentRotation - AnchorJoint.CurrentKeyElement.Rotation;
				}
			}
		}

		private void UpdateTranslation(ref Vector2 position, float parentRotation, float scale, bool ignoreRagdoll)
		{
			if (!AnchorJoint.CurrentKeyElement.Ragdoll || ignoreRagdoll)
			{
				if (Flipped)
				{
					Vector2 animationTrans = AnchorJoint.CurrentKeyElement.Translation;
					animationTrans.X *= -1.0f;
					position += MatrixExt.Orientation(parentRotation).Multiply(animationTrans * scale);
				}
				else
				{
					position += MatrixExt.Orientation(parentRotation).Multiply(AnchorJoint.CurrentKeyElement.Translation * scale);
				}
			}
			//grab the position (joint location + animation translation)
			AnchorPosition = position;
		}

		private void UpdateImageAndJoints(Vector2 position, float scale, bool ignoreRagdoll)
		{
			//Get the correct location of the anchor coord
			var currentImage = GetCurrentImage();
			var anchorCoord = Vector2.Zero;
			if (null != currentImage)
			{
				anchorCoord = currentImage.GetFlippedAnchorCoord(Flipped, scale);
			}

			//create the rotation matrix and flip it if necessary

			//Create matrix for the position of this dude
			Matrix myMatrix = Matrix.Identity;
			MatrixExt.SetPosition(ref myMatrix, position);

			//create -translation matrix to move to and from the origin
			Matrix myTranslation = Matrix.Identity;
			MatrixExt.SetPosition(ref myTranslation, -position);

			//okay multiply through! move to origin, rotate, move back to my position
			myTranslation = myTranslation * MatrixExt.Orientation(Rotation);
			myTranslation = myTranslation * myMatrix;

			myMatrix = myTranslation;

			//myMatrix = myMatrix * MatrixExt.Orientation(Rotation);
			//myMatrix = myMatrix * myTranslation;

			if (!AnchorJoint.CurrentKeyElement.Ragdoll || !AnchorJoint.Data.Floating || ignoreRagdoll)
			{
				//Update my position based on the offset of the anchor coord
				Position = myMatrix.Multiply(position - anchorCoord);
			}

			//update all the circle data
			if (null != currentImage)
			{
				currentImage.Update(Position, Rotation, Flipped, scale);
			}

			//update all the joints
			for (var i = 0; i < Joints.Count; i++)
			{
				//update the positions of all the joints
				Vector2 jointPosition = new Vector2(0.0f);
				if (null != currentImage)
				{
					//get my joint translation from my current image
					jointPosition = currentImage.GetFlippedJointCoord(i, Flipped, scale);

					//get teh joint data
					Joints[i].Data = Images[ImageIndex].JointCoords[i];
				}

				if (!AnchorJoint.CurrentKeyElement.Ragdoll || ignoreRagdoll)
				{
					//to get the joint position, subtract anchor coord from joint position, and add my position
					jointPosition = jointPosition - anchorCoord;
					jointPosition = position + jointPosition;

					Joints[i].OldPosition = Joints[i].Position;
					Joints[i].Position = myMatrix.Multiply(jointPosition);
				}
			}
		}

		#endregion //Update Methods

		#region Drawing

		/// <summary>
		/// Render this guy out to a draw list
		/// </summary>
		/// <param name="drawlist">the draw list to render to</param>
		public void Render(DrawList drawlist)
		{
			//render out all the children first, so that they will be drawn on top if there are any layer clashes
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].Render(drawlist);
			}

			//Render out the current image
			if ((ImageIndex >= 0) && (ImageIndex < Images.Count))
			{
				Images[ImageIndex].Render(drawlist, Position, PrimaryColor, SecondaryColor, Rotation, Flipped, CurrentLayer);
			}
		}

		/// <summary>
		/// draw the joints of either just this bone, or recurse and also draw all joints of all child bones
		/// </summary>
		/// <param name="renderer">renderer to draw to</param>
		/// <param name="recurse">whether or not to recurse and draw child bones</param>
		/// <param name="color">the color to use</param>
		public void DrawJoints(IRenderer renderer, bool recurse, Color color)
		{
			for (var i = 0; i < Joints.Count; i++)
			{
				renderer.Primitive.Point(Joints[i].Position, color);
			}

			if (recurse)
			{
				for (var i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawJoints(renderer, true, color);
				}
			}
		}

		public void DrawSkeleton(IRenderer renderer, bool recurse, Color color)
		{
			for (var i = 0; i < Joints.Count; i++)
			{
				renderer.Primitive.Line(AnchorPosition, Joints[i].Position, color);
			}

			if (recurse)
			{
				for (var i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawSkeleton(renderer, recurse, color);
				}
			}
		}

		public void DrawPhysics(IRenderer renderer, bool recurse, Color color)
		{
			//draw all my circles
			if (null != GetCurrentImage())
			{
				GetCurrentImage().DrawPhysics(renderer, color);
			}

			//draw all child circles
			if (recurse)
			{
				for (var i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawPhysics(renderer, recurse, color);
				}
			}
		}

		public void DrawOutline(IRenderer renderer, float scale)
		{
			//get the current image
			if (ImageIndex < 0)
			{
				return;
			}
			Image myImage = Images[ImageIndex];

			renderer.Primitive.Rectangle(
				new Vector2((int)Position.X,
						  (int)Position.Y),
				new Vector2((int)(Position.X + myImage.LowerRight.X),
						  (int)(Position.Y + myImage.LowerRight.Y)),
				Rotation,
				scale,
				Color.White);
		}

		#endregion //Drawing

		#endregion //Methods

		#region Ragdoll

		/// <summary>
		/// Add gravity to the forces for this bone add recurse into child bones
		/// </summary>
		/// <param name="gravity"></param>
		public virtual void AddGravity()
		{
			var image = GetCurrentImage();
			if (null != image)
			{
				RagdollForces.Add(image.RagdollGravity);
			}

			//update the children
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].AddGravity();
			}
		}

		public void AccumulateForces(float parentAngle, float scale)
		{
			//Collect all the forces
			Vector2 collectedForces = Vector2.Zero;
			for (var i = 0; i < RagdollForces.Count; i++)
			{
				collectedForces += RagdollForces[i];
			}

			//clear out forces so they dont accumulate
			RagdollForces.Clear();

			//get the spring strength
			var image = GetCurrentImage();

			//update the children
			for (var i = 0; i < Joints.Count; i++)
			{
				Joints[i].Acceleration = collectedForces;

				//if the joint[i].data is floating, add some spring force
				if ((null != image) && AnchorJoint.CurrentKeyElement.Ragdoll)
				{
					AnchorJoint.SolveRagdollSpring(parentAngle, this, Joints[i], image.SpringForce, scale);
				}
			}

			//update the children
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].AccumulateForces(this.Rotation, scale);
			}
		}

		public void RunVerlet(float timeStep)
		{
			Debug.Assert(null != AnchorJoint);

			//solve all the joints
			if (AnchorJoint.CurrentKeyElement.Ragdoll)
			{
				for (var i = 0; i < Joints.Count; i++)
				{
					Joints[i].RunVerlet(timeStep);
				}
			}

			//update all the children
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].RunVerlet(timeStep);
			}
		}

		public void SolveConstraints(bool isParentRagdoll, float scale)
		{
			Debug.Assert(null != AnchorJoint);

			if ((ImageIndex >= 0) && (AnchorJoint.CurrentKeyElement.Ragdoll || isParentRagdoll))
			{
				//update all the joints
				for (var i = 0; i < Joints.Count; i++)
				{
					//if there is no image, all the joints go to the loc of the anchor joint

					//solve constraints from anchor to each joint location
					AnchorJoint.SolveConstraint(Joints[i], Joints[i].Data.Length, scale,
						(isParentRagdoll ? ERagdollMove.MoveAll : ERagdollMove.OnlyHim));

					//solve constraints from each joint to each other joint
					for (var j = (i + 1); j < Joints.Count; j++)
					{
						//get the distance the joints are supposed to be
						float fDistance = 0.0f;
						if (ImageIndex >= 0)
						{
							//Get the vector from one joint to the other
							Vector2 jointVect = Joints[j].Data.Location - Joints[i].Data.Location;
							fDistance = jointVect.Length();
						}

						//move them joints
						Joints[i].SolveConstraint(Joints[j], fDistance, scale,
							((0 == i) ? ERagdollMove.OnlyHim : ERagdollMove.MoveAll)); //don't pull on the first joint though
					}
				}
			}

			//update the children
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].SolveConstraints(AnchorJoint.CurrentKeyElement.Ragdoll, scale);
			}
		}

		public void SolveLimits(float parentRotation)
		{
			if (AnchorJoint.CurrentKeyElement.Ragdoll &&
				!AnchorJoint.Data.Floating &&
				(0 <= ImageIndex) &&
				((-Math.PI < AnchorJoint.FirstLimit) && (Math.PI > AnchorJoint.SecondLimit)))//are there any limits on this bone, or just letting it spin?
			{
				//Get the current rotation
				Rotation = GetRagDollRotation();
				var actualRotation = Helper.ClampAngle(Rotation - parentRotation);
				float firstLimit, secondLimit;
				GetLimits(out firstLimit, out secondLimit);

				bool hitLimit = false;
				if (actualRotation < firstLimit)
				{
					//if it is less than the first limit, update rotation and move the joints to fit 

					//use the first limit as the rotation
					hitLimit = true;
					Rotation = parentRotation + firstLimit;
				}
				else if (actualRotation > secondLimit)
				{
					//if it is more than the second limit, update rotation and move the joints to fit

					//use the second limit as the rotation
					hitLimit = true;
					Rotation = parentRotation + secondLimit;
				}

				if (hitLimit)
				{
					//update joint positions
					Matrix myMatrix = MatrixExt.Orientation(Rotation);
					for (var i = 0; i < Joints.Count; i++)
					{
						//get the vector from the anchor to the joint
						Vector2 jointPos = GetCurrentImage().JointCoords[i].AnchorVect;
						if (Flipped)
						{
							jointPos.X *= -1.0f;
						}
						jointPos = myMatrix.Multiply(jointPos);
						Joints[i].Position = AnchorJoint.Position + jointPos;
					}
				}
			}

			//Solve limits for all the child bones
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].SolveLimits(Rotation);
			}
		}

		public void GetLimits(out float firstLimit, out float secondLimit)
		{
			firstLimit = AnchorJoint.FirstLimit;
			secondLimit = AnchorJoint.SecondLimit;

			//swap the limits if the bone is flipped
			if (Flipped)
			{
				float temp = firstLimit;
				firstLimit = secondLimit * -1.0f;
				secondLimit = temp * -1.0f;
			}
		}

		public void GetRotatedLimits(float parentAngle, out float firstLimit, out float secondLimit)
		{
			firstLimit = AnchorJoint.FirstLimit;
			secondLimit = AnchorJoint.SecondLimit;

			//swap the limits if the bone is flipped
			if (Flipped)
			{
				float temp = firstLimit;
				firstLimit = secondLimit * -1.0f;
				secondLimit = temp * -1.0f;
			}

			firstLimit = parentAngle + firstLimit;
			secondLimit = parentAngle + secondLimit;
		}

		public float GetRagDollRotation()
		{
			if (!AnchorJoint.Data.Floating && (0 <= ImageIndex) && (0 < Joints.Count))
			{
				//get the difference between the anchor position and the joint
				Vector2 diff = Joints[0].Position - AnchorPosition;

				//get the desired rotation 
				Debug.Assert(null != Joints[0].Data);
				Vector2 anchorVect = Joints[0].Data.AnchorVect;

				//adjust for the flip
				if (Flipped)
				{
					anchorVect.X *= -1.0f;
				}

				//get the angle between the two vectors
				return (Helper.atan2(anchorVect) - Helper.atan2(diff));
			}
			else
			{
				return Rotation;
			}
		}

		public void PostUpdate(float scale, bool isParentRagdoll)
		{
			//update this dudes rotation
			Debug.Assert(null != AnchorJoint);
			Debug.Assert(null != AnchorJoint.CurrentKeyElement);
			if ((0 <= ImageIndex) && (AnchorJoint.CurrentKeyElement.Ragdoll || isParentRagdoll))
			{
				if (AnchorJoint.Data.Floating && (0 < Joints.Count))
				{
					//Float based on the joint position

					//Get the vector from the current joint position
					Matrix myRotation = MatrixExt.Orientation(Rotation);
					Vector2 jointPos = GetCurrentImage().JointCoords[0].Location * scale;
					if (Flipped)
					{
						//it flipped?
						jointPos.X = GetCurrentImage().Width - jointPos.X;
					}
					jointPos = myRotation.Multiply(jointPos);

					//update this dude's position 
					Position = Joints[0].Position - jointPos;
				}
				else
				{
					Rotation = GetRagDollRotation();

					//rotate the anchor position
					Matrix myRotation = MatrixExt.Orientation(Rotation);
					Vector2 anchorCoord = GetCurrentImage().AnchorCoord;
					if (Flipped)
					{
						//it flipped?
						anchorCoord.X = GetCurrentImage().Width - anchorCoord.X;
					}
					anchorCoord = (myRotation.Multiply(anchorCoord)) * scale;

					//update this dude's position 
					Position = AnchorJoint.Position - anchorCoord;
				}
			}

			//update the current image
			if (ImageIndex >= 0)
			{
				Images[ImageIndex].Update(Position, Rotation, Flipped, scale);
			}

			//go through & update the children too
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].PostUpdate(scale, AnchorJoint.CurrentKeyElement.Ragdoll);
			}
		}

		#endregion //Ragdoll

		#region Tools

		/// <summary>
		/// override this dude's update to do some special properties different from the ones in the animation
		/// </summary>
		/// <param name="layer">a special layer to display this bone at</param>
		/// <param name="imageIndex">the image index to use to display this dude</param>
		/// <param name="rotation">the rotation to display this bone at</param>
		/// <param name="parentFlip"></param>
		/// <param name="scale"></param>
		/// <param name="translationHack"></param>
		public void HackUpdate(
			int layer,
			int imageIndex,
			float rotation,
			bool parentFlip,
			float scale,
			Vector2 translationHack)
		{
			if (null == AnchorJoint)
			{
				return;
			}

			ManualKeyFrame(layer, imageIndex, rotation, translationHack, false);

			//re-update from this guy onward
			Update(AnchorJoint.CurrentKeyElement.Time,
				AnchorPosition,
				null,
				0.0f,
				parentFlip,
				CurrentLayer - AnchorJoint.CurrentKeyElement.Layer,
				scale,
				true);
		}

		/// <summary>
		/// Manually set a bunch of the properties of this bone before updating
		/// </summary>
		/// <param name="layer">a special layer to display this bone at</param>
		/// <param name="imageIndex">the image index to use to display this dude</param>
		/// <param name="rotation">the rotation to display this bone at</param>
		/// <param name="translationHack"></param>
		/// <param name="ragdoll"></param>
		public void ManualKeyFrame(int layer, int imageIndex, float rotation, Vector2 translationHack, bool ragdoll)
		{
			//get the key element and copy it
			KeyElement currentElement = AnchorJoint.CurrentKeyElement;
			KeyElement myElement = new KeyElement();
			myElement.Copy(currentElement);

			//hack up the image
			myElement.ImageIndex = imageIndex;

			//hack the layer
			myElement.Layer = layer;

			//hack the translation
			myElement.Translation = translationHack;

			//hack the rotation
			myElement.Rotation = rotation;

			myElement.Ragdoll = ragdoll;

			//inject the hacked keyframe into the anchor joint
			AnchorJoint.CurrentKeyElement = myElement;
		}

		/// <summary>
		/// Manually set the rotation of this bone before updating
		/// </summary>
		/// <param name="rotation">the rotation to display this bone at</param>
		public void ManualRotation(float rotation)
		{
			//get the current key element
			KeyElement currentElement = AnchorJoint.CurrentKeyElement;

			ManualKeyFrame(currentElement.Layer,
				currentElement.ImageIndex,
				rotation,
				currentElement.Translation,
				false);
			//true); //Ragdoll doesnt work on base skeleton :P

			//re-update from this guy onward
			Update(AnchorJoint.CurrentKeyElement.Time,
				AnchorPosition,
				null,
				0.0f,
				GetParentFlip(),
				CurrentLayer - AnchorJoint.CurrentKeyElement.Layer,
				1.0f,
				false);
		}

		/// <summary>
		/// Given some screen coordinates, convert to an offset from this bone's location
		/// </summary>
		/// <param name="screenLocation">screen coord</param>
		/// <return></return>
		public Vector2 ConvertCoord(Vector2 screenLocation, float scale, Vector2? prevLocation = null)
		{
			if (!prevLocation.HasValue)
			{
				prevLocation = Position;
			}

			//rotate both points around by the -current rotation
			Matrix myRotation = MatrixExt.Orientation(-Rotation);
			var rotatedPrev = myRotation.Multiply(prevLocation.Value);
			var rotatedScreen = myRotation.Multiply(screenLocation);

			//get teh offset from the bone location
			var myLocation = rotatedScreen - rotatedPrev;
			if (Flipped)
			{
				myLocation.X *= -1f;
			}

			//compensate for the graphical scaling
			myLocation /= scale;

			return myLocation;
		}

		/// <summary>
		/// Given some screen coordinates, convert to an offset from the anchor location
		/// </summary>
		/// <param name="screenLocation">screen coord</param>
		/// <return></return>
		public Vector2 ConvertTranslation(Vector2 screenLocation, float scale)
		{
			//get difference between current angle and the one specified in the animation to get parent rotation
			float parentAngle = GetParentAngle();
			Matrix myRotation = MatrixExt.Orientation(-parentAngle);
			var rotatedPrev = myRotation.Multiply(AnchorJoint.Position);
			var rotatedScreen = myRotation.Multiply(screenLocation);

			var myLocation = rotatedScreen - rotatedPrev;
			if (Flipped)
			{
				myLocation.X *= -1f;
			}

			//compensate for the graphical scaling
			myLocation /= scale;

			return myRotation.Multiply(myLocation);
		}

		public float GetAngle(Vector2 screenPos)
		{
			if (null == AnchorJoint)
			{
				return 0.0f;
			}

			//get teh offset from the bone location
			var myLocation = screenPos - (AnchorJoint.Position + AnchorJoint.CurrentKeyElement.Translation);

			//get the angle to that vector
			float angle = Helper.atan2(myLocation);

			//get difference between current angle and the one specified in the animation to get parent rotation
			float parentAngle = GetParentAngle();

			if (Flipped && !AnchorJoint.CurrentKeyElement.Flip)
			{
				angle = parentAngle + GetBoneAngle() - angle;
			}
			else if (!Flipped && AnchorJoint.CurrentKeyElement.Flip)
			{
				angle = parentAngle + GetBoneAngle() - angle;
			}
			else
			{
				angle = parentAngle + GetBoneAngle() - angle;
			}

			return Helper.ClampAngle(angle);
		}

		public float GetAngleToScreenPosition(Vector2 screenPos)
		{
			//get teh offset from the bone location
			var myLocation = screenPos - (AnchorJoint.Position + AnchorJoint.CurrentKeyElement.Translation);

			//get the angle to that vector
			return Helper.atan2(myLocation) * -1f;
		}

		/// <summary>
		/// Get the amount of rotation that this bone inherited from its parent
		/// </summary>
		/// <returns></returns>
		public float GetParentAngle()
		{
			return Rotation - AnchorJoint.CurrentKeyElement.Rotation;
		}

		/// <summary>
		/// Get if this bone is flipped from the parent
		/// </summary>
		/// <returns></returns>
		public bool IsParentFlip()
		{
			return (Flipped && !AnchorJoint.CurrentKeyElement.Flip);
		}

		/// <summary>
		/// Get the angle from the anchor position to the position of the first joint
		/// </summary>
		/// <returns></returns>
		public float GetBoneAngle()
		{
			if ((null == GetCurrentImage()) || (Joints.Count <= 0))
			{
				return 0.0f;
			}

			//get the difference between the anchor position and the joint
			Vector2 diff = Joints[0].Data.Location - GetCurrentImage().AnchorCoord;

			return Helper.ClampAngle(Helper.atan2(diff));
		}

		public void AddJoint(string jointName)
		{
			Joint myJoint = new Joint(Joints.Count);
			myJoint.Name = jointName;
			Joints.Add(myJoint);

			//add the data for that joint to all the images
			for (var i = 0; i < Images.Count; i++)
			{
				Images[i].AddJoint();
			}
		}

		/// <summary>
		/// Take this bone, see if there is a matching bone on teh right side of the model,
		/// copy its info into this dude.
		/// </summary>
		/// <param name="rootBone">the root bone of the model, used to search for matching bones</param>
		/// <param name="actionCollection"></param>
		public void MirrorRightToLeft(Bone rootBone, CommandStack actionCollection)
		{
			Debug.Assert(null != rootBone);

			//Check if this bone starts with the work "left"
			string[] nameTokens = Name.Split(new Char[] { ' ' });
			if ((nameTokens.Length >= 2) && (nameTokens[0] == "Left"))
			{
				//find if there is a matching bone that starts with "Right"
				string strRightBone = "Right";
				for (var i = 1; i < nameTokens.Length; i++)
				{
					strRightBone += " ";
					strRightBone += nameTokens[i];
				}
				Bone mirrorBone = rootBone.GetBone(strRightBone);
				if (null != mirrorBone)
				{
					//copy that dude's info into this guy
					Copy(mirrorBone, actionCollection);
				}
			}

			//mirror all the child bones too
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].MirrorRightToLeft(rootBone, actionCollection);
			}
		}

		/// <summary>
		/// Copy another bone's data into this dude
		/// </summary>
		/// <param name="sourceBone">the source to copy from</param>
		/// <param name="actionCollection"></param>
		private void Copy(Bone sourceBone, CommandStack actionCollection)
		{
			Debug.Assert(null != sourceBone);

			for (var i = 0; i < Images.Count; i++)
			{
				//check if the other bone uses this image
				Image matchingImage = sourceBone.GetImage(Images[i].ImageFile);
				if (null != matchingImage)
				{
					Images[i].Copy(this, matchingImage, actionCollection);
				}
			}
		}

		/// <summary>
		/// Rename all the joints to name of the bone that attaches to them
		/// </summary>
		/// <param name="animations">null or an animation container to also rename joints</param>
		public void RenameJoints(AnimationContainer animations)
		{
			//rename the joints in the animations
			if (null != AnchorJoint.Name)
			{
				if (null != animations)
				{
					for (var i = 0; i < animations.Animations.Count; i++)
					{
						animations.Animations[i].RenameJoint(AnchorJoint.Name, Name);
					}
				}

				//rename my dude
				AnchorJoint.Name = Name;
			}

			//recurse into all the child bones
			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].RenameJoints(animations);
			}
		}

		/// <summary>
		/// Get a list of all the weapon
		/// </summary>
		/// <param name="listWeapons"></param>
		public void GetAllWeaponBones(List<string> listWeapons)
		{
			//get all the weapons from this dude's model
			if (EBoneType.Weapon == BoneType)
			{
				if (!listWeapons.Contains(Name))
				{
					listWeapons.Add(Name);
				}
			}

			//get all the weapons loaded into the garment manager
			foreach (Bone curBone in Bones)
			{
				curBone.GetAllWeaponBones(listWeapons);
			}
		}

		/// <summary>
		/// Redo the whole scale of this model
		/// </summary>
		/// <param name="scale"></param>
		public void Rescale(float scale)
		{
			for (var i = 0; i < Images.Count; i++)
			{
				Images[i].Rescale(scale);
			}

			for (var i = 0; i < Bones.Count; i++)
			{
				Bones[i].Rescale(scale);
			}
		}

		public void GetLimitRotations(out float limit1, out float limit2)
		{
			GetRotatedLimits(GetParentAngle(), out limit1, out limit2);
		}

		#endregion //Tools
	}
}