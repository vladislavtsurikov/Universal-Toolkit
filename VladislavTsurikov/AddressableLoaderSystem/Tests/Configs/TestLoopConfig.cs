using UnityEngine;

namespace VladislavTsurikov.AddressableLoaderSystem.Tests
{
    [CreateAssetMenu(fileName = "TestLoopConfig", menuName = "Tests/TestLoopConfig")]
    public class TestLoopConfig : ScriptableObject
    {
        public TestLoopData data;
    }
    
    [System.Serializable]
    public class TestLoopData
    {
        public TestLoopConfig backReference;
    }
}