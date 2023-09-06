using System;
using System.Collections.Generic;

namespace DAL.Entities;

public partial class Reservations
{
    public int Id { get; set; }

    public int? DeskId { get; set; }

    public string? Username { get; set; }

    public DateTime DateCreation { get; set; }

    public DateTime DateReservation { get; set; }

    public string? Location { get; set; }
}
