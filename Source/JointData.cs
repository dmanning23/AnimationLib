using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Xml;
using Vector2Extensions;

namespace AnimationLib
{
	public class JointData
	{
		#region Members

		/// <summary>
		/// the joint location
		/// </summary>
		private Vector2 _location;

		/// <summary>
		/// the first rotation limit for ragdoll
		/// user -180 and 180 as default limits
		/// </summary>
		private float _firstLimit = -MathHelper.Pi;

		/// <summary>
		/// the second rotation limit for ragdoll
		/// user -180 and 180 as default limits
		/// </summary>
		private float _secondLimit = MathHelper.Pi;

		/// <summary>
		/// This is the vector from this joint position to the anchor location
		/// </summary>
		private Vector2 _anchorVect;

		#endregion //Members

		#region Properties

		/// <summary>
		/// get the joint location
		/// </summary>
		public Vector2 Location
		{
			get { return _location; }
			set { _location = value; }
		}

		/// <summary>
		/// The distance from teh anchor position to this joint position
		/// </summary>
		public float Length { get; private set; }

		public Vector2 AnchorVect
		{
			get { return _anchorVect; }
		}

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

		public float FirstLimit
		{
			get { return _firstLimit; }
			set 
			{
				_firstLimit = value;
				UpdateFlippedLimits();
			}
		}

		public float SecondLimit
		{
			get { return _secondLimit; }
			set 
			{ 
				_secondLimit = value;
				UpdateFlippedLimits();
			}
		}

		/// <summary>
		/// the first rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		public float FirstLimitFlipped { get; private set; }

		/// <summary>
		/// the second rotation limit for ragdoll when parent bone is flipped
		/// </summary>
		public float SecondLimitFlipped { get; private set; }

		#endregion

		#region Methods

		public JointData()
		{
			_location = Vector2.Zero;
			_anchorVect = Vector2.Zero;
			Length = 0.0f;
			FloatRadius = 0.0f;
			Floating = false;
		}

		public void Copy(JointData inst)
		{
			_location = inst._location;
			FirstLimit = inst.FirstLimit;
			SecondLimit = inst.SecondLimit;
			FloatRadius = inst.FloatRadius;
			Floating = inst.Floating;
			Length = inst.Length;
			_anchorVect = inst._anchorVect;
		}

		/// <summary>
		/// Called when one of the limits is set to update the flipped limits
		/// </summary>
		private void UpdateFlippedLimits()
		{
			//Subtract the limits from 180 and switch them
			SecondLimitFlipped = -1.0f * FirstLimit;
			FirstLimitFlipped = -1.0f * SecondLimit;
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read in all the bone information from a file in the serialized XML format
		/// </summary>
		/// <param name="node">The xml node to read from</param>
		/// <param name="image"></param>
		/// <returns>bool: whether or not it was able to read from the xml</returns>
		public bool ReadXmlFormat(XmlNode node, Image image)
		{
			//get the vector from the anchor position to this joint position
			_anchorVect = Location - image.AnchorCoord;
			Length = _anchorVect.Length();

			return true;
		}

		/// <summary>
		/// Redo the whole scale of this model
		/// </summary>
		/// <param name="scale"></param>
		public void Rescale(float scale)
		{
			Location = Location*scale;

			FloatRadius *= scale;
		}

		#endregion
	}
}