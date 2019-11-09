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
        if (GUILayout.Button("2d"))
        {
            controller.display2d();
        }
        if (GUILayout.Button("load png"))
        {
            controller.ReadPng();
        }
        if (GUILayout.Button("generate Perlin noise"))
        {
            controller.GeneratePerlinNoise();
        }
        if (GUILayout.Button("rain"))
        {
            controller.RainErosion();
        }
        if (GUILayout.Button("rain at"))
        {
            controller.RainErosionAt();
        }
        if (GUILayout.Button("terrain"))
        {
            controller.Terrain();
        }
    }
}
