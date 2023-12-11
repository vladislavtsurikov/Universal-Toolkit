namespace VladislavTsurikov.SceneDataSystem.Runtime.Attributes
{
    public class SceneTypeAttribute : AllowCreateComponentAttribute
    {
        private readonly SceneType _sceneType;
        
        public SceneTypeAttribute(SceneType sceneType)
        {
            _sceneType = sceneType;
        }
        
        public override bool Allow(SceneDataManager sceneDataManager)
        {
            return _sceneType == sceneDataManager.SceneType; 
        }
    }
}