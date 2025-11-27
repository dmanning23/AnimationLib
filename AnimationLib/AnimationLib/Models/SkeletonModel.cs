using AnimationLib.Core.Json;
using FilenameBuddy;
using Microsoft.Xna.Framework.Content;
using System.Xml;
using XmlBuddy;

namespace AnimationLib
{
    /// <summary>
    /// This is the object that contains the whole skeleton + all animations
    /// </summary>
    public class SkeletonModel : XmlFileBuddy
    {
        #region Properties

        /// <summary>
        /// get access to the model thing
        /// </summary>
        public BoneModel RootBone { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        /// hello, standard constructor!
        /// </summary>
        public SkeletonModel(Filename filename, float scale, float fragmentScale)
            : base("skeleton", filename)
        {
            RootBone = new BoneModel(this, scale, fragmentScale);
        }

        public SkeletonModel(Skeleton skeleton, Filename filename)
            : base("skeleton", filename)
        {
            RootBone = new BoneModel(this, skeleton.RootBone);
        }

        #endregion //Methods

        #region File IO

        public override void ParseXmlNode(XmlNode node)
        {
            ReadChildNodes(node, RootBone.ParseXmlNode);
        }

        public override void WriteXmlNodes(XmlTextWriter xmlFile)
        {
            RootBone.WriteXmlNodes(xmlFile);
        }

        public override void ReadJsonFile(ContentManager content = null)
        {
            using (var jsonModel = new SkeletonJsonModel(this.ContentName, this.Filename))
            {
                //read the json file
                jsonModel.ReadJsonFile(content);

                //load from the json structure
                var scale = RootBone.Scale;
                var fragmentScale = RootBone.FragmentScale;
                RootBone = new BoneModel(this, jsonModel.RootBone, scale, fragmentScale);
            }
        }

        #endregion //Model File IO
    }
}