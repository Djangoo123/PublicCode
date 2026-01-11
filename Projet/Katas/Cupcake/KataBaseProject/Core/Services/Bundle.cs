using Kata_Cupcake.Core.Abstractions;


namespace Kata_Cupcake.Core.Services
{

    public class Bundle : ICake
    {
        private readonly List<ICake> _cakes = new();

        public void Add(ICake cake)
        {
            _cakes.Add(cake);
        }

        public string GetName()
            => $"Bundle of {_cakes.Count} cakes";

        public decimal GetPrice()
        {
            var total = _cakes.Sum(c => c.GetPrice());
            return total * 0.9m; // -10%
        }
    }
}
