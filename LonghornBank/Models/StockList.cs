using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using LonghornBank.Models;

namespace LonghornBank.Models
{public enum Type { Ordinary, Index, ETF, Mutual, Futures }

    public class StockList
    {
        [Required(ErrorMessage = "Stock ID is required.")]
        [Display(Name = "Stock ID")]
        public Int32 StockListID { get; set; }

        [Required(ErrorMessage = "Stock Name is required.")]
        [Display(Name = "Stock Name")]
        public String Name { get; set; }

        [Required(ErrorMessage = "Stock Ticker is required.")]
        [Display(Name = "Stock Ticker")]
        public String Ticker { get; set; }

        [Required(ErrorMessage = "Stock Type is required.")]
        [Display(Name = "Stock Type")]
        public Type StockType { get; set; }

        [Display(Name = "Transaction Fee")]
        public Decimal TransactionFee { get; set; }

        public virtual List<StockBridge> StockBridges { get; set; }
    }
}