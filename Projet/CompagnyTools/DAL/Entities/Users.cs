using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Users
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Email { get; set; }

    public byte[] Salt { get; set; } = null!;

    public virtual ICollection<UsersRoles> UsersRoles { get; set; } = new List<UsersRoles>();
}
