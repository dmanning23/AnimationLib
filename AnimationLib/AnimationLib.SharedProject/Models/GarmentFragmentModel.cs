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

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; set; }

		private GarmentModel Garment { get; set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(float scale, GarmentModel garment, ContentManager content = null)
		{
			Scale = scale;
			_fragmentScale = 1f;
			Content = content;
			Garment = garment;
		}

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(GarmentFragment fragment, GarmentModel garment)
			: this(1f, garment)
		{
			Skeleton = new GarmentSkeletonModel(fragment.SkeletonFile, fragment.AnimationContainer.Skeleton as GarmentSkeleton, this);
			AnimationContainer = new AnimationsModel(fragment.AnimationFile, fragment.AnimationContainer);
			FragmentScale = fragment.FragmentScale;

			ParentBoneName = fragment.ParentBoneName;
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
							ReadModel(skeletonFile);
						}
						break;
					case "animation":
						{
							//read in the animations
							var animationFile = new Filename(value);
							ReadAnimations(animationFile);
						}
						break;
					case "model1":
						{
							//read in the model, relative to the garment file
							var skeletonFile = new Filename();
							skeletonFile.SetFilenameRelativeToPath(Garment.Filename, value);
							ReadModel(skeletonFile);
						}
						break;
					case "animation1":
						{
							//read in the animations, relative to the garment file
							var animationFile = new Filename();
							animationFile.SetFilenameRelativeToPath(Garment.Filename, value);
							ReadAnimations(animationFile);
						}
						break;
					case "scale":
						{
							//Make sure to put the scale first in the file!
							FragmentScale = Convert.ToSingle(value);
						}
						break;
					case "parentBone":
						{
							//set teh parent bone of this dude
							ParentBoneName = value;
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

		private void ReadAnimations(Filename animationFile)
		{
			AnimationContainer = new AnimationsModel(animationFile, Scale);
			AnimationContainer.ReadXmlFile(Content);
		}

		private void ReadModel(Filename skeletonFile)
		{
			Skeleton = new GarmentSkeletonModel(skeletonFile, Scale, this);
			Skeleton.ReadXmlFile(Content);
		}

#if !WINDOWS_UWP
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("fragment");

			WriteFragmentData(xmlWriter);

			xmlWriter.WriteEndElement();

			//write out the model file
			Skeleton.WriteXml();

			//write out the animation file
			AnimationContainer.WriteXml();
		}

		protected virtual void WriteFragmentData(XmlTextWriter xmlWriter)
		{
			if (1f != FragmentScale)
			{
				xmlWriter.WriteStartElement("scale");
				xmlWriter.WriteString(Scale.ToString());
				xmlWriter.WriteEndElement();
			}

			//write out model filename to use
			xmlWriter.WriteStartElement("model1");
			xmlWriter.WriteString(Skeleton.Filename.GetFilenameRelativeToPath(Garment.Filename));
			xmlWriter.WriteEndElement();

			//write out animation filename to use
			xmlWriter.WriteStartElement("animation1");
			xmlWriter.WriteString(AnimationContainer.Filename.GetFilenameRelativeToPath(Garment.Filename));
			xmlWriter.WriteEndElement();

			//add the parentBone
			xmlWriter.WriteStartElement("parentBone");
			xmlWriter.WriteString(ParentBoneName);
			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //File IO
	}
}