#if UNITY_EDITOR
using UnityEngine;
using VladislavTsurikov.Utility.Runtime;
using VladislavTsurikov.UnityUtility.Editor;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Stamper
{
    public class AreaVisualisation
    {
        public static void DrawBox(Area area, float alpha)
        {
            Transform newTransform = area.StamperTool.transform;
            newTransform.rotation = Quaternion.identity;
            var localScale = newTransform.transform.localScale;
            localScale = new Vector3 (Mathf.Max(1f, localScale.z), Mathf.Max(1f, localScale.y), Mathf.Max(1f, localScale.z));
            newTransform.transform.localScale = localScale;

            if(area.HandleSettingsMode == HandleSettingsMode.Custom)
            {
                Color color = area.ColorCube;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, area.PixelWidth, area.Dotted);
            }
            else
            {
                float thickness = 4.0f;
                Color color = Color.yellow;
                color.a *= alpha;
                DrawHandles.DrawCube(newTransform.localToWorldMatrix, color, thickness);
            }
        }
    }
}
#endif