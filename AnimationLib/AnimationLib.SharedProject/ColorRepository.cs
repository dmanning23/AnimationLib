using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace AnimationLib
{
	public class ColorRepository
	{
		#region Properties

		public Dictionary<string, Color> Colors { get; private set; }

		#endregion //Properties

		#region Methods

		public ColorRepository()
		{
			Colors = new Dictionary<string, Color>();
		}

		public void AddColor(string tag, Color color)
		{
			Colors[tag] = color;
		}

		public bool HasColor(string tag)
		{
			return (!string.IsNullOrEmpty(tag) && Colors.ContainsKey(tag));
		}

		public Color GetColor(string tag)
		{
			if (string.IsNullOrEmpty(tag) || !Colors.ContainsKey(tag))
			{
				return Color.White;
			}
			else
			{
				return Colors[tag];
			}
		}

		#endregion //Methods
	}
}
