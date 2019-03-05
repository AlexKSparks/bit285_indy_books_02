using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IndyBooks.Models;
using IndyBooks.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IndyBooks.Controllers
{
    public class AdminController : Controller
    {
        private IndyBooksDataContext _db;
        public AdminController(IndyBooksDataContext db) { _db = db; }

        public IActionResult Search()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Search(SearchViewModel search)
        {
            //Full Collection Search
            IQueryable<Book> foundBooks = _db.Books; // start with entire collection

            //Partial Title Search
            if (search.Title != null)
            {
                foundBooks = foundBooks
                                .Include(b => b.Author)
                             .Where(b => b.Title.Contains(search.Title))
                             .OrderBy(b => b.Title)
                             ;
            }
            //full Collection search
            if (search.Title == null && search.AuthorLastName == null && search.MaxPrice == 0 && search.MinPrice == 0)
            {
                foundBooks = foundBooks
                            .Include(b => b.Author);
            }
            //Author's Last Name Search
            if (search.AuthorLastName != null)
            {
                //TODO:Update to use the Name property of the Book's Author entity
                foundBooks = foundBooks
                              .Include(b => b.Author)
                             .Where(b => b.Author.Name.EndsWith(search.AuthorLastName, StringComparison.CurrentCulture))
                             ;
            }
            //Priced Between Search (min and max price entered)
            if (search.MinPrice > 0 && search.MaxPrice > 0)
            {
                foundBooks = foundBooks
                               .Include(b => b.Author)              
                             .Where(b => b.Price >= search.MinPrice && b.Price <= search.MaxPrice)
                             .OrderByDescending(b => b.Price)
                             ;
            }
            //Highest Priced Book Search (only max price entered)
            if (search.MinPrice == 0 && search.MaxPrice > 0)
            {
                decimal max = _db.Books.Max(b => b.Price);
                foundBooks = foundBooks
                    .Include(b => b.Author)
                                .Where(b => b.Price == max);
                             ;

            }

            //Composite Search Results
            return View("SearchResults", foundBooks);
        }
    }
}
