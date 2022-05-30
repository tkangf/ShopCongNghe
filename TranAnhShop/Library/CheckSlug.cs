using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TranAnhShop.Models;

namespace TranAnhShop
{
    public class CheckSlug
    {
        TranAnhShopDbContext db = new TranAnhShopDbContext();
        public bool KiemTraSlug(String Table, String Slug, int? id)
        {
            switch (Table)
            {
                case "Category":
                    if (id != null)
                    {
                        if (db.Category.Where(m => m.Slug == Slug && m.ID != id).Count() > 0)
                            return false;
                    }
                    else
                    {
                        if (db.Category.Where(m => m.Slug == Slug).Count() > 0)
                            return false;
                    }
                    break;
                case "Topic":
                    break;
                case "Post":
                    break;
                case "Product":
                    break;
            }
            return true;


        }

    }
}