using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class LoaderRegistrarUtility
    {
        internal static void RegisterLoaderInitializers(ResourceLoaderManager manager)
        {
            var resourceLoaderRegistrar = ReflectionFactory.CreateAllInstances<ResourceLoaderRegistrar>();

            foreach (var registrar in resourceLoaderRegistrar)
            {
                registrar.RegisterLoaders(manager);
            }
        }
    }
}