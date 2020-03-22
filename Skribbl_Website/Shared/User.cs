using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared
{
    public class User
    {
        public string Name { get; set; }
        public bool IsHost { get; set; }
        public string Id { get; set; }
        public bool IsConnected { get; set; }

        public User(string name)
        {
            Name = name;
            IsHost = false;
            IsConnected = false;
            Id = Guid.NewGuid().ToString();
        }
        public User()
        {

        }
    }
}
