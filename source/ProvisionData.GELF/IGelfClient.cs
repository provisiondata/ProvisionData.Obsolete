namespace ProvisionData.GELF
{
    using System;
    using System.Threading.Tasks;

    public interface IGelfClient : IDisposable
    {
        Task SendMessageAsync(Message message);
    }
}