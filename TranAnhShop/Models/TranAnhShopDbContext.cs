using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace TranAnhShop.Models
{
    public class TranAnhShopDbContext: DbContext
    {
        public TranAnhShopDbContext() : base("name=Connect") { }
        public virtual DbSet<mCategory> Category { get; set; }
        public virtual DbSet<mContact> Contact { get; set; }
        public virtual DbSet<mMenu> Menu { get; set; }
        public virtual DbSet<mOrder> Order { get; set; }
        public virtual DbSet<mOrderDetail> OrderDetail { get; set; }
        public virtual DbSet<mPost> Post { get; set; }
        public virtual DbSet<mProduct> Product { get; set; }
        public virtual DbSet<mSlider> Slider { get; set; }
        public virtual DbSet<mTopic> Topic { get; set; }
        public virtual DbSet<mUser> User { get; set; }
        public virtual DbSet<mLink> Link { get; set; }
        public virtual DbSet<role> Roles { get; set; }
    }
}