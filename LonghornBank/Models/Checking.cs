using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LonghornBank.Models
{
    public enum Enabled { Yes, No} 

    public class Checking
    {

        [Required(ErrorMessage = "Checking ID is required.")]
        [Display(Name = "Checking ID")]
        public Int32 CheckingID { get; set; }

        [Required(ErrorMessage = "Checking number is required.")]
        [Display(Name = "Checking Number")]
        public Int32 CheckingNumber { get; set; }

        [Required(ErrorMessage = "Checking Name is required.")]
        [Display(Name = "Checking Name")]
        public String CheckingName { get; set; }

        [Required(ErrorMessage = "Checking Balance is required.")]
        [Display(Name = "Checking Balance")]
        public Decimal CheckingBalance { get; set; }

        public Enabled Enableds { get; set; }

        public virtual AppUser AppUser { get; set; }
        public virtual List<Transfer> Transfers { get; set; }
        
    }
}