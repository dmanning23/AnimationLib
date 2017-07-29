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
			//first make sure the animation doesn't exist
			var animation = AnimationContainer.FindAnimation(AnimationName);
			if (null != animation)
			{
				throw new Exception($"Couldn't add animation with duplicate name {AnimationName}");
			}

			//create the animation model
			var animationModel = new AnimationModel()
			{
				Name = AnimationName,
				Length = Length
			};

			//create the animation
			animation = new Animation(AnimationContainer.Skeleton, animationModel);

			//add the animation
			AnimationContainer.Animations.Add(animation);

			return true;
		}

		public bool Undo()
		{
			return true;
		}

		#endregion //Methods
	}
}
