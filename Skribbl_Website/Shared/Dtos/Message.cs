using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class Message
    {
        public enum MessageType { Guess, CloseGuess, Guessed, Join, Host, Disconnect, Ban, Leave, Server }
        public MessageType Type { get; set; }
        public string Sender { get; set; }
        public string MessageContent { get; set; }

        public Message(string messageContent, MessageType messageType = MessageType.Guess, string sender = "")
        {
            MessageContent = messageContent;
            Type = messageType;
            Sender = sender;
        }
        public Message()
        {

        }
    }
}
