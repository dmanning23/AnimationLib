using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
using System;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is a peice of a garment that will get added to the skeleton
	/// </summary>
	public class GarmentFragmentModel : XmlObject
	{
		#region Properties

		public AnimationsModel AnimationContainer { get; set; }

		public GarmentSkeletonModel Skeleton { get; set; }

		private ContentManager Content { get; set; }

		private float Scale { get; set; }

		private float _fragmentScale;
		public float FragmentScale
		{
			get { return _fragmentScale; }
			set
			{
				_fragmentScale = value;
				Scale *= _fragmentScale;
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(float scale, ContentManager content = null)
		{
			Scale = scale;
			_fragmentScale = 1f;
			Content = content;
		}

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(GarmentFragment fragment)
			: this(1f)
		{
			Skeleton = new GarmentSkeletonModel(fragment.SkeletonFile, fragment.AnimationContainer.Skeleton as GarmentSkeleton);
			AnimationContainer = new AnimationsModel(fragment.AnimationFile, fragment.AnimationContainer);
			FragmentScale = fragment.FragmentScale;
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;
			try
			{

				switch (name)
				{
					case "Type":
						{
							//throw these attributes out
						}
						break;
					case "model":
						{
							//read in the model 
							var skeletonFile = new Filename(value);
							Skeleton = new GarmentSkeletonModel(skeletonFile, Scale, FragmentScale);
							Skeleton.ReadXmlFile(Content);
						}
						break;
					case "animation":
						{
							//read in the animations
							var animationFile = new Filename(value);
							AnimationContainer = new AnimationsModel(animationFile, Scale);
							AnimationContainer.ReadXmlFile(Content);
						}
						break;
					case "scale":
						{
							//Make sure to put the scale first in the file!
							FragmentScale = Convert.ToSingle(value);
						}
						break;
					default:
						{
							base.ParseXmlNode(node);
						}
						break;
				}
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("error in garment fragment file \"{0}\"", value), ex);
			}
		}

#if !WINDOWS_UWP
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("fragment");

			if (1f != FragmentScale)
			{
				xmlWriter.WriteStartElement("scale");
				xmlWriter.WriteString(Scale.ToString());
				xmlWriter.WriteEndElement();
			}

			//write out model filename to use
			xmlWriter.WriteStartElement("model");
			xmlWriter.WriteString(Skeleton.Filename.GetRelFilename());
			xmlWriter.WriteEndElement();

			//write out animation filename to use
			xmlWriter.WriteStartElement("animation");
			xmlWriter.WriteString(AnimationContainer.Filename.GetRelFilename());
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();

			//write out the model file
			Skeleton.WriteXml();

			//write out the animation file
			AnimationContainer.WriteXml();
		}
#endif

		#endregion //File IO
	}
}