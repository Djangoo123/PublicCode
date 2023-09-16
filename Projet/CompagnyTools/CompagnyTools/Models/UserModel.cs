using DAL.Entities;

namespace CompagnyTools.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Username { get; set; } = null!;

        public string? Email { get; set; }

        public ICollection<UsersRolesModel> UsersRoles { get; set; } = new List<UsersRolesModel>();
    }
}
