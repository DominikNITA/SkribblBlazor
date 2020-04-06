using System;

namespace Skribbl_Website.Shared.Dtos
{
    public class Player : PlayerClient
    {
        private string _connection;

        public string Id { get; set; }

        public string Connection
        {
            get => _connection; 
            set {
                IsConnected = true;
                _connection = value;
            }
        }

        public Player(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
        }
        public Player()
        {

        }

    }
}
