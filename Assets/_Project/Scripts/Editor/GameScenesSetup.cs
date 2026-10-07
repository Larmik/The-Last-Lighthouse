using System;
using System.IO;
using Game.Bootstrap;
using Game.Data.Scenes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Game.Editor
{
    public static class GameScenesSetup
    {
        private const string ScenesFolder = "Assets/_Project/Scenes";
        private const string SceneTemplatePath = "Assets/Settings/Scenes/URP2DSceneTemplate.unity";
        private const string SceneExtension = ".unity";

        private static string ScenePath(GameScene scene) => $"{ScenesFolder}/{scene}{SceneExtension}";

        [MenuItem("Game/Scenes/Create Missing Scenes And Build Settings")]
        public static void CreateMissingScenesAndBuildSettings()
        {
            var scenes = (GameScene[])Enum.GetValues(typeof(GameScene));
            foreach (var scene in scenes)
            {
                CreateSceneIfMissing(scene);
            }

            var buildScenes = new EditorBuildSettingsScene[scenes.Length];
            for (var i = 0; i < scenes.Length; i++)
            {
                buildScenes[i] = new EditorBuildSettingsScene(ScenePath(scenes[i]), true);
            }

            EditorBuildSettings.scenes = buildScenes;
            AssetDatabase.SaveAssets();
        }

        private static void CreateSceneIfMissing(GameScene scene)
        {
            var path = ScenePath(scene);
            if (File.Exists(path))
            {
                return;
            }

            if (!AssetDatabase.CopyAsset(SceneTemplatePath, path))
            {
                throw new InvalidOperationException($"Cannot create scene {path} from {SceneTemplatePath}.");
            }

            if (scene == GameScene.Boot)
            {
                AddBootStartup(path);
            }
        }

        private static void AddBootStartup(string path)
        {
            var bootScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
            new GameObject(nameof(BootStartup), typeof(BootStartup));
            EditorSceneManager.SaveScene(bootScene);
        }
    }
}
