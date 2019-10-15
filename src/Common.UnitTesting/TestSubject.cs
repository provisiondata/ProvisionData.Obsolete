using System;
using AutoFixture;
using ProvisionData.Extensions;

namespace ProvisionData.UnitTesting
{
    public abstract class TestSubject<T> : Test
    {
        private readonly Lazy<T> _lazy;
        protected T Sut => _lazy.Value;

        protected TestSubject() => _lazy = new Lazy<T>(CreateSut);

        protected virtual T CreateSut() => Fixture.Create<T>();

        private Boolean _disposed;

        protected override void Dispose(Boolean disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (Sut is IDisposable)
                    {
                        Sut.As<IDisposable>(sut => sut.Dispose());
                    }
                }

                _disposed = true;
            }
            base.Dispose(disposing);
        }
    }
}
