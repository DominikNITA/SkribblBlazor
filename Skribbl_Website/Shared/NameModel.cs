using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Skribbl_Website.Shared
{
    public class NameModel
    {
        [Required]
        [StringLength(20, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }

        public NameModel()
        {

        }

        public NameModel(string name)
        {
            Name = name;
        }
    }
}
