using FilenameBuddy;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using RenderBuddy;
using System;
using System.Collections.Generic;

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

		public ColorRepository Colors { get; private set; }

		public bool IsAdded { get; private set; }

		private Skeleton _parentSkeleton;

		public event EventHandler OnGarmentRemoved;

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
			Colors = new ColorRepository();
			IsAdded = false;
		}

		public Garment(Filename filename, Skeleton skeleton, IRenderer renderer)
			: this(skeleton.Animations.Scale)
		{
			InitializeGarment(null, filename, skeleton, renderer);
		}

		public Garment(ContentManager xmlContent, Filename filename, Skeleton skeleton, IRenderer renderer)
			: this(skeleton.Animations.Scale)
		{
			InitializeGarment(xmlContent, filename, skeleton, renderer);
		}

		public Garment(GarmentModel garmentModel, Skeleton skeleton, IRenderer renderer)
			: this(skeleton.Animations.Scale)
		{
			InitializeGarment(garmentModel, skeleton, renderer);
		}

		private void InitializeGarment(ContentManager xmlContent, Filename filename, Skeleton skeleton, IRenderer renderer)
		{
			
			var garmentModel = new GarmentModel(filename, Scale);
			garmentModel.ReadXmlFile(xmlContent);

			InitializeGarment(garmentModel, skeleton, renderer);
		}

		private void InitializeGarment(GarmentModel garmentModel, Skeleton skeleton, IRenderer renderer)
		{
			GarmentFile = garmentModel.Filename;
			Name = garmentModel.Name;
			foreach (var fragmentModel in garmentModel.Fragments)
			{
				var fragment = new GarmentFragment(fragmentModel, renderer, this);
				Fragments.Add(fragment);
			}

			foreach (var color in garmentModel.Colors)
			{
				Colors.AddColor(color.Tag, color.Color);
			}

			_parentSkeleton = skeleton;
			SetGarmentBones(skeleton);
		}

		/// <summary>
		/// set all the data for the garment bones in this dude after they have been read in
		/// </summary>
		/// <param name="skeleton">the main character skeleton</param>
		private void SetGarmentBones(Skeleton skeleton)
		{
			//set garment name in all bones
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

			UpdateColors();
		}

		private void UpdateColors()
		{
			if (Colors.Colors.ContainsKey("primary"))
			{
				SetPrimaryColor(Colors.GetColor("primary"));
			}

			if (Colors.Colors.ContainsKey("secondary"))
			{
				SetSecondaryColor(Colors.GetColor("secondary"));
			}

			foreach (var fragment in Fragments)
			{
				fragment.SetColors(Colors);
			}
		}

		public void SetColor(string tag, Color color)
		{
			Colors.AddColor(tag, color);
			UpdateColors();
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
				//add to the skeleton
				Fragments[i].AddToSkeleton();

				//update the colors from the parent skeleton
				Fragments[i].SetColors(_parentSkeleton.Animations.Colors);
			}
			IsAdded = true;
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
			IsAdded = false;

			if (null != OnGarmentRemoved)
			{
				OnGarmentRemoved(this, new EventArgs());
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

		public override string ToString()
		{
			return this.Name;
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