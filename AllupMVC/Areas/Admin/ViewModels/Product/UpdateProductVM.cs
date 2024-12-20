using AllupMVC.Models;
using System.ComponentModel.DataAnnotations;

namespace AllupMVC.Areas.Admin.ViewModels
{
    public class UpdateProductVM
    {
        public string Name { get; set; }

        [Required]
        public decimal? Price { get; set; }
        public string ProdcutCode { get; set; }
        public string Avabialability { get; set; }
        public string Description { get; set; }

        [Required]
        public int? CategoryId { get; set; }

        public int? BrandId { get; set; }

        public List<int>? TagIds { get; set; }  
        public List<Tag>? Tags { get; set; }
        public List<Category>? Categories { get; set; }
        public List<Brand>? Brands { get; set; }
        public IFormFile? MainPhoto { get; set; }
        public IFormFile? HoverPhoto { get; set; }
        public List<IFormFile>? AdditionalPhotos { get; set; }
        public List<ProductImage>? ProductImages { get; set; }

        public List<int>? ImageIds { get; set; }
    }
}
