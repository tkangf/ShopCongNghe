using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    [Table("Link")]
    public class mLink
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Slug { get; set; }
        [Required]
        // Type = Category, Topic, Page
        public string Type { get; set; }
        // ID theo bảng lấy ra (_)
        public int TableID { get; set; }

    }
}