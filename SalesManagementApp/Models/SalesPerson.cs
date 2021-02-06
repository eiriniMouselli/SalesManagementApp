using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SalesManagementApp.Models
{
    public class SalesPerson
    {
        
        public int Id { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Commission { get; set; }
        public ICollection<Sale> Sales{ get; set; }

    }
}
