using System;
using System.Collections.Generic;

namespace CompagnyTools.Entities;

public partial class DataOffice
{
    public int DeskId { get; set; }

    public string Chairdirection { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public virtual ICollection<Equipment> Equipment { get; set; } = new List<Equipment>();
}
