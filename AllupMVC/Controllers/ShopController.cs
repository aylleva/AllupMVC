using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Utilities.Enums;
using AllupMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.CodeDom;

namespace AllupMVC.Controllers
{
    public class ShopController : Controller
    {
        private readonly AppDBContext _context;

        public ShopController(AppDBContext context)
        {
            _context = context;
        }
        public async Task<ActionResult> Index(int page=1,int key=1)
        {
            IQueryable<Product> query=_context.Products.Include(p=>p.Images.Where(i=>i.IsPrimary!=null));

            switch (key)
            {
                case (int)SortType.Name:
                    query=query.OrderBy(i=>i.Name);
                    break;
                case (int)SortType.Price:
                    query = query.OrderByDescending(i => i.Price);
                    break;
                case (int)SortType.Time:
                    query = query.OrderByDescending(i => i.DateTime);
                    break;
            }

            int count=query.Count();
            double total = Math.Ceiling((double)count/2);

            query = query.Skip((page - 1) * 2).Take(2);
            ShopVM shopvm = new ShopVM
            {
                TotalPage = total,
                CurrectPage = page,
                Key= key,
                ProductVM=await query.Select(q=> new GetProductVM
                {
                    Id = q.Id,
                    Name = q.Name,
                    Price = q.Price,
                    Image=q.Images.FirstOrDefault(q=>q.IsPrimary==true).Image,


                }).ToListAsync()
            };
            return View(shopvm);
        }

        public async Task<IActionResult> Detail(int? id)
        {
          
            if (id == null || id < 1) return BadRequest();

             Product? product=await _context.Products.Include(p=>p.Category)
             .Include(p=>p.Images)
             .Include(p=>p.Brand)
             .Include(p=>p.ProductTags)
             .ThenInclude(p=>p.Tag)
             .FirstOrDefaultAsync(p=>p.Id==id);

            if(product == null) return NotFound();

            DetailVM detailVM = new DetailVM 
            { 
              Product = product,
              RelatedProducts=await _context.Products.Where(p=>p.CategoryId==product.Id && p.Id!=id)
             .Include(p=>p.Images)
             .ToListAsync()
            };
            return View(detailVM);
        }
    }
}
