using MassTransit;

namespace MassTransitMessageDataTest
{
    public interface MyCustomizedFile
    {
        string Name { set; get; }
        MessageData<byte[]> Content { set; get; }
    }
    public interface MyEventMessage
    {
        string Name { get; set; }
        MyCustomizedFile MyFile { get; set; }
    }
}