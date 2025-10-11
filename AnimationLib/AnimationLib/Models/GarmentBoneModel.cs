using AnimationLib.Core.Json;
using Newtonsoft.Json;
using System;
#if !BRIDGE
using System.Xml;
#endif

namespace AnimationLib
{
	/// <summary>
	/// This is a special type of bone, that is animated based on the parent bone, can be added and removed at runtime, etc.
	/// </summary>
	public class GarmentBoneModel : BoneModel
	{
		#region Properties

		[JsonIgnore]
		public GarmentFragmentModel FragmentModel { get; private set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBoneModel(SkeletonModel skeleton, float scale, GarmentFragmentModel fragment)
			: base(skeleton, scale, fragment.FragmentScale)
		{
			FragmentModel = fragment;
		}

		public GarmentBoneModel(SkeletonModel skeleton, BoneJsonModel bone, float scale, float fragmentScale, GarmentFragmentModel fragment)
			: base(skeleton, bone, scale, fragmentScale)
		{
			FragmentModel = fragment;
		}

		public GarmentBoneModel(SkeletonModel skeleton, GarmentBone bone, GarmentFragmentModel fragment)
			: base(skeleton, bone)
		{
			FragmentModel = fragment;
		}

		#endregion //Methods

		#region File IO

#if !BRIDGE
		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

			switch (name)
			{
				case "parentBone":
				{
					//this is the legacy stuff
					FragmentModel.ParentBoneName = value;
				}
				break;
				case "addToTools":
					{
						AddToTools = Convert.ToBoolean(value);
					}
					break;
				default:
				{
					//Let the base class parse the rest of the xml
					base.ParseXmlNode(node);
				}
				break;
			}
		}

		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("garmentbone");

			//add whether or not this garment is hacked into the bone tools
			if (AddToTools)
			{
				xmlWriter.WriteAttributeString("addToTools", "true");
			}

			WriteChildXmlNode(xmlWriter);

			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //File IO
	}
}