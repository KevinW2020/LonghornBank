using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using LonghornBank.Models;
using System.ComponentModel;

namespace LonghornBank.Models
{
    public class IRA
    {

        [Required(ErrorMessage = "IRA ID is required.")]
        [Display(Name = "IRA ID")]
        public Int32 IRAID { get; set; }

        [Required(ErrorMessage = "IRA number is required.")]
        [Display(Name = "IRA Number")]
        public Int32 IRANumber { get; set; }

        [Required(ErrorMessage = "IRA Name is required.")]
        [Display(Name = "IRA Name")]
        public String IRAName { get; set; }

        [Required(ErrorMessage = "IRA Balance is required.")]
        [Display(Name = "IRA Balance")]
        public Decimal IRABalance { get; set; }

        [Display(Name = "Total Contribution")]
        public Decimal Contribution { get; set; }

        public Enabled Enableds { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual List<Transfer> Transfers { get; set; }

    }
}
