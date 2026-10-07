using System;
using System.IO;
using System.Linq;
using Game.Data.Scenes;
using NUnit.Framework;
using UnityEditor;

namespace Game.Tests.EditMode
{
    public sealed class BuildSettingsTests
    {
        [Test]
        public void EnabledScenes_InBuildSettings_FollowGameSceneOrder()
        {
            var enabledSceneNames = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => Path.GetFileNameWithoutExtension(scene.path));

            Assert.That(enabledSceneNames, Is.EqualTo(Enum.GetNames(typeof(GameScene))));
        }

        [Test]
        public void EnabledScenes_InBuildSettings_ExistOnDisk()
        {
            var missingScenePaths = EditorBuildSettings.scenes
                .Where(scene => scene.enabled)
                .Select(scene => scene.path)
                .Where(path => !File.Exists(path));

            Assert.That(missingScenePaths, Is.Empty);
        }
    }
}
