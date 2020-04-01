using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class Player : PlayerClient
    {
        public string Id { get; set; }

        private string Connection { get; set; }

        public Player(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
        }
        public Player()
        {

        }
        
    }
}
