using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PustokStart.DAL;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Areas.Manage.Controllers
{
    [Area("manage")]
    public class AuthorController : Controller
    {
        private readonly PustokContext _context;

        public AuthorController(PustokContext context)
        {
            _context = context;
        }
        public IActionResult Index(int page=1)
        {

            var query =
                  _context.Authors.Include(x => x.Books).AsQueryable();
            return View(PaginatedList<Author>.Create(query, page, 2));
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Author author) 
        {
            if (!ModelState.IsValid)
            {
                return View();

            }
            Author existAuthor = _context.Authors.Include(x=>x.Books).FirstOrDefault(x=>x.FullName==author.FullName);
            if (existAuthor != null)
            {
                ModelState.AddModelError("FullName", "Already used");
                return View(existAuthor);
            }
          
            _context.Authors.Add(author);
            _context.SaveChanges();
            return RedirectToAction("index");
            
        }
        public IActionResult Edit(int id)
        {
            Author author = _context.Authors.FirstOrDefault(x=>x.Id==id);
           
            return View(author);
        }
        [HttpPost]
        public IActionResult Edit(Author author)
        {
            if (!ModelState.IsValid)
            {
                return View(author);
            }
            Author existAuthor = _context.Authors.Find(author.Id);
            if (existAuthor == null)
            {
                return View("error");
            }
            if (author.FullName!=existAuthor.FullName && _context.Authors.Any(x => x.FullName == author.FullName)){
                ModelState.AddModelError("FullName", "Already used");
                return View(existAuthor);
            }
            existAuthor.FullName= author.FullName;
            _context.SaveChanges();
            return RedirectToAction("index");
        }
        public IActionResult Delete(int id)
        {
            Author existAuthor = _context.Authors.Find(id);
            if(existAuthor==null)
            {
                return StatusCode(404);
            }
            _context.Authors.Remove(existAuthor);
            _context.SaveChanges();
            return StatusCode(200);
        }
    }
}
