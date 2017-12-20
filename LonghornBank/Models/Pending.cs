using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LonghornBank.Models
{
    public enum PendingStatus { Submitted, Accepted, Denied }
    public class Pending
    {
        [Key]
        [Required(ErrorMessage = "Pending ID is required.")]
        [Display(Name = "Pending ID")]
        public Int32 PendingID { get; set; }

        [Required(ErrorMessage = "Pending Status is required.")]
        [Display(Name = "Pending Status")]
        public PendingStatus PendingStatus { get; set; }

        public String AccountType { get; set; }
        public Int32 AccountNumber { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Display(Name = "Amount in Question")]
        public Decimal Amount { get; set; }

        public virtual AppUser AppUser { get; set; }
    }
}