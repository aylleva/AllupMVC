using AllupMVC.Models.Base;

namespace AllupMVC.Models
{
    public class Tag:BaseEntity
    {
        public string Name {  get; set; }
       public List<ProductTags> ProductTags { get; set; }
    }
}
