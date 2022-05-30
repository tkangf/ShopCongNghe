using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    public class zListOrder
    {
        public int IDO { get; set; }
        public String CustomerName { get; set; }
        public double SAmount { get; set; }
        public int? Status { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? ExportDate { get; set; }
    }
}