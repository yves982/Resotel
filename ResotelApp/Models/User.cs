using System.ComponentModel.DataAnnotations;

namespace ResotelApp.Models
{
    class User
    {
        public int Id { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Login { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Email { get; set; }

        public string Service { get; set; }

        public bool Manager { get; set; }

        [Required]
        public UserRights Rights { get; set; }
    }
}