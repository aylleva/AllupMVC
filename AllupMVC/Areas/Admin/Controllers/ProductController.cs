using AllupMVC.Areas.Admin.ViewModels;

using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Drawing;

namespace AllupMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;
        public readonly string root = Path.Combine("assets", "images");
        public ProductController(AppDBContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }



        public async Task<IActionResult> Index()
        {
            List<GetProductVM> productvm = await _context.Products
                .Where(p => p.IsDeleted == false)
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.Images.Where(i => i.IsPrimary == true))
                 .Select( p => new GetProductVM
                 {
                     Id = p.Id,
                     Name = p.Name,
                     Price = p.Price,
                     ProductCode = p.ProdcutCode,
                     CategoryName = p.Category.Name,
                     BrandName = p.Brand.Name,
                     Image = p.Images.FirstOrDefault(i=>i.IsPrimary==true).Image
                 }

                 ).ToListAsync();


            return View(productvm);
        }

         public async Task<IActionResult> Create()
        {
            CreateProductVM productvm = new CreateProductVM 
            { 
              Categories=await _context.Categories.ToListAsync(),
              Brands=await _context.Brands.ToListAsync(),
              Tags=await _context.Tags.ToListAsync(),

              
            };
            return View(productvm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateProductVM productvm)
        {
            productvm.Categories = await _context.Categories.ToListAsync();
            productvm.Brands = await _context.Brands.ToListAsync();
            productvm.Tags = await _context.Tags.ToListAsync();

            if (!ModelState.IsValid)
            {
                return View(productvm);
            }

            if (!productvm.MainPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(productvm.MainPhoto), "Wrong Type!");
                return View(productvm);
            }
            if (!productvm.MainPhoto.ValidateSize(Utilities.Enums.Filesize.MG, 2))
            {
                ModelState.AddModelError(nameof(productvm.MainPhoto), "Image size must contains max 2 MG");
                return View(productvm);
            }


            if (!productvm.HoverPhoto.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(productvm.HoverPhoto), "Wrong Type!");
                return View(productvm);
            }
            if (!productvm.HoverPhoto.ValidateSize(Utilities.Enums.Filesize.MG, 2))
            {
                ModelState.AddModelError(nameof(productvm.HoverPhoto), "Image size must contains max 2 MG");
                return View(productvm);
            }


            bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
            if (!result)
            {
                ModelState.AddModelError(nameof(productvm.CategoryId), "Wrong Category!");
                return View(productvm);
            }
            if(productvm.TagIds is not null)
            {
                bool tagresult = productvm.TagIds.Any(ti => !productvm.Tags.Exists(t => t.Id == ti));
                if(tagresult)
                {
                    ModelState.AddModelError(nameof(CreateProductVM.TagIds), "This Tag does not exist!");
                    return View(productvm);
                }
            }
            ProductImage main = new()
            {
                Image = await productvm.MainPhoto.CreateFile(_env.WebRootPath, root),
                IsDeleted = false,
                DateTime = DateTime.Now,
                IsPrimary = true

            };
            ProductImage hover = new()
            {
                Image = await productvm.HoverPhoto.CreateFile(_env.WebRootPath, root),
                IsDeleted = false,
                DateTime = DateTime.Now,
                IsPrimary = false

            };

            Product product = new()
            {
                Name = productvm.Name,
                DateTime = DateTime.Now,
                IsDeleted = false,
                Price = productvm.Price.Value,
                Description = productvm.Description,
                ProdcutCode = productvm.ProdcutCode,
                Avabialability = productvm.Avabialability,
                CategoryId = productvm.CategoryId.Value,
                BrandId = productvm.BrandId.Value,
                
                Images = new List<ProductImage> { main, hover }

            };
            if(productvm.TagIds is not null)
            {
               product.ProductTags=productvm.TagIds.Select(ti=> new ProductTags { TagId=ti}).ToList();
            }

            if (productvm.AdditionalPhotos is not null)
            {
                string text = string.Empty;

                foreach (var file in productvm.AdditionalPhotos)
                {
                    if (!file.ValidateType("image/"))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} type is not correct!!\r\n</div>";
                        continue;
                    }
                    if (!file.ValidateSize(Utilities.Enums.Filesize.MG, 2))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {file.FileName} size is not correct!!\r\n</div>";
                        continue;
                    }

                    product.Images.Add(new ProductImage
                    {
                        Image = await file.CreateFile(_env.WebRootPath, root),
                        IsDeleted = false,
                        DateTime = DateTime.Now,
                        IsPrimary = null

                    });

                    TempData["FileErrors"] = text;
                }
            }
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
             
            return RedirectToAction(nameof(Index));
        }

        
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Product product= await _context.Products.Include(p=>p.ProductTags).Include(p=>p.Images).Include(p=>p.ProductTags).FirstOrDefaultAsync(p=>p.Id == id);
            if(product is null) return NotFound();

            UpdateProductVM productvm = new()
            {
             Name = product.Name,
             Price = product.Price,
             ProdcutCode = product.ProdcutCode,
             Avabialability = product.Avabialability,
             Description = product.Description,
             
             CategoryId = product.CategoryId,
             Categories=await _context.Categories.ToListAsync(),

             BrandId = product.BrandId,
             Brands=await _context.Brands.ToListAsync(),

             Tags=await _context.Tags.ToListAsync(),
             TagIds=product.ProductTags.Select(p=>p.TagId).ToList(),

             ProductImages=product.Images,
            };
            return View(productvm);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UpdateProductVM productvm, int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Product existed = await _context.Products.Include(p=>p.ProductTags).Include(p => p.Images).Include(p=>p.ProductTags).FirstOrDefaultAsync(p => p.Id == id);
            if (existed is null) return NotFound();

            productvm.Categories = await _context.Categories.ToListAsync();
            productvm.Brands = await _context.Brands.ToListAsync();
            productvm.Tags = await _context.Tags.ToListAsync();
            productvm.ProductImages=existed.Images;


            if (!ModelState.IsValid)
            {
                return View(productvm);
            }

            if (productvm.MainPhoto is not null)
            {
                if (!productvm.MainPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "Wrong Format!");
                    return View(productvm);
                }
                if (!productvm.MainPhoto.ValidateSize(Utilities.Enums.Filesize.MG, 2))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.MainPhoto), "Wrong Size!");
                    return View(productvm);
                }
            }
            if (productvm.HoverPhoto is not null)
            {

                if (!productvm.HoverPhoto.ValidateType("image/"))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.HoverPhoto), "Wrong Format!");
                    return View(productvm);
                }
                if (!productvm.HoverPhoto.ValidateSize(Utilities.Enums.Filesize.MG, 2))
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.HoverPhoto), "Wrong Size!");
                    return View(productvm);
                }

            }

            if (existed.CategoryId != productvm.CategoryId)
            {
                bool result = await _context.Categories.AnyAsync(c => c.Id == productvm.CategoryId);
                if (!result)
                {
                    ModelState.AddModelError(nameof(Category.Id), "This category does not exist!");
                    return View(productvm);
                }
            }

            if (existed.BrandId != productvm.BrandId)
            {
                bool result = await _context.Brands.AnyAsync(c => c.Id == productvm.BrandId);
                if (!result)
                {
                    ModelState.AddModelError(nameof(Brand.Id), "This brand does not exist!");
                    return View(productvm);
                }
            }

            if(productvm.TagIds is not null)
            {
                bool tagresult = productvm.TagIds.Any(ti =>! productvm.Tags.Exists(t => t.Id == ti));
                if (tagresult)
                {
                    ModelState.AddModelError(nameof(UpdateProductVM.TagIds), "This Tag Does not Exist");
                    return View(productvm);
                }

            }


            if(productvm.TagIds is null)
            {
                productvm.TagIds = new();
            }
            else
            {
                productvm.TagIds = productvm.TagIds.Distinct().ToList();
            }

                _context.ProductTags.RemoveRange(existed.ProductTags
                .Where(pt=>!productvm.TagIds
                .Exists(t=>t==pt.TagId)).ToList());

               _context.ProductTags.AddRange(productvm.TagIds
                   .Where(ti=>!existed.ProductTags
                   .Exists(pt=>pt.TagId==ti))
                   .Select(ti=>new ProductTags { TagId=ti,ProductId=existed.Id})
                   .ToList());

            if (productvm.MainPhoto is not null)
            {
                string file = await productvm.MainPhoto.CreateFile(_env.WebRootPath, root);
                ProductImage main =  existed.Images.FirstOrDefault(i => i.IsPrimary == true);
                main.Image.Delete(_env.WebRootPath, root);
                existed.Images.Remove(main);

                existed.Images.Add(new ProductImage
                {
                    Image = file,
                    IsDeleted = false,
                    IsPrimary = true,
                    DateTime = DateTime.Now,

                });
            }

            if (productvm.HoverPhoto is not null)
            {
                string file = await productvm.HoverPhoto.CreateFile(_env.WebRootPath, root);
                ProductImage hover =existed.Images.FirstOrDefault(i => i.IsPrimary == false);
                hover.Image.Delete(_env.WebRootPath, root);
                existed.Images.Remove(hover);

                existed.Images.Add(new ProductImage
                {
                    Image = file,
                    IsDeleted = false,
                    IsPrimary = false,
                    DateTime = DateTime.Now,
                });
            }

            if(productvm.ImageIds is null)
            {
                productvm.ImageIds = new List<int>(); 
            }
            var deletedfiles=existed.Images.Where(i=>!productvm.ImageIds.Exists(id=>id==i.Id) && i.IsPrimary==null).ToList();
            deletedfiles.ForEach(di => di.Image.Delete(_env.WebRootPath,root));

            _context.Images.RemoveRange(deletedfiles);

            if (productvm.AdditionalPhotos is not null)
            {
                String text=string.Empty;
                foreach(var image in  productvm.AdditionalPhotos)
                {
                    if (!image.ValidateType("image/"))
                    { 
                        text+= $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {image.FileName} Type is not correct!!\r\n</div>";
                        continue ;
                    }
                    if (!image.ValidateSize(Utilities.Enums.Filesize.MG, 2))
                    {
                        text += $"<div class=\"alert alert-warning\" role=\"alert\">\r\n {image.FileName} Size is not correct!!\r\n</div>";
                        continue ;
                    }

                    existed.Images.Add(new ProductImage {
                    Image=await image.CreateFile(_env.WebRootPath,root),
                    IsDeleted = false,
                    IsPrimary = null,
                    DateTime = DateTime.Now,
                    });
                  
                }
                TempData["ErrorMessage"] = text;
            }
            existed.Name = productvm.Name;
            existed.Description = productvm.Description;
            existed.Price=productvm.Price.Value;
            existed.Avabialability = productvm.Avabialability;
            existed.ProdcutCode = productvm.ProdcutCode;
            existed.CategoryId=productvm.CategoryId.Value;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Product product=await _context.Products.FirstOrDefaultAsync(p=> p.Id == id);

            if (product == null) return NotFound();

            product.IsDeleted = true;
           _context.Products.Remove(product);
            await _context.SaveChangesAsync();
           return RedirectToAction(nameof(Index));
            
        }

    }  
}
