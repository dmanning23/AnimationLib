using FilenameBuddy;
using System.Collections.Generic;
#if !BRIDGE
using System.Xml;
#endif
using XmlBuddy;

namespace AnimationLib.Core.Json
{
	/// <summary>
	/// This object reads a list of aniamtions from a file.
	/// </summary>
	public class AnimationsJsonModel : XmlFileBuddy
	{
		/// <summary>
		/// the list of animations 
		/// </summary>
		/// <value>The animations.</value>
		public List<AnimationJsonModel> Animations { get; set; }

		public AnimationsJsonModel(string contentName) : base(contentName)
		{
		}

		public AnimationsJsonModel(string contentName, Filename file) : base(contentName, file)
		{
		}

#if !BRIDGE
		public AnimationsJsonModel(XmlFileBuddy obj) : base(obj)
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