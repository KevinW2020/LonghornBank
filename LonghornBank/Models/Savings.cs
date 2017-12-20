using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LonghornBank.Models
{

    public class Savings
    {

        [Required(ErrorMessage = "Saving ID is required.")]
        [Display(Name = "Saving ID")]
        public Int32 SavingsID { get; set; }

        [Required(ErrorMessage = "Saving number is required.")]
        [Display(Name = "Saving Number")]
        public Int32 SavingsNumber { get; set; }

        [Required(ErrorMessage = "Saving Name is required.")]
        [Display(Name = "Saving Name")]
        public String SavingsName { get; set; }

        [Required(ErrorMessage = "Saving Balance is required.")]
        [Display(Name = "Saving Balance")]
        public Decimal SavingsBalance { get; set; }

        public Enabled Enableds { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual List<Transfer> Transfers { get; set; }

    }
}