using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PustokStart.DAL;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Areas.Manage.Controllers
{
    [Area("manage")]
    public class BookController : Controller
    {
        private readonly PustokContext _context;

        public BookController(PustokContext context)
        {
            _context = context;
        }
        
        public IActionResult Index(int page=1,string search=null)
        {
            var query =
                _context.Books.
                Include(x => x.Author).
                Include(x => x.Genre).AsQueryable();
          if(search != null)
            {
                query=query.Where(x=>x.Name.Contains(search));  
            }

          
            ViewBag.Search = search;

               
            return View(PaginatedList<Book>.Create(query,page,3));
        }
    }
}
