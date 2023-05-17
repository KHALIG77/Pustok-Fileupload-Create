
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PustokStart.DAL;
using PustokStart.Models;
using PustokStart.ViewModels;

namespace PustokStart.Controllers
{
    public class BookController:Controller
    {
        public readonly PustokContext _context;
        public BookController(PustokContext context)
        {
            _context= context;
        }
        public IActionResult GetBookDetail(int id)
        {
            Book book =_context.Books.Include(x=>x.Author).Include(x=>x.BookImages).Include(x=>x.Tags).ThenInclude(x=>x.Tag).FirstOrDefault(x=>x.Id==id);
            if (book == null) StatusCode(404);

          return PartialView("_BookModalPartial",book);
        }
        public IActionResult AddToBasket(int? id)
        {
          
           
            List<BasketItemCookieViewModel> cookieItems = new List<BasketItemCookieViewModel>();
            BasketItemCookieViewModel cookieitem;
            var basketStr = Request.Cookies["Basket"];

            if (basketStr != null)
            {
                cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);
                 cookieitem = cookieItems.FirstOrDefault(x=>x.BookId==id);
                if(cookieitem != null) 
                {
                    cookieitem.Count++;
                }
                else
                {
                    cookieitem = new BasketItemCookieViewModel { BookId = id, Count = 1 };
                    cookieItems.Add(cookieitem);
                    HttpContext.Response.Cookies.Append("Basket",JsonConvert.SerializeObject(cookieItems));

                    //return Json(new
                    //{
                    //    length=cookieItems.Count,
                    //});
                }
            }
            else
            {
                cookieitem = new BasketItemCookieViewModel {BookId=id,Count=1};
                cookieItems.Add(cookieitem);
                
            }

            
            HttpContext.Response.Cookies.Append("Basket",JsonConvert.SerializeObject(cookieItems) );

            BasketViewModel bv= new BasketViewModel();    
            foreach (var ci in cookieItems)
            {
                BasketItemViewModel bi = new BasketItemViewModel
                {
                    Count = (int)ci.Count,
                    Book=_context.Books.Include(x=>x.BookImages).FirstOrDefault(x=>x.Id==ci.BookId),

                };
                bv.BasketItems.Add(bi);
                bv.TotalPrice += (bi.Book.DiscountPerctent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPerctent) / 100) : bi.Book.SalePrice) * bi.Count;
            }

            return PartialView("_BasketCartPartialView", bv);
            //return RedirectToAction("index","home");
        }
        public IActionResult ShowBasket()
        {
            var basket = new List<BasketItemCookieViewModel>();
            var basketStr = HttpContext.Request.Cookies["Basket"];
            if (basket!=null)
            {

                basket = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);
            }

           
            return Json(new { basket });
        }

        public IActionResult RemoveBasket(int id)
        {
            List<BasketItemCookieViewModel> cookieItems = new List<BasketItemCookieViewModel>();
            var basketStr = HttpContext.Request.Cookies["basket"];
            if (basketStr != null)
            {
                cookieItems = JsonConvert.DeserializeObject<List<BasketItemCookieViewModel>>(basketStr);
                var item = cookieItems.FirstOrDefault(x => x.BookId == id);
                BasketViewModel bv = new BasketViewModel();
                if (item != null)
                {
                    cookieItems.Remove(item);
                    Response.Cookies.Append("basket", JsonConvert.SerializeObject(cookieItems));

                    foreach (var ci in cookieItems)
                    {
                        BasketItemViewModel bi = new BasketItemViewModel
                        {
                            Count = (int)ci.Count,
                            Book = _context.Books.Include(x => x.BookImages).FirstOrDefault(x => x.Id == ci.BookId),

                        };
                        bv.BasketItems.Add(bi);
                        bv.TotalPrice += (bi.Book.DiscountPerctent > 0 ? (bi.Book.SalePrice * (100 - bi.Book.DiscountPerctent) / 100) : bi.Book.SalePrice) * bi.Count;
                    }
                }
                return PartialView("_BasketCartPartialView", bv);


            }
            else
            {
                return NotFound();
            }



        }


    }
}
