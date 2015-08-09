using FilenameBuddy;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;

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
				Debug.Assert(null != AnimationContainer);
				Debug.Assert(null != AnimationContainer.Skeleton);
				return AnimationContainer.Skeleton.RootBone.Name; 
			}
		}

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// constructor!
		/// </summary>
		public GarmentFragment()
		{
			AnimationContainer = new GarmentAnimationContainer();
		}

		public GarmentFragment(GarmentFragmentModel fragment, IRenderer renderer)
			: this()
		{
			AnimationContainer = new GarmentAnimationContainer(fragment.AnimationContainer, fragment.Skeleton, renderer);
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
			AnimationContainer.SetGarmentBones(characterSkeleton);
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

		#endregion //Color Methods

		#endregion //Methods
	}
}