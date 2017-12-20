using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace LonghornBank.Models
{
    public enum PayeeType { CreditCard, Utilities, Rent, Mortgage, Other }

    public class Payee
    {
        [Required(ErrorMessage = "Payee ID is required.")]
        [Display(Name = "Payee ID")]
        public Int32 PayeeID { get; set; }

        //general
        [Required(ErrorMessage = "Payee Name is required.")]
        [Display(Name = "Payee Name")]
        public String Name { get; set; }

        //address
        [Required(ErrorMessage = "Street Address is required.")]
        [Display(Name = "Street Address")]
        public String StreetAddress { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [Display(Name = "City")]
        public String City { get; set; }

        [Required(ErrorMessage = "State is required.")]
        [Display(Name = "State")]
        public StateAbbr State { get; set; }

        [Required(ErrorMessage = "ZIP Code is required.")]
        [Display(Name = "ZIP Code")]
        public String ZIP { get; set; }

        //contact
        [Required(ErrorMessage = "Phone number is required.")]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        [DisplayFormat(DataFormatString = "{0:###-###-####}", ApplyFormatInEditMode = true)]
        public String PhoneNumber { get; set; }

        [Required(ErrorMessage = "Payee Type is required.")]
        [Display(Name = "Payee Type")]
        public PayeeType PayeeType { get; set; }

        public virtual List<Transaction> Transactions { get; set; }
        public virtual ICollection<AppUser> AppUsers { get; set; }

    }
}