using Microsoft.Xna.Framework;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationLib.Tests
{
	[TestFixture]
	public class BoneToolTests
	{
		#region Fields

		Bone _bone;

		#endregion //Fields

		#region Setup

		[SetUp]
		public void Setup()
		{
			_bone = new Bone();
		}

		#endregion //Setup

		#region Tests

		#region ConvertCoord Tests

		[Test]
		public void ConvertCoord_Regular_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(5f, result.X);
		}

		[Test]
		public void ConvertCoord_Regular_Y()
		{
			_bone.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertCoord_Scaled_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertCoord(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(5f, result.X);
		}

		[Test]
		public void ConvertCoord_Scaled_Y()
		{
			_bone.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertCoord(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertCoord_Rotated_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(15f, result.X);
		}

		[Test]
		public void ConvertCoord_Rotated_Y()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertCoord(new Vector2(15f, 45f), 1f);
			Assert.AreEqual(5f, result.Y);
		}

		[Test]
		public void ConvertCoord_Flipped_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(-5f, result.X);
		}

		[Test]
		public void ConvertCoord_Flipped_Y()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertCoord_Flipped_Scaled_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertCoord(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(-5f, result.X);
		}

		[Test]
		public void ConvertCoord_Flipped_Rotated_X()
		{
			_bone.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertCoord(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(-15f, result.X);
		}

		#endregion //ConvertCoord Tests

		#region ConvertTranslation Tests

		[Test]
		public void ConvertTranslation_Regular_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(5f, result.X);
		}

		[Test]
		public void ConvertTranslation_Regular_Y()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertTranslation_Scaled_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertTranslation(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(5f, result.X);
		}

		[Test]
		public void ConvertTranslation_Scaled_Y()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			var result = _bone.ConvertTranslation(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertTranslation_Rotated_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(5f, result.X);
		}

		[Test]
		public void ConvertTranslation_Rotated_Y()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertTranslation(new Vector2(15f, 45f), 1f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertTranslation_Flipped_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(-5f, result.X);
		}

		[Test]
		public void ConvertTranslation_Flipped_Y()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(15f, result.Y);
		}

		[Test]
		public void ConvertTranslation_Flipped_Scaled_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			var result = _bone.ConvertTranslation(new Vector2(30f, 60f), 2f);
			Assert.AreEqual(-5f, result.X);
		}

		[Test]
		public void ConvertTranslation_Flipped_Rotated_X()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			var result = _bone.ConvertTranslation(new Vector2(25f, 45f), 1f);
			Assert.AreEqual(-5f, result.X);
		}

		#endregion //ConvertTranslation Tests

		//[Test]
		//public void GetAngle_NoRotation_0()
		//{
		//	_bone.Position = new Vector2(20f, 30f);
		//	var result = _bone.GetAngle(new Vector2(25f, 30f));
		//	Assert.AreEqual(0f, result);
		//}

		//[Test]
		//public void GetAngle_NoRotation_90()
		//{
		//	_bone.Position = new Vector2(20f, 30f);
		//	var result = _bone.GetAngle(new Vector2(20f, 35f));
		//	Assert.AreEqual(MathHelper.ToRadians(90f), result);
		//}

		#endregion //Tests
	}
}
