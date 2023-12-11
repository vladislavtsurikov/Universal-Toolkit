using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [MenuItem("Mask Filter Settings")]
    public class MaskFilterComponentSettings : ComponentStack.Runtime.Component
    {
	    [OdinSerialize]
	    public MaskFilterStack MaskFilterStack = new MaskFilterStack();
	    [NonSerialized]
	    public MaskFilterContext FilterContext;
        [NonSerialized]
		public Texture2D FilterMaskTexture2D;
    }
}