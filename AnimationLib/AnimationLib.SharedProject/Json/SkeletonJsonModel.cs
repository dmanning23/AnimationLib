using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
#if !BRIDGE
using System.Xml;
#endif
using XmlBuddy;

namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class SkeletonJsonModel : XmlFileBuddy
	{
		/// <summary>
		/// get access to the model thing
		/// </summary>
		public BoneJsonModel RootBone { get; set; }

		public SkeletonJsonModel(string contentName) : base(contentName)
		{
		}

		public SkeletonJsonModel(string contentName, Filename file) : base(contentName, file)
		{
		}

#if !BRIDGE
		public SkeletonJsonModel(XmlFileBuddy obj) : base(obj)
		{
		}

		public override void ParseXmlNode(XmlNode node)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			throw new System.NotImplementedException();
		}
#endif
	}
}