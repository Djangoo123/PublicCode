namespace SegmentBackend.Models
{
    public sealed class User
    {
        public string Id { get; set; } = default!;
        public string Country { get; set; } = default!;
        public string Plan { get; set; } = default!;
        public int OrdersCount { get; set; }
        public decimal TotalSpend { get; set; }
        public DateTime CreatedUtc { get; set; }
    }
}