using AnimationLib.Core.Json;
using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using System;
using System.ComponentModel;
#if !BRIDGE
using System.Xml;
#endif
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is a peice of a garment that will get added to the skeleton
	/// </summary>
	public class GarmentFragmentModel : XmlObject
	{
		#region Properties

		[JsonIgnore]
		public AnimationsModel AnimationContainer { get; set; }

		[JsonIgnore]
		public GarmentSkeletonModel Skeleton { get; set; }

		private ContentManager Content { get; set; }

		private float Scale { get; set; }

		private float _fragmentScale;
		[DefaultValue(1f)]
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

		/// <summary>
		/// Flag for whether or not this fragment completely covers other fragments underneath it.
		/// </summary>
		public bool DoesCover { get; set; }

		private Filename _modelFile { get; set; }

		private Filename _animationFile { get; set; }

		public string ModelFile
		{
			get
			{
				return _modelFile.HasFilename ? _modelFile.GetFilenameRelativeToPath(Garment.Filename) : string.Empty;
			}
			set
			{
				_modelFile.SetFilenameRelativeToPath(Garment.Filename, value);
			}
		}

		public string AnimationFile
		{
			get
			{
				return _animationFile.HasFilename ? _animationFile.GetFilenameRelativeToPath(Garment.Filename) : string.Empty;
			}
			set
			{
				_animationFile.SetFilenameRelativeToPath(Garment.Filename, value);
			}
		}

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(GarmentModel garment, float scale, ContentManager content = null)
		{
			Scale = scale;
			_fragmentScale = 1f;
			Content = content;
			Garment = garment;
			DoesCover = false;
			_modelFile = new Filename();
			_animationFile = new Filename();
		}

		public GarmentFragmentModel(GarmentModel garment, GarmentFragmentJsonModel fragment, float scale, ContentManager content = null) : this(garment, scale, content)
		{
			ParentBoneName = fragment.ParentBoneName;
			ModelFile = fragment.ModelFile;
			AnimationFile = fragment.AnimationFile;
			DoesCover = fragment.DoesCover;

			ReadModelJson(_modelFile);
			ReadAnimationsJson(_animationFile);
		}

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(GarmentFragment fragment, GarmentModel garment) : this(garment, 1f)
		{
			_modelFile = fragment.SkeletonFile;
			_animationFile = fragment.AnimationFile;
			Skeleton = new GarmentSkeletonModel(fragment.SkeletonFile, fragment.AnimationContainer.Skeleton as GarmentSkeleton, this);
			AnimationContainer = new AnimationsModel(fragment.AnimationFile, fragment.AnimationContainer);
			FragmentScale = fragment.FragmentScale;

			ParentBoneName = fragment.ParentBoneName;
			DoesCover = fragment.DoesCover;
		}

		#endregion //Methods

		#region File IO

		private void ReadAnimationsXml(Filename animationFile)
		{
			AnimationContainer = new AnimationsModel(animationFile, Scale);
			AnimationContainer.ReadXmlFile(Content);
		}

		private void ReadModelXml(Filename skeletonFile)
		{
			Skeleton = new GarmentSkeletonModel(skeletonFile, Scale, this);
			Skeleton.ReadXmlFile(Content);
		}

#if !BRIDGE
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
							_modelFile = new Filename(value);
							ReadModelXml(_modelFile);
						}
						break;
					case "animation":
						{
							//read in the animations
							_animationFile = new Filename(value);
							ReadAnimationsXml(_animationFile);
						}
						break;
					case "model1":
						{
							//read in the model, relative to the garment file
							_modelFile = new Filename();
							_modelFile.SetFilenameRelativeToPath(Garment.Filename, value);
							ReadModelXml(_modelFile);
						}
						break;
					case "animation1":
						{
							//read in the animations, relative to the garment file
							_animationFile = new Filename();
							_animationFile.SetFilenameRelativeToPath(Garment.Filename, value);
							ReadAnimationsXml(_animationFile);
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
					case "doesCover":
						{
							DoesCover = Convert.ToBoolean(value);
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
			_modelFile.ChangeExtension("xml");
			Skeleton.Filename.ChangeExtension("xml");
			xmlWriter.WriteStartElement("model1");
			xmlWriter.WriteString(Skeleton.Filename.GetFilenameRelativeToPath(Garment.Filename));
			xmlWriter.WriteEndElement();

			//write out animation filename to use
			_animationFile.ChangeExtension("xml");
			AnimationContainer.Filename.ChangeExtension("xml");
			xmlWriter.WriteStartElement("animation1");
			xmlWriter.WriteString(AnimationContainer.Filename.GetFilenameRelativeToPath(Garment.Filename));
			xmlWriter.WriteEndElement();

			//add the parentBone
			xmlWriter.WriteStartElement("parentBone");
			xmlWriter.WriteString(ParentBoneName);
			xmlWriter.WriteEndElement();

			if (DoesCover)
			{
				xmlWriter.WriteStartElement("doesCover");
				xmlWriter.WriteString(DoesCover.ToString());
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();

			//write out the model file
			Skeleton.WriteXml();

			//write out the animation file
			AnimationContainer.WriteXml();
		}

		public void WriteJson()
		{
			_modelFile.ChangeExtension("json");
			Skeleton.Filename.ChangeExtension("json");
			Skeleton.WriteJson();

			_animationFile.ChangeExtension("json");
			AnimationContainer.Filename.ChangeExtension("json");
			AnimationContainer.WriteJson();
		}
#endif

		private void ReadAnimationsJson(Filename animationFile)
		{
			AnimationContainer = new AnimationsModel(animationFile, Scale);
			AnimationContainer.ReadJsonFile(Content);
		}

		private void ReadModelJson(Filename skeletonFile)
		{
			Skeleton = new GarmentSkeletonModel(skeletonFile, Scale, this);
			Skeleton.ReadJsonFile(Content);
		}

		#endregion //File IO
	}
}