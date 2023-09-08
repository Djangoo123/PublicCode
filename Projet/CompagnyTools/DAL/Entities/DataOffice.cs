using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class DataOffice
{
    public int Id { get; set; }

    public string Chairdirection { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public DateTime DateCreation { get; set; }

    public string Location { get; set; } = null!;

    public virtual ICollection<Equipments> Equipments { get; set; } = new List<Equipments>();

    public virtual ICollection<Reservations> Reservations { get; set; } = new List<Reservations>();
}
