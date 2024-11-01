using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools;
#if !VRC_CLIENT && UNITY_EDITOR && VRC_SDK_VRCSDK3
using VRC.Core;
#endif

namespace HoshinoLabs.Sardinject.Tests {
    [TestFixture]
    public class ContainerTest : IPrebuildSetup, IPostBuildCleanup {
#if !VRC_CLIENT && UNITY_EDITOR && VRC_SDK_VRCSDK3
        IDictionary InitialTargetAccessFilters {
            get {
                var initialTargetAccessFiltersField = typeof(UnityEventFilter)
                    .GetField("_initialTargetAccessFilters", BindingFlags.Static | BindingFlags.NonPublic);
                return (IDictionary)initialTargetAccessFiltersField.GetValue(null);
            }
        }
#endif

#if !VRC_CLIENT && UNITY_EDITOR && VRC_SDK_VRCSDK3
        readonly string[] AllowedTarget = new[] {
            "UnityEditor.TestTools.TestRunner.Api.CallbacksDelegatorListener",
            "UnityEditor.TestTools.TestRunner.TestRunnerCallback",
            "UnityEngine.TestRunner.Utils.TestRunCallbackListener",
            "UnityEngine.TestTools.TestRunner.Callbacks.PlayModeRunnerCallback",
        };
#endif

        public void Setup() {
#if !VRC_CLIENT && UNITY_EDITOR && VRC_SDK_VRCSDK3
            var allowedMethodFilterType = typeof(UnityEventFilter).Assembly.GetTypes()
                .Where(x => x.FullName == "VRC.Core.UnityEventFilter+AllowedMethodFilter")
                .First();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (!AllowedTarget.Contains(type.FullName)) {
                        continue;
                    }
                    InitialTargetAccessFilters.Remove(type);
                    var allowedTargetMethodNames = type
                        .GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                        .Select(x => x.Name)
                        .ToList();
                    var allowedTargetPropertyNames = new List<string>();
                    var allowedMethodFilter = Activator.CreateInstance(allowedMethodFilterType, new[] {
                        allowedTargetMethodNames,
                        allowedTargetPropertyNames,
                    });
                    InitialTargetAccessFilters.Add(type, allowedMethodFilter);
                }
            }
#endif
        }

        public void Cleanup() {
#if !VRC_CLIENT && UNITY_EDITOR && VRC_SDK_VRCSDK3
            var allowedMethodFilterType = typeof(UnityEventFilter).Assembly.GetTypes()
                .Where(x => x.FullName == "VRC.Core.UnityEventFilter+AllowedMethodFilter")
                .First();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (var type in assembly.GetTypes()) {
                    if (!AllowedTarget.Contains(type.FullName)) {
                        continue;
                    }
                    InitialTargetAccessFilters.Remove(type);
                }
            }
#endif
        }

        interface ISimpleComponent {

        }

        class SimpleComponent : MonoBehaviour, ISimpleComponent {

        }

        class SimpleComponentWithValue : MonoBehaviour, ISimpleComponent {
            [Inject]
            public readonly int Value;
        }

        // Œ^‚Å‰ğŒˆ,ƒCƒ“ƒXƒ^ƒ“ƒX‚Å“o˜^,í‚É“¯‚¶’l‚ª•Ô‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterByInstance_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            var simple = new GameObject().AddComponent<SimpleComponent>();
            builder.RegisterInstance(simple);
            var container = builder.Build();
            var simple1 = container.Resolve<SimpleComponent>();
            var simple2 = container.Resolve<SimpleComponent>();
            Assert.AreEqual(simple1, simple);
            Assert.AreEqual(simple2, simple);
            Assert.AreEqual(simple1, simple2);
        }

        // Œ^‚Å‰ğŒˆ,ƒtƒ@ƒNƒgƒŠ‚Å“o˜^,í‚ÉV‚µ‚¢’l‚ª•Ô‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterByFactory_ShouldReturnAlwaysANewInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterFactory((_) => new GameObject().AddComponent<SimpleComponent>(), Lifetime.Transient);
            var container = builder.Build();
            var simple1 = container.Resolve<SimpleComponent>();
            var simple2 = container.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple1, simple2);
        }

        // Œ^‚Å‰ğŒˆ,–¼‘Ow’è‚Å’l‚ğw’è,w’è‚³‚ê‚½’l‚ªİ’è‚³‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_NamedParameterFromValue_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponentWithValue>(Lifetime.Transient).WithParameter("Value", 123);
            var container = builder.Build();
            Assert.AreEqual(123, container.Resolve<SimpleComponentWithValue>().Value);
        }

        // Œ^‚Å‰ğŒˆ,Œ^‚Å“o˜^,ƒXƒ[‚³‚ê‚È‚¢‚×‚«
        [Test]
        public void Resolve_RegisterComponent_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponent>(Lifetime.Transient);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<SimpleComponent>());
        }

        // Œ^‚Å‰ğŒˆ,ƒCƒ“ƒXƒ^ƒ“ƒX‚Å“o˜^,ƒXƒ[‚³‚ê‚È‚¢‚×‚«
        [Test]
        public void Resolve_RegisterComponentInstance_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            var simple = new GameObject().AddComponent<SimpleComponent>();
            builder.RegisterComponentInstance(simple);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<SimpleComponent>());
        }

        // Œ^‚Å‰ğŒˆ,ŠK‘w‚©‚çŒ^‚Å“o˜^,ƒXƒ[‚³‚ê‚È‚¢‚×‚«
        [Test]
        public void Resolve_RegisterComponentInHierarchy_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            new GameObject().AddComponent<SimpleComponent>();
            builder.RegisterComponentInHierarchy<SimpleComponent>();
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<SimpleComponent>());
        }

        // Œ^‚Å‰ğŒˆ,ƒvƒŒƒnƒu‚Å“o˜^,ƒXƒ[‚³‚ê‚È‚¢‚×‚«
        [Test]
        public void Resolve_RegisterComponentInNewPrefab_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            var prefab = new GameObject();
            prefab.AddComponent<SimpleComponent>();
            builder.RegisterComponentInNewPrefab<SimpleComponent>(Lifetime.Transient, prefab);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<SimpleComponent>());
        }

        // Œ^‚Å‰ğŒˆ,V‹KƒIƒuƒWƒFƒNƒgì¬‚Å“o˜^,ƒXƒ[‚³‚ê‚È‚¢‚×‚«
        [Test]
        public void Resolve_RegisterComponentOnNewGameObject_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.RegisterComponentOnNewGameObject<SimpleComponent>(Lifetime.Transient);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<SimpleComponent>());
        }

        // Œ^‚Å‰ğŒˆ,Œ^‚Å“o˜^‚µŠK‘w‚ğ’l‚Åw’è,w’è‚µ‚½ŠK‘w‚Éì‚ç‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterComponentWithUnderTransformFromValue_ShouldCreatedUnderHierarchy() {
            var builder = new ContainerBuilder();
            var parent = new GameObject().transform;
            builder.RegisterComponent<SimpleComponent>(Lifetime.Transient).UnderTransform(parent);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponent>();
            Assert.AreEqual(parent, simple.transform);
        }

        // Œ^‚Å‰ğŒˆ,Œ^‚Å“o˜^‚µŠK‘w‚ğ¶¬ŠÖ”‚Åw’è,w’è‚µ‚½ŠK‘w‚Éì‚ç‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterComponentWithUnderTransformFromFactory_ShouldCreatedUnderHierarchy() {
            var builder = new ContainerBuilder();
            var parent = new GameObject().transform;
            builder.RegisterComponent<SimpleComponent>(Lifetime.Transient).UnderTransform((_) => parent);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponent>();
            Assert.AreEqual(parent, simple.transform);
        }

        // Œ^‚Å‰ğŒˆ,V‹KƒIƒuƒWƒFƒNƒgì¬‚Å“o˜^‚µŠK‘w‚ğ’l‚Åw’è,w’è‚µ‚½ŠK‘w‚Éì‚ç‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterComponentOnNewGameObjectWithUnderTransformFromValue_ShouldCreatedUnderHierarchy() {
            var builder = new ContainerBuilder();
            var parent = new GameObject().transform;
            builder.RegisterComponentOnNewGameObject<SimpleComponent>(Lifetime.Transient).UnderTransform(parent);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponent>();
            Assert.NotNull(simple.transform.parent);
            Assert.AreEqual(parent, simple.transform.parent);
        }

        // Œ^‚Å‰ğŒˆ,V‹KƒIƒuƒWƒFƒNƒgì¬‚Å“o˜^‚µŠK‘w‚ğ¶¬ŠÖ”‚Åw’è,w’è‚µ‚½ŠK‘w‚Éì‚ç‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_RegisterComponentOnNewGameObjectWithUnderTransformFromFactory_ShouldCreatedUnderHierarchy() {
            var builder = new ContainerBuilder();
            var parent = new GameObject().transform;
            builder.RegisterComponentOnNewGameObject<SimpleComponent>(Lifetime.Transient).UnderTransform((_) => parent);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponent>();
            Assert.NotNull(simple.transform.parent);
            Assert.AreEqual(parent, simple.transform.parent);
        }

        // Œ^‚Å‰ğŒˆ,”jŠü‚µ‚È‚¢‚æ‚¤‚Éw’è,”jŠü‚µ‚È‚¢‚æ‚¤ƒ}[ƒN‚³‚ê‚Ä‚¢‚é‚Í‚¸
        [Test]
        public void Resolve_MarkDontDestroy_ShouldMarkedDontDestroy() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponent>(Lifetime.Transient).DontDestroyOnLoad();
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponent>();
            Assert.AreEqual("DontDestroyOnLoad", container.Resolve<SimpleComponent>().gameObject.scene.name);
        }

        // Œ^‚Å‰ğŒˆ,ˆêƒ^ƒCƒv‚Å,í‚ÉV‚µ‚¢’l‚ğ•Ô‚·
        [Test]
        public void Resolve_AsTransientFromLifetime_ShouldReturnAlwaysANewInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponent>(Lifetime.Transient);
            var container = builder.Build();
            var simple1 = container.Resolve<SimpleComponent>();
            var simple2 = container.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple1, simple2);
        }

        // Œ^‚Å‰ğŒˆ,ˆêƒ^ƒCƒv‚Å,í‚ÉV‚µ‚¢’l‚ğ•Ô‚·‚ªŠK‘w‚Íg‚í‚È‚¢
        [Test]
        public void Resolve_AsTransientFromLifetime_ShouldReturnAlwaysANewInstanceButDoesNotHierarchy() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Transient));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Transient));
            Assert.Throws<SardinjectException>(() => container1.Resolve<SimpleComponent>());
            var simple1 = container2.Resolve<SimpleComponent>();
            var simple2 = container2.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple1, simple2);
            var simple3 = container3.Resolve<SimpleComponent>();
            var simple4 = container3.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreNotEqual(simple3, simple4);
            var simple5 = container4.Resolve<SimpleComponent>();
            var simple6 = container4.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple5, simple1);
            Assert.AreNotEqual(simple6, simple1);
            Assert.AreNotEqual(simple5, simple2);
            Assert.AreNotEqual(simple6, simple2);
            Assert.AreNotEqual(simple5, simple3);
            Assert.AreNotEqual(simple6, simple3);
            Assert.AreNotEqual(simple5, simple4);
            Assert.AreNotEqual(simple6, simple4);
            Assert.AreNotEqual(simple5, simple6);
        }

        // Œ^‚Å‰ğŒˆ,ƒLƒƒƒbƒVƒ…ƒ^ƒCƒv‚Å,í‚É“¯‚¶’l‚ğ•Ô‚·
        [Test]
        public void Resolve_AsCachedFromLifetime_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponent>(Lifetime.Cached);
            var container = builder.Build();
            var simple1 = container.Resolve<SimpleComponent>();
            var simple2 = container.Resolve<SimpleComponent>();
            Assert.AreEqual(simple1, simple2);
        }

        // Œ^‚Å‰ğŒˆ,ƒLƒƒƒbƒVƒ…ƒ^ƒCƒv‚Å,ŠK‘wŠÔ‚Å‚Å‚«‚é‚¾‚¯í‚É“¯‚¶’l‚ğ•Ô‚·
        [Test]
        public void Resolve_AsCachedFromLifetime_ShouldReturnAlwaysSameInstanceBetweenHierarchyAsMuchAsPossible() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Cached));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Cached));
            Assert.Throws<SardinjectException>(() => container1.Resolve<SimpleComponent>());
            var simple1 = container2.Resolve<SimpleComponent>();
            var simple2 = container2.Resolve<SimpleComponent>();
            Assert.AreEqual(simple1, simple2);
            var simple3 = container3.Resolve<SimpleComponent>();
            var simple4 = container3.Resolve<SimpleComponent>();
            Assert.AreEqual(simple3, simple1);
            Assert.AreEqual(simple4, simple1);
            Assert.AreEqual(simple3, simple2);
            Assert.AreEqual(simple4, simple2);
            Assert.AreEqual(simple3, simple4);
            var simple5 = container4.Resolve<SimpleComponent>();
            var simple6 = container4.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple5, simple1);
            Assert.AreNotEqual(simple6, simple1);
            Assert.AreNotEqual(simple5, simple2);
            Assert.AreNotEqual(simple6, simple2);
            Assert.AreNotEqual(simple5, simple3);
            Assert.AreNotEqual(simple6, simple3);
            Assert.AreNotEqual(simple5, simple4);
            Assert.AreNotEqual(simple6, simple4);
            Assert.AreEqual(simple5, simple6);
        }

        // Œ^‚Å‰ğŒˆ,ƒXƒR[ƒvƒ^ƒCƒv‚Å,í‚É“¯‚¶’l‚ğ•Ô‚·
        [Test]
        public void Resolve_AsScopedFromLifetime_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponent>(Lifetime.Scoped);
            var container = builder.Build();
            var simple1 = container.Resolve<SimpleComponent>();
            var simple2 = container.Resolve<SimpleComponent>();
            Assert.AreEqual(simple1, simple2);
        }

        // Œ^‚Å‰ğŒˆ,ƒXƒR[ƒvƒ^ƒCƒv‚Å,í‚É“¯‚¶’l‚ğ•Ô‚·‚ªŠK‘w‚Íg‚í‚È‚¢
        [Test]
        public void Resolve_AsScopedFromLifetime_ShouldReturnAlwaysSameInstanceButDoesNotHierarchy() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Scoped));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.RegisterComponent<SimpleComponent>(Lifetime.Scoped));
            Assert.Throws<SardinjectException>(() => container1.Resolve<SimpleComponent>());
            var simple1 = container2.Resolve<SimpleComponent>();
            var simple2 = container2.Resolve<SimpleComponent>();
            Assert.AreEqual(simple1, simple2);
            var simple3 = container3.Resolve<SimpleComponent>();
            var simple4 = container3.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreEqual(simple3, simple4);
            var simple5 = container4.Resolve<SimpleComponent>();
            var simple6 = container4.Resolve<SimpleComponent>();
            Assert.AreNotEqual(simple5, simple1);
            Assert.AreNotEqual(simple6, simple1);
            Assert.AreNotEqual(simple5, simple2);
            Assert.AreNotEqual(simple6, simple2);
            Assert.AreNotEqual(simple5, simple3);
            Assert.AreNotEqual(simple6, simple3);
            Assert.AreNotEqual(simple5, simple4);
            Assert.AreNotEqual(simple6, simple4);
            Assert.AreEqual(simple5, simple6);
        }

        class SimpleComponentWithMethod : MonoBehaviour, ISimpleComponent {
            public bool Ok { get; private set; }
            public int Value { get; private set; }

            [Inject]
            void InjectedMethod(int value) {
                Value = value;
                Ok = true;
            }
        }

        class SimpleComponentWithProperty : MonoBehaviour, ISimpleComponent {
            public bool Ok { get; private set; }
            public int Value { get; private set; }

            [Inject]
            int InjectedProperty {
                set {
                    Value = value;
                    Ok = true;
                }
            }
        }

        // Œ^‚Å‰ğŒˆ,ƒƒ\ƒbƒh‚ªŒÄ‚Ño‚³‚ê‚Ä‚¢‚é,w’è‚³‚ê‚½’l‚ªİ’è‚³‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_MethodCalled_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponentWithMethod>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponentWithMethod>();
            Assert.IsTrue(simple.Ok);
            Assert.AreEqual(123, simple.Value);
        }

        // Œ^‚Å‰ğŒˆ,ƒvƒƒpƒeƒB‚ªŒÄ‚Ño‚³‚ê‚Ä‚¢‚é,w’è‚³‚ê‚½’l‚ªİ’è‚³‚ê‚é‚Í‚¸
        [Test]
        public void Resolve_PropertyCalled_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleComponentWithProperty>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            var simple = container.Resolve<SimpleComponentWithProperty>();
            Assert.IsTrue(simple.Ok);
            Assert.AreEqual(123, simple.Value);
        }

        class SimpleMonoBehaviour : MonoBehaviour, ISimpleComponent {
            public bool Ok { get; private set; }
            public int Value { get; private set; }

            private void Awake() {
                Ok = true;
            }

            private void Update() {
                Value++;
            }
        }

        // Œ^‚Å‰ğŒˆ,Awake‚ªŒÄ‚Ño‚³‚ê‚Ä‚¢‚é,True‚É‚È‚Á‚Ä‚¢‚é‚Í‚¸
        [Test]
        public void Resolve_AwakeCalled_ShouldTrue() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleMonoBehaviour>(Lifetime.Transient);
            var container = builder.Build();
            Assert.IsTrue(container.Resolve<SimpleMonoBehaviour>().Ok);
        }

        // Œ^‚Å‰ğŒˆ,Update‚ªŒÄ‚Ño‚³‚ê‚Ä‚¢‚é,1ˆÈã‚É‚È‚Á‚Ä‚¢‚é‚Í‚¸
        [UnityTest]
        public IEnumerator Resolve_UpdateCalled_ShoulOneOrHigher() {
            var builder = new ContainerBuilder();
            builder.RegisterComponent<SimpleMonoBehaviour>(Lifetime.Transient);
            var container = builder.Build();
            var simple = container.Resolve<SimpleMonoBehaviour>();
            yield return null;
            Assert.IsTrue(simple.Ok);
            Assert.LessOrEqual(1, simple.Value);
        }
    }
}
