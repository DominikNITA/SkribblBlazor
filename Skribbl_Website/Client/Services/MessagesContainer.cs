using Microsoft.JSInterop;
using Skribbl_Website.Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Skribbl_Website.Client.Services
{
    public class MessagesContainer
    {
        public IReadOnlyCollection<Message> Messages { get; private set; }

        private readonly IJSRuntime _jsRuntime;
        private List<Message> _messages= new List<Message>();

        public MessagesContainer(IJSRuntime jsRuntime)
        {
            _jsRuntime = jsRuntime;
            Messages = _messages.AsReadOnly();
        }

        public async void AddNewMessageAsync(Message message)
        {
            _messages.Add(message);
            _jsRuntime.InvokeVoidAsync("onMessageAdd");
        }

        public void ClearMessages()
        {
            _messages = new List<Message>();
        }
    }
}
