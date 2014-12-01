using DrawListBuddy;
using FilenameBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UndoRedoBuddy;
using Vector2Extensions;

namespace AnimationLib
{
	public class Bone
	{
		#region Member Variables

		/// <summary>
		/// the joint this guy attachs to
		/// </summary>
		public Joint AnchorJoint { get; protected set; }

		/// <summary>
		/// The current position of this guys upper-left corner in screen cooridnates
		/// </summary>
		private Vector2 m_CurrentPosition;

		/// <summary>
		/// The position of this dude's anchor joint in screen coordinates
		/// </summary>
		private Vector2 m_CurAnchorPos;

		/// <summary>
		/// whether to flip this dude or not when rendering
		/// </summary>
		private bool m_bCurrentFlip;
		public bool Flipped
		{
			get { return m_bCurrentFlip; }
			set
			{
				m_bCurrentFlip = value;

				//set parent flip in all the joints
				for (int i = 0; i < Joints.Count; i++)
				{
					Joints[i].ParentFlip = m_bCurrentFlip;
				}
			}
		}

		#endregion

		#region Properties

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

		public Vector2 Position
		{
			get { return m_CurrentPosition; }
			set { m_CurrentPosition = value; }
		}

		public Vector2 AnchorPosition
		{
			get { return m_CurAnchorPos; }
			protected set { m_CurAnchorPos = value; }
		}

		/// <summary>
		/// The name of this bone
		/// </summary>
		public string Name { get; protected set; }

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

		#endregion //Properties

		#region Methods

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
			m_bCurrentFlip = false;
			Colorable = false;
			BoneType = EBoneType.Normal;
		}

		public void RestartAnimation()
		{
			//reset all my shit
			for (int i = 0; i < Joints.Count; i++)
			{
				Joints[i].RestartAnimation();
			}

			//reset child shit
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].RestartAnimation();
			}
		}

		/// <summary>
		/// Put a garment on this bone
		/// </summary>
		/// <param name="rBone">Add a garment over this bone</param>
		public void AddGarment(GarmentBone rBone)
		{
			//just add the bone to the end of the list
			Bones.Add(rBone);
		}

		/// <summary>
		/// Find and remove a garment from this bone
		/// </summary>
		/// <param name="strGarment">name of the garment to remove</param>
		public void RemoveGarment(string strGarment)
		{
			//find and remove the garment bone that matches the garment
			for (int i = 0; i < Bones.Count; i++)
			{
				if (Bones[i] is GarmentBone)
				{
					GarmentBone myBone = Bones[i] as GarmentBone;
					Debug.Assert(null != myBone);
					if (strGarment == myBone.GarmentName)
					{
						Bones.RemoveAt(i);
						return;
					}
				}
			}

			//if it gets here, for some reason that garment wasnt found
			Debug.Assert(false);
		}

		/// <summary>
		/// Get a specific joint from the list
		/// </summary>
		/// <param name="iIndex">The index of the joint to get</param>
		public Joint GetJoint(string strJointName)
		{
			for (int i = 0; i < Joints.Count; i++)
			{
				if (Joints[i].Name == strJointName)
				{
					return Joints[i];
				}
			}
			
			//didnt find a joint with that name
			return null;
		}

		/// <summary>
		/// get a bone by name.  Recurse into teh tree until we find it!!!
		/// </summary>
		/// <param name="strBoneName">the name of teh bone we are looking for</param>
		/// <returns>The first bone we found with that name.</returns>
		public Bone GetBone(string strBoneName)
		{
			//is this the dude?
			if (Name == strBoneName)
			{
				return this;
			}

			//is the requested bone underneath this dude?
			for (int i = 0; i < Bones.Count; i++)
			{
				Bone myBone = Bones[i].GetBone(strBoneName);
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
		/// <param name="strBoneName">name of the bone we want the parent of</param>
		/// <returns>Bone: bone that owns a bone with the requested name.  null if not found</returns>
		public Bone GetParentBone(string strBoneName)
		{
			//are any of my children the correct bone?
			for (int i = 0; i < Bones.Count; i++)
			{
				if (strBoneName == Bones[i].Name)
				{
					return this;
				}
			}

			//do any of my child have the correct bone?
			for (int i = 0; i < Bones.Count; i++)
			{
				Bone FoundBone = Bones[i].GetParentBone(strBoneName);
				if (null != FoundBone)
				{
					return FoundBone;
				}
			}

			//couldn't find it
			return null;
		}

		/// <summary>
		/// Find and return an image that this bone uses (non-recursively)
		/// </summary>
		/// <param name="strFileName">filename of the image to find</param>
		/// <returns>the first instance of an image using that name, null if not found</returns>
		public Image GetImage(Filename strFileName)
		{
			for (int i = 0; i < Images.Count; i++)
			{
				if (Images[i].ImageFile.File == strFileName.File)
				{
					return Images[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Find and return the index of an image that this bone uses (non-recursively)
		/// </summary>
		/// <param name="strFileName">filename of the image to find (no path info!)</param>
		/// <returns>the index of the first instance of an image using that name, -1 if not found</returns>
		public int GetImageIndex(string strFileName)
		{
			//don't check for default value
			if ("" == strFileName)
			{
				return -1;
			}

			//check my images
			for (int i = 0; i < Images.Count; i++)
			{
				if (Images[i].ImageFile.GetFile() == strFileName)
				{
					return i;
				}
			}

			//check child bones images
			for (int i = 0; i < Bones.Count; i++)
			{
				int iResult = Bones[i].GetImageIndex(strFileName);
				if (-1 != iResult)
				{
					return iResult;
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
			for (int i = 0; i < Images.Count; i++)
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
			this.ImageIndex = -1;
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].Hide();
			}
		}

		#region Update Methods

		/// <summary>
		/// update all this dude's stuff
		/// </summary>
		/// <param name="iTime"></param>
		/// <param name="myPosition"></param>
		/// <param name="myKeyBone"></param>
		/// <param name="fParentRotation"></param>
		/// <param name="bParentFlip"></param>
		virtual public void Update(int iTime,
		                           Vector2 myPosition,
		                           KeyBone myKeyBone,
		                           float fParentRotation,
		                           bool bParentFlip,
		                           int iParentLayer,
		                           float fScale,
		                           bool bIgnoreRagdoll)
		{
			Debug.Assert(null != AnchorJoint);

			//update my anchor joint
			UpdateAnchorJoint(iTime, myKeyBone);

			//update the image
			UpdateImage();

			//get the current layer
			UpdateLayer(iParentLayer);

			//update the flip
			UpdateFlip(bParentFlip);

			//update the rotation
			UpdateRotation(fParentRotation, bParentFlip, bIgnoreRagdoll);

			//update the translation
			UpdateTranslation(ref myPosition, fParentRotation, fScale, bIgnoreRagdoll);

			//Update this bone and all its joint positions
			UpdateImageAndJoints(myPosition, fScale, bIgnoreRagdoll);

			//this layer counter is incremented to layer garments on to pof each other
			int iCurrentLayer = CurrentLayer;

			//update all the bones
			for (int i = 0; i < Bones.Count; i++)
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
				if (null != myKeyBone)
				{
					childKeyBone = myKeyBone.GetChildBone(i);
				}
				Bones[i].Update(iTime,
				                childVector,
				                childKeyBone, 
				                Rotation,
				                Flipped,
				                iCurrentLayer,
				                fScale,
				                bIgnoreRagdoll);

				//if that was a garment, increment the counter for the next garment
				if (EBoneType.Garment == Bones[i].BoneType)
				{
					iCurrentLayer--; //subtract to put the next garment on top of this one
				}
			}
		}

		/// <summary>
		/// Updates the anchor joint.
		/// </summary>
		/// <param name="iTime">The current time of of the animation frames</param>
		/// <param name="myKeyBone">My key bone.</param>
		void UpdateAnchorJoint(int iTime, KeyBone myKeyBone)
		{
			//first update teh joint by the keyjoint
			if (null != myKeyBone)//if null == keybone, this is a hack update
			{
				KeyJoint rChildKeyJoint = myKeyBone.KeyJoint;
				AnchorJoint.Update(rChildKeyJoint, iTime);
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
		/// <param name="iParentLayer">the parent's layer.</param>
		private void UpdateLayer(int iParentLayer)
		{
			CurrentLayer = iParentLayer + AnchorJoint.CurrentKeyElement.Layer;
		}

		/// <summary>
		/// Updates the flip.
		/// </summary>
		/// <param name="bParentFlip">whether or not the parent is flipped</param>
		private void UpdateFlip(bool bParentFlip)
		{
			if (bParentFlip && AnchorJoint.CurrentKeyElement.Flip)
			{
				//they cancel each other
				Flipped = false;
			}
			else if (bParentFlip && !AnchorJoint.CurrentKeyElement.Flip)
			{	
				//the parent is flipped
				Flipped = true;
			}
			else if (!bParentFlip && AnchorJoint.CurrentKeyElement.Flip)
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

		/// <summary>
		/// update the rotation of this bone
		/// </summary>
		/// <param name="fParentRotation">the parent rotation.</param>
		/// <param name="bParentFlip">whether or not the parent is flipped</param>
		/// <param name="bIgnoreRagdoll">If set to <c>true</c> b ignore ragdoll.</param>
		private void UpdateRotation(float fParentRotation, bool bParentFlip, bool bIgnoreRagdoll)
		{
			if (!AnchorJoint.CurrentKeyElement.RagDoll || bIgnoreRagdoll || (0 == Joints.Count) || (AnchorJoint.CurrentKeyElement.RagDoll && AnchorJoint.Data.Floating))
			{
				//add my rotation to the parents rotation
				if (!bParentFlip)
				{
					Rotation = fParentRotation + AnchorJoint.CurrentKeyElement.Rotation;
				}
				else
				{
					Rotation = fParentRotation - AnchorJoint.CurrentKeyElement.Rotation;
				}
			}
		}

		private void UpdateTranslation(ref Vector2 myPosition, float fParentRotation, float fScale, bool bIgnoreRagdoll)
		{
			if (!AnchorJoint.CurrentKeyElement.RagDoll || bIgnoreRagdoll)
			{
				if (Flipped)
				{
					Vector2 animationTrans = AnchorJoint.CurrentKeyElement.Translation;
					animationTrans.X *= -1.0f;
					myPosition += MatrixExt.Orientation(fParentRotation).Multiply(animationTrans * fScale);
				}
				else
				{
					myPosition += MatrixExt.Orientation(fParentRotation).Multiply(AnchorJoint.CurrentKeyElement.Translation * fScale);
				}
			}
			//grab the position (joint location + animation translation)
			AnchorPosition = myPosition;
		}

		private void UpdateImageAndJoints(Vector2 myPosition, float fScale, bool bIgnoreRagdoll)
		{
			//Get the correct location of the anchor coord
			Image CurrentImage = GetCurrentImage();
			Vector2 anchorCoord = Vector2.Zero;
			if (null != CurrentImage)
			{
				anchorCoord = CurrentImage.GetFlippedAnchorCoord(Flipped, fScale);
			}

			//create the rotation matrix and flip it if necessary

			//Create matrix for the position of this dude
			Matrix myMatrix = Matrix.Identity;
			MatrixExt.SetPosition(ref myMatrix, myPosition);

			//create -translation matrix to move to and from the origin
			Matrix myTranslation = Matrix.Identity;
			MatrixExt.SetPosition(ref myTranslation, -myPosition);

			//okay multiply through! move to origin, rotate, move back to my position
			myTranslation = myTranslation * MatrixExt.Orientation(Rotation);
			myTranslation = myTranslation * myMatrix;

			myMatrix = myTranslation;

			//myMatrix = myMatrix * MatrixExt.Orientation(Rotation);
			//myMatrix = myMatrix * myTranslation;

			if (!AnchorJoint.CurrentKeyElement.RagDoll || !AnchorJoint.Data.Floating || bIgnoreRagdoll)
			{
				//Update my position based on the offset of the anchor coord
				m_CurrentPosition = myMatrix.Multiply(myPosition - anchorCoord);
			}

			//update all the circle data
			if (null != CurrentImage)
			{
				CurrentImage.Update(m_CurrentPosition, Rotation, Flipped, fScale);
			}

			//update all the joints
			for (int i = 0; i < Joints.Count; i++)
			{
				//update the positions of all the joints
				Vector2 jointPosition = new Vector2(0.0f);
				if (null != CurrentImage)
				{
					//get my joint translation from my current image
					jointPosition = CurrentImage.GetFlippedJointCoord(i, Flipped, fScale);

					//get teh joint data
					Joints[i].Data = Images[ImageIndex].JointCoords[i];
				}

				if (!AnchorJoint.CurrentKeyElement.RagDoll || bIgnoreRagdoll)
				{
					//to get the joint position, subtract anchor coord from joint position, and add my position
					jointPosition = jointPosition - anchorCoord;
					jointPosition = myPosition + jointPosition;

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
		/// <param name="DrawList">the draw list to render to</param>
		public void Render(DrawList MyDrawList, Color PaletteSwap)
		{
			//render out all the children first, so that they will be drawn on top if there are any layer clashes
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].Render(MyDrawList, PaletteSwap);
			}

			//Render out the current image
			if ((ImageIndex >= 0) && (ImageIndex < Images.Count))
			{
				//Does this bone ignore palette swaps?
				Color FinalColor = Color.White;
				if (Colorable)
				{
					FinalColor = PaletteSwap;
				}

				Images[ImageIndex].Render(m_CurrentPosition,
				                          MyDrawList,
				                          CurrentLayer,
				                          Rotation,
				                          Flipped,
				                          FinalColor);
			}
		}

		/// <summary>
		/// draw the joints of either just this bone, or recurse and also draw all joints of all child bones
		/// </summary>
		/// <param name="myRenderer">renderer to draw to</param>
		/// <param name="bRecurse">whether or not to recurse and draw child bones</param>
		/// <param name="rColor">the color to use</param>
		public void DrawJoints(IRenderer myRenderer, bool bRecurse, Color rColor)
		{
			for (int i = 0; i < Joints.Count; i++)
			{
				myRenderer.Primitive.Point(Joints[i].Position, rColor);
			}

			if (bRecurse)
			{
				for (int i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawJoints(myRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawSkeleton(IRenderer myRenderer, bool bRecurse, Color rColor)
		{
			for (int i = 0; i < Joints.Count; i++)
			{
				myRenderer.Primitive.Line(AnchorPosition, Joints[i].Position, rColor);
			}

			if (bRecurse)
			{
				for (int i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawSkeleton(myRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawPhysics(IRenderer myRenderer, bool bRecurse, Color rColor)
		{
			//draw all my circles
			if (null != GetCurrentImage())
			{
				GetCurrentImage().DrawPhysics(myRenderer, rColor);
			}

			//draw all child circles
			if (bRecurse)
			{
				for (int i = 0; i < Bones.Count; i++)
				{
					Bones[i].DrawPhysics(myRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawOutline(IRenderer myRenderer, float fScale)
		{
			//get the current image
			if (ImageIndex < 0)
			{
				return;
			}
			Image myImage = Images[ImageIndex];

			myRenderer.Primitive.Rectangle(
				new Vector2((int)m_CurrentPosition.X,
			              (int)m_CurrentPosition.Y),
				new Vector2((int)(m_CurrentPosition.X + myImage.LowerRight.X),
			              (int)(m_CurrentPosition.Y + myImage.LowerRight.Y)),
				Rotation,
				fScale,
				Color.White);
		}

		#endregion //Drawing

		#endregion //methods

		#region Ragdoll

		public void AccumulateForces(Vector2 rForce)
		{
			//update the children
			for (int i = 0; i < Joints.Count; i++)
			{
				Joints[i].Acceleration = rForce;
			}

			//update the children
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].AccumulateForces(rForce);
			}
		}

		public void RunVerlet(float fTimeStep)
		{
			Debug.Assert(null != AnchorJoint);

			//solve all the joints
			if (AnchorJoint.CurrentKeyElement.RagDoll)
			{
				for (int i = 0; i < Joints.Count; i++)
				{
					Joints[i].RunVerlet(fTimeStep);
				}
			}

			//update all the children
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].RunVerlet(fTimeStep);
			}
		}

		public void SolveConstraints(bool bParentRagdoll, float fScale)
		{
			Debug.Assert(null != AnchorJoint);

			if ((ImageIndex >= 0) && (AnchorJoint.CurrentKeyElement.RagDoll || bParentRagdoll))
			{
				//update all the joints
				for (int i = 0; i < Joints.Count; i++)
				{
					//if there is no image, all the joints go to the loc of the anchor joint

					//solve constraints from anchor to each joint location
					AnchorJoint.SolveConstraint(Joints[i], Joints[i].Data.Length, fScale, 
						(bParentRagdoll ? ERagdollMove.MoveAll : ERagdollMove.OnlyHim));

					//solve constraints from each joint to each other joint
					for (int j = (i + 1); j < Joints.Count; j++)
					{
						//get the distance the joints are supposed to be
						float fDistance = 0.0f;
						if (ImageIndex >= 0)
						{
							//Get the vector from one joint to the other
							Vector2 JointVect = Joints[j].Data.Location - Joints[i].Data.Location;
							fDistance = JointVect.Length();
						}

						//move them joints
						Joints[i].SolveConstraint(Joints[j], fDistance, fScale,
							((0 == i) ? ERagdollMove.OnlyHim : ERagdollMove.MoveAll)); //don't pull on the first joint though
					}
				}
			}

			//update the children
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].SolveConstraints(AnchorJoint.CurrentKeyElement.RagDoll, fScale);
			}
		}

		public void SolveLimits(float fParentRotation)
		{
			if (AnchorJoint.CurrentKeyElement.RagDoll &&
			    !AnchorJoint.Data.Floating &&
				(0 <= ImageIndex))
			{
				//Get the current rotation
				Rotation = GetRagDollRotation();
				float fMyActualRotation = Helper.ClampAngle(Rotation - fParentRotation);

				bool bHitLimit = false;
				if (fMyActualRotation < AnchorJoint.FirstLimit)
				{
					//if it is less than the first limit, update rotation and move the joints to fit 

					//use the first limit as the rotation
					bHitLimit = true;
					float fLimit = AnchorJoint.FirstLimit;
					if (Flipped)
					{
						//If it hit and the bone if flipped, swap the rotation
						fLimit = MathHelper.Pi - fLimit;
					}
					Rotation = fParentRotation + AnchorJoint.FirstLimit;
				}
				else if (fMyActualRotation > AnchorJoint.SecondLimit)
				{
					//if it is more than the second limit, update rotation and move the joints to fit

					//use the second limit as the rotation
					bHitLimit = true;
					float fLimit = AnchorJoint.SecondLimit;
					if (Flipped)
					{
						//If it hit and the bone if flipped, swap the rotation
						fLimit = MathHelper.Pi - fLimit;
					}
					Rotation = fParentRotation + AnchorJoint.SecondLimit;
				}

				if (bHitLimit)
				{
					//update joint positions
					Matrix myMatrix = MatrixExt.Orientation(Rotation);
					for (int i = 0; i < Joints.Count; i++)
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
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].SolveLimits(Rotation);
			}
		}

		public float GetRagDollRotation()
		{
			if (!AnchorJoint.Data.Floating && (0 <= ImageIndex) && (0 < Joints.Count))
			{
				//get the difference between the anchor position and the joint
				Vector2 diff = Joints[0].Position - AnchorPosition;

				//get the desired rotation 
				Debug.Assert(null != Joints[0].Data);
				Vector2 AnchorVect = Joints[0].Data.AnchorVect;

				//adjust for the flip
				if (Flipped)
				{
					AnchorVect.X *= -1.0f;
				}

				//get the angle between the two vectors
				return (Helper.atan2(AnchorVect) - Helper.atan2(diff));
			}
			else
			{
				return Rotation;
			}
		}

		public void PostUpdate(float fScale, bool bParentRagdoll)
		{
			//update this dudes rotation
			Debug.Assert(null != AnchorJoint);
			Debug.Assert(null != AnchorJoint.CurrentKeyElement);
			if ((0 <= ImageIndex) && (AnchorJoint.CurrentKeyElement.RagDoll || bParentRagdoll))
			{
				if (AnchorJoint.Data.Floating && (0 < Joints.Count))
				{
					//Float based on the joint position

					//Get the vector from the current joint position
					Matrix myRotation = MatrixExt.Orientation(Rotation);
					Vector2 JointPos = GetCurrentImage().JointCoords[0].Location * fScale;
					if (Flipped)
					{
						//it flipped?
						JointPos.X = GetCurrentImage().Width - JointPos.X;
					}
					JointPos = myRotation.Multiply(JointPos);

					//update this dude's position 
					m_CurrentPosition = Joints[0].Position - JointPos;
				}
				else
				{
					Rotation = GetRagDollRotation();

					//rotate the anchor position
					Matrix myRotation = MatrixExt.Orientation(Rotation);
					Vector2 AnchorCoord = GetCurrentImage().AnchorCoord;
					if (Flipped)
					{
						//it flipped?
						AnchorCoord.X = GetCurrentImage().Width - AnchorCoord.X;
					}
					AnchorCoord = (myRotation.Multiply(AnchorCoord)) * fScale;

					//update this dude's position 
					m_CurrentPosition = AnchorJoint.Position - AnchorCoord;
				}
			}

			//update the current image
			if (ImageIndex >= 0)
			{
				Images[ImageIndex].Update(m_CurrentPosition, Rotation, Flipped, fScale);
			}

			//go through & update the children too
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].PostUpdate(fScale, AnchorJoint.CurrentKeyElement.RagDoll);
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

			ManualKeyFrame(layer, imageIndex, rotation, translationHack);

			//re-update from this guy onward
			Update(AnchorJoint.CurrentKeyElement.Time,
				AnchorPosition,
				null,
				0.0f,
				parentFlip,
				0,
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
		public void ManualKeyFrame(int layer, int imageIndex, float rotation, Vector2 translationHack)
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
				currentElement.Translation);
		}

		/// <summary>
		/// Given some screen coordinates, convert to an offset from this bone's location
		/// </summary>
		/// <param name="iScreenX">screen x coord</param>
		/// <param name="iScreenY">screen y coord</param>
		/// <param name="iX">the converted x coord</param>
		/// <param name="iY">the converted y coord</param>
		public void ConvertCoord(int iScreenX, int iScreenY, ref int iX, ref int iY, float fScale)
		{
			//get teh offset from the bone location
			var screenLocation = new Vector2((float)iScreenX, (float)iScreenY);
			var myLocation = screenLocation - m_CurrentPosition;

			//compensate for the graphical scaling
			myLocation /= fScale;

			//rotate around by the -current rotation
			Matrix myRotation = MatrixExt.Orientation(-Rotation);

			myLocation = myRotation.Multiply(myLocation);
			iX = (int)myLocation.X;
			iY = (int)myLocation.Y;
		}

		/// <summary>
		/// Given some screen coordinates, convert to an offset from this bone's location
		/// </summary>
		/// <param name="iScreenX">screen x coord</param>
		/// <param name="iScreenY">screen y coord</param>
		/// <param name="iX">the converted x coord</param>
		/// <param name="iY">the converted y coord</param>
		public void ConvertTranslation(int iScreenX, int iScreenY, ref int iX, ref int iY, float fScale)
		{
			if (null == AnchorJoint)
			{
				iX = 0;
				iY = 0;
				return;
			}

			//get teh offset from the bone location
			var screenLocation = new Vector2((float)iScreenX, (float)iScreenY);
			var myLocation = screenLocation - AnchorJoint.Position;

			//compensate for the graphical scaling
			myLocation /= fScale;

			//get difference between current angle and the one specified in the animation to get parent rotation
			float fParentAngle = Rotation - AnchorJoint.CurrentKeyElement.Rotation;

			//rotate around by the -current rotation
			Matrix myRotation = MatrixExt.Orientation(-fParentAngle);

			myLocation = myRotation.Multiply(myLocation);
			iX = (int)myLocation.X;
			iY = (int)myLocation.Y;
		}

		public float GetAngle(int iX, int iY)
		{
			if (null == AnchorJoint)
			{
				return 0.0f;
			}

			//get teh offset from the bone location
			var screenLocation = new Vector2((float)iX, (float)iY);
			var myLocation = screenLocation - AnchorJoint.Position;

			//get the angle to that vector
			float fAngle = Helper.atan2(myLocation);

			//get difference between current angle and the one specified in the animation to get parent rotation
			float fParentAngle = Rotation - AnchorJoint.CurrentKeyElement.Rotation;

			float fDiff = 0.0f;
			if (Flipped && !AnchorJoint.CurrentKeyElement.Flip)
			{
				fDiff = fAngle + fParentAngle;
			}
			else
			{
				fDiff = fAngle - fParentAngle;
			}
			return Helper.ClampAngle(fDiff);
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
			Vector2 anchorPos = GetCurrentImage().AnchorCoord;
			Vector2 jointPos = Joints[0].Data.Location;
			Vector2 diff = jointPos - anchorPos;

			return Helper.ClampAngle(Helper.atan2(diff));
		}

		public void AddJoint(string strJointName)
		{
			Joint myJoint = new Joint(Joints.Count);
			myJoint.Name = strJointName;
			Joints.Add(myJoint);

			//add the data for that joint to all the images
			for (int i = 0; i < Images.Count; i++)
			{
				Images[i].AddJoint();
			}
		}

		/// <summary>
		/// factory method to create a new bone
		/// override in child methods if want a differnt type.
		/// </summary>
		/// <returns>The bone.</returns>
		public virtual Bone CreateBone()
		{
			return new Bone();
		}

		/// <summary>
		/// Take this bone, see if there is a matching bone on teh right side of the model,
		/// copy its info into this dude.
		/// </summary>
		/// <param name="RootBone">the root bone of the model, used to search for matching bones</param>
		public void MirrorRightToLeft(Bone RootBone, Macro ActionCollection)
		{
			Debug.Assert(null != RootBone);

			//Check if this bone starts with the work "left"
			string[] nameTokens = Name.Split(new Char[] { ' ' });
			if ((nameTokens.Length >= 2) && (nameTokens[0] == "Left"))
			{
				//find if there is a matching bone that starts with "Right"
				string strRightBone = "Right";
				for (int i = 1; i < nameTokens.Length; i++)
				{
					strRightBone += " ";
					strRightBone += nameTokens[i];
				}
				Bone MirrorBone = RootBone.GetBone(strRightBone);
				if (null != MirrorBone)
				{
					//copy that dude's info into this guy
					Copy(MirrorBone, ActionCollection);
				}
			}

			//mirror all the child bones too
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].MirrorRightToLeft(RootBone, ActionCollection);
			}
		}

		/// <summary>
		/// Copy another bone's data into this dude
		/// </summary>
		/// <param name="SourceBone">the source to copy from</param>
		private void Copy(Bone SourceBone, Macro ActionCollection)
		{
			Debug.Assert(null != SourceBone);

			for (int i = 0; i < Images.Count; i++)
			{
				//check if the other bone uses this image
				Image matchingImage = SourceBone.GetImage(Images[i].ImageFile);
				if (null != matchingImage)
				{
					Images[i].Copy(matchingImage, ActionCollection);
				}
			}
		}

		/// <summary>
		/// Rename all the joints to name of the bone that attaches to them
		/// </summary>
		/// <param name="rAnimations">null or an animation container to also rename joints</param>
		public void RenameJoints(AnimationContainer rAnimations)
		{
			//rename the joints in the animations
			if (null != AnchorJoint.Name)
			{
				if (null != rAnimations)
				{
					for (int i = 0; i < rAnimations.Animations.Count; i++)
					{
						rAnimations.Animations[i].RenameJoint(AnchorJoint.Name, Name);
					}
				}

				//rename my dude
				AnchorJoint.Name = Name;
			}

			//recurse into all the child bones
			for (int i = 0; i < Bones.Count; i++)
			{
				Bones[i].RenameJoints(rAnimations);
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

		#endregion //Tools

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <param name="ParentBone">The parent bone for this dude.</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public virtual bool ReadXMLFormat(XmlNode rXMLNode, Bone ParentBone, IRenderer rRenderer)
		{
			Debug.Assert(null != rXMLNode);

#if DEBUG
			//should have an attribute Type
			XmlNamedNodeMap mapAttributes = rXMLNode.Attributes;
			for (int i = 0; i < mapAttributes.Count; i++)
			{
				//will only have the name attribute
				string strName = mapAttributes.Item(i).Name;
				string strValue = mapAttributes.Item(i).Value;
				if ("Type" != strName)
				{
					Debug.Assert(false);
					return false;
				}
			}
#endif

			//Read in child nodes
			if (rXMLNode.HasChildNodes)
			{
				for (XmlNode childNode = rXMLNode.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					if (!ParseChildXMLNode(childNode, ParentBone, rRenderer))
					{
						Debug.Assert(false);
						return false;
					}
				}
			}

			//if it didnt find an anchor joint, set it to the joint that matches this dudes name
			Debug.Assert(null != AnchorJoint);
			if ("" == AnchorJoint.Name)
			{
				if (null != ParentBone)
				{
					AnchorJoint = ParentBone.GetJoint(Name);
					if (null == AnchorJoint)
					{
						//add a joint to the parent
						AnchorJoint = new Joint(ParentBone.Joints.Count);
						AnchorJoint.Name = Name;
						ParentBone.Joints.Add(AnchorJoint);
					}
				}
			}

			//set the 'foot' flag as appropriate
			if (Name == "Left Foot" || Name == "Right Foot")
			{
				BoneType = EBoneType.Foot;
			}

			return true;
		}

		/// <summary>
		/// Parse a child node of this BoneXML
		/// </summary>
		/// <param name="childNode">teh node to parse</param>
		/// <param name="ParentBone"></param>
		/// <param name="rRenderer"></param>
		/// <returns></returns>
		protected virtual bool ParseChildXMLNode(XmlNode childNode, Bone ParentBone, IRenderer rRenderer)
		{
			//what is in this node?
			string strName = childNode.Name;
			string strValue = childNode.InnerText;

			switch (strName)
			{
				case "name":
				{
					//set the name of this bone
					Name = strValue;
				}
				break;
				case "anchor":
				{
					//get the name of the anchor joint
					string strAnchorJoint = strValue;

					//get the anchor joint from the parent bone
					if (null != ParentBone)
					{
						AnchorJoint = ParentBone.GetJoint(strAnchorJoint);
						if (null == AnchorJoint)
						{
							Debug.Assert(false);
							return false;
						}
					}
				}
				break;
				case "type":
				{
					if ("Foot" == strValue)
					{
						BoneType = EBoneType.Foot;
					}
					else if ("Weapon" == strValue)
					{
						BoneType = EBoneType.Weapon;
					}
					else
					{
						BoneType = EBoneType.Normal;
					}
				}
				break;
				case "colorable":
				{
					Colorable = Convert.ToBoolean(strValue);
				}
				break;
				case "joints":
				{
					//Read in all the joints
					if (childNode.HasChildNodes)
					{
						for (XmlNode jointNode = childNode.FirstChild;
							null != jointNode;
							jointNode = jointNode.NextSibling)
						{
							Joint childJoint = new Joint(Joints.Count);
							if (!childJoint.ReadXMLFormat(jointNode))
							{
								Debug.Assert(false);
								return false;
							}
							Joints.Add(childJoint);
						}
					}
				}
				break;
				case "images":
				{
					//Read in all the images
					if (childNode.HasChildNodes)
					{
						for (XmlNode imageNode = childNode.FirstChild;
							null != imageNode;
							imageNode = imageNode.NextSibling)
						{
							Image childImage = new Image();
							if (!childImage.ReadXMLFormat(imageNode, rRenderer, this))
							{
								Debug.Assert(false);
								return false;
							}
							Images.Add(childImage);
						}
					}
				}
				break;
				case "bones":
				{
					//read in all the child bones
					if (childNode.HasChildNodes)
					{
						for (XmlNode boneNode = childNode.FirstChild;
							null != boneNode;
							boneNode = boneNode.NextSibling)
						{
							Bone childBone = CreateBone();
							if (!childBone.ReadXMLFormat(boneNode, this, rRenderer))
							{
								Debug.Assert(false);
								return false;
							}
							Bones.Add(childBone);
						}
					}
				}
				break;
				default:
				{
					Debug.Assert(false);
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="rXMLFile">the xml file to add this dude as a child of</param>
		/// <param name="bStartElement">whether to tag element as "Asset" or "Item"</param>
		public virtual void WriteXMLFormat(XmlTextWriter rXMLFile, bool bStartElement, float fEnbiggify)
		{
			//add the xml node
			if (bStartElement)
			{
				rXMLFile.WriteStartElement("XnaContent");
				rXMLFile.WriteStartElement("Asset");
			}
			else
			{
				rXMLFile.WriteStartElement("Item");
			}
			rXMLFile.WriteAttributeString("Type", "AnimationLib.BoneXML");

			WriteChildXMLNode(rXMLFile, fEnbiggify);

			if (bStartElement)
			{
				//write out extra end element for XnaContent
				rXMLFile.WriteEndElement();
			}
			rXMLFile.WriteEndElement();
		}

		public virtual void WriteChildXMLNode(XmlTextWriter rXMLFile, float fEnbiggify)
		{
			//add the name attribute
			rXMLFile.WriteStartElement("name");
			rXMLFile.WriteString(Name);
			rXMLFile.WriteEndElement();

			//add the type attribute
			rXMLFile.WriteStartElement("type");
			rXMLFile.WriteString(BoneType.ToString());
			rXMLFile.WriteEndElement();

			//add whether or not this bone ignores palette swap
			rXMLFile.WriteStartElement("colorable");
			rXMLFile.WriteString(Colorable ? "true" : "false");
			rXMLFile.WriteEndElement();

			//write out joints
			rXMLFile.WriteStartElement("joints");
			for (int i = 0; i < Joints.Count; i++)
			{
				Joints[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			//write out images
			rXMLFile.WriteStartElement("images");
			for (int i = 0; i < Images.Count; i++)
			{
				Images[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			//write out child bones
			rXMLFile.WriteStartElement("bones");
			for (int i = 0; i < Bones.Count; i++)
			{
				//dont write out child garment bones
				if (Bones[i] is GarmentBone)
				{
					continue;
				}
				Bones[i].WriteXMLFormat(rXMLFile, false, fEnbiggify);
			}
			rXMLFile.WriteEndElement();
		}

		#endregion //File IO
	}
}