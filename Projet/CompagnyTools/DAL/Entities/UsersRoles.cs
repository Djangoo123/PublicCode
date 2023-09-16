using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class UsersRoles
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string UserRight { get; set; } = null!;

    public virtual Users User { get; set; } = null!;
}
