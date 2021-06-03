using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace PoWebApi.Models
{
    public class PurchaseOrder
    {
        public static string StatusNew { get; set; } = "New";
        public static string StatusEdit { get; set; } = "Edit";
        public static string StatusReview { get; set; } = "Review";
        public static string StatusApproved { get; set; } = "Approved";
        public static string StatusRejected { get; set; } = "Rejected";

        public int Id { get; set; }
        [Required, StringLength(80)]
        public string Description { get; set; }
        [Required, StringLength(20)]
        public string Status { get; set; } = PurchaseOrder.StatusNew;
        [Column(TypeName = "decimal(9,2)")]
        public decimal Total { get; set; } = 0;
        public bool Active { get; set; } = true;        
        public int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public PurchaseOrder() { }
    }
}
