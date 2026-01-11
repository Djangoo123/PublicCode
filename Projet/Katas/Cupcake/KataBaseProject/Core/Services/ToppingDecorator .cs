using Kata_Cupcake.Core.Abstractions;

namespace Kata_Cupcake.Core.Services
{
    public abstract class ToppingDecorator : ICake
    {
        protected readonly ICake Cake;

        protected ToppingDecorator(ICake cake)
        {
            Cake = cake;
        }

        public abstract string GetName();
        public abstract decimal GetPrice();
    }
}
