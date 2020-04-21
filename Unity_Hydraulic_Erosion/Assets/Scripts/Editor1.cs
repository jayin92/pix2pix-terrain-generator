using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ctrl))]
public class Editor1 : Editor {

    public override void OnInspectorGUI()
    {
        ctrl controller = (ctrl)target;
        DrawDefaultInspector();
        if (GUILayout.Button("load png"))
        {
            controller.ReadPng();
        }
        if (GUILayout.Button("generate Perlin noise"))
        {
            controller.GeneratePerlinNoise();
        }
        if (GUILayout.Button("flat"))
        {
            controller.Flat();
        }
        if (GUILayout.Button("terrain"))
        {
            controller.Terrain();
        }
        if(GUILayout.Button("save as PNG")){
            controller.SaveAsPNG(controller.heightMap);
        }
        if (GUILayout.Button("HeightmapGAN"))
        {
            controller.HeightmapGAN();
        }
        if (GUILayout.Button("SatelliteGAN"))
        {
            controller.SatelliteGAN();
        }
        if (GUILayout.Button("ReadPngFromFile"))
        {
            controller.ReadPngFromDisk();
        }
    }
}

[CustomEditor(typeof(Rain3))]
public class Editor2 : Editor
{
    public override void OnInspectorGUI()
    {
        Rain3 rain3 = (Rain3)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Init"))
        {
            rain3.Init();
        }
        if (GUILayout.Button("Rain"))
        {
            rain3.Rain();
        }
        if (GUILayout.Button("Set params"))
        {
            rain3.setParams();
        }
        if (GUILayout.Button("Release"))
        {
            rain3.Realease();
        }
    }
}

[CustomEditor(typeof(ColorDesign))]
public class Editor3 : Editor
{
    public override void OnInspectorGUI()
    {
        ColorDesign colorDesign = (ColorDesign)target;
        DrawDefaultInspector();
        if (GUILayout.Button("Generate"))
        {
            colorDesign.generate();
        }

    }
}

