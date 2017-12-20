using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LonghornBank.Models
{
    public class Transfer
    {

        [Required(ErrorMessage = "Transfer ID is required.")]
        [Display(Name = "Transfer ID")]
        public Int32 TransferID { get; set; }

        [Required(ErrorMessage = "Tranfer Date is required.")]
        [Display(Name = "Transfer Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransferDate { get; set; }

        [Required(ErrorMessage = "Transfer Amount is required.")]
        [Display(Name = "Transfer Amount")]
        public Decimal TransferAmount { get; set; }

        [Required(ErrorMessage = "Transfer Description is required.")]
        [Display(Name = "Transfer Description")]
        public String TransferDescription { get; set; }

        [Display(Name = "Transfer Comments")]
        public String TransferComments { get; set; }

        public virtual List<Checking> Checkings { get; set; }
        public virtual List<Savings> Savings { get; set; }
        public virtual IRA IRA { get; set; }
        public virtual StockPortfolio StockPortfolio { get; set; }
    }
}