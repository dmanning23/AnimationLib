using FilenameBuddy;
using RenderBuddy;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

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

		/// <summary>
		/// The file this garment was laoded from
		/// </summary>
		public Filename GarmentFile { get; private set; }

		private float Scale { get; set; }

		#endregion //Properties

		#region Initialization

		/// <summary>
		/// constructor!
		/// </summary>
		public Garment(float scale = 1f)
		{
			Scale = scale;
			Fragments = new List<GarmentFragment>();
			HasPhysics = false;
		}

		public Garment(Filename filename, Skeleton skeleton, IRenderer renderer, float scale = 1f)
			: this(scale)
		{
			InitializeGarment(null, filename, skeleton, renderer);
		}

		public Garment(ContentManager content, Filename filename, Skeleton skeleton, IRenderer renderer, float scale = 1f)
			: this(scale)
		{
			InitializeGarment(content, filename, skeleton, renderer);
		}

		private void InitializeGarment(ContentManager content, Filename filename, Skeleton skeleton, IRenderer renderer)
		{
			GarmentFile = filename;
			var garmentModel = new GarmentModel(filename, Scale);
			garmentModel.ReadXmlFile(content);

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

		public void Write()
		{
			WriteXmlFile(GarmentFile);
		}

		public void WriteXmlFile(Filename filename)
		{
			GarmentFile = filename;
			var garment = new GarmentModel(filename, this);
			garment.WriteXml();
		}

		#region Color Methods

		/// <summary>
		/// Set the primary color of this garment
		/// </summary>
		/// <param name="color"></param>
		public void SetPrimaryColor(Color color)
		{
			//Set the color in all the garment fragments
			foreach (var fragment in Fragments)
			{
				fragment.SetPrimaryColor(color);
			}
		}

		/// <summary>
		/// Set the secondary color of this garment
		/// </summary>
		/// <param name="color"></param>
		public void SetSecondaryColor(Color color)
		{
			//Set the color in all the garment fragments
			foreach (var fragment in Fragments)
			{
				fragment.SetSecondaryColor(color);
			}
		}

		#endregion //Color Methods

		#endregion //Methods
	}
}