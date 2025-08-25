﻿using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "ConfigSceneA", menuName = "Test/ConfigSceneA")]
    public class ConfigSceneA : ScriptableObject
    {
        public string Message = "Scene A";
    }
}