
namespace AnimationLib
{
	/// <summary>
	/// These are the different types of things bones can be.
	/// They will respond differently to collisions depending on the bone type.
	/// </summary>
	public enum EBoneType
	{
		Foot, //this bone collides with the floor (also a "normal" bone)
		Weapon, //this bone only collides when it is in an active attack
		Normal, //this bone only collides with active weapons
	}
}