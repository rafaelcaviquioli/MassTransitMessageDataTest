
namespace MassTransitMessageDataTest
{

    public interface ArrayFilesMessage
    {
        string Name { get; }
        MyCustomizedFile[] Files { get; }
    }
}