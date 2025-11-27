using AnimationLib.Core.Json;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
    public class PhysicsCircleModel : XmlObject
    {
        #region Properties

        public Vector2 Center { get; set; }

        public float Radius { get; set; }

        [JsonProperty]
        private float Scale { get; set; }

        [JsonProperty]
        private float FragmentScale { get; set; }

        #endregion //Properties

        #region Methods

        /// <summary>
        /// hello, standard constructor!
        /// </summary>
        [JsonConstructor]
        public PhysicsCircleModel(float scale, float fragmentScale)
        {
            Scale = scale;
            FragmentScale = fragmentScale;
            Center = Vector2.Zero;
        }

        public PhysicsCircleModel(PhysicsCircleJsonModel circle, float scale, float fragmentScale)
            : this(scale, fragmentScale)
        {
            Center = circle.Center;
            Radius = circle.Radius;
        }

        public PhysicsCircleModel(PhysicsCircle circle, float scale, float fragmentScale)
            : this(scale, fragmentScale)
        {
            Center = circle.LocalPosition;
            Radius = circle.LocalRadius;
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
                case "center":
                    {
                        Center = value.ToVector2() * Scale;
                    }
                    break;
                case "radius":
                    {
                        Radius = Convert.ToSingle(value) * Scale;
                    }
                    break;
                default:
                    {
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
            //write out the item tag
            xmlWriter.WriteStartElement("Item");
            xmlWriter.WriteAttributeString("center", (Center / FragmentScale).StringFromVector());
            xmlWriter.WriteAttributeString("radius", (Radius / FragmentScale).ToString());
            xmlWriter.WriteEndElement();
        }

        #endregion //File IO
    }
}