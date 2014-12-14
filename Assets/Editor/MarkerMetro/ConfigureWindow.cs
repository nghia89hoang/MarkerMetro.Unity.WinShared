﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using BuildConfig = Assets.Editor.MarkerMetro.EditorPrefsHelper.BuildConfig;
using PluginSource = Assets.Editor.MarkerMetro.EditorPrefsHelper.PluginSource;

namespace Assets.Editor.MarkerMetro
{
    internal sealed class ConfigureWindow : EditorWindow
    {
        enum DirType
        {
            WinLegacy,
            WinIntegration,
            NuGet
        }

        string _winLegacyDir;
        string _winIntegrationDir;
        string _nugetDir;
        PluginSource _pluginSource;
        BuildConfig _buildConfig;

        void OnEnable()
        {
            _pluginSource = (PluginSource)EditorPrefsHelper.CurrentPluginSource;
            _buildConfig = (BuildConfig)EditorPrefsHelper.CurrentBuildConfig;
            _winLegacyDir = EditorPrefsHelper.WinLegacyDir;
            _winIntegrationDir = EditorPrefsHelper.WinIntegrationDir;
            _nugetDir = EditorPrefsHelper.NugetScriptsDir;
        }

        void OnGUI()
        {
            DrawUpdateSource();
            if (_pluginSource == PluginSource.Local)
            {
                DrawBuildConfig();
            }
            DrawChooseDir(DirType.WinLegacy);
            DrawChooseDir(DirType.WinIntegration);
            DrawChooseDir(DirType.NuGet);
        }

        /// <summary>
        /// GUI for Update Source.
        /// </summary>
        void DrawUpdateSource()
        {
            GUILayout.Space(5f);
            GUI.changed = false;
            _pluginSource = (PluginSource)EditorGUILayout.EnumPopup("Plugin Source", _pluginSource, GUILayout.MaxWidth(250f));
            if (GUI.changed)
            {
                EditorPrefsHelper.CurrentPluginSource = (int)_pluginSource;
            }
            GUILayout.Space(10f);
        }

        /// <summary>
        /// GUI for Build Config.
        /// </summary>
        void DrawBuildConfig()
        {
            GUI.changed = false;
            _buildConfig = (BuildConfig)EditorGUILayout.EnumPopup("Build Local", _buildConfig, GUILayout.MaxWidth(250f));
            if (GUI.changed)
            {
                EditorPrefsHelper.CurrentBuildConfig = (int)_buildConfig;
            }
            GUILayout.Space(10f);
        }

        /// <summary>
        /// GUI for selecting directory.
        /// </summary>
        void DrawChooseDir(DirType dirType)
        {
            string dir = GetDir(dirType);

            EditorGUILayout.BeginVertical();
            GUILayout.Label(dirType.ToString() + " Dir:");
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.BeginVertical();
            GUILayout.Space(4f);
            GUILayout.Label(dir, "AS TextArea", GUILayout.Height(20f));
            EditorGUILayout.EndVertical();
            GUILayout.Space(3f);
            if (GUILayout.Button("Choose", "LargeButtonMid", GUILayout.Height(20f), GUILayout.ExpandWidth(false)))
            {
                dir = EditorUtility.OpenFolderPanel("Choose Folder", Application.dataPath, "");
                if (!string.IsNullOrEmpty(dir))
                {
                    SetDir(dirType, dir);
                }
            }
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10f);
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Return cached dir based on DirType.
        /// </summary>
        string GetDir(DirType dirType)
        {
            string dir = string.Empty;
            switch (dirType)
            {
                case DirType.WinLegacy:
                    dir = _winLegacyDir;
                    break;
                case DirType.WinIntegration:
                    dir = _winIntegrationDir;
                    break;
                case DirType.NuGet:
                    dir = _nugetDir;
                    break;
            }
            return dir;
        }

        /// <summary>
        /// Store dir of DirType to EditorPrefs.
        /// </summary>
        void SetDir(DirType dirType, string dir)
        {
            dir = System.IO.Path.GetFullPath(dir);
            switch (dirType)
            {
                case DirType.WinLegacy:
                    _winLegacyDir = dir;
                    EditorPrefsHelper.WinLegacyDir = _winLegacyDir;
                    break;
                case DirType.WinIntegration:
                    _winIntegrationDir = dir;
                    EditorPrefsHelper.WinIntegrationDir = _winIntegrationDir;
                    break;
                case DirType.NuGet:
                    _nugetDir = dir;
                    EditorPrefsHelper.NugetScriptsDir = _nugetDir;
                    break;
            }
        }
    }
}