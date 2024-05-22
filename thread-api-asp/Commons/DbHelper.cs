using thread_api_asp.Commons;
using thread_api_asp.Models;

namespace thread_api_asp.DbHelper
{
    public class DbHelper
    {
        public static string SaveChangeHandleError(ThreadsContext context)
        {
            try { context.SaveChanges(); }
            catch (Exception e) { return e.Message; }
            return string.Empty;
        }
    }
}