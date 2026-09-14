
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnimationLib.Core.Json
{
    public class ImageJsonModel
    {
        public string Name { get; set; }

        public Vector2 UpperLeft { get; set; }

        public Vector2 LowerRight { get; set; }

        public Vector2 AnchorCoord { get; set; }

        public Vector2 RagdollGravity { get; set; }

        public float RagdollSpring { get; set; } = 1.5f;

        public string Image { get; set; }

        public string NormalMap { get; set; }

        public string ColorMask { get; set; }

        /// <summary>
        /// list of joint locations
        /// These are the coordinates of the joints for this frame
        /// </summary>
        /// <value>The JointCoords.</value>
        public List<JointDataJsonModel> JointCoords { get; set; }

        /// <summary>
        /// Physics JointCoords!
        /// </summary>
        /// <value>The circles.</value>
        public List<PhysicsCircleJsonModel> Circles { get; set; }

        /// <summary>
        /// physics JointCoords for level objects
        /// </summary>
        public List<PhysicsLineJsonModel> Lines { get; set; }
    }
}