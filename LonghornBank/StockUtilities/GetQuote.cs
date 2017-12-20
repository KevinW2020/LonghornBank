using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LonghornBank.Models;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

namespace LonghornBank.StockUtilities
{
    public static class GetQuote
    {
        // Get stock quote by passing symbol
        public static StockQuote GetStock(string symbol)
        {
            //s -> symbol
            //g -> days low
            //h -> days high
            //c -> change
            //o -> open
            //p -> previous close
            //v -> volume
            //l1 -> last trade (price only) -> close
            //n -> name
            string baseURL = "http://finance.yahoo.com/d/quotes.csv?s={0}&f=pl1vn";
            string url = string.Format(baseURL, symbol);

            //Get page showing the table with the chosen indices
            System.Net.HttpWebRequest request = null;

            //csv content
            string docText = string.Empty;
            StockQuote stock = null;
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                request.Timeout = 300000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (StreamReader stReader = new StreamReader(response.GetResponseStream()))
                {
                    string output = stReader.ReadLine();
                    //"\"Apple Inc.\",587.44,572.98,36820544"

                    string[] sa = output.Split(new char[] { ',' });

                    stock = new StockQuote();
                    stock.Symbol = symbol;
                    stock.Name = sa[3];
                    stock.PreviousClose = double.Parse(sa[0]);
                    stock.LastTradePrice = double.Parse(sa[1]);
                    stock.Volume = double.Parse(sa[2]);
                }
            }
            catch (Exception e)
            {
                //Throw some exception here.
            }
            return stock;
        }


        public static StockQuote GetStock(String symbol, DateTime date)
        {
            if (date == DateTime.Today)
            {
                return GetStock(symbol);
            }

            //http://finance.yahoo.com/q/hp?s=WU&a=01&b=19&c=2010&d=01&e=19&f=2010&g=d

            String month = (date.Month - 1).ToString();
            String day = date.Day.ToString();
            String year = date.Year.ToString();

            string baseURL = "http://ichart.finance.yahoo.com/table.csv?s={0}&a={1}&b={2}&c={3}&d={4}&e={5}&f={6}&g=d&ignore=.csv&h=npl1v";
            //http://ichart.finance.yahoo.com/table.csv?s=WU&a=01&b=19&c=2010&d=01&e=19&f=2010&g=d&ignore=.csv&h=pl1vn


            string url = string.Format(baseURL, symbol, month, day, year, month, day, year);

            //Get page showing the table with the chosen indices
            System.Net.HttpWebRequest request = null;

            Console.WriteLine(url);

            //csv content
            string docText = string.Empty;
            StockQuote stock = null;
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                request.Timeout = 300000;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (StreamReader stReader = new StreamReader(response.GetResponseStream()))
                {
                    string output = stReader.ReadLine();
                    //go to line with actual data
                    output = stReader.ReadLine();
                    //output = stReader.ReadLine();
                    //Date,Open,High,Low,Close,Volume,Adj Close
                    //2010 - 02 - 19,16.389999,16.459999,16.27,16.35,6963200,13.508243


                    string[] sa = output.Split(new char[] { ',' });

                    stock = new StockQuote();
                    stock.Symbol = symbol;
                    stock.Name = GetStock(symbol).Name;
                    stock.PreviousClose = double.Parse(sa[4]);
                    stock.LastTradePrice = double.Parse(sa[6]);
                    stock.Volume = double.Parse(sa[5]);
                }
            }
            catch (Exception e)
            {
                //Throw some exception here.
            }
            return stock;

        }

    }
}