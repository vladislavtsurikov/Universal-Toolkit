#if UNITY_EDITOR
using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool.MouseActions;

namespace VladislavTsurikov.MegaWorld.Editor.PrecisePlaceTool
{
    [Serializable]
    [MenuItem("Precise Place Tool Settings")]
    public class PrecisePlaceToolSettings : ComponentStack.Runtime.Component
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

        protected override void SetupElement(object[] args = null)
        {
            MouseActionStack.Setup();
        }
    }
}
#endif