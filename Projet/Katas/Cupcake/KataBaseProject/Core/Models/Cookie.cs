using Kata_Cupcake.Core.Abstractions;

namespace Kata_Cupcake.Core.Models;
public class Cookie : ICake
{
    public string GetName() => "🍪";
    public decimal GetPrice() => 2.0m;
}
