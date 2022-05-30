using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    [Table("Product")]
    public class mProduct
    {
        [Key]
        [Required]
        public int ID { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn nhập tên sản phẩm")]
        public string Name { get; set; }
        public string Slug { get; set; }
        [Required(ErrorMessage = "Vui lòng chọn loại sản phẩm")]
        public int CateID { get; set; }
        public string Image { get; set; }
        public string ImageList { get; set; }
        public string NewPromotion { get; set; }
        public int Installment { get; set; }
        public int Discount { get; set; }
        public string Detail { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập mô tả sản phẩm")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập thông số kỹ thuật")]
        public string Specification { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập giá gốc")]
        [Range(1000, float.MaxValue, ErrorMessage = "Bạn phải nhập thấp nhất từ 1000")]
        public double Price { get; set; }
        public int Quantity { get; set; }
        [Range(1000, float.MaxValue, ErrorMessage = "Bạn phải nhập thấp nhất từ 1000")]
        [Required(ErrorMessage = "Vui lòng nhập giá khuyến mại")]
        public double ProPrice { get; set; }
        public string MetaKey { get; set; }
        public string MetaDesc { get; set; }

        public int Status { get; set; }
        public DateTime Created_at { get; set; }
        public int Created_by { get; set; }
        public DateTime Updated_at { get; set; }
        public int Updated_by { get; set; }

    }
}