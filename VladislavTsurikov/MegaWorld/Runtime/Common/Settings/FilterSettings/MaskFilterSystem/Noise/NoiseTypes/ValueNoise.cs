using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.NoiseTypes
{
    /// <summary>
    /// A NoiseType implementation for Value noise
    /// </summary>
    [System.Serializable]
    public class ValueNoise : NoiseType<ValueNoise>
    {
        private static ValueNoise _sInstance;

        public static ValueNoise Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new ValueNoise();
                }

                return _sInstance;
            }
        }
        
        private static NoiseTypeDescriptor _desc = new NoiseTypeDescriptor
        {
            Name = "Value",
            OutputDir = MaskFilterShadersPath.Path + "/NoiseLib/",
            SourcePath = MaskFilterShadersPath.Path + "/NoiseLib/Implementation/ValueImpl.hlsl",
            SupportedDimensions = NoiseDimensionFlags._1D | NoiseDimensionFlags._2D | NoiseDimensionFlags._3D,
            InputStructDefinition = null
        };

        public override NoiseTypeDescriptor GetDescription() => _desc;
    }
}