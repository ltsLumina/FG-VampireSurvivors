﻿using UnityEditor;
using UnityEngine;

public class FlareInspector : ShaderGUI
{
    bool colorSettings;

    bool fadeSettings;
    bool noiseSettings;
    bool shaderSettings;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {


        GUIStyle foldoutStyle = new GUIStyle(GUI.skin.GetStyle("toolbarPopup"));
        foldoutStyle.fontStyle = FontStyle.Bold;
        foldoutStyle.margin = new RectOffset(30, 10, 5, 5);


        Rect space = new Rect(0, 0, EditorGUIUtility.currentViewWidth, EditorGUIUtility.currentViewWidth * 0.15f);

        EditorGUILayout.Space(space.height);

        Texture banner = (Texture)AssetDatabase.LoadAssetAtPath("Assets/Distant Lands/Lumen - Stylized Light Effects/Contents/Scripts/Editor/Icons/Titlebar.png", typeof(Texture));

        GUI.DrawTexture(space, banner);


        colorSettings = EditorGUILayout.BeginFoldoutHeaderGroup(colorSettings, "    Main Settings", foldoutStyle);
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (colorSettings)
        {
            MaterialProperty colorProperty = FindProperty("_MainColor", properties);
            materialEditor.ShaderProperty(colorProperty, colorProperty.displayName);
            MaterialProperty intensity = FindProperty("_Intensity", properties);
            materialEditor.ShaderProperty(intensity, intensity.displayName);
            EditorGUILayout.Space();

        }


        fadeSettings = EditorGUILayout.BeginFoldoutHeaderGroup(fadeSettings, "    Fading Settings", foldoutStyle);
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (fadeSettings)
        {
            MaterialProperty useCameraDepth = FindProperty("_UseCameraDepthFade", properties);
            materialEditor.ShaderProperty(useCameraDepth, useCameraDepth.displayName);
            EditorGUI.BeginDisabledGroup(useCameraDepth.floatValue != 1);
            MaterialProperty cameraDepthStart = FindProperty("_CameraDepthFadeStart", properties);
            materialEditor.ShaderProperty(cameraDepthStart, cameraDepthStart.displayName);
            MaterialProperty cameraDepthEnd = FindProperty("_CameraDepthFadeEnd", properties);
            materialEditor.ShaderProperty(cameraDepthEnd, cameraDepthEnd.displayName);
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();

            MaterialProperty useCameraDistance = FindProperty("_UseCameraDistanceFade", properties);
            materialEditor.ShaderProperty(useCameraDistance, useCameraDistance.displayName);
            EditorGUI.BeginDisabledGroup(useCameraDistance.floatValue != 1);
            MaterialProperty cameraDistanceStart = FindProperty("_CameraDistanceFadeStart", properties);
            materialEditor.ShaderProperty(cameraDistanceStart, cameraDistanceStart.displayName);
            MaterialProperty cameraDistanceEnd = FindProperty("_CameraDistanceFadeEnd", properties);
            materialEditor.ShaderProperty(cameraDistanceEnd, cameraDistanceEnd.displayName);
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();

            MaterialProperty useZScaleFade = FindProperty("_UseZScaleFade", properties);
            materialEditor.ShaderProperty(useZScaleFade, useZScaleFade.displayName);
            EditorGUILayout.Space();

            MaterialProperty useSceneDepth = FindProperty("_UseSceneDepthFade", properties);
            materialEditor.ShaderProperty(useSceneDepth, useSceneDepth.displayName);
            EditorGUI.BeginDisabledGroup(useSceneDepth.floatValue != 1);
            MaterialProperty sceneDepthStart = FindProperty("_DepthFadeStartDistance", properties);
            materialEditor.ShaderProperty(sceneDepthStart, sceneDepthStart.displayName);
            MaterialProperty sceneDepthEnd = FindProperty("_DepthFadeEndDistance", properties);
            materialEditor.ShaderProperty(sceneDepthEnd, sceneDepthEnd.displayName);
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();

        }


        noiseSettings = EditorGUILayout.BeginFoldoutHeaderGroup(noiseSettings, "    Variation Settings", foldoutStyle);
        EditorGUILayout.EndFoldoutHeaderGroup();
        if (noiseSettings)
        {
            MaterialProperty useVariation = FindProperty("_UseVariation", properties);
            materialEditor.ShaderProperty(useVariation, useVariation.displayName);
            EditorGUI.BeginDisabledGroup(useVariation.floatValue != 1);

            MaterialProperty variationColor = FindProperty("_VariationColor", properties);
            materialEditor.ShaderProperty(variationColor, variationColor.displayName);
            MaterialProperty variationSpeed = FindProperty("_VariationSpeed", properties);
            materialEditor.ShaderProperty(variationSpeed, variationSpeed.displayName);
            MaterialProperty variationScale = FindProperty("_VariationScale", properties);
            materialEditor.ShaderProperty(variationScale, variationScale.displayName);
            EditorGUILayout.Space();
            EditorGUI.EndDisabledGroup();
        }

        
    }
}