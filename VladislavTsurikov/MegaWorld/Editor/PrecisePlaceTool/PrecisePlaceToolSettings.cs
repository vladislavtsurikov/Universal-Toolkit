#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    [Serializable]
    [Name("Precise Place Tool Settings")]
    public class PrecisePlaceToolSettings : Component
    {
        public MouseActionStack MouseActionStack = new MouseActionStack();

        public bool RememberPastTransform = true;

        public float Spacing = 5;
        public bool RandomSelectPrototype = true;

        public bool UseTransformComponents = true;
        public bool OverlapCheck;
        public bool VisualizeOverlapCheckSettings;

        public bool EnableSnapMove;
        public Vector3 SnapMove = new Vector3(2.5f, 2.5f, 2.5f);

        #region Move Action
        public bool Align;
        public float WeightToNormal = 1;
        public bool AlongStroke;
        #endregion

        protected override void SetupComponent(object[] setupData = null)
        {
            MouseActionStack.Setup();
        }
    }
}
#endif