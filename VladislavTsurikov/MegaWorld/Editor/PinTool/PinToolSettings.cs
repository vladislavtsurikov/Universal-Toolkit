using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.Utility.Runtime.Extensions;

namespace VladislavTsurikov.MegaWorld.Editor.PinTool
{
    public enum TransformMode
    {
        Free,
        Snap,
        Fixed,
    }

    [Serializable]
    [MenuItem("Pin Tool Settings")]
    public class PinToolSettings : ComponentStack.Runtime.Component
    {
        #region Position
        public float Offset;
        #endregion

        #region Rotation
        public TransformMode RotationTransformMode = TransformMode.Free;
        public FromDirection FromDirection;
        public float WeightToNormal = 1;
        public float SnapRotationValue = 30;
        public Vector3 FixedRotationValue;
        #endregion

        #region Scale
        public TransformMode ScaleTransformMode = TransformMode.Free;
        public Vector3 FixedScaleValue;
        public float SnapScaleValue = 0.3f;
        #endregion
    }
}
    