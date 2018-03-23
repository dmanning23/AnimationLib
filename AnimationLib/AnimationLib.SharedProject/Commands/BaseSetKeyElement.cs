using System;
using UndoRedoBuddy;

namespace AnimationLib.Commands
{
	public abstract class BaseSetKeyElement : ICommand
    {
		#region Fields

		//the animation the element is in
		protected Animation Animation { get; set; }

		//the old element 
		protected KeyElement PrevKeyElement { get; set; }

		//the current element to replace it
		protected KeyElement NextKeyElement { get; set; }

		#endregion // Fields

		#region Methods

		public BaseSetKeyElement(Animation animation)
		{
			if (null == animation)
			{
				throw new ArgumentNullException("animation");
			}

			Animation = animation;
		}

		protected void PopulateKeyElements(Bone bone, int time)
		{
			//create the new keyframe
			PrevKeyElement = new KeyElement(bone);
			NextKeyElement = new KeyElement(bone);

			//get the old keyelement
			Animation.GetKeyJoint(bone.Name).GetKeyElement(time, PrevKeyElement);

			//copy from the current keyframe
			NextKeyElement.Copy(PrevKeyElement);
		}

		/// <summary>
		/// run this action!
		/// </summary>
		/// <returns>bool: whether or not the action executed successfully</returns>
		public bool Execute()
		{
			return ReplaceKeyElement(PrevKeyElement, NextKeyElement);
		}

		/// <summary>
		/// undo an action!
		/// </summary>
		/// <returns>bool: whether or not the action was undone successfully</returns>
		public bool Undo()
		{
			//replace the "new" key element with the "old" one
			return ReplaceKeyElement(NextKeyElement, PrevKeyElement);
		}

		private bool ReplaceKeyElement(KeyElement replacedKey, KeyElement replacementKey)
		{
			//find the keyjoint we want 
			KeyJoint myKeyJoint = Animation.KeyBone.GetKeyJoint(replacementKey.BoneName);
			if (null == myKeyJoint)
			{
				return false;
			}

			//set the keyelement
			if (replacementKey.KeyFrame)
			{
				myKeyJoint.AddKeyElement(replacementKey);
			}
			else
			{
				//just remove the 'replaced' keyframe
				myKeyJoint.RemoveKeyElement(replacedKey.Time);
			}

			return true;
		}

		#endregion //Methods
	}
}
