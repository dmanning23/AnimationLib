using FilenameBuddy;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;

namespace AnimationLib
{
	/// <summary>
	/// This is a class that is used to put together a garment that can be added to a model skeleton
	/// </summary>
	public class Garment
	{
		#region Properties

		/// <summary>
		/// The name of this garment.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// A list of all the garment fragments that get added to the skeleton
		/// </summary>
		public List<GarmentFragment> Fragments { get; private set; }

		/// <summary>
		/// Whether or not the garment has any physics data in it.
		/// </summary>
		public bool HasPhysics { get; private set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// constructor!
		/// </summary>
		public Garment()
		{
			Fragments = new List<GarmentFragment>();
			HasPhysics = false;
		}

		public Garment(Filename filename, Skeleton skeleton, IRenderer renderer)
			: this()
		{
			var garmentModel = new GarmentModel(filename);
			garmentModel.ReadXmlFile();
			Name = garmentModel.Name;
			foreach (var fragment in garmentModel.Fragments)
			{
				Fragments.Add(new GarmentFragment(fragment, renderer));
			}

			SetGarmentBones(skeleton);
		}

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		/// <param name="skeleton">the main character skeleton</param>
		private void SetGarmentBones(Skeleton skeleton)
		{
			//set garment name in all bones
			Debug.Assert(Name.Length > 0);
			for (var i = 0; i < Fragments.Count; i++)
			{
				Fragments[i].GarmentName = Name;
			}

			//set the parent bone of all those root node garment bones
			for (var i = 0; i < Fragments.Count; i++)
			{
				Fragments[i].SetGarmentBones(skeleton);
			}

			//check whether or not there is any physics data in the garment
			HasPhysics = false;
			for (var i = 0; i < Fragments.Count; i++)
			{
				if (Fragments[i].AnimationContainer.Skeleton.RootBone.HasPhysicsData())
				{
					HasPhysics = true;
					break;
				}
			}
		}

		#endregion //Initialization

		#region Methods

		/// <summary>
		/// Add this garment to the skeleton structure
		/// </summary>
		public void AddToSkeleton()
		{
			//add all the garment bones to the bones they attach to
			for (var i = 0; i < Fragments.Count; i++)
			{
				Fragments[i].AddToSkeleton();
			}
		}

		/// <summary>
		/// Remove this garment from the skeleton structure
		/// </summary>
		public void RemoveFromSkeleton()
		{
			//remove all the garment bones from the bones they attach to
			for (var i = 0; i < Fragments.Count; i++)
			{
				Fragments[i].RemoveFromSkeleton();
			}
		}

		#region Tools

		/// <summary>
		/// Get a list of all the weapon
		/// </summary>
		/// <param name="listWeapons"></param>
		public void GetAllWeaponBones(List<string> listWeapons)
		{
			foreach (GarmentFragment curFragment in Fragments)
			{
				curFragment.GetAllWeaponBones(listWeapons);
			}
		}

		#endregion //Tools

		#endregion //Methods
	}
}