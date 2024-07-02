using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;

namespace VladislavTsurikov.SceneManagerTool.Runtime.SettingsSystem
{
    [Name("User Data")]
    public class UserData : SettingsComponent
    {
        public ScriptableObject ScriptableObject;
    }
}