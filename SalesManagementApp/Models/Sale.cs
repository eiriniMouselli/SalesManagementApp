using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SalesManagementApp.Models
{
    public class Sale
    {
        public int SaleId { get; set; }
       
        [Display(Name = "Salesperson Full Name")]
        public string SalesPersonFullName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; }

        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [ForeignKey("SalesPersonFullName")]
        public virtual SalesPerson SalesEmployee { get; set; }
    }
}
