using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public class AddAnimation : ICommand
	{
		#region Properties

		AnimationContainer AnimationContainer { get; set; }

		string AnimationName { get; set; }

		float Length { get; set; }

		#endregion //Properties

		#region Methods

		public AddAnimation(AnimationContainer animationContainer, string animationName, float length)
		{
			AnimationContainer = animationContainer;
			AnimationName = animationName;
			Length = length;
		}

		public bool Execute()
		{
			return ExecuteAddAnimation(AnimationContainer, AnimationName, Length);
		}

		public static bool ExecuteAddAnimation(AnimationContainer animationContainer, string animationName, float length)
		{
			//first make sure the animation doesn't exist
			var animation = animationContainer.FindAnimation(animationName);
			if (null != animation)
			{
				throw new Exception($"Couldn't add animation with duplicate name {animationName}");
			}

			//create the animation model
			var animationModel = new AnimationModel()
			{
				Name = animationName,
				Length = length
			};

			//create the animation
			animation = new Animation(animationContainer.Skeleton, animationModel);

			//add the animation
			animationContainer.Animations.Add(animation);

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
