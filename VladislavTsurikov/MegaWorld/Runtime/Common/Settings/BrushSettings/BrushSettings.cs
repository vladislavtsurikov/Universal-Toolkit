﻿using System;
using UnityEngine;
using VladislavTsurikov.ColliderSystem.Runtime.Scene;
using VladislavTsurikov.ComponentStack.Runtime.Attributes;
using VladislavTsurikov.MegaWorld.Runtime.Common.Area;
using VladislavTsurikov.OdinSerializer.Core.Misc;

namespace VladislavTsurikov.MegaWorld.Runtime.Common.Settings.BrushSettings
{
    public enum MaskType
    {
        Custom,
        Procedural
    }

    public enum SpacingEqualsType 
    { 
        BrushSize, 
        HalfBrushSize,
        Custom,
    }


    [Serializable]
    [MenuItem("Brush Settings")]
    public class BrushSettings : ComponentStack.Runtime.Component
    {
        [OdinSerialize]
        private float _brushSpacing = 30;

        [OdinSerialize]
        private float _brushRotation;

        [OdinSerialize]
        private float _brushSize = 100;

        public ProceduralMask ProceduralMask = new ProceduralMask();
        public CustomMasks CustomMasks = new CustomMasks();
        public MaskType MaskType = MaskType.Procedural;
        public SpacingEqualsType SpacingEqualsType = SpacingEqualsType.HalfBrushSize;
        public BrushJitterSettings BrushJitterSettings = new BrushJitterSettings();

        public float Spacing
        {
            set => _brushSpacing = Mathf.Max(0.01f, value);
            get => _brushSpacing;
        }

        public float BrushRotation
        {
            get => _brushRotation;
            set => _brushRotation = value;
        }

        public float BrushSize
        {
            get => _brushSize;
            set => _brushSize = Mathf.Max(0.01f, value);
        }

        public float BrushRadius => _brushSize / 2;

        public float GetCurrentSpacing()
        {
            switch (SpacingEqualsType)
            {
                case SpacingEqualsType.BrushSize:
                {
                    return BrushSize;
                }
                case SpacingEqualsType.HalfBrushSize:
                {
                    return BrushSize / 2;
                }
                default:
                {
                    return Mathf.Max(0.01f, Spacing);
                }
            }
        }

        public Texture2D GetCurrentRaw()
        {
            switch (MaskType)
            {
                case MaskType.Custom:
                {
                    Texture2D texture = CustomMasks.GetSelectedBrush();

                    return texture;
                }
                case MaskType.Procedural:
                {
                    Texture2D texture = ProceduralMask.Mask;

                    return texture;
                }
            }

            return Texture2D.whiteTexture;
        }

        public void ScrollBrushRadiusEvent()
        {
            if(Event.current.shift)
            {
                if (Event.current.type == EventType.ScrollWheel) 
                {
                    BrushSize += Event.current.delta.y;
                    Event.current.Use();
			    }
            }
        }

        public BoxArea GetAreaVariables(RayHit hit)
        {
            if (hit == null)
            {
                return null;
            }
            
            BoxArea area = new BoxArea(hit, BrushSize)
            {
                Mask = GetCurrentRaw(),
                Rotation = BrushRotation,
            };

            return area;
        }
    }
}