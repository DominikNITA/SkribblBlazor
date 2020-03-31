using System;
using System.Collections.Generic;
using System.Text;

namespace Skribbl_Website.Shared.Dtos
{
    public class UserDto : PlayerClient
    {
        public string Id { get; set; }

        public UserDto(string name) : base(name)
        {
            Id = Guid.NewGuid().ToString();
        }
        public UserDto()
        {

        }
        
    }
}
