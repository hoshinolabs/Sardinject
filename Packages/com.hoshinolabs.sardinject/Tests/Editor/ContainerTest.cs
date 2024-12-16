using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HoshinoLabs.Sardinject.Tests {
    [TestFixture]
    public class ContainerTest {
        //class SimpleComponent : MonoBehaviour { }

        //[SetUp]
        //public void Setup()
        //{
        //    // Make sure we have a clean state
        //    Settings.CleanSerializedData();
        //    var graphs = Settings.GraphSettingsList.GetGraphSettings();

        //    foreach (var graph in graphs)
        //    {
        //        //Validate that the graph settings are in a good state
        //        Assert.IsNotNull(graph.programAsset);
        //    }
        //}

        //[InitializeOnLoadMethod()]
        //public static void _InitializeOnLoadMethod() {
        //    Debug.LogWarning("InitializeOnLoadMethod");
        //}
        //[InitializeOnEnterPlayMode()]
        //public static void _InitializeOnEnterPlayMode() {
        //    Debug.LogWarning("InitializeOnEnterPlayMode");
        //}
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        //public static void _RuntimeInitializeLoadType_AfterSceneLoad() {
        //    Debug.LogWarning("RuntimeInitializeLoadType.AfterSceneLoad");
        //}
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        //public static void _RuntimeInitializeLoadType_BeforeSceneLoad() {
        //    Debug.LogWarning("RuntimeInitializeLoadType.BeforeSceneLoad");
        //}
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        //public static void _RuntimeInitializeLoadType_AfterAssembliesLoaded() {
        //    Debug.LogWarning("RuntimeInitializeLoadType.AfterAssembliesLoaded");
        //}
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        //public static void _RuntimeInitializeLoadType_BeforeSplashScreen() {
        //    Debug.LogWarning("RuntimeInitializeLoadType.BeforeSplashScreen");
        //}
        //[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        //public static void _RuntimeInitializeLoadType_SubsystemRegistration() {
        //    Debug.LogWarning("RuntimeInitializeLoadType.SubsystemRegistration");
        //}

        //[Test]
        //public void AddInstance() {
        //    //var instance = new GameObject(nameof(SimpleComponent)).AddComponent<SimpleComponent>();

        //    //var context = new Context();
        //    //context.Enqueue(builder => {
        //    //    builder.AddInstance(instance);
        //    //});

        //    //var container = context.Build();

        //    //Assert.That(container.Resolve<SimpleComponent>(), Is.EqualTo(instance));
        //    //Assert.That(container.Resolve<SimpleComponent>(), Is.InstanceOf<SimpleComponent>());
        //    //Assert.Throws<SardinjectException>(() => container.Resolve<MonoBehaviour>());
        //}

        //[Test]
        //public void AddInstanceAsInterface() {
        //    //var instance = new GameObject(nameof(SimpleComponent)).AddComponent<SimpleComponent>();

        //    //var context = new Context();
        //    //context.Enqueue(builder => {
        //    //    builder.AddInstance<MonoBehaviour>(instance);
        //    //});

        //    //var container = context.Build();

        //    //Assert.That(container.Resolve<MonoBehaviour>(), Is.EqualTo(instance));
        //}




        interface ISimple {

        }

        class Simple : ISimple {

        }

        class Simple<T> : Simple {

        }

        enum Kinds {
            Simple1,
            Simple2,
        }

        class SimpleWithId : ISimple {
            [Inject(Kinds.Simple1)]
            public readonly SimpleWithValue Simple1;
            [Inject(Kinds.Simple2)]
            public readonly SimpleWithValue Simple2;
        }

        class SimpleWithValue : ISimple {
            [Inject]
            public readonly int Value;
        }

        // 型で解決,未登録タイプ,スローするべき
        [Test]
        public void Resolve_UnregisterByType_ShouldThrow() {
            var container = new ContainerBuilder().Build();
            Assert.Throws<SardinjectException>(() => container.Resolve<Simple>());
        }

        // 型で解決,登録済みタイプ,スローされないべき
        [Test]
        public void Resolve_RegisterByType_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<Simple>());
        }

        // 汎用型で解決,登録済みタイプ,スローされないべき
        [Test]
        public void Resolve_RegisterByTypeOfOpenGeneric_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register(typeof(Simple<>), Lifetime.Transient);
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<Simple<int>>());
        }

        // 配列型で解決,未登録配列タイプ,空の配列を返す
        [Test]
        public void Resolve_UnregisterByType_ShouldReturnEmptyArray() {
            var container = new ContainerBuilder().Build();
            Assert.IsEmpty(container.Resolve<Simple[]>());
        }

        // 配列型で解決,登録済みタイプ,配列で全ての値を登録順で返す
        [Test]
        public void Resolve_RegisterByType_ShouldReturnArrayOfAllValuesInOrderRegistered() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter(0);
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter(1);
            var container = builder.Build();
            var simples = container.Resolve<SimpleWithValue[]>();
            Assert.AreEqual(2, simples.Count());
            var simple1 = simples.ElementAt(0);
            Assert.AreEqual(0, simple1.Value);
            var simple2 = simples.ElementAt(1);
            Assert.AreEqual(1, simple2.Value);
            Assert.AreNotEqual(simple1, simple2);
        }

        // 列挙型で解決,未登録列挙タイプ,空の列挙子を返す
        [Test]
        public void Resolve_UnregisterByType_ShouldReturnEmptyEnumerable() {
            var container = new ContainerBuilder().Build();
            Assert.IsEmpty(container.Resolve<IEnumerable<Simple>>());
        }

        // 列挙型で解決,登録済みタイプ,列挙子で全ての値を登録順で返す
        [Test]
        public void Resolve_RegisterByType_ShouldReturnEnumerableOfAllValuesInOrderRegistered() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter(0);
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter(1);
            var container = builder.Build();
            var simples = container.Resolve<IEnumerable<SimpleWithValue>>();
            Assert.AreEqual(2, simples.Count());
            Assert.AreEqual(0, simples.ElementAt(0).Value);
            Assert.AreEqual(1, simples.ElementAt(1).Value);
        }

        // 型で解決,インスタンスで登録,常に同じ値が返るはず
        [Test]
        public void Resolve_RegisterByInstance_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            var simple = new Simple();
            builder.RegisterInstance(simple);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple>();
            var simple2 = container.Resolve<Simple>();
            Assert.AreEqual(simple1, simple);
            Assert.AreEqual(simple2, simple);
            Assert.AreEqual(simple1, simple2);
        }

        // 型で解決,ファクトリで登録,常に新しい値が返るはず
        [Test]
        public void Resolve_RegisterByFactory_ShouldReturnAlwaysANewInstance() {
            var builder = new ContainerBuilder();
            builder.RegisterFactory((_) => new Simple(), Lifetime.Transient);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple>();
            var simple2 = container.Resolve<Simple>();
            Assert.AreNotEqual(simple1, simple2);
        }

        // 型で解決,コンテナ型,常に自分自身を返すはず
        [Test]
        public void Resolve_Container_ShouldReturnAlwaysSelfContainer() {
            var container = new ContainerBuilder().Build();
            Assert.AreEqual(container, container.Resolve<Container>());
            Assert.AreEqual(1, container.Resolve<Container[]>().Count());
            Assert.AreEqual(container, container.Resolve<Container[]>().ElementAt(0));
        }

        // 型で解決,コンテナ型,階層間で常に自分自身を返すはず
        [Test]
        public void Resolve_Container_ShouldReturnAlwaysSelfContainerButDoesNotHierarchy() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope();
            Assert.AreEqual(container1, container1.Resolve<Container>());
            Assert.AreEqual(container2, container2.Resolve<Container>());
            Assert.AreEqual(1, container1.Resolve<Container[]>().Count());
            Assert.AreEqual(container1, container1.Resolve<Container[]>().ElementAt(0));
            Assert.AreEqual(2, container2.Resolve<Container[]>().Count());
            Assert.AreEqual(container1, container2.Resolve<Container[]>().ElementAt(0));
            Assert.AreEqual(container2, container2.Resolve<Container[]>().ElementAt(1));
        }

        // 型で解決,ISimpleを継承するSimple,未登録の型はスローするべき
        [Test]
        public void Resolve_SimpleThatInheritsAsInterface_ShouldUnspecifiedInterfacesThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient).As<ISimple>();
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<Simple>());
            Assert.DoesNotThrow(() => container.Resolve<ISimple>());
            Assert.Throws<SardinjectException>(() => container.Resolve<IDisposable>());
        }

        // 型で解決,ISimpleを継承するSimple,未登録の型はスローするべき
        [Test]
        public void Resolve_SimpleThatInheritsAsImplementedInterfaces_ShouldUnspecifiedInterfacesThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient).AsImplementedInterfaces();
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<Simple>());
            Assert.DoesNotThrow(() => container.Resolve<ISimple>());
            Assert.Throws<SardinjectException>(() => container.Resolve<IDisposable>());
        }

        // 型で解決,ISimpleを継承するSimple,未登録の型はスローするべき
        [Test]
        public void Resolve_SimpleThatInheritsAsSelf_ShouldUnspecifiedInterfacesThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient).AsSelf();
            var container = builder.Build();
            Assert.DoesNotThrow(() => container.Resolve<Simple>());
            Assert.Throws<SardinjectException>(() => container.Resolve<ISimple>());
            Assert.Throws<SardinjectException>(() => container.Resolve<IDisposable>());
        }

        // 型で解決,名前指定で値を指定,指定された値が設定されるはず
        [Test]
        public void Resolve_NamedParameterFromValue_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter("Value", 123);
            var container = builder.Build();
            Assert.AreEqual(123, container.Resolve<SimpleWithValue>().Value);
        }

        // 型で解決,名前指定で生成関数を指定,指定された値が設定されるはず
        [Test]
        public void Resolve_NamedParameterFromFactoryValue_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter("Value", (_) => 123);
            var container = builder.Build();
            Assert.AreEqual(123, container.Resolve<SimpleWithValue>().Value);
        }

        // 型で解決,型指定で値を指定,指定された値が設定されるはず
        [Test]
        public void Resolve_TypedParameterFromValue_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            Assert.AreEqual(123, container.Resolve<SimpleWithValue>().Value);
        }

        // 型で解決,型指定で生成関数を指定,指定された値が設定されるはず
        [Test]
        public void Resolve_TypedParameterFromFactoryValue_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithParameter((_) => 123);
            var container = builder.Build();
            Assert.AreEqual(123, container.Resolve<SimpleWithValue>().Value);
        }

        // 型で解決,ID指定で型を登録,指定した値が設定されるはず
        [Test]
        public void Resolve_RegisterTypeWithId_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithId>(Lifetime.Transient);
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithId(Kinds.Simple1).WithParameter(1);
            builder.Register<SimpleWithValue>(Lifetime.Transient).WithId(Kinds.Simple2).WithParameter(2);
            var container = builder.Build();
            var simple = container.Resolve<SimpleWithId>();
            Assert.AreEqual(1, simple.Simple1.Value);
            Assert.AreEqual(2, simple.Simple2.Value);
        }

        // 型で解決,一時タイプで,常に新しい値を返す
        [Test]
        public void Resolve_AsTransientFromLifetime_ShouldReturnAlwaysANewInstance() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple>();
            var simple2 = container.Resolve<Simple>();
            Assert.AreNotEqual(simple1, simple2);
        }

        // 汎用型で解決,一時タイプで,常に新しい値を返す
        [Test]
        public void Resolve_AsTransientFromLifetimeOfOpenGeneric_ShouldReturnAlwaysANewInstance() {
            var builder = new ContainerBuilder();
            builder.Register(typeof(Simple<>), Lifetime.Transient);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple<int>>();
            var simple2 = container.Resolve<Simple<int>>();
            Assert.AreNotEqual(simple1, simple2);
            var simple3 = container.Resolve<Simple<uint>>();
            var simple4 = container.Resolve<Simple<uint>>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreNotEqual(simple3, simple4);
        }

        // 型で解決,一時タイプで,常に新しい値を返すが階層は使わない
        [Test]
        public void Resolve_AsTransientFromLifetime_ShouldReturnAlwaysANewInstanceButDoesNotHierarchy() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.Register<Simple>(Lifetime.Transient));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.Register<Simple>(Lifetime.Transient));
            Assert.Throws<SardinjectException>(() => container1.Resolve<Simple>());
            var simple1 = container2.Resolve<Simple>();
            var simple2 = container2.Resolve<Simple>();
            Assert.AreNotEqual(simple1, simple2);
            var simple3 = container3.Resolve<Simple>();
            var simple4 = container3.Resolve<Simple>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreNotEqual(simple3, simple4);
            var simple5 = container4.Resolve<Simple>();
            var simple6 = container4.Resolve<Simple>();
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

        // 型で解決,キャッシュタイプで,常に同じ値を返す
        [Test]
        public void Resolve_AsCachedFromLifetime_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Cached);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple>();
            var simple2 = container.Resolve<Simple>();
            Assert.AreEqual(simple1, simple2);
        }

        // 汎用型で解決,キャッシュタイプで,常に同じ値を返す
        [Test]
        public void Resolve_AsCachedFromLifetimeOfOpenGeneric_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            builder.Register(typeof(Simple<>), Lifetime.Cached);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple<int>>();
            var simple2 = container.Resolve<Simple<int>>();
            Assert.AreEqual(simple1, simple2);
            var simple3 = container.Resolve<Simple<uint>>();
            var simple4 = container.Resolve<Simple<uint>>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreEqual(simple3, simple4);
        }

        // 型で解決,キャッシュタイプで,階層間でできるだけ常に同じ値を返す
        [Test]
        public void Resolve_AsCachedFromLifetime_ShouldReturnAlwaysSameInstanceBetweenHierarchyAsMuchAsPossible() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.Register<Simple>(Lifetime.Cached));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.Register<Simple>(Lifetime.Cached));
            Assert.Throws<SardinjectException>(() => container1.Resolve<Simple>());
            var simple1 = container2.Resolve<Simple>();
            var simple2 = container2.Resolve<Simple>();
            Assert.AreEqual(simple1, simple2);
            var simple3 = container3.Resolve<Simple>();
            var simple4 = container3.Resolve<Simple>();
            Assert.AreEqual(simple3, simple1);
            Assert.AreEqual(simple4, simple1);
            Assert.AreEqual(simple3, simple2);
            Assert.AreEqual(simple4, simple2);
            Assert.AreEqual(simple3, simple4);
            var simple5 = container4.Resolve<Simple>();
            var simple6 = container4.Resolve<Simple>();
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

        // 型で解決,スコープタイプで,常に同じ値を返す
        [Test]
        public void Resolve_AsScopedFromLifetime_ShouldReturnAlwaysSameInstance() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Scoped);
            var container = builder.Build();
            var simple1 = container.Resolve<Simple>();
            var simple2 = container.Resolve<Simple>();
            Assert.AreEqual(simple1, simple2);
        }

        // 型で解決,スコープタイプで,常に同じ値を返すが階層は使わない
        [Test]
        public void Resolve_AsScopedFromLifetime_ShouldReturnAlwaysSameInstanceButDoesNotHierarchy() {
            var container1 = new ContainerBuilder().Build();
            var container2 = container1.Scope((builder) => builder.Register<Simple>(Lifetime.Scoped));
            var container3 = container2.Scope();
            var container4 = container3.Scope((builder) => builder.Register<Simple>(Lifetime.Scoped));
            Assert.Throws<SardinjectException>(() => container1.Resolve<Simple>());
            var simple1 = container2.Resolve<Simple>();
            var simple2 = container2.Resolve<Simple>();
            Assert.AreEqual(simple1, simple2);
            var simple3 = container3.Resolve<Simple>();
            var simple4 = container3.Resolve<Simple>();
            Assert.AreNotEqual(simple3, simple1);
            Assert.AreNotEqual(simple4, simple1);
            Assert.AreNotEqual(simple3, simple2);
            Assert.AreNotEqual(simple4, simple2);
            Assert.AreEqual(simple3, simple4);
            var simple5 = container4.Resolve<Simple>();
            var simple6 = container4.Resolve<Simple>();
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

        class SimpleWithConstructor : ISimple {
            public bool Ok { get; }
            public int Value { get; }

            [Inject]
            SimpleWithConstructor(int value) {
                Value = value;
                Ok = true;
            }
        }

        class SimpleWithMethod : ISimple {
            public bool Ok { get; private set; }
            public int Value { get; private set; }

            [Inject]
            void InjectedMethod(int value) {
                Value = value;
                Ok = true;
            }
        }

        class SimpleWithProperty : ISimple {
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

        // 型で解決,コンストラクタが呼び出されている,指定された値が設定されるはず
        [Test]
        public void Resolve_ConstructorCalled_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithConstructor>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            var simple = container.Resolve<SimpleWithConstructor>();
            Assert.IsTrue(simple.Ok);
            Assert.AreEqual(123, simple.Value);
        }

        // 型で解決,メソッドが呼び出されている,指定された値が設定されるはず
        [Test]
        public void Resolve_MethodCalled_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithMethod>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            var simple = container.Resolve<SimpleWithMethod>();
            Assert.IsTrue(simple.Ok);
            Assert.AreEqual(123, simple.Value);
        }

        // 型で解決,プロパティが呼び出されている,指定された値が設定されるはず
        [Test]
        public void Resolve_PropertyCalled_ShouldInjectSpecifiedValue() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleWithProperty>(Lifetime.Transient).WithParameter(123);
            var container = builder.Build();
            var simple = container.Resolve<SimpleWithProperty>();
            Assert.IsTrue(simple.Ok);
            Assert.AreEqual(123, simple.Value);
        }
    }
}
