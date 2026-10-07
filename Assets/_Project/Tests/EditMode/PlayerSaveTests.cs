using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Core.Save;
using NUnit.Framework;

namespace Game.Tests.EditMode
{
    public sealed class PlayerSaveTests
    {
        private static readonly string[] OptionalReferenceFields =
        {
            nameof(PlayerSave) + "." + nameof(PlayerSave.ActiveRun)
        };

        [Test]
        public void New_Version_IsCurrentVersion() =>
            Assert.That(new PlayerSave().Version, Is.EqualTo(PlayerSave.CurrentVersion));

        [Test]
        public void New_Progression_StartsEmptyOnFirstIsland()
        {
            var save = new PlayerSave();

            Assert.That(save.Balances, Is.Empty);
            Assert.That(save.MetaLevels, Is.Empty);
            Assert.That(save.Islands, Is.Empty);
            Assert.That(save.HighestIsland, Is.EqualTo(1));
            Assert.That(save.EquippedKeeper, Is.Null);
            Assert.That(save.ActiveRun, Is.Null);
            Assert.That(save.RunsCompleted, Is.Zero);
        }

        [TestCaseSource(nameof(SaveModelTypes))]
        public void New_ReferenceFields_AreNotNullExceptOptional(Type saveModelType)
        {
            var instance = Activator.CreateInstance(saveModelType);
            var nullFields = ReferenceFields(saveModelType)
                .Where(field => field.GetValue(instance) == null)
                .Select(field => saveModelType.Name + "." + field.Name)
                .Except(OptionalReferenceFields);

            Assert.That(nullFields, Is.Empty);
        }

        [TestCaseSource(nameof(SaveModelTypes))]
        public void New_Collections_AreEmpty(Type saveModelType)
        {
            var instance = Activator.CreateInstance(saveModelType);
            var nonEmptyCollections = ReferenceFields(saveModelType)
                .Where(field => field.GetValue(instance) is IEnumerable items && items.Cast<object>().Any())
                .Select(field => field.Name);

            Assert.That(nonEmptyCollections, Is.Empty);
        }

        private static IEnumerable<Type> SaveModelTypes() =>
            typeof(PlayerSave).Assembly.GetTypes()
                .Where(type => type.Namespace == typeof(PlayerSave).Namespace)
                .Where(type => type.IsClass && type.IsSerializable);

        private static IEnumerable<FieldInfo> ReferenceFields(Type type) =>
            type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(field => !field.FieldType.IsValueType && field.FieldType != typeof(string));
    }
}
