using FilenameBuddy;
using System.Collections.Generic;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This object reads a list of aniamtions from a file.
	/// </summary>
	public class AnimationsModel : XmlFileBuddy
	{
		#region Properties

		/// <summary>
		/// the list of animations 
		/// </summary>
		/// <value>The animations.</value>
		public List<AnimationModel> Animations { get; protected set; }

		private float Scale { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationsModel(Filename filename, float scale)
			: base("animationContainer", filename)
		{
			Scale = scale;
			Animations = new List<AnimationModel>();
		}

		public AnimationsModel(Filename filename, AnimationContainer animations)
			: this(filename, 1f)
		{
			foreach (var animation in animations.Animations)
			{
				Animations.Add(new AnimationModel(animation.Value, animations.Skeleton));
			}
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			string name = node.Name;
			string value = node.InnerText;

			switch (name)
			{
				case "Asset":
				{
					//This is an old garment file that still has the XNA format.
					XmlFileBuddy.ReadChildNodes(node, ParseXmlNode);
				}
				break;
				case "Type":
				{
					//throw these attributes out
				}
				break;
				case "animations":
				{
					XmlFileBuddy.ReadChildNodes(node, ReadAnimation);
				}
				break;
				default:
				{
					NodeError(node);
				}
				break;
			}
		}

		/// <summary>
		/// This node reads in an animation from an xml node
		/// </summary>
		public void ReadAnimation(XmlNode node)
		{
			var animation = new AnimationModel(Scale);
			XmlFileBuddy.ReadChildNodes(node, animation.ParseXmlNode);
			Animations.Add(animation);
		}

#if !WINDOWS_UWP
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			//write out joints
			xmlWriter.WriteStartElement("animations");
			foreach (var animation in Animations)
			{
				animation.WriteXmlNodes(xmlWriter);
			}
			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //File IO
	}
}