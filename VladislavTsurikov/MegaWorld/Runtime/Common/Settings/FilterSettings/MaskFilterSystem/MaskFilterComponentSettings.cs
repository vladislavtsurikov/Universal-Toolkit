using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.AdvancedComponentStack;
using VladislavTsurikov.OdinSerializer.Core.Misc;
using Component = VladislavTsurikov.ComponentStack.Runtime.Core.Component;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem
{
    [MenuItem("Mask Filter Settings")]
    public class MaskFilterComponentSettings : Component
    {
	    [OdinSerialize]
	    public MaskFilterStack MaskFilterStack = new MaskFilterStack();
	    [NonSerialized]
	    public MaskFilterContext FilterContext;
        [NonSerialized]
		public Texture2D FilterMaskTexture2D;
    }
}