using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorDesign : MonoBehaviour
{
    public Gradient colorOverHeight;
    public Gradient colorOverNormal;
    public AnimationCurve normalAffect;
    public Material targetMaterial;
    public void Start()
    {
        generate();
    }
    public void generate()
    {
        int w = 256;
        int h = 256;
        Texture2D texture = new Texture2D(w, h);
        Color[] img = new Color[w * h];
        for (int i = 0; i < w; i++)
        {
            for (int j = 0; j < h; j++)
            {
                img[i + j * h] =Color.Lerp( colorOverHeight.Evaluate((float)i / w), colorOverNormal.Evaluate(1-(float)j / h), normalAffect.Evaluate((float)i / w));
            }
        }
        texture.SetPixels(img);
        texture.Apply();
        texture.wrapMode = TextureWrapMode.Clamp;
        targetMaterial.SetTexture("_Gradient", texture);
    }
}
