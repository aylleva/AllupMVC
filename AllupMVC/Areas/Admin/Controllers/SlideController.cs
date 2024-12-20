using AllupMVC.Areas.Admin.ViewModels;
using AllupMVC.DAL;
using AllupMVC.Models;
using AllupMVC.Utilities.Extentions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SlideController : Controller
    {
        private readonly AppDBContext _context;
        private readonly IWebHostEnvironment _env;
        public readonly string root = Path.Combine("assets", "images");
        public SlideController(AppDBContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IActionResult> Index()
        {
            List<Slide> slides=await _context.Slides.OrderBy(s=>s.Order).ToListAsync();
            return View(slides);
        }
         
        public IActionResult Create()
        {
            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Slide slide)
        {
            if (!slide.Photo.ValidateType("image/"))
            {
                ModelState.AddModelError(nameof(slide.Photo), "Wrong Image Type!!");
                return View();
            }
            if (!slide.Photo.ValidateSize(Utilities.Enums.Filesize.MG,2))
            {
                ModelState.AddModelError(nameof(slide.Photo), "Image length must exist max 2MB");
                return View();
            }

            slide.Image = await slide.Photo.CreateFile(_env.WebRootPath,root);

            
            await _context.Slides.AddAsync(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null || id < 1) return BadRequest();
            Slide slide = await _context.Slides.FirstOrDefaultAsync(s=>s.Id==id);
            if(slide==null) return NotFound();

            UpdateSlideVM slidevm = new()
            {
                Image = slide.Image,
                Description = slide.Description,
                Title = slide.Title,
                SubTitle = slide.SubTitle,
                Order = slide.Order,


            };
            return View(slidevm);
        }
        [HttpPost]
        public async Task<IActionResult> Update(UpdateSlideVM slidevm,int? id)
        {
            if (!ModelState.IsValid)
            {
                return View(slidevm);
            }

             Slide existed=await _context.Slides.FirstOrDefaultAsync(p=>p.Id==id);

            if(existed==null) return NotFound();

            if(slidevm.Photo is not null)
            {
                if (!slidevm.Photo.ValidateType("image/"))
                {
                    ModelState.AddModelError("Photo", "Wrong Type");
                    return View();
                }
                if (!slidevm.Photo.ValidateSize(Utilities.Enums.Filesize.MG, 2))
                {
                    ModelState.AddModelError("Photo", "Wrong Size");
                    return View();
                }
                string file = await slidevm.Photo.CreateFile(_env.WebRootPath, root);
                existed.Image.Delete(_env.WebRootPath, root);
                existed.Image = file;

            }


            existed.Order = slidevm.Order;
            existed.Title = slidevm.Title;
            existed.Description = slidevm.Description;
            existed.SubTitle= slidevm.SubTitle;
         
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Slide slide = await _context.Slides.FirstOrDefaultAsync(s => s.Id == id);
            if (slide == null) return NotFound();

            slide.Image.Delete(_env.WebRootPath, "assets", "images");

            _context.Slides.Remove(slide);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        
    }
}
