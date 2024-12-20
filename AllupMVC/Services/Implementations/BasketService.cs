using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Services.Interfaces;
using AllupMVC.ViewModels;
using Azure.Core;
using Azure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace AllupMVC.Services.Implementations
{
    public class BasketService : IBasketService
    {
        private readonly AppDBContext _context;
        private readonly IHttpContextAccessor _http;
        private readonly ClaimsPrincipal _user;
        public BasketService(AppDBContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
            _user = http.HttpContext.User;
        }

        public async  Task<List<BasketitemVM>> GetBasketAsync()
        {
            List<BasketitemVM> basketvm = new();

            if (_user.Identity.IsAuthenticated)
            {
                basketvm = await _context.BasketItems.Where(b => b.UserId == _user.FindFirstValue(ClaimTypes.NameIdentifier))
                    .Select(b => new BasketitemVM
                    {
                        ProductId = b.ProductId,
                        Name = b.Product.Name,
                        Price = b.Product.Price,
                        Count = b.Count,
                        Image = b.Product.Images.FirstOrDefault(i => i.IsPrimary == true).Image,
                        SubTotal = b.Product.Price * b.Count,

                    }).ToListAsync();
            }

            else
            {
                List<BasketCookieItemVM> cookievm;

                string cookie =_http.HttpContext.Request.Cookies["basket"];

                if (cookie is null)
                {
                    return basketvm;
                }
                cookievm = JsonConvert.DeserializeObject<List<BasketCookieItemVM>>(cookie);
                foreach (var item in cookievm)
                {
                    Product product = await _context.Products.Include(p => p.Images.Where(i => i.IsPrimary == true))
                        .FirstOrDefaultAsync(p => p.Id == item.Id);

                    if (product is not null)
                    {
                        basketvm.Add(new BasketitemVM
                        {
                            ProductId = product.Id,
                            Name = product.Name,
                            Price = product.Price,
                            Count = item.Count,
                            Image = product.Images[0].Image,
                            SubTotal = product.Price * item.Count,

                        });

                    }
                }
               
            }
            return basketvm;
        }
    }
}
