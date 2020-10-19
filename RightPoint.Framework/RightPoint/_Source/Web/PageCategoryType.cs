using System;
using System.Collections.Generic;
using System.Web;

namespace EventInventory.Web
{
    public class PageCategoryType
    {
        public static PageCategoryType Sports = new PageCategoryType("Sports", "SP");
        public static PageCategoryType Concerts = new PageCategoryType("Concerts", "CO");
        public static PageCategoryType Theater = new PageCategoryType("Theater", "TH");
        public static PageCategoryType Misc = new PageCategoryType("Misc", "MI");
        public static PageCategoryType Error = new PageCategoryType("Error", "ER");
        public static PageCategoryType[] AllTypes = { Sports, Concerts, Theater, Misc, Error };

        public readonly string Name;
        public readonly string Code;

        private PageCategoryType(string name, string code)
        {
            this.Name = name;
            this.Code = code;
        }

        public static PageCategoryType FindByCode(string code)
        {
            foreach (PageCategoryType type in AllTypes)
            {
                if (code == type.Code) return type;
            }
            return null;
        }
    }
}