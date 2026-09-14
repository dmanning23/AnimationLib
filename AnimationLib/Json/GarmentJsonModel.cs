using FilenameBuddy;
using System.Collections.Generic;
using System.Xml;
using XmlBuddy;

namespace AnimationLib.Core.Json
{
    /// <summary>
    /// This is a class that is used to put together a garment that can be added to a model skeleton
    /// </summary>
    public class GarmentJsonModel : XmlFileBuddy
    {
        /// <summary>
        /// The name of this garment.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// A list of all the garment fragments that get added to the skeleton
        /// </summary>
        public List<GarmentFragmentJsonModel> Fragments { get; set; }

        public List<ColorTagJsonModel> Colors { get; set; }

        public GarmentJsonModel(string contentName) : base(contentName)
        {
        }

        public GarmentJsonModel(string contentName, Filename file) : base(contentName, file)
        {
        }

        public GarmentJsonModel(XmlFileBuddy obj) : base(obj)
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
    }
}