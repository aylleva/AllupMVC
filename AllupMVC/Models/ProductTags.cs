namespace AllupMVC.Models
{
    public class ProductTags
    {
        public int Id { get; set; }
        public int TagId {  get; set; }
        public int ProductId {  get; set; } 
        public Tag Tag { get; set; }
        public Product Product { get; set; }
    }
}
