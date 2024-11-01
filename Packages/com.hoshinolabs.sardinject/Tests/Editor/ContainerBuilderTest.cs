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

        // �^�œo�^,�X���[����ׂ��ł͂Ȃ�
        [Test]
        public void Register_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.Register<Simple>(Lifetime.Transient));
        }

        // �^�œo�^,ISimple���p������Simple,�X���[����ׂ��ł͂Ȃ�
        [Test]
        public void Register_SimpleThatInheritsISimple_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.Register<Simple>(Lifetime.Transient).As<ISimple>());
        }

        // �^�œo�^,IDisposable���p������Simple,�X���[����ׂ�
        [Test]
        public void Register_SimpleThatInheritsIDisposable_ShouldThrow() {
            var builder = new ContainerBuilder();
            Assert.Throws<SardinjectException>(() => builder.Register<Simple>(Lifetime.Transient).As<IDisposable>());
        }

        // �C���X�^���X�œo�^,�X���[����ׂ��ł͂Ȃ�
        [Test]
        public void RegisterInstance_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.RegisterInstance(new Simple()));
        }

        // �t�@�N�g���œo�^,�X���[����ׂ��ł͂Ȃ�
        [Test]
        public void RegisterFactory_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            Assert.DoesNotThrow(() => builder.RegisterFactory(() => new Simple(), Lifetime.Transient));
            Assert.DoesNotThrow(() => builder.RegisterFactory((_) => new Simple(), Lifetime.Transient));
        }

        // �r���h,�X���[����ׂ��ł͂Ȃ�
        [Test]
        public void Build_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<Simple>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // �r���h,�ˑ������z���Ă���,�X���[����ׂ�
        [Test]
        public void Build_CircularDependencies_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleCircularDependency>(Lifetime.Transient);
            Assert.Throws<SardinjectException>(() => builder.Build());
        }

        // �r���h,�P��̃R���X�g���N�^,�X���[���Ȃ��ׂ�
        [Test]
        public void Build_SingleConstructor_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleSingleConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // �r���h,�����̃R���X�g���N�^,�X���[���Ȃ��ׂ�
        [Test]
        public void Build_MultipleConstructor_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleMultipleConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // �r���h,�P��̃R���X�g���N�^,�X���[���Ȃ��ׂ�
        [Test]
        public void Build_SingleInjectedConstructor_ShouldNotThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleSingleInjectedConstructor>(Lifetime.Transient);
            Assert.DoesNotThrow(() => builder.Build());
        }

        // �r���h,�����̃R���X�g���N�^,�X���[����ׂ�
        [Test]
        public void Build_MultipleInjectedConstructor_ShouldThrow() {
            var builder = new ContainerBuilder();
            builder.Register<SimpleMultipleInjectedConstructor>(Lifetime.Transient);
            Assert.Throws<SardinjectException>(() => builder.Build());
        }
    }
}
