using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LonghornBank.Models
{
    public enum DisputeStatus { Submitted, Accepted, Rejected, Adjusted }

    public class Dispute
    {
        [Key]
        [Required(ErrorMessage = "Dispute ID is required.")]
        [Display(Name = "Dispute ID")]
        public Int32 DisputeID { get; set; }

        [Required(ErrorMessage = "Dispute Status is required.")]
        [Display(Name = "Dispute Status")]
        public DisputeStatus DisputeStatus { get; set; }

        [Required(ErrorMessage = "Comments are required.")]
        [Display(Name = "Comments")]
        public String Comments { get; set; }

        [Display(Name = "Manager Comments")]
        public String ManagerComments { get; set; }

        [Required(ErrorMessage = "Correct Amount is required.")]
        [Display(Name = "Correct Amount")]
        public Decimal CorrectAmount { get; set; }

        // NOT REQUIRED
        [Required(ErrorMessage = "Delete?")]
        [Display(Name = "Delete Transaction?")]
        public Boolean DeleteTransaction { get; set; }

        public virtual Transaction Transaction { get; set; }
    }
}