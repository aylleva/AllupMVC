using AllupMVC.Models;
using System.Text;

namespace AllupMVC.Areas.Admin.ViewModels
{
    public class GetProductVM
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string ProductCode { get; set; }
      

       public string CategoryName {  get; set; }
        public string BrandName {  get; set; }
       public string Image {  get; set; }   

    }
}
