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
		/// The upper left corner in the file of this image
		/// </summary>
		private Vector2 _upperLeft;

		/// <summary>
		/// the lower right corner in the file of this image
		/// </summary>
		private Vector2 _lowerRight;

		/// <summary>
		/// The anchor point for this frame
		/// this is where the series connects to the joint
		/// </summary>
		private Vector2 _anchorCoord;

		/// <summary>
		/// The bitmap for this frame
		/// </summary>
		private ITexture _texture;

		#endregion //Fields

		#region Properties

		public Vector2 UpperLeft
		{
			get { return _upperLeft; }
			set { _upperLeft = value; }
		}

		public Vector2 LowerRight
		{
			get { return _lowerRight; }
			set { _lowerRight = value; }
		}

		public Filename ImageFile { get; set; }

		/// <summary>
		/// Get or set the m_AnchorCoord member variable
		/// </summary>
		public Vector2 AnchorCoord
		{
			get { return _anchorCoord; }
			set { _anchorCoord = value; }
		}

		public float Width
		{
			get { return LowerRight.X - UpperLeft.X; }
		}

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
			UpperLeft = Vector2.Zero;
			LowerRight = Vector2.Zero;
			_anchorCoord = Vector2.Zero;
			_texture = null;
			ImageFile = new Filename();
		}

		public Image(ImageModel image)
			: this()
		{
			UpperLeft = image.UpperLeft;
			LowerRight = image.LowerRight;
			AnchorCoord = image.AnchorCoord;
			ImageFile = image.ImageFile;
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
			if (ImageFile.GetFileExt().Length > 0)
			{
				//do we need to load the image?
				if (null != renderer)
				{
					_texture = renderer.LoadImage(ImageFile);
					if (null == _texture)
					{
						Debug.Assert(false);
					}

					UpperLeft = Vector2.Zero;
					LowerRight = new Vector2(_texture.Width, _texture.Height);
				}
			}
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Render this image
		/// </summary>
		/// <param name="position">where on the screen to draw the image</param>
		/// <param name="drawList">the drawlist to put it in</param>
		/// <param name="layer">the layer to put the image at</param>
		/// <param name="rotation"></param>
		/// <param name="isFlipped"></param>
		/// <param name="color"></param>
		public void Render(Vector2 position, DrawList drawList, int layer, float rotation, bool isFlipped, Color color)
		{
			if (null != _texture)
			{
				drawList.AddQuad(_texture, position, color, rotation, isFlipped, layer);
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

		public void AddJoint()
		{
			JointData myJointCoords = new JointData();
			JointCoords.Add(myJointCoords);
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
			Vector2 updatedJointCoord = GetJointLocation(jointIndex).Location;
			if (isFlipped)
			{
				//flip the x coord
				updatedJointCoord.X = Width - updatedJointCoord.X;
			}

			updatedJointCoord *= scale;
			return updatedJointCoord;
		}

		public void Copy(Image myInst, Macro actionCollection)
		{
			//copy the anchor coord
			SetAnchorLocation myAnchorAction = new SetAnchorLocation(this, (int)myInst.AnchorCoord.X, (int)myInst.AnchorCoord.Y);
			actionCollection.Add(myAnchorAction);

			//copy all the joint JointCoords
			for (var i = 0; ((i < JointCoords.Count) && (i < myInst.JointCoords.Count)); i++)
			{
				//create the new joint JointCoords
				JointData myNewJointCoords = new JointData();
				myNewJointCoords.Copy(myInst.JointCoords[i]);

				//create the action to set it
				SetJointCoords mySetJointJointCoordsAction = new SetJointCoords(this, i, myNewJointCoords);
				actionCollection.Add(mySetJointJointCoordsAction);
			}

			//copy circles
			for (var i = 0; ((i < Circles.Count) && (i < myInst.Circles.Count)); i++)
			{
				PhysicsCircle myCircle = new PhysicsCircle();
				myCircle.Copy(myInst.Circles[i]);

				//create the action to set it
				SetCircleData mySetCircleAction = new SetCircleData(this, i, myCircle);
				actionCollection.Add(mySetCircleAction);
			}

			//copy lines
			for (var i = 0; ((i < Lines.Count) && (i < myInst.Lines.Count)); i++)
			{
				PhysicsLine myLine = new PhysicsLine();
				myLine.Copy(myInst.Lines[i]);

				//create the action to set it
				SetLineData mySetCircleAction = new SetLineData(this, i, myLine);
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
			return ImageFile.GetFile();
		}

		#endregion //Methods
	}
}