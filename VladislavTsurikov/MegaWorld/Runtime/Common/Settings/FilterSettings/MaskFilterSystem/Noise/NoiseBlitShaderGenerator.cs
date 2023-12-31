﻿using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Shaders;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise
{
    internal class NoiseBlitShaderGenerator : NoiseShaderGenerator<NoiseBlitShaderGenerator>
    {
        private static NoiseBlitShaderGenerator _sInstance;

        public static NoiseBlitShaderGenerator Instance
        {
            get
            {
                if (_sInstance == null)
                {
                    _sInstance = new NoiseBlitShaderGenerator();
                }

                return _sInstance;
            }
        }
        
        private static readonly ShaderGeneratorDescriptor _mDesc = new ShaderGeneratorDescriptor
        {
            Name = "NoiseBlit",
            ShaderCategory = "Hidden/TerrainTools/Noise/NoiseBlit",
            OutputDir = MaskFilterShadersPath.Path + "/Generated/",
            TemplatePath = MaskFilterShadersPath.Path + "/NoiseLib/Templates/Blit.noisehlsltemplate"
        };

        public override ShaderGeneratorDescriptor GetDescription() => _mDesc;   
    }
}