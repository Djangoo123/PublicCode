using System;
using System.Collections.Generic;

namespace CompagnyTools.Entities;

public partial class Dataoffice
{
    public int Id { get; set; }

    public string Chairdirection { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
