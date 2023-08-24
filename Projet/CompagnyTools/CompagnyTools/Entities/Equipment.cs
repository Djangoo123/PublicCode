using System;
using System.Collections.Generic;

namespace CompagnyTools.Entities;

public partial class Equipment
{
    public int Id { get; set; }

    public int? DeskId { get; set; }

    public string Type { get; set; } = null!;

    public string? Specification { get; set; }

    public virtual DataOffice? Desk { get; set; }
}
