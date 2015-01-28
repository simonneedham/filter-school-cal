using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FilterSchoolCal.Web
{
    public static class CacheHelper
    {
        public static T Get<T>(string key, Func<T> getItemFunction) where T : class
        {
            T item = HttpRuntime.Cache.Get(key) as T;

            if(item == null)
            {
                item = getItemFunction();
                HttpContext.Current.Cache.Insert(key, item);
            }

            return item;
        }
    }
}