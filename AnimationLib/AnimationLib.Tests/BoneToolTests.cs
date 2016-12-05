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

		#region ParentAngle Tests

		[Test]
		public void GetParentAngle_Rotation0()
		{
			_bone.Rotation = 0f;
			_bone.AnchorJoint.CurrentKeyElement.Rotation = 0f;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Rotation0()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(0f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Flip_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Flip_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Flip_Rotation0()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		[Test]
		public void GetParentAngle_ParentRotation90_Flip_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetParentAngle());
		}

		#endregion //ParentAngle Tests

		#region GetAngle Tests

		[Test]
		public void GetAngle_Rotation0()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = 0f;
			_bone.AnchorJoint.CurrentKeyElement.Rotation = 0f;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_Rotation90()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Rotation0()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Rotation90()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_Rotation0_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(0f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_Rotation90_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Rotation0_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Rotation90_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Flip_Rotation0_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Flip_Rotation90_Flip()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Flip_Rotation0()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		[Test]
		public void GetAngle_ParentRotation90_Flip_Rotation90()
		{
			_bone.AnchorJoint.Position = new Vector2(20f, 30f);
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(-90f), _bone.GetAngle(new Vector2(25f, 30f)));
		}

		#endregion GetAngle Tests

		#region Limit Rotations Tests

		[Test]
		public void GetLimitRotations_Rotation0()
		{
			_bone.Rotation = 0f;
			_bone.AnchorJoint.CurrentKeyElement.Rotation = 0f;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(15f), limit1);
		}

		[Test]
		public void GetLimitRotations_Rotation0_2()
		{
			_bone.Rotation = 0f;
			_bone.AnchorJoint.CurrentKeyElement.Rotation = 0f;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(60f), limit2);
		}

		[Test]
		public void GetLimitRotations_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(15f, MathHelper.ToDegrees(limit1));
		}

		[Test]
		public void GetLimitRotations_Rotation90_2()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(60f, MathHelper.ToDegrees(limit2));
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation0()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(105f, MathHelper.ToDegrees(limit1));
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation0_2()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(150f, MathHelper.ToDegrees(limit2));
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(105f, MathHelper.ToDegrees(limit1));
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation90_2()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(150f, MathHelper.ToDegrees(limit2));
		}

		/*
		[Test]
		public void GetLimitRotations_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(0f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(0f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Flip_Rotation0_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Flip_Rotation90_Flip()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = true;
			_bone.AnchorJoint.CurrentKeyElement.Flip = false;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Flip_Rotation0()
		{
			_bone.Rotation = MathHelper.ToRadians(90f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(0f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}

		[Test]
		public void GetLimitRotations_ParentRotation90_Flip_Rotation90()
		{
			_bone.Rotation = MathHelper.ToRadians(180f);
			_bone.AnchorJoint.CurrentKeyElement.Rotation = MathHelper.ToRadians(90f);
			_bone.Flipped = false;
			_bone.AnchorJoint.CurrentKeyElement.Flip = true;

			_bone.AnchorJoint.Data.FirstLimit = MathHelper.ToRadians(15f);
			_bone.AnchorJoint.Data.SecondLimit = MathHelper.ToRadians(60f);
			float limit1, limit2;
			_bone.GetLimitRotations(out limit1, out limit2);

			Assert.AreEqual(MathHelper.ToRadians(90f), _bone.GetLimitRotations());
		}
		*/

		#endregion //Limit Rotations Tests

		#endregion //Tests
	}
}
