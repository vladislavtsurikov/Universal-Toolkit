#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Editor.Attributes;
using VladislavTsurikov.IMGUIUtility.Editor;
using VladislavTsurikov.RendererStack.Editor.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.Core.PrototypeRendererSystem.PrototypeSettings;
using VladislavTsurikov.RendererStack.Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components;

namespace VladislavTsurikov.RendererStack.Editor.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting
{
    [ElementEditor(typeof(Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting.Scripting))]
    public class ScriptingEditor : PrototypeComponentEditor
    {
        private ScriptStackEditor _settingsStackEditor;
        private Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting.Scripting _scripting;
        
        public override void OnEnable()
        {
            _scripting = (Runtime.TerrainObjectRenderer.ScriptingSystem.PrototypeSettings.Components.Scripting.Scripting)Target;
            _settingsStackEditor = new ScriptStackEditor(_scripting.ScriptStack);
        }
        
        public override void OnGUI(Rect rect, int index)
        {
            Colliders colliders = (Colliders)Prototype.GetSettings(typeof(Colliders));

            if(!PrototypeComponent.IsValid(colliders))
            {
                _scripting.MaxDistance = CustomEditorGUI.FloatField(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight),  new GUIContent("Max Distance"), _scripting.MaxDistance);
                rect.y += CustomEditorGUI.SingleLineHeight;
            }
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            _settingsStackEditor.OnGUI(new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight));
            EditorGUI.EndDisabledGroup();
        }

        public override float GetElementHeight(int index)
        {
            float height = 0;
            
            Colliders colliders = (Colliders)Prototype.GetSettings(typeof(Colliders));

            if(!PrototypeComponent.IsValid(colliders))
            {
                height += CustomEditorGUI.SingleLineHeight;
            }
            
            height += _settingsStackEditor.GetElementStackHeight();

            return height;
        }
    }
}
#endif