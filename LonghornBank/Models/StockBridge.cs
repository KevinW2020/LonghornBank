using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LonghornBank.Models
{
    public class StockBridge
    {
        [Required(ErrorMessage = "Stock Bridge ID is required.")]
        [Display(Name = "Stock BridgeID")]
        public Int32 StockBridgeID { get; set; }


        [Required(ErrorMessage = "Number of Shares is required.")]
        [Display(Name = "Number of Shares")]
        public int? NumberShares { get; set; }

        [Required(ErrorMessage = "Purchase Price")]
        [Display(Name = "Original Purchase Price")]
        public Decimal OriginalPrice { get; set; }

        [Required(ErrorMessage ="Enter date")]
        [Display(Name = "Date")]
        public DateTime PurchaseDate { get; set; }
        public virtual StockPortfolio StockPortfolio { get; set; }
        public virtual StockList StockList { get; set; }

    }
}