using Kata_Cupcake.Core.Abstractions;

namespace Kata_Cupcake.Core.Services
{
    public class Nuts : ToppingDecorator
    {
        public Nuts(ICake cake) : base(cake) { }

        public override string GetName()
            => $"{Cake.GetName()} and 🥜";

        public override decimal GetPrice()
            => Cake.GetPrice() + 0.2m;
    }
}
