using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.SceneManagerTool.Runtime.SceneTypeSystem;
using VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [MenuItem("Scene Behavior")]
    [Scene]
    public class SceneBehavior : SettingsComponentElement
    {
        public SceneCloseBehavior SceneCloseBehavior = SceneCloseBehavior.Close;
        public SceneOpenBehavior SceneOpenBehavior = SceneOpenBehavior.Open;
    }
}