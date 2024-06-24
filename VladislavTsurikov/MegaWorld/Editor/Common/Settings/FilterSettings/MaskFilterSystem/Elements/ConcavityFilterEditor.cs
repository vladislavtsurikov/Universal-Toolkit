#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem;
using VladislavTsurikov.UnityUtility.Runtime;

namespace VladislavTsurikov.MegaWorld.Editor.Common.Settings.FilterSettings.MaskFilterSystem 
{
    [ElementEditor(typeof(ConcavityFilter))]
    public class ConcavityFilterEditor : MaskFilterEditor
    {
        private ConcavityFilter _concavityFilter;

        public override void OnEnable()
        {
            _concavityFilter = (ConcavityFilter)Target;
        }

        public override void OnGUI(Rect rect, int index)
        {
            //Precaculate dimensions
            float epsilonLabelWidth = GUI.skin.label.CalcSize(_epsilonLabel).x;
            float modeLabelWidth = GUI.skin.label.CalcSize(_modeLabel).x;
            float strengthLabelWidth = GUI.skin.label.CalcSize(_strengthLabel).x;
            float curveLabelWidth = GUI.skin.label.CalcSize(_curveLabel).x;
            float labelWidth = Mathf.Max(Mathf.Max(Mathf.Max(modeLabelWidth, epsilonLabelWidth), strengthLabelWidth), curveLabelWidth) + 4.0f;
            labelWidth += 50;

            //Concavity Mode Drop Down
            Rect modeRect = new Rect(rect.x + labelWidth, rect.y, rect.width - labelWidth, EditorGUIUtility.singleLineHeight);
            ConcavityFilter.ConcavityMode mode = _concavityFilter.ConcavityScalar > 0.0f ? ConcavityFilter.ConcavityMode.Recessed : ConcavityFilter.ConcavityMode.Exposed;
            switch(EditorGUI.EnumPopup(modeRect, mode)) {
                case ConcavityFilter.ConcavityMode.Recessed:
                    _concavityFilter.ConcavityScalar = 1.0f;
                    break;
                case ConcavityFilter.ConcavityMode.Exposed:
                    _concavityFilter.ConcavityScalar = -1.0f;
                    break;
            }

            //Strength Slider
            Rect strengthLabelRect = new Rect(rect.x, modeRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(strengthLabelRect, _strengthLabel);
            Rect strengthSliderRect = new Rect(strengthLabelRect.xMax, strengthLabelRect.y, rect.width - labelWidth, strengthLabelRect.height);
            _concavityFilter.ConcavityStrength = EditorGUI.Slider(strengthSliderRect, _concavityFilter.ConcavityStrength, 0.0f, 1.0f);

            //Epsilon (kernel size) Slider
            Rect epsilonLabelRect = new Rect(rect.x, strengthSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(epsilonLabelRect, _epsilonLabel);
            Rect epsilonSliderRect = new Rect(epsilonLabelRect.xMax, epsilonLabelRect.y, rect.width - labelWidth, epsilonLabelRect.height);
            _concavityFilter.ConcavityEpsilon = EditorGUI.Slider(epsilonSliderRect, _concavityFilter.ConcavityEpsilon, 1.0f, 20.0f);

            //Value Remap Curve
            Rect curveLabelRect = new Rect(rect.x, epsilonSliderRect.yMax, labelWidth, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(curveLabelRect, _curveLabel);
            Rect curveRect = new Rect(curveLabelRect.xMax, curveLabelRect.y, rect.width - labelWidth, curveLabelRect.height);

            EditorGUI.BeginChangeCheck();
            _concavityFilter.ConcavityRemapCurve = EditorGUI.CurveField(curveRect, _concavityFilter.ConcavityRemapCurve);
            if(EditorGUI.EndChangeCheck()) {
                Vector2 range = TextureUtility.AnimationCurveToTexture(_concavityFilter.ConcavityRemapCurve, ref _concavityFilter.ConcavityRemapTex);
            }
        }

        public override float GetElementHeight(int index) 
        {
            return EditorGUIUtility.singleLineHeight * 5;
        }

        private static readonly GUIContent _strengthLabel = EditorGUIUtility.TrTextContent("Strength", "Controls the strength of the masking effect.");
        private static readonly GUIContent _epsilonLabel = EditorGUIUtility.TrTextContent("Feature Size", "Specifies the scale of Terrain features that affect the mask. This determines the size of features to which to apply the effect.");
        private static readonly GUIContent _modeLabel = EditorGUIUtility.TrTextContent("Mode");
        private static readonly GUIContent _curveLabel = EditorGUIUtility.TrTextContent("Remap Curve", "Remaps the concavity input before computing the final mask.");
    }
}
#endif