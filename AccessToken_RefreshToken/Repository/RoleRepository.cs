using thread_api_asp.Entities;

namespace thread_api_asp.Repository
{
    public interface IRoleRepository
    {
        public List<Role> GetDefaultRoles();
    }

    public class RoleRepository(ThreadsContext context) : IRoleRepository
    {
        public List<Role> GetDefaultRoles() => context.Roles.Where(x => x.IsDefault == true).ToList();
    }
}