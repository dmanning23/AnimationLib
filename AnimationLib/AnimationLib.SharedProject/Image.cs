using AnimationLib.Commands;
using DrawListBuddy;
using FilenameBuddy;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using UndoRedoBuddy;

namespace AnimationLib
{
	public class Image
	{
		#region Fields

		/// <summary>
		/// The anchor point for this frame
		/// this is where the series connects to the joint
		/// </summary>
		private Vector2 _anchorCoord;

		/// <summary>
		/// The bitmap for this frame
		/// </summary>
		private TextureInfo _texture;

		/// <summary>
		/// The amount of gravity to apply to ragdoll physics when this image is active
		/// </summary>
		private Vector2 _ragdollGravity;

		private float _ragdollSpring;

		#endregion //Fields

		#region Properties

		public string Name { get; set; }

		private Rectangle _sourceRectangle;
		public Rectangle SourceRectangle
		{
			get
			{
				return _sourceRectangle;
			}
			set
			{
				_sourceRectangle = value;
				if (null != TextureInfo)
				{
					TextureInfo.SourceRectangle = value;
				}
			}
		}

		public Filename ImageFile { get; set; }

		public Filename NormalMapFile { get; set; }

		public Filename ColorMaskFile { get; set; }

		/// <summary>
		/// Get or set the m_AnchorCoord member variable
		/// </summary>
		public Vector2 AnchorCoord
		{
			get { return _anchorCoord; }
			set { _anchorCoord = value; }
		}

		public Vector2 RagdollGravity
		{
			get
			{
				return _ragdollGravity;
			}
			set
			{
				_ragdollGravity = value;
				SpringForce = RagdollGravity.Length() * _ragdollSpring;
			}
		}

		/// <summary>
		/// The amount to spring back to center for this image.
		/// This is a percentage of gravity.
		/// This has to be bigger than 1.0 to beat gravity
		/// </summary>
		public float RagdollSpring 
		{
			get
			{
				return _ragdollSpring;
			}
			set
			{
				_ragdollSpring = value;
				SpringForce = RagdollGravity.Length() * _ragdollSpring;
			}
		}

		/// <summary>
		/// This is the actual numeric amount to bounce back to center
		/// </summary>
		public float SpringForce { get; protected set; }

		/// <summary>
		/// list of joint locations
		/// These are the coordinates of the joints for this frame
		/// </summary>
		/// <value>The JointCoords.</value>
		public List<JointData> JointCoords { get; protected set; }

		/// <summary>
		/// Physics JointCoords!
		/// </summary>
		/// <value>The circles.</value>
		public List<PhysicsCircle> Circles { get; private set; }

		/// <summary>
		/// physics JointCoords for level objects
		/// </summary>
		public List<PhysicsLine> Lines { get; private set; }

		public float Width
		{
			get { return SourceRectangle.Width; }
		}

		public float Height
		{
			get { return SourceRectangle.Height; }
		}

		public TextureInfo TextureInfo => _texture;

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Image()
		{
			JointCoords = new List<JointData>();
			Circles = new List<PhysicsCircle>();
			Lines = new List<PhysicsLine>();
			SourceRectangle = Rectangle.Empty;
			_anchorCoord = Vector2.Zero;
			_texture = null;
			ImageFile = new Filename();
			NormalMapFile = new Filename();
			ColorMaskFile = new Filename();
			_ragdollGravity = new Vector2(0, 1500f);
			RagdollSpring = 1.5f;
		}

		public Image(ImageModel image)
			: this()
		{
			Name = image.Name;
			SourceRectangle = new Rectangle((int)image.UpperLeft.X,
				(int)image.UpperLeft.Y,
				(int)(image.LowerRight.X - image.UpperLeft.X),
				(int)(image.LowerRight.Y - image.UpperLeft.Y));
			AnchorCoord = image.AnchorCoord;
			RagdollGravity = image.RagdollGravity;
			RagdollSpring = image.RagdollSpring;
			ImageFile = image.ImageFile;
			NormalMapFile = image.NormalMapFile;
			ColorMaskFile = image.ColorMaskFile;
			foreach (var jointCoord in image.JointCoords)
			{
				JointCoords.Add(new JointData(jointCoord, this));
			}
			foreach (var circle in image.Circles)
			{
				Circles.Add(new PhysicsCircle(circle));
			}
			foreach (var line in image.Lines)
			{
				Lines.Add(new PhysicsLine(line));
			}

			if (string.IsNullOrEmpty(Name))
			{
				Name = ImageFile.GetFile();
			}
		}

		/// <summary>
		/// Make sure this image has enough joint objects to match the bone
		/// </summary>
		/// <param name="bone">the bone that owns this image</param>
		public void SetAnchorJoint(Bone bone)
		{
			Debug.Assert(null != bone);

			//check if there are too many joint JointCoords objects
			while (JointCoords.Count > bone.Joints.Count)
			{
				JointCoords.RemoveAt(JointCoords.Count - 1);
			}

			//okay if there are not enough joint JointCoords objects, add some blank ones
			while (JointCoords.Count < bone.Joints.Count)
			{
				JointCoords.Add(new JointData());
			}
		}

		/// <summary>
		/// Load all the images for the model
		/// </summary>
		/// <param name="renderer"></param>
		public void LoadImage(IRenderer renderer)
		{
			//add the ability to have blank image, which means a skeletal structure that is not displayed
			if (!string.IsNullOrEmpty(ImageFile.GetFileExt()))
			{
				//do we need to load the image?
				if (null != renderer)
				{
					_texture = renderer.LoadImage(ImageFile, NormalMapFile, ColorMaskFile);

					//if the image is less than one pixel, set to the full image
					if (Width <= 0 || Height <= 0)
					{
						SourceRectangle = new Rectangle(Point.Zero, new Point(_texture.Width, _texture.Height));
					}
					_texture.SourceRectangle = SourceRectangle;
				}
			}
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Render this image
		/// </summary>
		/// <param name="drawList">the drawlist to put it in</param>
		/// <param name="position">where on the screen to draw the image</param>
		/// <param name="primaryColor"></param>
		/// <param name="secondaryColor"></param>
		/// <param name="rotation"></param>
		/// <param name="isFlipped"></param>
		/// <param name="layer">the layer to put the image at</param>
		public void Render(DrawList drawList, 
			Vector2 position,
			Color primaryColor,
			Color secondaryColor,
			float rotation, 
			bool isFlipped, 
			int layer)
		{
			if (null != _texture)
			{
				drawList.AddQuad(_texture, position, primaryColor, secondaryColor, rotation, isFlipped, layer);
			}
		}

		/// <summary>
		/// Get a joint vector from this dude
		/// </summary>
		/// <param name="index">teh index of the joint location to get</param>
		public JointData GetJointLocation(int index)
		{
			Debug.Assert(index >= 0);
			Debug.Assert(index < JointCoords.Count);
			return JointCoords[index];
		}

		public void AddJoint(bool insertBeginning)
		{
			JointData myJointCoords = new JointData();

			if (insertBeginning)
			{
				JointCoords.Insert(0, myJointCoords);
			}
			else
			{
				JointCoords.Add(myJointCoords);
			}

		}

		/// <summary>
		/// Gets the flipped anchor coordinate.
		/// </summary>
		/// <returns>The flipped anchor coordinate.</returns>
		/// <param name="isFlipped">Whether or not we want the anchor coord flipped</param>
		/// <param name="scale">The scale to get the anchor coord</param>
		public Vector2 GetFlippedAnchorCoord(bool isFlipped, float scale)
		{
			//Get the anchor coord
			Vector2 updatedAnchorCoord = new Vector2(0.0f);
			if (isFlipped)
			{
				//flip the x coord
				updatedAnchorCoord.X = Width - AnchorCoord.X;
				updatedAnchorCoord.Y = AnchorCoord.Y;
			}
			else
			{
				updatedAnchorCoord = AnchorCoord;
			}

			updatedAnchorCoord *= scale;
			return updatedAnchorCoord;
		}

		/// <summary>
		/// Gets the flipped anchor coordinate.
		/// </summary>
		/// <param name="jointIndex">the index of the joint to get the coord of</param>
		/// <param name="isFlipped">Whether or not we want the anchor coord flipped</param>
		/// <param name="scale">The scale to get the anchor coord</param>
		/// <returns>The flipped anchor coordinate.</returns>
		public Vector2 GetFlippedJointCoord(int jointIndex, bool isFlipped, float scale)
		{
			var updatedJointCoord = GetJointLocation(jointIndex).Location;
			if (isFlipped)
			{
				//flip the x coord
				updatedJointCoord.X = Width - updatedJointCoord.X;
			}

			updatedJointCoord *= scale;
			return updatedJointCoord;
		}

		public void Copy(Bone parent, Image myInst, CommandStack actionCollection)
		{
			_texture = new TextureInfo(myInst.TextureInfo);
			ImageFile = new Filename(myInst.ImageFile);
			NormalMapFile = new Filename(myInst.NormalMapFile);
			ColorMaskFile = new Filename(myInst.ColorMaskFile);
			RagdollGravity = myInst.RagdollGravity;
			RagdollSpring = myInst.RagdollSpring;

			//copy the anchor coord
			var myAnchorAction = new SetAnchorLocation(parent, this, myInst.AnchorCoord);
			actionCollection.Add(myAnchorAction);

			//copy all the joint JointCoords
			for (var i = 0; ((i < JointCoords.Count) && (i < myInst.JointCoords.Count)); i++)
			{
				//create the new joint JointCoords
				var myNewJointCoords = new JointData();
				myNewJointCoords.Copy(myInst.JointCoords[i]);

				//create the action to set it
				var mySetJointJointCoordsAction = new SetJointCoords(parent, this, i, myNewJointCoords);
				actionCollection.Add(mySetJointJointCoordsAction);
			}

			//copy circles
			for (var i = 0; ((i < Circles.Count) && (i < myInst.Circles.Count)); i++)
			{
				//create the actions to set the ciurcle
				actionCollection.Add(new SetCircleRadius(Circles[i], myInst.Circles[i].LocalRadius));
				actionCollection.Add(new SetCirclePosition(Circles[i], myInst.Circles[i].LocalPosition));
			}

			//copy lines
			for (var i = 0; ((i < Lines.Count) && (i < myInst.Lines.Count)); i++)
			{
				var myLine = new PhysicsLine();
				myLine.Copy(myInst.Lines[i]);

				//create the action to set it
				var mySetCircleAction = new SetLineData(this, i, myLine);
				actionCollection.Add(mySetCircleAction);
			}
		}

		public void SetJointCoords(int index, JointData newJointCoords)
		{
			Debug.Assert(index >= 0);
			Debug.Assert(index < JointCoords.Count);
			JointCoords[index] = newJointCoords;
		}

		public void Update(Vector2 bonePosition, float rotation, bool isFlipped, float scale)
		{
			//update all the circles
			for (var i = 0; i < Circles.Count; i++)
			{
				Debug.Assert(null != Circles);
				Debug.Assert(null != Circles[i]);
				Circles[i].Update(this, bonePosition, rotation, isFlipped, scale);
			}

			//update all the lines
			for (var i = 0; i < Lines.Count; i++)
			{
				Lines[i].Update(this, bonePosition, rotation, isFlipped, scale);
			}
		}

		public void DrawPhysics(IRenderer renderer, Color color)
		{
			for (var i = 0; i < Circles.Count; i++)
			{
				Circles[i].Render(renderer, color);
			}

			for (var i = 0; i < Lines.Count; i++)
			{
				Lines[i].Render(renderer, color);
			}
		}

		/// <summary>
		/// Check if this image contains any physics JointCoords
		/// </summary>
		/// <returns>true if there is physics JointCoords, false if not</returns>
		public bool HasPhysics()
		{
			return ((Circles.Count > 0) || (Lines.Count > 0));
		}

		/// <summary>
		/// Redo the whole scale of this model
		/// </summary>
		/// <param name="scale"></param>
		public void Rescale(float scale)
		{
			AnchorCoord = AnchorCoord * scale;

			for (var i = 0; i < JointCoords.Count; i++)
			{
				JointCoords[i].Rescale(scale);
			}
			
			for (var i = 0; i < Circles.Count; i++)
			{
				Circles[i].Rescale(scale);
			}

			for (var i = 0; i < Lines.Count; i++)
			{
				Lines[i].Rescale(scale);
			}
		}

		public override string ToString()
		{
			return Name;
		}

		#endregion //Methods
	}
}