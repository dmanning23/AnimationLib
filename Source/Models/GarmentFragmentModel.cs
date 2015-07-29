using FilenameBuddy;
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

		#endregion //Properties

		#region Methods

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel()
		{
		}

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragmentModel(GarmentFragment fragment)
		{
			Skeleton = new GarmentSkeletonModel(fragment.SkeletonFile, fragment.AnimationContainer.Skeleton as GarmentSkeleton);

			AnimationContainer = new AnimationsModel(fragment.AnimationFile, fragment.AnimationContainer);
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

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
					Skeleton = new GarmentSkeletonModel(skeletonFile);
					Skeleton.ReadXmlFile();
				}
				break;
				case "animation":
				{
					//read in the animations
					var animationFile = new Filename(value);
					AnimationContainer = new AnimationsModel(animationFile);
					AnimationContainer.ReadXmlFile();
				}
				break;
				default:
				{
					base.ParseXmlNode(node);
				}
				break;
			}
		}

		public override void WriteXmlNode(XmlTextWriter xmlWriter)
		{
			//write out model filename to use
			xmlWriter.WriteStartElement("model");
			xmlWriter.WriteString(Skeleton.Filename.GetRelFilename());
			xmlWriter.WriteEndElement();

			//write out animation filename to use
			xmlWriter.WriteStartElement("animation");
			xmlWriter.WriteString(AnimationContainer.Filename.GetRelFilename());
			xmlWriter.WriteEndElement();

			//write out the model file
			Skeleton.WriteXml();

			//write out the animation file
			AnimationContainer.WriteXml();
		}

		#endregion File IO
	}
}