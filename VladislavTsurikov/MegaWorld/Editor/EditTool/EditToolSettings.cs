#if UNITY_EDITOR
using System;
using VladislavTsurikov.ComponentStack.Runtime;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.ComponentStack.Runtime.Core;
using VladislavTsurikov.MegaWorld.Editor.EditTool.ActionSystem;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Editor.EditTool
{
    [Serializable]
    [Name("Edit Tool Settings")]
    public class EditToolSettings : Component 
    {
        [OdinSerialize]
		private ActionStack _actionStack = new ActionStack();

		public ActionStack ActionStack 
		{
			get
			{
				_actionStack.RemoveInvalidElements();
				return _actionStack;
			}
		}

		public float SphereSize = 40;
        public float MaxDistance = 200;

        protected override void SetupComponent(object[] setupData = null)
        {
	        _actionStack.Setup();
        }
    }
}
#endif