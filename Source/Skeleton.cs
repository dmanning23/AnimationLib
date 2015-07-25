using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using GameTimer;
using FilenameBuddy;
using DrawListBuddy;
using Microsoft.Xna.Framework.Graphics;
using RenderBuddy;
#if OUYA
using Ouya.Console.Api;
#endif
using Vector2Extensions;
using XmlBuddy;

namespace AnimationLib
{
	/// <summary>
	/// This is the object that contains the whole skeleton + all animations
	/// </summary>
	public class Skeleton
	{
		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public Bone RootBone { get; protected set; }

		public Filename ModelFile { get; set; }

		private IRenderer Renderer { get; set; }

		private AnimationContainer Animations { get; set; }

		public float Scale { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public Skeleton(AnimationContainer animations, IRenderer renderer)
		{
			Animations = animations;
			Renderer = renderer;
			RootBone = null;
			ModelFile = new Filename();
		}

		#endregion //Methods

		#region File IO

		public void ReadXmlFile()
		{
			Animations.CreateBone();

			//base.ReadXmlFile();

			//Set the anchor joints of the whole model
			RootBone.SetAnchorJoint(null);

			//Load all the images
			RootBone.LoadImages(Renderer);
		}

		public void WriteXml()
		{
			//first rename all the joints so they are correct
			RootBone.RenameJoints(Animations);
			//base.WriteXml();
		}

		#endregion //Model File IO
	}
}