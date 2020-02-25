namespace ProvisionData.GELF
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IGelfClient : IDisposable
    {
        Task SendMessageAsync(Message message, CancellationToken cancellationToken = default);
    }
}