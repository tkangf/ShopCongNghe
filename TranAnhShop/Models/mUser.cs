using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    [MetadataType(typeof(mUser))]
    

    [Table("User")]
    public class mUser
    {
        [Key]
        [Required]
        public int ID { get; set; }
        //update
        //public int UserID { get; set; }
        public string Fullname { get; set; }
        public string Name { get; set; }

        /*[Required(AllowEmptyStrings = false, ErrorMessage = "Mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [MinLength(2, ErrorMessage = "Yêu cầu tối thiểu 2 ký tự")]*/
        public string Password { get; set; }
        //update
        //public string Password { get; set; }
        public string Email { get; set; }
        public int Gender { get; set; }//Giới tính
        public int Phone { get; set; }
        public string Address { get; set; }
        public string Image { get; set; }
        public int Access { get; set; }
        public int Status { get; set; }
        public DateTime Created_at { get; set; }
        public int Created_by { get; set; }
        public DateTime Updated_at { get; set; }
        public int Updated_by { get; set; }

        //update
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailID { get; set; }
       /* public DateTime DateOfBirth { get; set; }*/
        
        public bool IsEmailVerified { get; set; }
        public System.Guid ActivationCode { get; set; }
        public string ResetPasswordCode { get; set; }

    }
}