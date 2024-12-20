
using AllupMVC.Areas.Admin.ViewModels;
using AllupMVC.DAL;
using AllupMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TagController : Controller
    {

        private readonly AppDBContext _context;

        public TagController(AppDBContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<GetTagVM> tagvm = await _context.Tags
            .Include(t => t.ProductTags)
            .ThenInclude(pt => pt.Product)
            .Select(t => new GetTagVM
            {
                Id = t.Id,
                Name = t.Name,
                ProductCount = t.ProductTags.Count,
            }).ToListAsync();

            return View(tagvm);
        }
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateTagVM tagvm)
        {
            if (!ModelState.IsValid)
            {
                return View(tagvm);
            }

            bool result = await _context.Tags.AnyAsync(t => t.Name == tagvm.Name);
            if (result)
            {
                ModelState.AddModelError(nameof(tagvm.Name), "This Tag is already exist!");
                return View(tagvm);
            }

            Tag tag = new()
            {
                Name = tagvm.Name,
                IsDeleted = false,
                DateTime = DateTime.Now,
            };
            await _context.Tags.AddAsync(tag);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Tag tag = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (tag == null) return NotFound();

            UpdateTagVM tagvm = new()
            {
                Name = tag.Name,

            };
            return View(tagvm);
        }

        [HttpPost]

        public async Task<IActionResult> Update(UpdateTagVM tagvm, int? id)
        {
            if (id is null || id < 1) return BadRequest();

            Tag existed = await _context.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existed == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(tagvm);
            }

            bool result = await _context.Tags.AnyAsync(t => t.Name.Trim() == tagvm.Name.Trim() && t.Id != id);
            if (result)
            {
                ModelState.AddModelError(nameof(UpdateTagVM.Name), "This Tag is already exist");
                return View(tagvm);
            }
            existed.Name = tagvm.Name;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Tag tag = await _context.Tags.FirstOrDefaultAsync( tag => tag.Id == id);
            if (tag == null) return NotFound();

            tag.IsDeleted = true;
             _context.Tags.Remove(tag);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
