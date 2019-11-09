/*******************************************************************************
 * MIT License
 *
 * Copyright 2020 Provision Data Systems Inc.  https://provisiondata.com
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a 
 * copy of this software and associated documentation files (the "Software"),
 * to deal in the Software without restriction, including without limitation
 * the rights to use, copy, modify, merge, publish, distribute, sublicense,
 * and/or sell copies of the Software, and to permit persons to whom the 
 * Software is furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 * DEALINGS IN THE SOFTWARE.
 *
 *******************************************************************************/

namespace ProvisionData.UnitTesting
{
    using System;
    using System.Linq;
    using AutoFixture;
    using AutoFixture.AutoFakeItEasy;
    using Microsoft.Extensions.DependencyInjection;
    using NodaTime;

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
