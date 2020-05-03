using System.ComponentModel.DataAnnotations;

namespace Skribbl_Website.Shared
{
    public class NameModel
    {
        public NameModel()
        {
        }

        public NameModel(string name)
        {
            Name = name;
        }

        [Required]
        [StringLength(20, ErrorMessage = "Name is too long.")]
        public string Name { get; set; }
    }
}