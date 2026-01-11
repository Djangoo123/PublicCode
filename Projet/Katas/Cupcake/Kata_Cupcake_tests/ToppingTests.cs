using Kata_Cupcake.Core.Models;
using Kata_Cupcake.Core.Services;


namespace KataTests
{
    public class ToppingTests
    {
        [Fact]
        public void Cupcake_With_Chocolate()
        {
            var cake = new Chocolate(new Cupcake());

            Assert.Equal("🧁 with 🍫", cake.GetName());
            Assert.Equal(1.1m, cake.GetPrice());
        }

        [Fact]
        public void Cookie_With_Chocolate_And_Nuts()
        {
            var cake1 = new Nuts(new Chocolate(new Cookie()));
            var cake2 = new Chocolate(new Nuts(new Cookie()));

            Assert.Equal("🍪 with 🍫 and 🥜", cake1.GetName());
            Assert.Equal("🍪 and 🥜 with 🍫", cake2.GetName());

            Assert.Equal(2.3m, cake1.GetPrice());
            Assert.Equal(2.3m, cake2.GetPrice());
        }
    }
}
