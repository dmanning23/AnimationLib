using AnimationLib.Commands;
using DrawListBuddy;
using FilenameBuddy;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;
using UndoRedoBuddy;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	public class ImageModel : XmlObject
	{
		#region Properties

		public Vector2 UpperLeft { get; set; }

		public Vector2 LowerRight { get; set; }

		public Vector2 AnchorCoord { get; set; }

		public Filename ImageFile { get; set; }

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

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public ImageModel()
		{
			JointCoords = new List<JointDataModel>();
			Circles = new List<PhysicsCircleModel>();
			Lines = new List<PhysicsLineModel>();
			UpperLeft = Vector2.Zero;
			LowerRight = Vector2.Zero;
			AnchorCoord = Vector2.Zero;
			ImageFile = new Filename();
		}

		public override string ToString()
		{
			return ImageFile.GetFile();
		}

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
					AnchorCoord = value.ToVector2();
				}
				break;
				case "filename":
				{
					//get the correct path & filename
					ImageFile.SetRelFilename(value);
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
			var jointData = new JointDataModel();
			jointData.ParseXmlNode(node);
			JointCoords.Add(jointData);
		}

		public void ReadCircle(XmlNode node)
		{
			var circle= new PhysicsCircleModel();
			circle.ParseXmlNode(node);
			Circles.Add(circle);
		}

		public void ReadLine(XmlNode node)
		{
			var line = new PhysicsLineModel();
			line.ParseXmlNode(node);
			Lines.Add(line);
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		/// <param name="scale"></param>
		public void WriteXmlFormat(XmlTextWriter xmlWriter, float scale)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.ImageXML");

			//write out upper left coords
			xmlWriter.WriteStartElement("upperleft");
			xmlWriter.WriteString(UpperLeft.X.ToString() + " " +
				UpperLeft.Y.ToString());
			xmlWriter.WriteEndElement();

			//write out lower right coords
			xmlWriter.WriteStartElement("lowerright");
			xmlWriter.WriteString(LowerRight.X.ToString() + " " +
				LowerRight.Y.ToString());
			xmlWriter.WriteEndElement();

			//write out lower right coords
			xmlWriter.WriteStartElement("anchorcoord");
			xmlWriter.WriteString((_anchorCoord.X * scale).ToString() + " " +
				(_anchorCoord.Y * scale).ToString());
			xmlWriter.WriteEndElement();

			//write out filename to use
			xmlWriter.WriteStartElement("filename");
			xmlWriter.WriteString(ImageFile.GetRelFilename());
			xmlWriter.WriteEndElement();

			//write out joint locations
			xmlWriter.WriteStartElement("joints");
			for (var i = 0; i < JointCoords.Count; i++)
			{
				JointCoords[i].WriteXmlFormat(xmlWriter, scale);
			}
			xmlWriter.WriteEndElement();

			//write out polygon info
			xmlWriter.WriteStartElement("circles");
			for (var i = 0; i < Circles.Count; i++)
			{
				Circles[i].WriteXmlFormat(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteStartElement("lines");
			for (var i = 0; i < Lines.Count; i++)
			{
				Lines[i].WriteXmlFormat(xmlWriter);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //Methods
	}
}