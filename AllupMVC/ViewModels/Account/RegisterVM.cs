using System.ComponentModel.DataAnnotations;

namespace AllupMVC.ViewModels
{
    public class RegisterVM
    {
        [MinLength(3)]
        [MaxLength(20)]
        public string Name { get; set; }
        [MinLength(6)]
        [MaxLength(20)]
        public string Surname {  get; set; }
        [MinLength(3)]
        [MaxLength(30)]
        public string UserName {  get; set; }
        [MinLength(5)]
        [MaxLength(256)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare(nameof(Password))] 
        public string ConfirmPassword {  get; set; }
    }
}
