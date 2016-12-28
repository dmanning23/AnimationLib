using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationLib
{
	/// <summary>
	/// The different types of ragdoll joints
	/// </summary>
    public enum RagdollType
    {
		/// <summary>
		/// the ragdoll joint is a hinge the spins full 360 degrees
		/// </summary>
		None,

		/// <summary>
		/// The ragdoll joint has a pivot that it is anchored to 
		/// </summary>
		Float,

		/// <summary>
		/// The ragdoll joint is a hinge with limits
		/// </summary>
		Limit
    }
}
