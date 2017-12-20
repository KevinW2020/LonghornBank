using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNet.Identity;


namespace LonghornBank.Models
{
    public class StockViewModel
    {
        [Required]
        [Display(Name = "Stock Name")]
        public string StockName { get; set; }

        [Required]
        [Display(Name = "Symbol")]
        public string Symbol { get; set; }

        [Required]
        [Display(Name = "Purchase Price")]
        public decimal PurchasePrice { get; set; }

        [Required]
        [Display(Name = "Current Price")]
        public decimal CurrentPrice { get; set; }

        [Required]
        [Display(Name = "Number of Shares")]
        public Int32 NumShares { get; set; }

        [Required]
        [Display(Name = "Gain/Loss")]
        public decimal Difference { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime PurchaseDate { get; set; }
        [Required]
        [Display(Name = "Total change")]
        public decimal TotalChange { get; set; }
        [Required]
        [Display(Name = "StockBridgeID")]
        public string StockBridgeID { get; set; }
    }
}