using FilenameBuddy;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class SkeletonModel : XmlFileBuddy
	{
		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public BoneModel RootBone { get; protected set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public SkeletonModel(Filename filename)
			: base("skeleton", filename)
		{
			RootBone = new BoneModel();
		}

		public SkeletonModel(Skeleton skeleton, Filename filename)
			: base("skeleton", filename)
		{
			RootBone = new BoneModel(skeleton.RootBone);
		}

		#endregion //Methods

		#region File IO

		public override void ParseXmlNode(XmlNode node)
		{
			ReadChildNodes(node, RootBone.ParseXmlNode);
		}

		public override void WriteXmlNodes(XmlTextWriter xmlFile)
		{
			RootBone.WriteXmlNode(xmlFile);
		}

		#endregion //Model File IO
	}
}