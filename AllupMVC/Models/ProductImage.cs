using AllupMVC.Models.Base;

namespace AllupMVC.Models
{
    public class ProductImage:BaseEntity
    {
        public string Image {  get; set; }
        public bool? IsPrimary {  get; set; }
        public Product Product { get; set; }
        public int ProductId {  get; set; }
    }
}
