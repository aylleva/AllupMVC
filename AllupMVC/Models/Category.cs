using AllupMVC.Models.Base;
using System.ComponentModel.DataAnnotations;

namespace AllupMVC.Models
{
    public class Category:BaseEntity
    {
        [MaxLength(20,ErrorMessage ="Category name must exists max 20 symbols!")]    
        public string Name {  get; set; }
        public List<Product>? Products { get; set; }
    }
}
