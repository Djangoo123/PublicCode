using Kata_Cupcake.Core.Abstractions;

namespace Kata_Cupcake.Core.Models
{
    public class Cupcake : ICake
    {
        public string GetName() => "🧁";
        public decimal GetPrice() => 1.0m;
    }
}
