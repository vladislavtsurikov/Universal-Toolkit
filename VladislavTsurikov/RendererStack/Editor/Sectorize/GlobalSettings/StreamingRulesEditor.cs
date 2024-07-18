#if UNITY_EDITOR
using VladislavTsurikov.ComponentStack.Editor.Core;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack;
using VladislavTsurikov.IMGUIUtility.Editor.ElementStack.ReorderableList;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules;
using VladislavTsurikov.RendererStack.Runtime.Sectorize.GlobalSettings.StreamingRules.StreamingRulesSystem;

namespace VladislavTsurikov.RendererStack.Editor.Sectorize.GlobalSettings
{
    [ElementEditor(typeof(StreamingRules))]
    public class StreamingRulesEditor : IMGUIElementEditor
    {
        private StreamingRules _streamingRules;

        private ReorderableListStackEditor<StreamingRule, ReorderableListComponentEditor> _streamingRuleComponentStackEditor;

        public override void OnEnable() 
        {
            _streamingRules = (StreamingRules)Target;

            _streamingRuleComponentStackEditor = new ReorderableListStackEditor<StreamingRule, ReorderableListComponentEditor>(_streamingRules.StreamingRuleComponentStack);
        }

        public override void OnGUI()
        {
            _streamingRuleComponentStackEditor.OnGUI();
        }
    }
}
#endif