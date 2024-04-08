namespace VladislavTsurikov.Core.Runtime
{
    public abstract class DataTypeSingleton<T> where T : DataTypeSingleton<T>, new()
    {
        private static T _instance = new T();
        
        public static T Instance => _instance;
    }
}

