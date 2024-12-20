using System.ComponentModel.DataAnnotations;

namespace AllupMVC.ViewModels
{
    public class LoginVM
    {
   

        [MaxLength(256)]
        public string UserNameorEmail {  get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool IsPersistent {  get; set; } 
    }
}
