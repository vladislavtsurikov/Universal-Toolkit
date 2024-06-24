namespace VladislavTsurikov.SceneDataSystem.Runtime.StreamingUtility
{
    public static class StreamingUtilityEvents
    {
        public delegate void DeleteAllAdditiveScenesBefore ();
        public static DeleteAllAdditiveScenesBefore BeforeDeleteAllAdditiveScenesEvent;
        
        public delegate void CreateSceneAfter ();
        public static CreateSceneAfter CreateSceneAfterEvent;
    }
}