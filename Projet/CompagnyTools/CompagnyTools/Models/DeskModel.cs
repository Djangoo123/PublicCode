namespace CompagnyTools.Models
{
    public class DeskModel
    {
        public int Id { get; set; }

        public string Chairdirection { get; set; } = null!;

        public int X { get; set; }

        public int Y { get; set; }
        public DateTime DateCreation { get; set; }

        public List<EquipmentsModel>? Equipments { get; set; }
    }
}
