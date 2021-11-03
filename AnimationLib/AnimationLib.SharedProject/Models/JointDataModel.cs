using Microsoft.Xna.Framework;
using System;
#if !BRIDGE
using System.Xml;
#endif
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
		/// Whether this joint will use floating or rotating ragdoll
		/// Float uses the ragdollradius to float around the anchor joint
		/// chained uses the limits to rotate around the anchor joint
		/// </summary>
		public RagdollType RagdollType { get; set; }

		/// <summary>
		/// The radius of the circle that ragdoll is allowed to float around
		/// </summary>
		public float FloatRadius { get; set; }

		public float FirstLimit { get; set; }

		public float SecondLimit { get; set; }

		private float Scale { get; set; }

		#endregion

		#region Methods

		public JointDataModel(float scale)
		{
			Scale = scale;
			Location = Vector2.Zero;
			FirstLimit = -180.0f;
			SecondLimit = 180.0f;
			FloatRadius = 0.0f;
			RagdollType = RagdollType.None;
		}

		public JointDataModel(JointData jointData, float scale = 1f) : this(scale)
		{
			Location = jointData.Location;
			RagdollType = jointData.RagdollType;
			FloatRadius = jointData.FloatRadius;
			FirstLimit = jointData.FirstLimit;
			SecondLimit = jointData.SecondLimit;
		}

#if !BRIDGE
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
				case "location":
					{
						Location = value.ToVector2() * Scale;
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
						FloatRadius = Convert.ToSingle(value) * Scale;
					}
					break;
				case "FloatOrRotate":
					{
						var ragdollType = Convert.ToBoolean(value);
						if (ragdollType)
						{
							RagdollType = RagdollType.Float;
						}
						else
						{
							RagdollType = RagdollType.Limit;
						}
					}
					break;
				case "RagdollType":
					{
						switch (value)
						{
							case "Float":
								{
									RagdollType = RagdollType.Float;
								}
								break;

							case "Limit":
								{
									RagdollType = RagdollType.Limit;
								}
								break;
							default:
								{
									RagdollType = RagdollType.None;
								}
								break;
						}
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
		public override void WriteXmlNodes(XmlTextWriter xmlWriter)
		{
			//write out the item tag
			xmlWriter.WriteStartElement("jointData");

			switch (RagdollType)
			{
				case RagdollType.Float:
					{
						xmlWriter.WriteAttributeString("RagdollType", "Float");

						//write out float radius
						if (0.0f != FloatRadius)
						{
							float fLimit2 = Helper.ClampAngle(SecondLimit);
							xmlWriter.WriteAttributeString("FloatRadius", FloatRadius.ToString());
						}
					}
					break;
				case RagdollType.Limit:
					{
						xmlWriter.WriteAttributeString("RagdollType", "Limit");

						//write first limit
						float fLimit1 = MathHelper.ToDegrees(Helper.ClampAngle(FirstLimit));
						if (-180f < fLimit1)
						{
							xmlWriter.WriteAttributeString("limit1", fLimit1.ToString());
						}

						//write 2nd limit 
						float fLimit2 = MathHelper.ToDegrees(Helper.ClampAngle(SecondLimit));
						if (180f > fLimit2)
						{
							xmlWriter.WriteAttributeString("limit2", fLimit2.ToString());
						}
					}
					break;
			}

			//write out joint offset
			if (Location != Vector2.Zero)
			{
				xmlWriter.WriteStartElement("location");
				xmlWriter.WriteString((Location / Scale).StringFromVector());
				xmlWriter.WriteEndElement();
			}

			xmlWriter.WriteEndElement();
		}
#endif

		#endregion //Methods
	}
}