using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [MenuItem("User Data")]
    public class UserData : SettingsComponent
    {
        public ScriptableObject ScriptableObject;
    }
}