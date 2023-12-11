using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem.Components
{
    [MenuItem("User Data")]
    public class UserData : SettingsComponentElement
    {
        public ScriptableObject ScriptableObject;
    }
}