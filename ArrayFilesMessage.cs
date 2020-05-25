using MassTransit;

namespace MassTransitMessageDataTest
{
    public interface ArrayFilesMessage
    {
        string Name { get; set; }
        MessageData<byte[]>[] Files { get; set; }
    }
}