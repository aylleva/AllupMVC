using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Permissions;

namespace AllupMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDBContext _context;

        public HomeController(AppDBContext context)
        {
             _context = context;
        }

      
        public IActionResult Index(string? search)
        {
            //IQueryable<Product> query = _context.Products;

            //if (!string.IsNullOrEmpty(search))
            //{
            //    query = query.Where(q => q.Name.ToLower().Contains(search.ToLower()));
            //}
            HomeVM homevm = new HomeVM
            {
                Slides =_context.Slides.OrderByDescending(s=>s.Order).ToList(),
                Products=_context.Products.Include(p=>p.Images).ToList(),
                //Search = search
                
            };
            return View(homevm);
        }
    }
}
