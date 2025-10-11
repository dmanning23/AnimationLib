using FilenameBuddy;
using Microsoft.Xna.Framework;
using RenderBuddy;
using System.Collections.Generic;

namespace AnimationLib
{
	/// <summary>
	/// This is a peice of a garment that will get added to the skeleton
	/// </summary>
	public class GarmentFragment
	{
		#region Properties

		public Filename SkeletonFile { get; set; }

		public Filename AnimationFile { get; set; }

		/// <summary>
		/// The name of the garment that this guy is a peice of.
		/// </summary>
		public string GarmentName { get; set; }

		public GarmentAnimationContainer AnimationContainer { get; private set; }

		public string BoneName
		{
			get
			{
				return AnimationContainer.Skeleton.RootBone.Name;
			}
		}

		public float FragmentScale { get; set; }

		/// <summary>
		/// The bone in the skeleton that this garment attaches to
		/// </summary>
		public string ParentBoneName { get; set; }

		/// <summary>
		/// Flag for whether or not this fragment completely covers other fragments underneath it.
		/// </summary>
		public bool DoesCover { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragment(Garment garment)
		{
			AnimationContainer = new GarmentAnimationContainer(garment);
			FragmentScale = 1f;
			DoesCover = false;
		}

		public GarmentFragment(GarmentFragmentModel fragment, IRenderer renderer, Garment garment)
		{
			SkeletonFile = new Filename(fragment.Skeleton.Filename);
			AnimationFile = new Filename(fragment.AnimationContainer.Filename);
			AnimationContainer = new GarmentAnimationContainer(fragment.AnimationContainer, fragment.Skeleton, renderer, garment);
			AnimationContainer.Scale *= fragment.FragmentScale;
			FragmentScale = fragment.FragmentScale;
			ParentBoneName = fragment.ParentBoneName;
			DoesCover = fragment.DoesCover;
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToSkeleton()
		{
			//add all the garment bones to the bones they attach to
			AnimationContainer.AddToSkeleton();
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromSkeleton()
		{
			//remove all the garment bones from the bones they attach to
			AnimationContainer.RemoveFromSkeleton();
		}

		/// <summary>
		/// Get a list of all the weapon
		/// </summary>
		/// <param name="listWeapons"></param>
		public void GetAllWeaponBones(List<string> listWeapons)
		{
			AnimationContainer.Skeleton.RootBone.GetAllWeaponBones(listWeapons);
		}

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		public void SetGarmentBones(Skeleton characterSkeleton)
		{
			//set garment name in the bones
			AnimationContainer.GarmentName = GarmentName;

			//set the parent bone of all those root node garment bones
			AnimationContainer.SetGarmentBones(characterSkeleton, ParentBoneName);
		}

		public override string ToString()
		{
			return ParentBoneName;
		}

		#region Color Methods

		/// <summary>
		/// Set the primary color of this garment
		/// </summary>
		/// <param name="color"></param>
		public void SetPrimaryColor(Color color)
		{
			AnimationContainer.Skeleton.RootBone.SetPrimaryColor(color);
		}

		/// <summary>
		/// Set the secondary color of this garment
		/// </summary>
		/// <param name="color"></param>
		public void SetSecondaryColor(Color color)
		{
			AnimationContainer.Skeleton.RootBone.SetSecondaryColor(color);
		}

		public void SetColors(ColorRepository colors)
		{
			AnimationContainer.Skeleton.RootBone.SetColors(colors);
		}

		#endregion //Color Methods

		#endregion //Methods
	}
}