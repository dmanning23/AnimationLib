using Microsoft.Xna.Framework;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
    public class ColorTagModel : XmlObject
    {
        public string Tag { get; set; }
        public Color Color { get; set; }

        public ColorTagModel()
        {
            Color = Color.White;
        }

        public ColorTagModel(string tag, Color color)
        {
            Tag = tag;
            Color = color;
        }

        public override void ParseXmlNode(XmlNode node)
        {
            //what is in this node?
            var name = node.Name;
            var value = node.InnerText;

            switch (name.ToLower())
            {
                case "tag":
                    {
                        Tag = value;
                    }
                    break;
                case "color":
                    {
                        Color = value.ToColor();
                    }
                    break;
                default:
                    {
                        base.ParseXmlNode(node);
                    }
                    break;
            }
        }

        public override void WriteXmlNodes(XmlTextWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("color");
            xmlWriter.WriteAttributeString("tag", Tag);
            xmlWriter.WriteAttributeString("color", Color.StringFromColor());
            xmlWriter.WriteEndElement();
        }
    }
}
