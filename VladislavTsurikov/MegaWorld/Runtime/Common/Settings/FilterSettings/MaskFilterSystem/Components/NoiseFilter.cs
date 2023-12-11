﻿using System;
using UnityEngine;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Noise.API;
using VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Utility;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.FilterSettings.MaskFilterSystem.Components
{
    [Serializable]
    [MenuItem("Noise")]
    public class NoiseFilter : MaskFilter
    {
        public BlendMode BlendMode = BlendMode.Multiply;

        public NoiseSettings NoiseSettings = new NoiseSettings();

        public override void Eval(MaskFilterContext fc, int index)
        {
            CreateNoiseSettingsIfNecessary();

            Vector3 brushPosWs = fc.BrushPos;
            float brushSize = fc.BoxArea.BoxSize;
            float brushRotation = fc.BoxArea.Rotation;

            // TODO(wyatt): remove magic number and tie it into NoiseSettingsGUI preview size somehow
            float previewSize = 1 / 512f;

            // get proper noise material from current noise settings
            Material mat = NoiseUtils.GetDefaultBlitMaterial(NoiseSettings);

            // setup the noise material with values in noise settings
            NoiseSettings.SetupMaterial( mat );

            // convert brushRotation to radians
            brushRotation *= Mathf.PI / 180;

            // change pos and scale so they match the noiseSettings preview
            bool isWorldSpace = true;
            //brushSize = isWorldSpace ? brushSize * previewSize : 1;
            brushSize = brushSize * previewSize;
            brushPosWs = isWorldSpace ? brushPosWs * previewSize : Vector3.zero;

            // // override noise transform
            Quaternion rotQ             = Quaternion.AngleAxis( -brushRotation, Vector3.up );
            Matrix4x4 translation       = Matrix4x4.Translate( brushPosWs );
            Matrix4x4 rotation          = Matrix4x4.Rotate( rotQ );
            Matrix4x4 scale             = Matrix4x4.Scale( Vector3.one * brushSize );
            Matrix4x4 noiseToWorld      = translation * scale;

            mat.SetMatrix(NoiseSettings.ShaderStrings.Transform, NoiseSettings.trs * noiseToWorld);

            int pass = NoiseUtils.KNumBlitPasses * NoiseLib.GetNoiseIndex( NoiseSettings.DomainSettings.NoiseTypeName );

            RenderTextureDescriptor desc = new RenderTextureDescriptor(fc.DestinationRenderTexture.width, fc.DestinationRenderTexture.height, RenderTextureFormat.RFloat);
            RenderTexture rt = RenderTexture.GetTemporary(desc);

            Graphics.Blit(fc.SourceRenderTexture, rt, mat, pass);

            Material blendMat = MaskFilterUtility.blendModesMaterial;
            
            if(index == 0)
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode.Multiply);
            }
            else
            {
                blendMat.SetInt("_BlendMode", (int)BlendMode);
            }

            blendMat.SetTexture("_MainTex", fc.SourceRenderTexture);
            blendMat.SetTexture("_BlendTex", rt);

            Graphics.Blit(fc.SourceRenderTexture, fc.DestinationRenderTexture, blendMat, 0);

            RenderTexture.ReleaseTemporary(rt);
        }

        private void CreateNoiseSettingsIfNecessary()
        {
            if(NoiseSettings == null)
            {
                NoiseSettings = new NoiseSettings();
            }
        }
    }
}