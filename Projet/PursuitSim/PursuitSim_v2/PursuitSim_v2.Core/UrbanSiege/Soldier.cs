namespace PursuitSim_v2.Core.UrbanSiege
{
    public class Soldier
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public Vec2 Pos { get; set; } = new(0, 0);
        public bool IsKO { get; set; } = false;
        public bool IsDefender { get; set; } = false;
        public string TeamName { get; set; } = "";
    }
}
