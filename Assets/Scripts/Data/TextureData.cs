using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu()]
public class TextureData : UpdatableData
{
    //public CustomGradient customGradients;

    public Layer[] biomes;

    float savedMinHeight;
    float savedMaxHeight;

    public void ApplyToMaterial(Material material)
    {
        material.SetInt("layerCount", biomes.Length);
        material.SetColorArray("baseColors", biomes.Select(x => x.tint).ToArray());
        material.SetFloatArray("baseStartHeights", biomes.Select(x => x.startHeight).ToArray());
        material.SetFloatArray("baseBlends", biomes.Select(x => x.blendStrength).ToArray());
        material.SetFloatArray("baseColorStrength", biomes.Select(x => x.tintStrength).ToArray());
        material.SetFloatArray("baseTextureScales", biomes.Select(x => x.textureScale).ToArray());

        UpdateMeshHeights(material, savedMinHeight, savedMaxHeight);
    }

    public void UpdateMeshHeights(Material material, float minHeight, float maxHeight)
    {
        savedMaxHeight = maxHeight;
        savedMinHeight = minHeight;

        material.SetFloat("minHeight", minHeight);
        material.SetFloat("maxHeight", maxHeight);
    }

    [System.Serializable]
    public class Layer
    {
        public Color tint;
        [Range(0,1)]
        public float tintStrength;
        [Range(0, 1)]
        public float startHeight;
        [Range(0, 1)]
        public float blendStrength;
        public float textureScale;

    }
}
