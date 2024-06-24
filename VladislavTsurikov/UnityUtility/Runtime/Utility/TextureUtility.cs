using UnityEditor;
using UnityEngine;

namespace VladislavTsurikov.UnityUtility.Runtime
{
    public static class TextureUtility
    {        
        public static float GetFromWorldPosition(Bounds bounds, Vector3 worldPosition, Texture2D texture)
        {
            if(texture == null)
            {
                return 0;
            }

            float inverseY = Mathf.InverseLerp(bounds.center.z - bounds.extents.z, bounds.center.z + bounds.extents.z, worldPosition.z);
            float inverseX = Mathf.InverseLerp(bounds.center.x - bounds.extents.x, bounds.center.x + bounds.extents.x, worldPosition.x);

            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, texture.width, inverseY));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, texture.height, inverseX));

            return texture.GetPixel(pixelX, pixelY).grayscale;
        }

        public static float Get(Vector2 normal, Texture2D texture)
        {
            if(texture == null)
            {
                return 0;
            }

            int pixelY = Mathf.RoundToInt(Mathf.Lerp(0, texture.width, normal.y));
            int pixelX = Mathf.RoundToInt(Mathf.Lerp(0, texture.height, normal.x));

            return texture.GetPixel(pixelX, pixelY).grayscale;
        }
        
        public static bool IsSameTexture(Texture2D tex1, Texture2D tex2, bool checkID = false)
        {
            if (tex1 == null || tex2 == null)
            {
                return false;
            }

            if (checkID)
            {
                if (tex1.GetInstanceID() != tex2.GetInstanceID())
                {
                    return false;
                }
                return true;
            }

            if (tex1.name != tex2.name)
            {
                return false;
            }

            if (tex1.width != tex2.width)
            {
                return false;
            }

            if (tex1.height != tex2.height)
            {
                return false;
            }

            return true;
        }
        
        public static Vector2 AnimationCurveToTexture(AnimationCurve curve, ref Texture2D tex) 
        {
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;

            float val = curve.Evaluate(0.0f);
            Vector2 range = new Vector2(val, val);

            Color[] pixels = new Color[tex.width * tex.height];
            pixels[0].r = val;
            for (int i = 1; i < tex.width; i++) {
                float pct = (float)i / (float)tex.width;
                pixels[i].r = curve.Evaluate(pct);
                range[0] = Mathf.Min(range[0], pixels[i].r);
                range[1] = Mathf.Max(range[1], pixels[i].r);
            }
            tex.SetPixels(pixels);
            tex.Apply();

            return range;
        }

        public static Texture2D ToTexture2D(RenderTexture rTex)
        {
            Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.ARGB32, false);
            
            RenderTexture.active = rTex;

            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
            tex.Apply();

            RenderTexture.active = null;
            return tex;
        }
        
        public static float GetAlpha(Vector2 pos, Vector2 size, Texture2D mask)
        {
            if (mask == null)
            {
                return 1.0f;
            }
            
            float x = (pos.x - 1) / size.x;
            float y = (pos.y - 1) / size.y;

            return mask.GetPixelBilinear(x, y).grayscale;
        }
        
        public static Texture2D GetPrefabPreviewTexture(GameObject prefab)
        {
#if UNITY_EDITOR
            Texture2D previewTexture;

            if((previewTexture = AssetPreview.GetAssetPreview(prefab)) != null)
            {
                return previewTexture;
            }
                
            return AssetPreview.GetMiniTypeThumbnail(typeof(GameObject));
#else
            return Texture2D.whiteTexture;
#endif
        }
    } 
}