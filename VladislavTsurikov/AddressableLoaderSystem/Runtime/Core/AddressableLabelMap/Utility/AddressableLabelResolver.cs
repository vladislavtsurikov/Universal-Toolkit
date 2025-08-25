using System;
using System.Collections.Generic;

namespace VladislavTsurikov.AddressableLoaderSystem.Runtime.Core.AddressableLabelMap
{
    public static class AddressableLabelResolver
    {
        public static string GetLabel(string address)
        {
            return AddressableLabelMapAsset.Instance.TryGetLabel(address, out var label) ? label : null;
        }

        public static Dictionary<string, string> GetLabelsByType(Type type)
        {
            return AddressableLabelMapAsset.Instance.GetLabelsByType(type);
        }
    }
}