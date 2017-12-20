using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LonghornBank.Models
{
    public enum TransactionType { Deposit, Withdrawal, Transfer, Fee, BillPay, Bonus }
    public enum TransactionStatus { Approved, Pending }

    public class Transaction
    {
        [Required(ErrorMessage = "Transaction ID is required.")]
        [Display(Name = "Transaction ID")]
        public Int32 TransactionID { get; set; }

        [Required(ErrorMessage = "Transaction Type is required.")]
        [Display(Name = "Transaction Type")]
        public TransactionType TypeOfTransaction { get; set; }

        [Required(ErrorMessage = "Transaction Status is required.")]
        [Display(Name = "Transaction Status")]
        public TransactionStatus ApprovalStatus { get; set; }

        [Required(ErrorMessage = "Transaction Date is required.")]
        [Display(Name = "Transaction Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }

        [Required(ErrorMessage = "Transaction Amount is required.")]
        [Display(Name = "Transaction Amount")]
        public Decimal TransactionAmount { get; set; }

        [Required(ErrorMessage = "Transaction Description is required.")]
        [Display(Name = "Transaction Description")]
        public String TransactionDescription { get; set; }

        [Display(Name = "Transaction Comments")]
        public String TransactionComments { get; set; }

        [Display(Name = "Manager Comments")]
        public String ManagerComments { get; set; }

        [Display(Name = "Manager Email")]
        public String ManagerEmail { get; set; }

        [Display(Name = "Account Sender")]
        public Int32 AccountSender { get; set; }

        [Display(Name = "Account Receiver")]
        public Int32 AccountReceiver { get; set; }

        [Display(Name = "Account Sender ID")]
        public Int32 AccountSenderID { get; set; }

        [Display(Name = "Account Receiver ID")]
        public Int32 AccountReceiverID { get; set; }

        public virtual Dispute Disputes { get; set; }

        public virtual Payee Payee { get; set; }
        public virtual AppUser AppUser { get; set; }

    }
}