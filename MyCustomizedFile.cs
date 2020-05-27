using MassTransit;

namespace MassTransitMessageDataTest
{
    public interface MyCustomizedFile
    {
        string Name { get; }
        MessageData<byte[]> Content { get; }
    }
}