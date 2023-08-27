﻿using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class DataOffice
{
    public int Id { get; set; }

    public string Chairdirection { get; set; } = null!;

    public int X { get; set; }

    public int Y { get; set; }

    public DateTime DateCreation { get; set; }

    public virtual ICollection<Equipments> Equipments { get; set; } = new List<Equipments>();
}