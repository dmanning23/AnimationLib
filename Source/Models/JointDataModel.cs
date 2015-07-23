using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	public class JointDataModel : XmlObject
	{
		#region Properties

		/// <summary>
		/// get the joint location
		/// </summary>
		public Vector2 Location { get; set; }

		/// <summary>
		/// The distance from teh anchor position to this joint position
		/// </summary>
		public float Length { get; private set; }

		/// <summary>
		/// Whether this joint will use floating or rotating ragdoll
		/// Float uses the ragdollradius to float around the anchor joint
		/// chained uses the limits to rotate around the anchor joint
		/// </summary>
		public bool Floating { get; set; }

		/// <summary>
		/// The radius of the circle that ragdoll is allowed to float around
		/// </summary>
		public float FloatRadius { get; set; }

		public float FirstLimit { get; set; }

		public float SecondLimit { get; set; }

		#endregion

		#region Methods

		public JointDataModel()
		{
			Location = Vector2.Zero;
			FirstLimit = 0.0f;
			SecondLimit = 0.0f;
			FloatRadius = 0.0f;
			Floating = false;
		}

		public override void ParseXmlNode(XmlNode node)
		{
			//what is in this node?
			var name = node.Name;
			var value = node.InnerText;

			switch (name)
			{
				case "location":
				{
					Location = value.ToVector2();
				}
				break;
				case "limit1":
				{
					float fMyLimit = Convert.ToSingle(value);
					FirstLimit = MathHelper.ToRadians(fMyLimit);
				}
				break;
				case "limit2":
				{
					float fMyLimit = Convert.ToSingle(value);
					SecondLimit = MathHelper.ToRadians(fMyLimit);
				}
				break;
				case "FloatRadius":
				{
					FloatRadius = Convert.ToSingle(value);
				}
				break;
				case "FloatOrRotate":
				{
					Floating = Convert.ToBoolean(value);
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
		/// <param name="scale"></param>
		public void WriteXmlFormat(XmlTextWriter xmlWriter, float scale)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("Item");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.JointDataXML");

			//write out joint offset
			xmlWriter.WriteStartElement("location");
			xmlWriter.WriteString((_location.X * scale).ToString() + " " +
				(_location.Y * scale).ToString());
			xmlWriter.WriteEndElement();

			//write first limit 
			xmlWriter.WriteStartElement("limit1");
			float fLimit1 = Helper.ClampAngle(FirstLimit);
			xmlWriter.WriteString(MathHelper.ToDegrees(fLimit1).ToString());
			xmlWriter.WriteEndElement();

			//write 2nd limit 
			xmlWriter.WriteStartElement("limit2");
			float fLimit2 = Helper.ClampAngle(SecondLimit);
			xmlWriter.WriteString(MathHelper.ToDegrees(fLimit2).ToString());
			xmlWriter.WriteEndElement();

			//write out float radius
			xmlWriter.WriteStartElement("FloatRadius");
			xmlWriter.WriteString((FloatRadius * scale).ToString());
			xmlWriter.WriteEndElement();

			//write out whether it uses float or rotate ragdoll
			xmlWriter.WriteStartElement("FloatOrRotate");
			xmlWriter.WriteString(Floating ? "true" : "false");
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
		}

		#endregion //Methods
	}
}