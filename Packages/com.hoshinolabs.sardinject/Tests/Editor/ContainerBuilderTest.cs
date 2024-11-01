using NUnit.Framework;
using System;

namespace HoshinoLabs.Sardinject.Tests {
    [TestFixture]
    public class ContainerBuilderTest {
        interface ISimple {

        }

        class Simple : ISimple {

        }

        //class SimpleWithValue : ISimple {
        //    [Inject]
        //    int value;

        //    public int Value => value;
        //}

        class SimpleCircularDependency {
            [Inject]
            SimpleCircularDependency simple;
        }

        class SimpleSingleConstructor {
            SimpleSingleConstructor() {

            }
        }

        class SimpleMultipleConstructor {
            SimpleMultipleConstructor(int a) {

            }

            SimpleMultipleConstructor(int a, int b) {

            }
        }

        class SimpleSingleInjectedConstructor {
            [Inject]
            SimpleSingleInjectedConstructor() {

            }
        }

        class SimpleMultipleInjectedConstructor {
            [Inject]
            SimpleMultipleInjectedConstructor(int a) {

            }

            [Inject]
            SimpleMultipleInjectedConstructor(int a, int b) {

            }
        }

        // 型で登録,スローするべきではない
        [Test]
        public void Register_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.Register<Simple>(Lifetime.Transient));
        }

        // 型で登録,ISimpleを継承するSimple,スローするべきではない
        [Test]
        public void Register_SimpleThatInheritsISimple_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.Register<Simple>(Lifetime.Transient).As<ISimple>());
        }

        // 型で登録,IDisposableを継承するSimple,スローするべき
        [Test]
        public void Register_SimpleThatInheritsIDisposable_ShouldThrow() {
            var builder = new ContainerBuilder();
            Assert.Throws<SardinjectException>(() => builder.Register<Simple>(Lifetime.Transient).As<IDisposable>());
        }

        // インスタンスで登録,スローするべきではない
        [Test]
        public void RegisterInstance_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.RegisterInstance(new Simple()));
        }

        // ファクトリで登録,スローするべきではない
        [Test]
        public void RegisterFactory_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.RegisterFactory(() => new Simple(), Lifetime.Transient));
            Assert.DoesNotThrow(() => builder.RegisterFactory((_) => new Simple(), Lifetime.Transient));
        }

        // ビルド,スローするべきではない
        [Test]
        public void Build_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // ビルド,依存性が循環している,スローするべき
        [Test]
        public void Build_CircularDependencies_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleCircularDependency>(Lifetime.Transient);
            Assert.Throws<SardinjectException>(() => builder.Build());
        }

        // ビルド,単一のコンストラクタ,スローしないべき
        [Test]
        public void Build_SingleConstructor_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleSingleConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // ビルド,複数のコンストラクタ,スローしないべき
        [Test]
        public void Build_MultipleConstructor_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleMultipleConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // ビルド,単一のコンストラクタ,スローしないべき
        [Test]
        public void Build_SingleInjectedConstructor_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleSingleInjectedConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // ビルド,複数のコンストラクタ,スローするべき
        [Test]
        public void Build_MultipleInjectedConstructor_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleMultipleInjectedConstructor>(Lifetime.Transient);
            Assert.Throws<SardinjectException>(() => builder.Build());
        }
    }
}
