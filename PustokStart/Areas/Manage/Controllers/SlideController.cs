using Microsoft.AspNetCore.Mvc;
using PustokStart.DAL;
using PustokStart.Helper.FileManager;
using PustokStart.Migrations;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Areas.Manage.Controllers
{
    [Area("manage")]
    public class SlideController : Controller
    {
        private readonly PustokContext _context;
        private readonly IWebHostEnvironment _env;

        public SlideController(PustokContext context,IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public IActionResult Index(int page=1)
        {

            var query =
                _context.Slides.OrderBy(x=>x.Order).AsQueryable();
            return View(PaginatedList<Slide>.Create(query, page, 2));

        }

        public IActionResult Create()
        {
            int orderCheck = _context.Slides.Max(x => x.Order)+1;
            ViewBag.OrderCheck = orderCheck;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Slide slide) 
        { 
            
            if(slide.ImageFile == null)
            {
                int orderCheck = _context.Slides.Max(x => x.Order) + 1;
                ViewBag.OrderCheck = orderCheck;
                ModelState.AddModelError("ImageFile", "Image is required");
            }
            if (!ModelState.IsValid)
            {
                int orderCheck = _context.Slides.Max(x => x.Order) + 1;
                ViewBag.OrderCheck = orderCheck;
                return View();
            }
            if (slide.Order >= _context.Slides.Max(x => x.Order) + 1)
            {
                int orderCheck = _context.Slides.Max(x => x.Order) + 1;
                ViewBag.OrderCheck = orderCheck;

                ModelState.AddModelError("Order", "Please write what is offered");
                return View();
            }
            if (slide.ImageFile.ContentType!="image/jpeg" &&slide.ImageFile.ContentType!="image/png")
            {
                int orderCheck = _context.Slides.Max(x => x.Order) + 1;
                ViewBag.OrderCheck = orderCheck;
                ModelState.AddModelError("ImageFile", "Content must be jpeg or png");
                return View();
            }
            if (slide.ImageFile.Length > 2097152)
            {
                int orderCheck = _context.Slides.Max(x => x.Order) + 1;
                ViewBag.OrderCheck = orderCheck;
                ModelState.AddModelError("ImageFile", "Size must be less than 2mb");
                return View();
            }
        
            foreach (var item in _context.Slides.Where(x=>x.Order>=slide.Order))
            {
                item.Order++;
            }
            //string path =_env.WebRootPath+"\\uploads\\sliders\\"+slide.ImageFile.FileName;
            //string path = Path.Combine(_env.WebRootPath, "uploads/sliders", slide.ImageFile.FileName);



            slide.ImageName = FileManager.Save(_env.WebRootPath, "uploads/sliders", slide.ImageFile);
            _context.Slides.Add(slide);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int id)
        {
            Slide slide = _context.Slides.Find(id);
            if(slide == null)
            {
                return View("Error");   
            }
            return View(slide);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Slide slide)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            string fileName = null;
            Slide existslide = _context.Slides.FirstOrDefault(x=>x.Id==slide.Id);
            if (existslide == null)
            {
                return View("Error");
            }
           
            if (slide.ImageFile != null)
            {
               
                if (slide.ImageFile.ContentType != "image/jpeg" && slide.ImageFile.ContentType != "image/png")
                {
                    ModelState.AddModelError("ImageFile", "Content must be jpeg or png");
                    return View();
                }
                if (slide.ImageFile.Length > 2097152)
                {
                    ModelState.AddModelError("ImageFile", "Size must be less than 2mb");
                    return View();
                }
                fileName = FileManager.Save(_env.WebRootPath, "uploads/sliders",slide.ImageFile);
         
            }
            existslide.Title1= slide.Title1;
            existslide.Title2= slide.Title2;
            existslide.Order=slide.Order;
            existslide.BtnText = slide.BtnText;
            existslide.BtnUrl = slide.BtnUrl;
            existslide.Desc=slide.Desc;
            existslide.ImageName = fileName??existslide.ImageName;

            _context.SaveChanges();
            return RedirectToAction("index");
        }
    }
}
