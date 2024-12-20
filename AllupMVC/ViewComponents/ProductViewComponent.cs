using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Utilities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.ViewComponents
{
    public class ProductViewComponent : ViewComponent
    {
        private readonly AppDBContext _context;

        public ProductViewComponent(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(SortType type)
        {
            List<Product> product =null;
            switch (type)
            {
                case SortType.Time:
                    product = await _context.Products.OrderByDescending(p => p.DateTime).Take(6)
                       .Include(p => p.Images.Where(i => i.IsPrimary != null))
                       .ToListAsync();
                    break;
                case SortType.Price:
                    product = await _context.Products.OrderByDescending(p => p.Price).Take(6)
                        .Include(p => p.Images.Where(i => i.IsPrimary != null))
                        .ToListAsync();
                    break;
                case SortType.Name:
                    product = await _context.Products.OrderBy(p => p.Name).Take(6)
                      .Include(p => p.Images.Where(i => i.IsPrimary != null))
                      .ToListAsync();
                    break;

            }

            return View(product);   
        }
    }
}
