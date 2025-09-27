using System.Collections.Generic;
using VladislavTsurikov.ReflectionUtility.Runtime;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core
{
    public static class LoaderRegistrarUtility
    {
        internal static void RegisterLoaderInitializers(ResourceLoaderManager manager)
        {
            foreach (ResourceLoaderRegistrar registrar in resourceLoaderRegistrar)
            {
                registrar.RegisterLoaders(manager);
            }
        }
    }
}
