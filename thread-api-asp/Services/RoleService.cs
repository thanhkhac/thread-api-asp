using thread_api_asp.Entities;

namespace thread_api_asp.Services
{
    public interface IRoleService
    {
        public List<Role> GetDefaultRoles();
    }
    public class RoleService(ThreadsContext context) : IRoleService
    {
        public List<Role> GetDefaultRoles() => context.Roles.Where(x => x.IsDefault == true).ToList();
    }
}
