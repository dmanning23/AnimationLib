using CollisionBuddy;
using MatrixExtensions;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	public class PhysicsCircleModel : XmlObject
	{
		#region Properties

		public Vector2 LocalPosition { get; set; }

		public float LocalRadius{ get; set; }

		#endregion //Properties

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public PhysicsCircleModel()
		{
			LocalPosition = Vector2.Zero;
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
				case "center":
				{
					LocalPosition = value.ToVector2();
				}
				break;
				case "radius":
				{
					LocalRadius = Convert.ToSingle(value);
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
		public void WriteXmlFormat(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("center", LocalPosition.X + " " +
				LocalPosition.Y);
			xmlWriter.WriteAttributeString("radius", LocalRadius.ToString());
			xmlWriter.WriteEndElement();
		}

		#endregion //File IO
	}
}