namespace CompagnyTools.Models
{
    public class OfficeModel
    {
        public int Id { get; set; }
        public string Chairdirection { get; set; } = null!;
        public int X { get; set; }
        public int Y { get; set; }
        public DateTime DateCreation { get; set; }
        public List<EquipmentsModel>? Equipments { get; set; }
        public DateTime? DateReservationStart { get; set; }
        public DateTime? DateReservationEnd { get; set; }
        public string? UserName { get; set; }    
        public string? Location { get; set; }   
    }
}
