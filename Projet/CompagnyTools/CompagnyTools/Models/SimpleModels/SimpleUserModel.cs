namespace CompagnyTools.Models.SimpleModels
{
    public class SimpleUserModel
    {
        public string Username { get; set; } = null!;

        public string? Email { get; set; }
        public ICollection<SimpleUserRolesModel> UsersRoles { get; set; } = new List<SimpleUserRolesModel>();

    }
}
