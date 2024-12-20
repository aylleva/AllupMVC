using AllupMVC.Models.Base;

namespace AllupMVC.Models
{
    public class Product:BaseEntity
    {
        public string Name {  get; set; }
        public decimal Price { get; set; }
        public string ProdcutCode {  get; set; }
        public string Avabialability {  get; set; }
        public string Description {  get; set; }

        public int CategoryId {  get; set; }
        public int BrandId {  get; set; }
       public List<ProductImage> Images { get; set; }
        public Category Category { get; set; }
        public Brand Brand { get; set; }

        public List<ProductTags> ProductTags { get; set; }



    }
}
