using DAL.Entities;

namespace CompagnyTools.Models
{
    public class UsersRolesModel
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UserRight { get; set; } = null!;

    }
}
