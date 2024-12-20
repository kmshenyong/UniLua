using GrinderApp.Services.Interfaces;

namespace GrinderApp.Services
{
    public class MessageService : IMessageService
    {
        public string GetMessage()
        {
            return "Hello from the Message Service";
        }
    }
}
