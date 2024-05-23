using thread_api_asp.Entities;

namespace thread_api_asp.Commons
{
    public static class DbHelper
    {
        public static string SaveChangeHandleError(ThreadsContext context)
        {
            try { context.SaveChanges(); }
            catch (Exception e) { return e.Message; }
            return string.Empty;
        }
    }
}