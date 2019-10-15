using System;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoFakeItEasy;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;

namespace ProvisionData.UnitTesting
{
    public class Test : IDisposable
    {
        private Boolean _disposed;

        protected IFixture Fixture { get; }

        public TestContext TestContext { get; }
        public ITestableClock Clock { get; }
        public IServiceProvider ServiceProvider { get; }

        public Test()
        {
            Clock = new TestableClock(SystemClock.Instance);
            TestContext = new TestContext(Clock);

            Fixture = new Fixture()
                .Customize(new AutoFakeItEasyCustomization
                {
                    ConfigureMembers = true
                });

            ServiceProvider = FakeServiceProvider();
            Fixture.Customize<IServiceProvider>(x => x.FromFactory(() => ServiceProvider));
            Fixture.Inject<IClock>(Clock);
        }

        protected T Fake<T>() => Fixture.Create<T>();
        protected T[] Many<T>(Int32 count = 3) => Fixture.CreateMany<T>(count).ToArray();
        protected void Inject<T>(T instance) => Fixture.Inject(instance);

        private IServiceProvider FakeServiceProvider()
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            // Scoped (Unit of Work)
            return new DefaultServiceProviderFactory().CreateServiceProvider(services);
        }

        protected virtual void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(Clock);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free managed resources.
                }

                // Free unmanaged resources.
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
