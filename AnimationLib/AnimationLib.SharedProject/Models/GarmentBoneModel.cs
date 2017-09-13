using System.Xml;

namespace AnimationLib
{
	/// <summary>
	/// This is a special type of bone, that is animated based on the parent bone, can be added and removed at runtime, etc.
	/// </summary>
	public class GarmentBoneModel : BoneModel
	{
		#region Properties

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; protected set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public GarmentBoneModel(float scale, float fragmentScale)
			: base(scale, fragmentScale)
		{
			BoneType = EBoneType.Garment;
		}

		public GarmentBoneModel(GarmentBone bone)
			: base(bone)
		{
			ParentBoneName = bone.ParentBoneName;
			BoneType = EBoneType.Garment;
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
				case "parentBone":
				{
					//set teh parent bone of this dude
					ParentBoneName = value;
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

#if !WINDOWS_UWP
		/// <summary>
		/// Write this dude out to the xml format
		/// </summary>
		/// <param name="xmlWriter">the xml file to add this dude as a child of</param>
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("garmentbone");

			//add the name attribute
			xmlWriter.WriteAttributeString("parentBone", ParentBoneName);

			WriteChildXmlNode(xmlWriter);

			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //File IO
	}
}