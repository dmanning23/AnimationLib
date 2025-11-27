using Microsoft.Xna.Framework;

namespace AnimationLib.Core.Json
{
    public class JointDataJsonModel
    {
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
    }
}