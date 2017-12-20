using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LonghornBank.Models
{

    public class StockPortfolio
    {

        [Required(ErrorMessage = "Stock Portfolio ID is required.")]
        [Display(Name = "Stock Portfolio ID")]
        public Int32 StockPortfolioID { get; set; }

        [Required(ErrorMessage = "Stock Portfolio number is required.")]
        [Display(Name = "Stock Portfolio Number")]
        public Int32 StockPortfolioNumber { get; set; }

        [Required(ErrorMessage = "Stock Portfolio Name is required.")]
        [Display(Name = "Stock Portfolio Name")]
        public String StockPortfolioName { get; set; }

        [Required(ErrorMessage = "Cash Balance is required.")]
        [Display(Name = "Cash Balance")]
        public decimal? CashBalance { get; set; }

        [Required(ErrorMessage = "Stock Balance is required.")]
        [Display(Name = "Stock Balance")]
        public decimal? StockBalance { get; set; }

        [Required(ErrorMessage = "Total Fees is required.")]
        [Display(Name = "Total Fees")]
        public decimal TotalFees { get; set; }
        [Display(Name = "Balanced/Unbalanced")]
        public string BalanceStatus { get; set; }

        [Required]
        [Display (Name = "Can buy stocks")]
        public bool CanBuy { get; set; }

        public Enabled Enableds { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual List<StockBridge> StockBridges { get; set; }
        public virtual List<Transfer> Transfers { get; set; }

    }
}   