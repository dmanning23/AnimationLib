using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using Microsoft.Xna.Framework;
using FilenameBuddy;
using DrawListBuddy;
using MatrixExtensions;

namespace AnimationLib
{
	/// <summary>
	/// These are the different types of things bones can be.
	/// They will respond differently to collisions depending on the bone type.
	/// </summary>
	public enum EBoneType
	{
		Foot, //this bone collides with the floor (also a "normal" bone)
		Weapon, //this bone only collides when it is in an active attack
		Normal, //this bone only collides with active weapons
		Garment //this bone is part of a garment and doesn't collide with anything
	}

	public class Bone
	{
		#region Member Variables

		/// <summary>
		/// The images in this bone
		/// </summary>
		private List<Image> m_listImages;

		/// <summary>
		/// The joints in this bone
		/// </summary>
		private List<Joint> m_listJoints;

		/// <summary>
		/// the child bones of this guy
		/// </summary>
		private List<Bone> m_listChildren;

		/// <summary>
		/// The name of this bone
		/// </summary>
		private string m_strBoneName;

		/// <summary>
		/// the joint this guy attachs to
		/// </summary>
		private Joint m_AnchorJoint;

		/// <summary>
		/// The current position of this guys upper-left corner in screen cooridnates
		/// </summary>
		private Vector2 m_CurrentPosition;

		/// <summary>
		/// The position of this dude's anchor joint in screen coordinates
		/// </summary>
		private Vector2 m_CurAnchorPos;

		/// <summary>
		/// this guys current image to render
		/// </summary>
		private int m_iCurrentImage;

		/// <summary>
		/// this guys current layer to render at
		/// </summary>
		private int m_iCurrentLayer;

		/// <summary>
		/// the rotation to render this guy at
		/// </summary>
		private float m_fCurrentRotation;

		/// <summary>
		/// whether to flip this dude or not when rendering
		/// </summary>
		private bool m_bCurrentFlip;

		/// <summary>
		/// Whether or not this bone should be colored by the palette swap
		/// </summary>
		private bool m_bColorable;

		/// <summary>
		/// Flag used to tell the difference between the bone types for collision purposes
		/// </summary>
		private EBoneType m_eBoneType;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the list of child bones
		/// </summary>
		public List<Bone> Bones
		{
			get { return m_listChildren; }
		}

		/// <summary>
		/// Gets the list of joints
		/// </summary>
		public List<Joint> Joints
		{
			get { return m_listJoints; }
		}

		/// <summary>
		/// get the list of images
		/// </summary>
		public List<Image> Images
		{
			get { return m_listImages; }
		}

		/// <summary>
		/// get teh anchor joint
		/// </summary>
		public Joint Anchor
		{
			get 
			{
				Debug.Assert(null != m_AnchorJoint);
				return m_AnchorJoint; 
			}
			protected set { m_AnchorJoint = value; }
		}

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

		public string Name
		{
			get { return m_strBoneName; }
			protected set { m_strBoneName = value; }
		}

		public float Rotation
		{
			get { return m_fCurrentRotation; }
			protected set { m_fCurrentRotation = value; }
		}

		public bool Flipped
		{
			get { return m_bCurrentFlip; }
			set
			{
				m_bCurrentFlip = value;

				//set parent flip in all the joints
				for (int i = 0; i < m_listJoints.Count; i++)
				{
					m_listJoints[i].ParentFlip = m_bCurrentFlip;
				}
			}
		}

		public int Layer
		{
			get { return m_iCurrentLayer; }
		}

		public bool IsFoot
		{
			get { return EBoneType.Foot == m_eBoneType; }
		}

		public bool IsWeapon
		{
			get { return EBoneType.Weapon == m_eBoneType; }
		}

		public EBoneType BoneType
		{
			get { return m_eBoneType; }
			protected set { m_eBoneType = value; }
		}

		public int ImageIndex
		{
			get { return m_iCurrentImage; }
			protected set { m_iCurrentImage = value; }
		}

		protected int CurrentLayer
		{
			get { return m_iCurrentLayer; }
			set { m_iCurrentLayer = value; }
		}

		protected bool Colorable
		{
			get { return m_bColorable; }
			set { m_bColorable = value; }
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Bone()
		{
			m_listImages = new List<Image>();
			m_listJoints = new List<Joint>();
			m_listChildren = new List<Bone>();
			m_iCurrentImage = -1;
			m_iCurrentLayer = -1;
			m_AnchorJoint = new Joint(0);
			m_strBoneName = "Root";
			m_fCurrentRotation = 0.0f;
			m_bCurrentFlip = false;
			m_bColorable = false;
			m_eBoneType = EBoneType.Normal;
		}

		public void RestartAnimation()
		{
			//reset all my shit
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				m_listJoints[i].RestartAnimation();
			}

			//reset child shit
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].RestartAnimation();
			}
		}

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
			Debug.Assert(null != m_AnchorJoint);

			//UPDATE MY ANCHOR JOINT

			//first update teh joint by the keyjoint
			if (null != myKeyBone) //if null == keybone, this is a hack update
			{
				KeyJoint rChildKeyJoint = myKeyBone.KeyJoint;
				m_AnchorJoint.Update(rChildKeyJoint, iTime);
			}

			//UPDATE THE IMAGE
			m_iCurrentImage = m_AnchorJoint.CurrentKeyElement.ImageIndex;

			//GET THE CURRENT LAYER
			m_iCurrentLayer = iParentLayer + m_AnchorJoint.CurrentKeyElement.Layer;

			//UPDATE THE FLIP
			if (bParentFlip && m_AnchorJoint.CurrentKeyElement.Flip)
			{
				//they cancel each other
				Flipped = false;
			}
			else if (bParentFlip && !m_AnchorJoint.CurrentKeyElement.Flip)
			{
				//the parent is flipped
				Flipped = true;
			}
			else if (!bParentFlip && m_AnchorJoint.CurrentKeyElement.Flip)
			{
				//I am flipped
				Flipped = true;
			}
			else
			{
				//no one is flipped
				Flipped = false;
			}

			//UPDATE THE ROTATION
			if (!Anchor.CurrentKeyElement.RagDoll || 
				bIgnoreRagdoll || 
				(0 == Joints.Count) ||
				(Anchor.CurrentKeyElement.RagDoll && Anchor.Data.Floating))
			{
				//add my rotation to the parents rotation
				if (!bParentFlip)
				{
					m_fCurrentRotation = fParentRotation + m_AnchorJoint.CurrentKeyElement.Rotation;
				}
				else
				{
					m_fCurrentRotation = fParentRotation - m_AnchorJoint.CurrentKeyElement.Rotation;
				}
			}

			//UPDATE THE TRANSLATION
			if (!m_AnchorJoint.CurrentKeyElement.RagDoll || bIgnoreRagdoll)
			{
				if (Flipped)
				{
					Vector2 animationTrans = m_AnchorJoint.CurrentKeyElement.Translation;
					animationTrans.X *= -1.0f;
					myPosition += MatrixExt.Orientation(fParentRotation).Mutliply(animationTrans * fScale);
				}
				else
				{
					myPosition += MatrixExt.Orientation(fParentRotation).Mutliply(m_AnchorJoint.CurrentKeyElement.Translation * fScale);
				}
			}

			//grab the position (joint location + animation translation)
			m_CurAnchorPos = myPosition;

			//create the rotation matrix and flip it if necessary

			//Create matrix for the position of this dude
			Matrix myMatrix = Matrix.Identity;
			myMatrix.XPos(myPosition.X);
			myMatrix.YPos(myPosition.Y);

			//create -translation matrix to move to and from the origin
			Matrix myTranslation = Matrix.Identity;
			myTranslation.XPos(-myPosition.X);
			myTranslation.YPos(-myPosition.Y);

			//okay multiply through! move to origin, rotate, move back to my position
			myMatrix = myMatrix * MatrixExt.Orientation(m_fCurrentRotation);
			myMatrix = myMatrix * myTranslation;

			//Get the anchor coord
			Vector2 anchorCoord = new Vector2(0.0f);
			if (m_iCurrentImage >= 0)
			{
				Debug.Assert(m_iCurrentImage < m_listImages.Count);
				if (Flipped)
				{
					//flip the x coord
					anchorCoord.X = m_listImages[m_iCurrentImage].Width - m_listImages[m_iCurrentImage].AnchorCoord.X;
					anchorCoord.Y = m_listImages[m_iCurrentImage].AnchorCoord.Y;
				}
				else
				{
					anchorCoord = m_listImages[m_iCurrentImage].AnchorCoord;
				}
			}

			anchorCoord *= fScale;

			if (!m_AnchorJoint.CurrentKeyElement.RagDoll || !m_AnchorJoint.Data.Floating || bIgnoreRagdoll)
			{
				//Update my position based on the offset of the anchor coord
				m_CurrentPosition = myMatrix.Mutliply(myPosition - anchorCoord);
			}

			//update all the circle data
			if (m_iCurrentImage >= 0)
			{
				Debug.Assert(null != GetCurrentImage());
				GetCurrentImage().Update(m_CurrentPosition, m_fCurrentRotation, Flipped, fScale);
			}

			//update all the joints
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				//update the positions of all the joints
				Vector2 jointPosition = new Vector2(0.0f);
				if (m_iCurrentImage >= 0)
				{
					Debug.Assert(m_iCurrentImage < m_listImages.Count);

					//get my joint translation from my current image
					if (Flipped)
					{
						//flip the x coord
						jointPosition.X = m_listImages[m_iCurrentImage].Width - m_listImages[m_iCurrentImage].GetJointLocation(i).Location.X;
						jointPosition.Y = m_listImages[m_iCurrentImage].GetJointLocation(i).Location.Y;
					}
					else
					{
						jointPosition = m_listImages[m_iCurrentImage].GetJointLocation(i).Location;
					}

					//get teh joint data
					m_listJoints[i].Data = m_listImages[m_iCurrentImage].Data[i];
				}

				if (!m_AnchorJoint.CurrentKeyElement.RagDoll || bIgnoreRagdoll)
				{
					jointPosition *= fScale;

					//to get the joint position, subtract anchor coord from joint position, and add my position
					jointPosition = jointPosition - anchorCoord;
					jointPosition = myPosition + jointPosition;

					m_listJoints[i].OldPosition = m_listJoints[i].Position;
					m_listJoints[i].Position = myMatrix.Mutliply(jointPosition);
				}
			}

			//this layer counter is incremented to layer garments on to pof each other
			int iCurrentLayer = m_iCurrentLayer;

			//update all the bones
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				//set the bones position to the anchor pos
				Vector2 childVector = m_listChildren[i].m_AnchorJoint.Position;

				//if the anchor pos is not in this bone, set it to the bones pos
				if (i >= Joints.Count)
				{
					childVector = m_CurAnchorPos;
				}

				//update the chlidren
				KeyBone childKeyBone = null;
				if (null != myKeyBone)
				{
					childKeyBone = myKeyBone.GetChildBone(i);
				}
				m_listChildren[i].Update(iTime,
					childVector,
					childKeyBone, 
					m_fCurrentRotation,
					Flipped,
					iCurrentLayer,
					fScale,
					bIgnoreRagdoll);

				//if that was a garment, increment the counter for the next garment
				if (EBoneType.Garment == m_listChildren[i].BoneType)
				{
					iCurrentLayer--; //subtract to put the next garment on top of this one
				}
			}
		}

		/// <summary>
		/// Put a garment on this bone
		/// </summary>
		/// <param name="rBone">Add a garment over this bone</param>
		public void AddGarment(GarmentBone rBone)
		{
			//just add the bone to the end of the list
			m_listChildren.Add(rBone);
		}

		/// <summary>
		/// Find and remove a garment from this bone
		/// </summary>
		/// <param name="strGarment">name of the garment to remove</param>
		public void RemoveGarment(string strGarment)
		{
			//find and remove the garment bone that matches the garment
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				if (m_listChildren[i] is GarmentBone)
				{
					GarmentBone myBone = m_listChildren[i] as GarmentBone;
					Debug.Assert(null != myBone);
					if (strGarment == myBone.Garment)
					{
						m_listChildren.RemoveAt(i);
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
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				if (m_listJoints[i].Name == strJointName)
				{
					return m_listJoints[i];
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
			for (int i = 0; i < m_listImages.Count; i++)
			{
				if (m_listImages[i].Filename.File == strFileName.File)
				{
					return m_listImages[i];
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
			for (int i = 0; i < m_listImages.Count; i++)
			{
				if (m_listImages[i].Filename.GetFile() == strFileName)
				{
					return i;
				}
			}

			//check child bones images
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				int iResult = m_listChildren[i].GetImageIndex(strFileName);
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
			if ((m_iCurrentImage >= 0) && (m_iCurrentImage < m_listImages.Count))
			{
				return m_listImages[m_iCurrentImage];
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
			return m_strBoneName;
		}

		#region Drawing

		/// <summary>
		/// Render this guy out to a draw list
		/// </summary>
		/// <param name="DrawList">the draw list to render to</param>
		public void Render(DrawList MyDrawList, Color PaletteSwap)
		{
			//Render out the current image
			if ((m_iCurrentImage >= 0) && (m_iCurrentImage < m_listImages.Count))
			{
				//Does this bone ignore palette swaps?
				Color FinalColor = Color.White;
				if (m_bColorable)
				{
					FinalColor = PaletteSwap;
				}

				m_listImages[m_iCurrentImage].Render(m_CurrentPosition,
					MyDrawList,
					m_iCurrentLayer,
					m_fCurrentRotation,
					Flipped,
					FinalColor);
			}

			//render out all the children
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].Render(MyDrawList, PaletteSwap);
			}
		}

		/// <summary>
		/// draw the joints of either just this bone, or recurse and also draw all joints of all child bones
		/// </summary>
		/// <param name="myRenderer">renderer to draw to</param>
		/// <param name="bRecurse">whether or not to recurse and draw child bones</param>
		/// <param name="rColor">the color to use</param>
		public void DrawJoints(Renderer myRenderer, bool bRecurse, Color rColor)
		{
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				myRenderer.DrawPoint(m_listJoints[i].Position, rColor);
			}

			if (bRecurse)
			{
				for (int i = 0; i < m_listChildren.Count; i++)
				{
					m_listChildren[i].DrawJoints(myRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawSkeleton(Renderer myRenderer, bool bRecurse, Color rColor)
		{
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				myRenderer.DrawLine(m_CurAnchorPos, m_listJoints[i].Position, rColor);
			}

			if (bRecurse)
			{
				for (int i = 0; i < m_listChildren.Count; i++)
				{
					m_listChildren[i].DrawSkeleton(myRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawPhysics(Renderer rRenderer, bool bRecurse, Color rColor)
		{
			//draw all my circles
			if (null != GetCurrentImage())
			{
				GetCurrentImage().DrawPhysics(rRenderer, rColor);
			}

			//draw all child circles
			if (bRecurse)
			{
				for (int i = 0; i < m_listChildren.Count; i++)
				{
					m_listChildren[i].DrawPhysics(rRenderer, bRecurse, rColor);
				}
			}
		}

		public void DrawOutline(Renderer myRenderer, float fScale)
		{
			//get the current image
			if (m_iCurrentImage < 0)
			{
				return;
			}
			Image myImage = m_listImages[m_iCurrentImage];

			myRenderer.DrawRectangle(
				m_CurrentPosition,
				new Vector2(m_CurrentPosition.X + myImage.LowerRight.X, m_CurrentPosition.Y + myImage.LowerRight.Y),
				m_fCurrentRotation,
				Color.White,
				fScale);
		}

		#endregion //Drawing

		#endregion //methods

		#region Ragdoll

		public void AccumulateForces(Vector2 rForce)
		{
			//update the children
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				m_listJoints[i].Acceleration = rForce;
			}

			//update the children
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].AccumulateForces(rForce);
			}
		}

		public void RunVerlet(float fTimeStep)
		{
			Debug.Assert(null != m_AnchorJoint);

			//solve all the joints
			if (m_AnchorJoint.CurrentKeyElement.RagDoll)
			{
				for (int i = 0; i < m_listJoints.Count; i++)
				{
					m_listJoints[i].RunVerlet(fTimeStep);
				}
			}

			//update all the children
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].RunVerlet(fTimeStep);
			}
		}

		public void SolveConstraints(bool bParentRagdoll, float fScale)
		{
			Debug.Assert(null != m_AnchorJoint);

			if ((m_iCurrentImage >= 0) && (m_AnchorJoint.CurrentKeyElement.RagDoll || bParentRagdoll))
			{
				//update all the joints
				for (int i = 0; i < m_listJoints.Count; i++)
				{
					//if there is no image, all the joints go to the loc of the anchor joint

					//solve constraints from anchor to each joint location
					m_AnchorJoint.SolveConstraint(m_listJoints[i], m_listJoints[i].Data.Length, fScale, 
						(bParentRagdoll ? ERagdollMove.MoveAll : ERagdollMove.OnlyHim));

					//solve constraints from each joint to each other joint
					for (int j = (i + 1); j < m_listJoints.Count; j++)
					{
						//get the distance the joints are supposed to be
						float fDistance = 0.0f;
						if (m_iCurrentImage >= 0)
						{
							//Get the vector from one joint to the other
							Vector2 JointVect = m_listJoints[j].Data.Location - m_listJoints[i].Data.Location;
							fDistance = JointVect.Length();
						}

						//move them joints
						m_listJoints[i].SolveConstraint(m_listJoints[j], fDistance, fScale,
							((0 == i) ? ERagdollMove.OnlyHim : ERagdollMove.MoveAll)); //don't pull on the first joint though
					}
				}
			}

			//update the children
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].SolveConstraints(m_AnchorJoint.CurrentKeyElement.RagDoll, fScale);
			}
		}

		public void SolveLimits(float fParentRotation)
		{
			if (Anchor.CurrentKeyElement.RagDoll &&
				!Anchor.Data.Floating &&
				(0 <= m_iCurrentImage))
			{
				//Get the current rotation
				m_fCurrentRotation = GetRagDollRotation();
				float fMyActualRotation = Helper.ClampAngle(m_fCurrentRotation - fParentRotation);

				bool bHitLimit = false;
				if (fMyActualRotation < Anchor.FirstLimit)
				{
					//if it is less than the first limit, update rotation and move the joints to fit 

					//use the first limit as the rotation
					bHitLimit = true;
					float fLimit = Anchor.FirstLimit;
					if (Flipped)
					{
						//If it hit and the bone if flipped, swap the rotation
						fLimit = MathHelper.Pi - fLimit;
					}
					m_fCurrentRotation = fParentRotation + Anchor.FirstLimit;
				}
				else if (fMyActualRotation > Anchor.SecondLimit)
				{
					//if it is more than the second limit, update rotation and move the joints to fit

					//use the second limit as the rotation
					bHitLimit = true;
					float fLimit = Anchor.SecondLimit;
					if (Flipped)
					{
						//If it hit and the bone if flipped, swap the rotation
						fLimit = MathHelper.Pi - fLimit;
					}
					m_fCurrentRotation = fParentRotation + Anchor.SecondLimit;
				}

				if (bHitLimit)
				{
					//update joint positions
					Matrix myMatrix = MatrixExt.Orientation(m_fCurrentRotation);
					for (int i = 0; i < m_listJoints.Count; i++)
					{
						//get the vector from the anchor to the joint
						Vector2 jointPos = GetCurrentImage().Data[i].AnchorVect;
						if (Flipped)
						{
							jointPos.X *= -1.0f;
						}
						jointPos = myMatrix.Mutliply(jointPos);
						m_listJoints[i].Position = Anchor.Position + jointPos;
					}
				}
			}

			//Solve limits for all the child bones
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].SolveLimits(m_fCurrentRotation);
			}
		}

		private float GetRagDollRotation()
		{
			if (!Anchor.Data.Floating && (0 <= m_iCurrentImage) && (0 < m_listJoints.Count))
			{
				//get the difference between the anchor position and the joint
				Vector2 diff = Joints[0].Position - m_CurAnchorPos;

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
				return m_fCurrentRotation;
			}
		}

		public void PostUpdate(float fScale, bool bParentRagdoll)
		{
			//update this dudes rotation
			Debug.Assert(null != m_AnchorJoint);
			Debug.Assert(null != m_AnchorJoint.CurrentKeyElement);
			if ((0 <= m_iCurrentImage) && (m_AnchorJoint.CurrentKeyElement.RagDoll || bParentRagdoll))
			{
				if (Anchor.Data.Floating && (0 < m_listJoints.Count))
				{
					//Float based on the joint position

					//Get the vector from the current joint position
					Matrix myRotation = MatrixExt.Orientation(m_fCurrentRotation);
					Vector2 JointPos = GetCurrentImage().Data[0].Location * fScale;
					if (Flipped)
					{
						//it flipped?
						JointPos.X = GetCurrentImage().Width - JointPos.X;
					}
					JointPos = myRotation.Mutliply(JointPos);

					//update this dude's position 
					m_CurrentPosition = Joints[0].Position - JointPos;
				}
				else
				{
					m_fCurrentRotation = GetRagDollRotation();

					//rotate the anchor position
					Matrix myRotation = MatrixExt.Orientation(m_fCurrentRotation);
					Vector2 AnchorCoord = GetCurrentImage().AnchorCoord;
					if (Flipped)
					{
						//it flipped?
						AnchorCoord.X = GetCurrentImage().Width - AnchorCoord.X;
					}
					AnchorCoord = (myRotation.Mutliply(AnchorCoord)) * fScale;

					//update this dude's position 
					m_CurrentPosition = Anchor.Position - AnchorCoord;
				}
			}

			//update the current image
			if (m_iCurrentImage >= 0)
			{
				m_listImages[m_iCurrentImage].Update(m_CurrentPosition, m_fCurrentRotation, Flipped, fScale);
			}

			//go through & update the children too
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].PostUpdate(fScale, Anchor.CurrentKeyElement.RagDoll);
			}
		}

		#endregion //Ragdoll

		#region Tools

#if WINDOWS

		/// <summary>
		/// override this dude's update to do some special properties different from the ones in the animation
		/// </summary>
		/// <param name="iLayer">a special layer to display this bone at</param>
		/// <param name="iImageIndex">the image index to use to display this dude </param>
		/// <param name="fRotation">the rotation to display this bone at</param>
		public void HackUpdate(
			int iLayer,
			int iImageIndex,
			float fRotation,
			KeyBone myBone,
			bool bParentFlip,
			float fScale,
			Vector2 TranslationHack)
		{
			if (null == Anchor)
			{
				return;
			}

			//get the key element and copy it
			KeyElement CurrentElement = Anchor.CurrentKeyElement;
			KeyElement myElement = new KeyElement();
			myElement.Copy(CurrentElement);

			//hack up the image
			myElement.ImageIndex = iImageIndex;

			//hack the layer
			myElement.Layer = iLayer;

			//hack the translation
			myElement.Translation = TranslationHack;

			//hack the rotation
			myElement.Rotation = fRotation;

			//inject the hacked keyframe into the anchor joint
			Anchor.CurrentKeyElement = myElement;

			//re-update from this guy onward
			Update(Anchor.CurrentKeyElement.Time,
				m_CurAnchorPos,
				null,
				0.0f,
				bParentFlip,
				0,
				fScale,
				true); 
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
			Vector2 MyLocation = new Vector2(0.0f);
			Vector2 ScreenLocation = new Vector2(0.0f);
			ScreenLocation.X = (float)iScreenX;
			ScreenLocation.Y = (float)iScreenY;
			MyLocation = ScreenLocation - m_CurrentPosition;

			//compensate for the graphical scaling
			MyLocation /= fScale;

			//rotate around by the -current rotation
			Matrix myRotation = Matrix.Orientation(-m_fCurrentRotation);

			MyLocation = myRotation * MyLocation;
			iX = (int)MyLocation.X;
			iY = (int)MyLocation.Y;
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
			if (null == Anchor)
			{
				iX = 0;
				iY = 0;
				return;
			}

			//get teh offset from the bone location
			Vector2 MyLocation = new Vector2(0.0f);
			Vector2 ScreenLocation = new Vector2(0.0f);
			ScreenLocation.X = (float)iScreenX;
			ScreenLocation.Y = (float)iScreenY;
			MyLocation = ScreenLocation - m_AnchorJoint.Position;

			//compensate for the graphical scaling
			MyLocation /= fScale;

			//get difference between current angle and the one specified in the animation to get parent rotation
			float fParentAngle = m_fCurrentRotation - m_AnchorJoint.CurrentKeyElement.Rotation;

			//rotate around by the -current rotation
			Matrix myRotation = Matrix.Orientation(-fParentAngle);

			MyLocation = myRotation * MyLocation;
			iX = (int)MyLocation.X;
			iY = (int)MyLocation.Y;
		}

		public float GetAngle(int iX, int iY)
		{
			if (null == Anchor)
			{
				return 0.0f;
			}

			//get teh offset from the bone location
			Vector2 MyLocation = new Vector2(0.0f);
			Vector2 ScreenLocation = new Vector2(0.0f);
			ScreenLocation.X = (float)iX;
			ScreenLocation.Y = (float)iY;
			MyLocation = ScreenLocation - m_CurAnchorPos;

			//get the angle to that vector
			float fAngle = Helper.atan2(MyLocation);

			//get difference between current angle and the one specified in the animation to get parent rotation
			float fParentAngle = m_fCurrentRotation - m_AnchorJoint.CurrentKeyElement.Rotation;

			float fDiff = 0.0f;
			if (Flipped && !Anchor.CurrentKeyElement.Flip)
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
			Joint myJoint = new Joint(m_listJoints.Count);
			myJoint.Name = strJointName;
			m_listJoints.Add(myJoint);

			//add the data for that joint to all the images
			for (int i = 0; i < m_listImages.Count; i++)
			{
				m_listImages[i].AddJoint();
			}
		}

		/// <summary>
		/// Take this bone, see if there is a matching bone on teh right side of the model,
		/// copy its info into this dude.
		/// </summary>
		/// <param name="RootBone">the root bone of the model, used to search for matching bones</param>
		public void MirrorRightToLeft(Bone RootBone, CPasteAction ActionCollection)
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
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				m_listChildren[i].MirrorRightToLeft(RootBone, ActionCollection);
			}
		}

		/// <summary>
		/// Copy another bone's data into this dude
		/// </summary>
		/// <param name="SourceBone">the source to copy from</param>
		private void Copy(Bone SourceBone, CPasteAction ActionCollection)
		{
			Debug.Assert(null != SourceBone);

			for (int i = 0; i < m_listImages.Count; i++)
			{
				//check if the other bone uses this image
				Image matchingImage = SourceBone.GetImage(m_listImages[i].Filename);
				if (null != matchingImage)
				{
					m_listImages[i].Copy(matchingImage, ActionCollection);
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
			if (null != Anchor.Name)
			{
				if (null != rAnimations)
				{
					for (int i = 0; i < rAnimations.Animations.Count; i++)
					{
						rAnimations.Animations[i].RenameJoint(Anchor.Name, Name);
					}
				}

				//rename my dude
				Anchor.Name = Name;
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

#endif

		#endregion //Tools

		#region File IO

#if WINDOWS

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="rXMLNode">The xml node to read from</param>
		/// <param name="ParentBone">The parent bone for this dude.</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public virtual bool ReadSerializedFormat(XmlNode rXMLNode, Bone ParentBone, Renderer rRenderer)
		{
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

			//Read in child nodes
			if (rXMLNode.HasChildNodes)
			{
				for (XmlNode childNode = rXMLNode.FirstChild;
					null != childNode;
					childNode = childNode.NextSibling)
				{
					if (!ParseChildXMLNode(childNode, ParentBone, rRenderer))
					{
						return false;
					}
				}
			}

			//if it didnt find an anchor joint, set it to the joint that matches this dudes name
			if ("" == Anchor.Name)
			{
				if (null != ParentBone)
				{
					m_AnchorJoint = ParentBone.GetJoint(Name);
					if (null == m_AnchorJoint)
					{
						//add a joint to the parent
						m_AnchorJoint = new Joint(ParentBone.m_listJoints.Count);
						m_AnchorJoint.Name = Name;
						ParentBone.m_listJoints.Add(m_AnchorJoint);
					}
				}
			}

			//set the 'foot' flag as appropriate
			if (Name == "Left Foot" || Name == "Right Foot")
			{
				m_eBoneType = EBoneType.Foot;
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
		protected virtual bool ParseChildXMLNode(XmlNode childNode, Bone ParentBone, Renderer rRenderer)
		{
			//what is in this node?
			string strName = childNode.Name;
			string strValue = childNode.InnerText;

			if (strName == "name")
			{
				//set the name of this bone
				m_strBoneName = strValue;
			}
			else if (strName == "anchor")
			{
				//get the name of the anchor joint
				string strAnchorJoint = strValue;

				//get the anchor joint from the parent bone
				if (null != ParentBone)
				{
					m_AnchorJoint = ParentBone.GetJoint(strAnchorJoint);
					if (null == m_AnchorJoint)
					{
						Debug.Assert(false);
						return false;
					}
				}
			}
			else if (strName == "type")
			{
				if ("Foot" == strValue)
				{
					m_eBoneType = EBoneType.Foot;
				}
				else if ("Weapon" == strValue)
				{
					m_eBoneType = EBoneType.Weapon;
				}
				else
				{
					m_eBoneType = EBoneType.Normal;
				}
			}
			else if (strName == "colorable")
			{
				m_bColorable = Convert.ToBoolean(strValue);
			}
			else if (strName == "joints")
			{
				//Read in all the joints
				if (childNode.HasChildNodes)
				{
					for (XmlNode jointNode = childNode.FirstChild;
						null != jointNode;
						jointNode = jointNode.NextSibling)
					{
						Joint childJoint = new Joint(m_listJoints.Count);
						if (!childJoint.ReadSerializedFormat(jointNode))
						{
							Debug.Assert(false);
							return false;
						}
						m_listJoints.Add(childJoint);
					}
				}
			}
			else if (strName == "images")
			{
				//Read in all the images
				if (childNode.HasChildNodes)
				{
					for (XmlNode imageNode = childNode.FirstChild;
						null != imageNode;
						imageNode = imageNode.NextSibling)
					{
						Image childImage = new Image();
						if (!childImage.ReadSerializedFormat(imageNode, rRenderer, this))
						{
							Debug.Assert(false);
							return false;
						}
						m_listImages.Add(childImage);
					}
				}
			}
			else if (strName == "bones")
			{
				//read in all the child bones
				if (childNode.HasChildNodes)
				{
					for (XmlNode boneNode = childNode.FirstChild;
						null != boneNode;
						boneNode = boneNode.NextSibling)
					{
						Bone childBone = new Bone();
						if (!childBone.ReadSerializedFormat(boneNode, this, rRenderer))
						{
							Debug.Assert(false);
							return false;
						}
						m_listChildren.Add(childBone);
					}
				}
			}
			else
			{
				Debug.Assert(false);
				return false;
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
			rXMLFile.WriteString(m_strBoneName);
			rXMLFile.WriteEndElement();

			//add the type attribute
			rXMLFile.WriteStartElement("type");
			rXMLFile.WriteString(m_eBoneType.ToString());
			rXMLFile.WriteEndElement();

			//add whether or not this bone ignores palette swap
			rXMLFile.WriteStartElement("colorable");
			rXMLFile.WriteString(m_bColorable ? "true" : "false");
			rXMLFile.WriteEndElement();

			//write out joints
			rXMLFile.WriteStartElement("joints");
			for (int i = 0; i < m_listJoints.Count; i++)
			{
				m_listJoints[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			//write out images
			rXMLFile.WriteStartElement("images");
			for (int i = 0; i < m_listImages.Count; i++)
			{
				m_listImages[i].WriteXMLFormat(rXMLFile, fEnbiggify);
			}
			rXMLFile.WriteEndElement();

			//write out child bones
			rXMLFile.WriteStartElement("bones");
			for (int i = 0; i < m_listChildren.Count; i++)
			{
				//dont write out child garment bones
				if (m_listChildren[i] is GarmentBone)
				{
					continue;
				}
				m_listChildren[i].WriteXMLFormat(rXMLFile, false, fEnbiggify);
			}
			rXMLFile.WriteEndElement();
		}

#endif

		/// <summary>
		/// Read in all the bone information from an object read in from a serialized XML file.
		/// </summary>
		/// <param name="rBone">the xml object to get data from</param>
		/// <param name="ParentBone">The parent bone for this dude.</param>
		/// <param name="MyRenderer">The renderer to use to load images</param>
		public virtual bool ReadSerializedFormat(AnimationLib.BoneXML rBone, Bone ParentBone, Renderer rRenderer)
		{
			//copy all the data from that dude
			m_strBoneName = rBone.name;
			if (null != ParentBone)
			{
				m_AnchorJoint = ParentBone.GetJoint(m_strBoneName);
				if (null == m_AnchorJoint)
				{
					Debug.Assert(false);
					return false;
				}
			}
			m_bColorable = rBone.colorable;

			for (int i = 0; i < rBone.joints.Count; i++)
			{
				Joint myJoint = new Joint(i);
				AnimationLib.JointXML myJointXML = rBone.joints[i];
				if (!myJoint.ReadSerializedFormat(myJointXML))
				{
					Debug.Assert(false);
					return false;
				}
				m_listJoints.Add(myJoint);
			}
			for (int i = 0; i < rBone.images.Count; i++)
			{
				Image myImage = new Image();
				AnimationLib.ImageXML myImageXML = rBone.images[i];
				if (!myImage.ReadSerializedFormat(myImageXML, rRenderer))
				{
					Debug.Assert(false);
					return false;
				}
				m_listImages.Add(myImage);
			}
			for (int i = 0; i < rBone.bones.Count; i++)
			{
				Bone myBone = new Bone();
				AnimationLib.BoneXML childBoneXML = rBone.bones[i];
				if (!myBone.ReadSerializedFormat(childBoneXML, this, rRenderer))
				{
					Debug.Assert(false);
					return false;
				}
				m_listChildren.Add(myBone);
			}

			//set the 'foot', 'weapon' flags as appropriate
			if ("Foot" == rBone.type)
			{
				m_eBoneType = EBoneType.Foot;
			}
			else if ("Weapon" == rBone.type)
			{
				m_eBoneType = EBoneType.Weapon;
			}
			else
			{
				m_eBoneType = EBoneType.Normal;
			}

			return true;
		}

		#endregion //File IO
	}
}