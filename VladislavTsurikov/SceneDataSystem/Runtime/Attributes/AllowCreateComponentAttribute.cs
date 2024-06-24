using System;

namespace VladislavTsurikov.SceneDataSystem.Runtime
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public abstract class AllowCreateComponentAttribute : Attribute
    {
        public abstract bool Allow(SceneDataManager sceneDataManager);
    }
}