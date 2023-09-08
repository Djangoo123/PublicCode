namespace CompagnyTools.Models
{
    public class ReservationResultModel
    {
        public string? Username { get; set; }

        public DateTime DateReservationStart { get; set; }

        public DateTime DateReservationEnd { get; set; }

        public string? Location { get; set; }
    }
}
