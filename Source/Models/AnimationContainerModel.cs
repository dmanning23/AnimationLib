using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using GameTimer;
using FilenameBuddy;
using DrawListBuddy;
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
	public class AnimationContainerModel : XmlFileBuddy
	{
		#region Properties

		/// <summary>
		/// get access to the model thing
		/// </summary>
		public SkeletonModel Skeleton { get; protected set; }

		/// 			<summary>
		/// the list of animations 
		/// </summary>
		/// <value>The animations.</value>
		public List<AnimationModel> Animations { get; protected set; }

		#endregion

		#region Methods

		/// <summary>
		/// hello, standard constructor!
		/// </summary>
		public AnimationContainerModel(Filename filename)
			: base("animationContainer", filename)
		{
			Skeleton = null;
			Animations = new List<AnimationModel>();
		}

		/// <summary>
		/// Overridden methoed to create the correct type of bone
		/// </summary>
		public virtual BoneModel CreateBone()
		{
			return new BoneModel();
		}

		#endregion //Methods

		#region File IO

		/// <summary>
		/// Read a model file from a serialized xml resource
		/// </summary>
		/// <param name="filename">filename of the resource to load</param>
		public void ReadSkeletonXml(Filename filename)
		{
			Skeleton = new SkeletonModel(filename);
			Skeleton.ReadXmlFile();
		}

		/// <summary>
		/// read in a list of animations from a serialized xml format file
		/// </summary>
		/// <param name="filename">filename of the animations to load</param>
		public virtual bool ReadAnimationXml(Filename filename)
		{
			Debug.Assert(null != Skeleton); //need a model to read in animations
			Animations.Clear();

			//gonna have to do this the HARD way

			//Open the file.
			#if ANDROID
			Stream stream = Game.Activity.Assets.Open(filename.File);
#else
			FileStream stream = File.Open(filename.File, FileMode.Open, FileAccess.Read);
			#endif
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(stream);
			XmlNode rootNode = xmlDoc.DocumentElement;

			//make sure it is actually an xml node
			if (rootNode.NodeType == XmlNodeType.Element)
			{
				//eat up the name of that xml node
				string strElementName = rootNode.Name;

#if DEBUG
				if (("XnaContent" != strElementName) || !rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
				if (!rootNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//next node is "Asset"
				XmlNode assetNode = rootNode.FirstChild;

#if DEBUG
				if (null == assetNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!assetNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//next node is "Animations"
				XmlNode animationsNode = assetNode.FirstChild;

#if DEBUG
				if (null == animationsNode)
				{
					Debug.Assert(false);
					return false;
				}
				if (!animationsNode.HasChildNodes)
				{
					Debug.Assert(false);
					return false;
				}
#endif

				//the rest of the nodes are animations
				if (!ReadAnimationsNode(animationsNode))
				{
					Debug.Assert(false);
					return false;
				}
			}
			else
			{
				//should be an xml node!!!
				Debug.Assert(false);
				return false;
			}

			// Close the file.
			stream.Close();

			//grab teh filename
			AnimationFile = filename;

			_playback = EPlayback.Forwards;
			CurrentAnimation = null;
			RestartAnimation();

			return true;
		}

		/// <summary>
		/// This node reads in all the animations from an xml node
		/// </summary>
		/// <param name="animationsNode">the xml node with all the animations underneath it</param>
		/// <returns>bool: whether or not an error occurred.</returns>
		protected virtual bool ReadAnimationsNode(XmlNode animationsNode)
		{
			for (var childNode = animationsNode.FirstChild;
				null != childNode;
				childNode = childNode.NextSibling)
			{
				Animation myAnimation = new Animation(Skeleton);
				Debug.Assert(null != Skeleton);
				if (!myAnimation.ReadXmlFormat(childNode, Skeleton))
				{
					Debug.Assert(false);
					return false;
				}
				Animations.Add(myAnimation);
			}

			return true;
		}

		public void WriteAnimationXml(Filename fileName)
		{
			//first rename all the joints so they are correct
			Debug.Assert(null != Skeleton);
			Skeleton.RenameJoints(this);

			//open the file, create it if it doesnt exist yet
			var xmlWriter = new XmlTextWriter(fileName.File, null);
			xmlWriter.Formatting = Formatting.Indented;
			xmlWriter.Indentation = 1;
			xmlWriter.IndentChar = '\t';

			xmlWriter.WriteStartDocument();

			//add the xml node
			xmlWriter.WriteStartElement("XnaContent");
			xmlWriter.WriteStartElement("Asset");
			xmlWriter.WriteAttributeString("Type", "AnimationLib.AnimationContainerXML");

			//write out joints
			xmlWriter.WriteStartElement("animations");
			for (int i = 0; i < Animations.Count; i++)
			{
				Debug.Assert(null != Skeleton);
				Animations[i].WriteXmlFormat(xmlWriter, Skeleton);
			}
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();

			xmlWriter.WriteEndDocument();

			// Close the file.
			xmlWriter.Flush();
			xmlWriter.Close();
		}

		public override void ParseXmlNode(XmlNode xmlNode)
		{
			throw new System.NotImplementedException();
		}

		public override void WriteXmlNodes(XmlTextWriter xmlFile)
		{
			throw new System.NotImplementedException();
		}

		#endregion //File IO

		
	}
}