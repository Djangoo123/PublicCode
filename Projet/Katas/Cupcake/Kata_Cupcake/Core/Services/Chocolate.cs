using Kata_Cupcake.Core.Abstractions;

namespace Kata_Cupcake.Core.Services
{
    public class Chocolate : ToppingDecorator
    {
        public Chocolate(ICake cake) : base(cake) { }

        public override string GetName()
            => $"{Cake.GetName()} with 🍫";

        public override decimal GetPrice()
            => Cake.GetPrice() + 0.1m;
    }

}
