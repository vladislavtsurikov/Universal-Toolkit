#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList.Attributes;
using VladislavTsurikov.SceneManagerTool.Editor.SceneTypeSystem;
using VladislavTsurikov.SceneUtility.Runtime;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.SceneManagerIntegration
{
    [DontDrawForAddButton]
    [ElementEditor(typeof(Runtime.Sectorize.SceneManagerIntegration.Sectorize))]
    public class SectorizeEditor : SceneTypeEditor
    {
        private Runtime.Sectorize.SceneManagerIntegration.Sectorize _sectorize;
        
        private ReorderableList _reorderableList;
        
        public override void OnEnable()
        {
            base.OnEnable();

            _sectorize = (Runtime.Sectorize.SceneManagerIntegration.Sectorize)Target;
            
            _reorderableList = new ReorderableList(_sectorize.SubScenes, typeof(SceneReference), false, true, false, false)
            {
                drawHeaderCallback = DrawHeader,
                drawElementCallback = DrawElement
            };
        }
        
        public override void OnGUI(Rect rect, int index)
        {
            _reorderableList.DoList(rect);
            
            rect.y += CustomEditorGUI.SingleLineHeight * 2;
            rect.y += _sectorize.SubScenes.Count * CustomEditorGUI.SingleLineHeight;

            base.OnGUI(rect, index);
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;

            height += CustomEditorGUI.SingleLineHeight * 2;
            height += _sectorize.SubScenes.Count * CustomEditorGUI.SingleLineHeight;

            height += base.GetElementHeight(index);
            height += CustomEditorGUI.SingleLineHeight;

            return height;
        }

        private void DrawElement(Rect totalRect, int index, bool isActive, bool isFocused)
        {
            using( new EditorGUI.DisabledScope(true))
            {
                Rect rectField = totalRect;
                rectField.width -= 14;
            
                _sectorize.SubScenes[index].SceneAsset = (SceneAsset)CustomEditorGUI.ObjectField(
                    new Rect(rectField.x, rectField.y, rectField.width, EditorGUIUtility.singleLineHeight),
                    null, _sectorize.SubScenes[index].SceneAsset, typeof(SceneAsset));
            }
        }

        private void DrawHeader(Rect rect)
        {
            CustomEditorGUI.Label(rect, "Scenes", CustomEditorGUI.GetStyle(StyleName.LabelFoldout));
        }
    }
}
#endif