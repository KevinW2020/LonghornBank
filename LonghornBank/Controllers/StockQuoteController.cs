using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using LonghornBank.Models;
using LonghornBank.StockUtilities;

namespace LonghornBank.Controllers
{
    public class StockQuoteController : Controller
    {

        public static AppDbContext db = new AppDbContext();

        // GET: Home
        [Authorize]
        public ActionResult Index()
        {
            List<StockQuote> Quotes = new List<StockQuote>();

            var query2 = from s in db.StockLists
                         select s.Ticker;

            List<string> StockTickers = query2.ToList();

            foreach (string Ticker in StockTickers)
            {
                StockQuote sq = GetQuote.GetStock(Ticker);
                Quotes.Add(sq);
            }

            return View(Quotes);
        }

    }
}