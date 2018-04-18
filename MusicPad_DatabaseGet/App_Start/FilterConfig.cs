using System.Web;
using System.Web.Mvc;

namespace MusicPad_DatabaseGet
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
