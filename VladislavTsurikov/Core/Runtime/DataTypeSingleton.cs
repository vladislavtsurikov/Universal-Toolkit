namespace VladislavTsurikov.Core.Runtime
{
    public abstract class DataTypeSingleton<T> where T : DataTypeSingleton<T>, new()
    {
        #region Private Static Variables
        /// <summary>
        /// The singleton instance.
        /// </summary>
        /// <remarks>
        /// Note: This assumes that the derived class must always have a public parameterless constructor.
        ///       This implies that the client code can create instances of the derived classes. We could
        ///       solve this using reflection, but in order to keep things simple and clean, we will avoid
        ///       doing that.
        /// </remarks>
        private static T _instance = new T();
        #endregion
        #region Public Static Properties
        public static T Instance { get { return _instance; } }
        #endregion
    }
}

