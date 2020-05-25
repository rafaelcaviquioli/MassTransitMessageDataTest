using System;
using MassTransit;

namespace MassTransitMessageDataTest
{
    public class MyCustomizedFile
    {
        public string Name { set; get; }
        public MessageData<string> Content { set; get; }
    }
    public interface MyEventMessage
    {
        string Name { get; set; }
        MyCustomizedFile MyFile { get; set; }
    }
}