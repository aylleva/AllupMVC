using AllupMVC.DAL;
using AllupMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AllupMVC.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly AppDBContext _context;

        public CategoryController(  AppDBContext context)
        {
           _context = context;
        }
        public async Task<IActionResult> Index()
        {
          List<Category> categories= await _context.Categories
                .Where(c=>c.IsDeleted!=true)
                .Include(c=>c.Products)

                .ToListAsync(); 
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Category category)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            bool result=await _context.Categories.AnyAsync(c=>c.Name == category.Name );

            if(result)
            {
                ModelState.AddModelError("Name", "This Category name is already exists!");
                return View();
            }


            category.DateTime= DateTime.Now;
            
            _context.Categories.Add(category);
            _context.SaveChanges();

            return RedirectToAction(nameof(Index));   
        }

    

        
        public async Task<IActionResult> Update(int? Id)
        {
            if (Id == null || Id<1) return BadRequest();

            Category category= await  _context.Categories.FirstOrDefaultAsync(c=>c.Id == Id);

            if(category == null) return NotFound();

            return View(category);
        }

        [HttpPost]

        public async Task<IActionResult> Update(Category category,int Id)
        {
            if (Id == null || Id < 1) return BadRequest();

            Category existed = _context.Categories.FirstOrDefault(c => c.Id == Id);

            if (category != null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View();
            }
            bool result = await _context.Categories.AnyAsync(c => c.Name == category.Name && c.Id!=Id );
            if (result)
            {
                ModelState.AddModelError("Name", "This Category Name is Already exists");
            }
            existed.Name = category.Name;

           
            await _context.SaveChangesAsync();  

            return RedirectToAction(nameof(Index));


        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

            category.IsDeleted= true;

         await _context.SaveChangesAsync();
          return RedirectToAction(nameof(Index));
        }


        public async  Task<IActionResult> Detail(int? id)
        {
            if (id == null || id < 1) return BadRequest();

            Category category = await _context.Categories.Include(c=>c.Products).ThenInclude(p=>p.Images).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();

          return View(category);    
        }
    
    }
}
