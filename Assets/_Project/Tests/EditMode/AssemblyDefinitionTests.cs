using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEditor.Compilation;
using UnityEngine;
using CompiledAssembly = UnityEditor.Compilation.Assembly;

namespace Game.Tests.EditMode
{
    public sealed class AssemblyDefinitionTests
    {
        private const string CoreAssembly = "Game.Core";
        private const string DataAssembly = "Game.Data";
        private const string EconomyAssembly = "Game.Economy";
        private const string MetaAssembly = "Game.Meta";
        private const string GameplayAssembly = "Game.Gameplay";
        private const string ServicesAssembly = "Game.Services";
        private const string UIAssembly = "Game.UI";
        private const string BootstrapAssembly = "Game.Bootstrap";
        private const string EditorAssembly = "Game.Editor";
        private const string ProjectAssemblyPrefix = "Game.";
        private const string TestAssemblyPrefix = "Game.Tests.";
        private const string GuidReferencePrefix = "GUID:";
        private const string NewtonsoftAssemblyFile = "Newtonsoft.Json.dll";

        private static readonly string[] EngineAssemblyPrefixes = { "UnityEngine", "UnityEditor" };

        private static readonly string[] RuntimeAssemblies =
        {
            CoreAssembly, DataAssembly, EconomyAssembly, MetaAssembly,
            GameplayAssembly, ServicesAssembly, UIAssembly, BootstrapAssembly
        };

        private static readonly Dictionary<string, string[]> AllowedProjectReferences = new Dictionary<string, string[]>
        {
            { CoreAssembly, Array.Empty<string>() },
            { DataAssembly, new[] { CoreAssembly } },
            { EconomyAssembly, new[] { CoreAssembly, DataAssembly } },
            { MetaAssembly, new[] { CoreAssembly, DataAssembly } },
            { GameplayAssembly, new[] { CoreAssembly, DataAssembly, EconomyAssembly } },
            { ServicesAssembly, new[] { CoreAssembly } },
            { UIAssembly, new[] { CoreAssembly, DataAssembly, EconomyAssembly, MetaAssembly, GameplayAssembly, ServicesAssembly } },
            {
                BootstrapAssembly,
                new[] { CoreAssembly, DataAssembly, EconomyAssembly, MetaAssembly, GameplayAssembly, ServicesAssembly, UIAssembly }
            },
            { EditorAssembly, RuntimeAssemblies }
        };

        private static IEnumerable<string> MatrixAssemblies => AllowedProjectReferences.Keys;

        [Test]
        public void ProjectAssemblies_AfterCompilation_MatchDependencyMatrix()
        {
            var compiledProjectAssemblies = CompilationPipeline.GetAssemblies()
                .Select(assembly => assembly.name)
                .Where(name => name.StartsWith(ProjectAssemblyPrefix, StringComparison.Ordinal))
                .Where(name => !name.StartsWith(TestAssemblyPrefix, StringComparison.Ordinal));

            Assert.That(compiledProjectAssemblies, Is.EquivalentTo(MatrixAssemblies));
        }

        [TestCaseSource(nameof(MatrixAssemblies))]
        public void ProjectReferences_OfAssembly_StayWithinDependencyMatrix(string assemblyName)
        {
            var projectReferences = FindCompiledAssembly(assemblyName).assemblyReferences
                .Select(reference => reference.name)
                .Where(name => name.StartsWith(ProjectAssemblyPrefix, StringComparison.Ordinal));

            Assert.That(projectReferences, Is.SubsetOf(AllowedProjectReferences[assemblyName]));
        }

        [TestCaseSource(nameof(MatrixAssemblies))]
        public void AssemblyDefinitionReferences_OfAssembly_UseNamesNotGuids(string assemblyName)
        {
            var definition = ReadAssemblyDefinition(assemblyName);

            Assert.That(definition.References, Has.None.StartsWith(GuidReferencePrefix));
        }

        [Test]
        public void CoreAssemblyDefinition_NoEngineReferences_IsEnabled() =>
            Assert.That(ReadAssemblyDefinition(CoreAssembly).NoEngineReferences, Is.True);

        [Test]
        public void CoreAssemblyDefinition_References_AreEmpty() =>
            Assert.That(ReadAssemblyDefinition(CoreAssembly).References, Is.Empty);

        [Test]
        public void CoreAssemblyDefinition_PrecompiledReferences_AreOnlyNewtonsoft()
        {
            var definition = ReadAssemblyDefinition(CoreAssembly);

            Assert.That(definition.OverrideReferences, Is.True);
            Assert.That(definition.PrecompiledReferences, Is.EqualTo(new[] { NewtonsoftAssemblyFile }));
        }

        [Test]
        public void CoreCompilerReferences_UnityAssemblies_AreAbsent()
        {
            var compilerReferences = FindCompiledAssembly(CoreAssembly, AssembliesType.Player).compiledAssemblyReferences
                .Select(Path.GetFileNameWithoutExtension);

            Assert.That(compilerReferences.Where(IsEngineAssembly), Is.Empty);
        }

        [Test]
        public void CoreLoadedAssembly_ReferencedAssemblies_ExcludeUnity()
        {
            var coreAssembly = AppDomain.CurrentDomain.GetAssemblies()
                .Single(assembly => assembly.GetName().Name == CoreAssembly);
            var referencedAssemblies = coreAssembly.GetReferencedAssemblies().Select(reference => reference.Name);

            Assert.That(referencedAssemblies.Where(IsEngineAssembly), Is.Empty);
        }

        [Test]
        public void PlayerAssemblies_RuntimeAssemblies_AreIncluded()
        {
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player).Select(assembly => assembly.name);

            Assert.That(playerAssemblies, Is.SupersetOf(RuntimeAssemblies));
        }

        [Test]
        public void PlayerAssemblies_EditorAssembly_IsExcluded()
        {
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player).Select(assembly => assembly.name);

            Assert.That(playerAssemblies, Has.No.Member(EditorAssembly));
        }

        [Test]
        public void EditorAssembly_Flags_MarkEditorOnly() =>
            Assert.That(FindCompiledAssembly(EditorAssembly).flags.HasFlag(AssemblyFlags.EditorAssembly), Is.True);

        private static CompiledAssembly FindCompiledAssembly(string assemblyName, AssembliesType assembliesType = AssembliesType.Editor)
        {
            var compiledAssembly = CompilationPipeline.GetAssemblies(assembliesType).SingleOrDefault(assembly => assembly.name == assemblyName);
            Assert.That(compiledAssembly, Is.Not.Null, $"Assembly {assemblyName} is not compiled.");
            return compiledAssembly;
        }

        private static AssemblyDefinitionFile ReadAssemblyDefinition(string assemblyName)
        {
            var path = CompilationPipeline.GetAssemblyDefinitionFilePathFromAssemblyName(assemblyName);
            Assert.That(path, Is.Not.Null.And.Not.Empty, $"Assembly definition of {assemblyName} not found.");
            return JsonUtility.FromJson<AssemblyDefinitionFile>(File.ReadAllText(path));
        }

        private static bool IsEngineAssembly(string assemblyName) =>
            EngineAssemblyPrefixes.Any(prefix => assemblyName.StartsWith(prefix, StringComparison.Ordinal));

        [Serializable]
        private sealed class AssemblyDefinitionFile
        {
            [SerializeField] private string[] references = Array.Empty<string>();
            [SerializeField] private bool noEngineReferences;
            [SerializeField] private bool overrideReferences;
            [SerializeField] private string[] precompiledReferences = Array.Empty<string>();

            public IReadOnlyList<string> References => references;
            public bool NoEngineReferences => noEngineReferences;
            public bool OverrideReferences => overrideReferences;
            public IReadOnlyList<string> PrecompiledReferences => precompiledReferences;
        }
    }
}
