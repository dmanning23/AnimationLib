using AnimationLib.Core.Json;
using FilenameBuddy;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if !BRIDGE
using System.Xml;
#endif
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	public class ImageModel : XmlObject
	{
		#region Properties

		private SkeletonModel SkeletonModel { get; set; }

		public string Name { get; set; }

		public Vector2 UpperLeft { get; set; }

		public Vector2 LowerRight { get; set; }

		public Vector2 AnchorCoord { get; set; }

		public Vector2 RagdollGravity { get; set; }

		[DefaultValue(1.5f)]
		public float RagdollSpring { get; set; }

		[JsonIgnore]
		public Filename ImageFile { get; set; }

		[DefaultValue("")]
		public string Image
		{
			get
			{
				return ImageFile.HasFilename ? ImageFile.GetFilenameRelativeToPath(SkeletonModel.Filename) : string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					ImageFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
				}
			}
		}

		[JsonIgnore]
		public Filename NormalMapFile { get; set; }

		[DefaultValue("")]
		public string NormalMap
		{
			get
			{
				return NormalMapFile.HasFilename ? NormalMapFile.GetFilenameRelativeToPath(SkeletonModel.Filename) : string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					NormalMapFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
				}
			}
		}

		[JsonIgnore]
		public Filename ColorMaskFile { get; set; }

		[DefaultValue("")]
		public string ColorMask
		{
			get
			{
				return ColorMaskFile.HasFilename ? ColorMaskFile.GetFilenameRelativeToPath(SkeletonModel.Filename) : string.Empty;
			}
			set
			{
				if (!string.IsNullOrEmpty(value))
				{
					ColorMaskFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
				}
			}
		}

		/// <summary>
		/// list of joint locations
		/// These are the coordinates of the joints for this frame
		/// </summary>
		/// <value>The JointCoords.</value>
		public List<JointDataModel> JointCoords { get; private set; }

		/// <summary>
		/// Physics JointCoords!
		/// </summary>
		/// <value>The circles.</value>
		public List<PhysicsCircleModel> Circles { get; private set; }

		/// <summary>
		/// physics JointCoords for level objects
		/// </summary>
		public List<PhysicsLineModel> Lines { get; private set; }

		[JsonIgnore]
		public float Scale { get; private set; }

		[JsonIgnore]
		public float FragmentScale { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public ImageModel(SkeletonModel skeleton, float scale, float fragmentScale)
		{
			Scale = scale;
			FragmentScale = fragmentScale;
			JointCoords = new List<JointDataModel>();
			Circles = new List<PhysicsCircleModel>();
			Lines = new List<PhysicsLineModel>();
			UpperLeft = Vector2.Zero;
			LowerRight = Vector2.Zero;
			AnchorCoord = Vector2.Zero;
			RagdollGravity = new Vector2(0f, 1500f);
			RagdollSpring = 1.5f;
			ImageFile = new Filename();
			NormalMapFile = new Filename();
			ColorMaskFile = new Filename();
			SkeletonModel = skeleton;
		}

		public ImageModel(SkeletonModel skeleton, ImageJsonModel image, float scale, float fragmentScale)
			: this(skeleton, scale, fragmentScale)
		{
			Name = image.Name;
			UpperLeft = image.UpperLeft;
			LowerRight = image.LowerRight;
			AnchorCoord = image.AnchorCoord;
			RagdollSpring = image.RagdollSpring;
			RagdollGravity = image.RagdollGravity;
			Image = image.Image;
			NormalMap = image.NormalMap;
			ColorMask = image.ColorMask;
			foreach (var jointCoord in image.JointCoords)
			{
				JointCoords.Add(new JointDataModel(jointCoord, FragmentScale));
			}
			foreach (var circle in image.Circles)
			{
				Circles.Add(new PhysicsCircleModel(circle, Scale, FragmentScale));
			}
			foreach (var line in image.Lines)
			{
				Lines.Add(new PhysicsLineModel(line, Scale));
			}
		}

		public ImageModel(SkeletonModel skeleton, Image image)
			: this(skeleton, 1f, 1f)
		{
			Scale = image.Scale;
			FragmentScale = image.FragmentScale;
			Name = image.Name;
			UpperLeft = new Vector2(image.SourceRectangle.Left, image.SourceRectangle.Top);
			LowerRight = new Vector2(image.SourceRectangle.Right, image.SourceRectangle.Bottom);
			AnchorCoord = image.AnchorCoord;
			RagdollSpring = image.RagdollSpring;
			RagdollGravity = image.RagdollGravity;
			ImageFile = image.TextureInfo != null ? image.ImageFile : new Filename();
			NormalMapFile = image.NormalMapFile;
			ColorMaskFile = image.ColorMaskFile;
			foreach (var jointCoord in image.JointCoords)
			{
				JointCoords.Add(new JointDataModel(jointCoord, FragmentScale));
			}
			foreach (var circle in image.Circles)
			{
				Circles.Add(new PhysicsCircleModel(circle, Scale, FragmentScale));
			}
			foreach (var line in image.Lines)
			{
				Lines.Add(new PhysicsLineModel(line));
			}
		}

		public override string ToString()
		{
			return Name;
		}

#if !BRIDGE
		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="node">The xml node to read from</param>
		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			string name = node.Name;
			string value = node.InnerText;

			switch (name)
			{
				case "Type":
					{
						//throw these attributes out
					}
					break;
				case "name":
					{
						Name = value;
					}
					break;
				case "upperleft":
					{
						//convert to the correct vector
						UpperLeft = value.ToVector2();
					}
					break;
				case "lowerright":
					{
						//convert to the correct vector
						LowerRight = value.ToVector2();
					}
					break;
				case "anchorcoord":
					{
						//convert to the correct vector
						AnchorCoord = value.ToVector2() * Scale;
					}
					break;
				case "RagdollGravity":
					{
						//convert to the correct vector
						RagdollGravity = value.ToVector2();
					}
					break;
				case "RagdollSpring":
					{
						RagdollSpring = Convert.ToSingle(value);
					}
					break;
				case "filename":
					{
						//get the correct path & filename
						ImageFile.SetRelFilename(value);
					}
					break;
				case "normalMapFilename":
					{
						//get the correct path & filename
						NormalMapFile.SetRelFilename(value);
					}
					break;
				case "colorMaskFilename":
					{
						//get the correct path & filename
						ColorMaskFile.SetRelFilename(value);
					}
					break;
				case "imageFile1":
					{
						//read in a relative to the model file
						ImageFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
					}
					break;
				case "normalMap1":
					{
						//get the correct path & filename
						NormalMapFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
					}
					break;
				case "colorMask1":
					{
						//get the correct path & filename
						ColorMaskFile.SetFilenameRelativeToPath(SkeletonModel.Filename, value);
					}
					break;
				case "joints":
					{
						//Read in all the joint JointCoords
						XmlFileBuddy.ReadChildNodes(node, ReadJointData);
					}
					break;
				case "circles":
					{
						//Read in all the circles
						XmlFileBuddy.ReadChildNodes(node, ReadCircle);
					}
					break;
				case "lines":
					{
						//Read in all the lines
						XmlFileBuddy.ReadChildNodes(node, ReadLine);
					}
					break;
				default:
					{
						base.ParseXmlNode(node);
					}
					break;
			}
		}

		public void ReadJointData(XmlNode node)
		{
			var jointData = new JointDataModel(Scale);
			XmlFileBuddy.ReadChildNodes(node, jointData.ParseXmlNode);
			JointCoords.Add(jointData);
		}

		public void ReadCircle(XmlNode node)
		{
			var circle = new PhysicsCircleModel(Scale, FragmentScale);
			XmlFileBuddy.ReadChildNodes(node, circle.ParseXmlNode);
			Circles.Add(circle);
		}

		public void ReadLine(XmlNode node)
		{
			var line = new PhysicsLineModel(Scale);
			XmlFileBuddy.ReadChildNodes(node, line.ParseXmlNode);
			Lines.Add(line);
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		/// <param name="scale"></param>
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("image");

			if (!string.IsNullOrEmpty(Name) && Name != ImageFile.GetFile())
			{
				xmlWriter.WriteAttributeString("name", Name);
			}

			if (RagdollGravity.X != 0f || RagdollGravity.Y != 1500f)
			{
				xmlWriter.WriteAttributeString("RagdollGravity", RagdollGravity.StringFromVector());
			}

			if (RagdollSpring != 1.5f)
			{
				xmlWriter.WriteAttributeString("RagdollSpring", RagdollSpring.ToString());
			}

			if (!string.IsNullOrEmpty(ImageFile.File))
			{
				//write out filename to use
				xmlWriter.WriteAttributeString("imageFile1", ImageFile.GetFilenameRelativeToPath(SkeletonModel.Filename));
			}

			if (!string.IsNullOrEmpty(NormalMapFile.File))
			{
				xmlWriter.WriteAttributeString("normalMap1", NormalMapFile.GetFilenameRelativeToPath(SkeletonModel.Filename));
			}

			if (!string.IsNullOrEmpty(ColorMaskFile.File))
			{
				xmlWriter.WriteAttributeString("colorMask1", ColorMaskFile.GetFilenameRelativeToPath(SkeletonModel.Filename));
			}

			//write out upper left coords
			if (UpperLeft != Vector2.Zero)
			{
				xmlWriter.WriteStartElement("upperleft");
				xmlWriter.WriteString(UpperLeft.StringFromVector());
				xmlWriter.WriteEndElement();
			}

			//write out lower right coords
			if (LowerRight != Vector2.Zero)
			{
				xmlWriter.WriteStartElement("lowerright");
				xmlWriter.WriteString(LowerRight.StringFromVector());
				xmlWriter.WriteEndElement();
			}

			//write out lower right coords
			if (AnchorCoord != Vector2.Zero)
			{
				xmlWriter.WriteStartElement("anchorcoord");
				var anchor = AnchorCoord / FragmentScale;
				xmlWriter.WriteString(anchor.StringFromVector());
				xmlWriter.WriteEndElement();
			}

			//write out joint locations
			if (JointCoords.Count > 0)
			{
				xmlWriter.WriteStartElement("joints");
				foreach (var jointCoord in JointCoords)
				{
					jointCoord.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			//write out polygon info
			if (Circles.Count > 0)
			{
				xmlWriter.WriteStartElement("circles");
				foreach (var circle in Circles)
				{
					circle.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			if (Lines.Count > 0)
			{
				xmlWriter.WriteStartElement("lines");
				foreach (var line in Lines)
				{
					line.WriteXmlNodes(xmlWriter);
				}
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //Methods
	}
}