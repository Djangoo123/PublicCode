using Kata_Cupcake.Core.Abstractions;
using Kata_Cupcake.Core.Models;

namespace KataTests
{
    public class CakeTests
    {
        [Fact]
        public void Cupcake_Has_Default_Name_And_Price()
        {
            ICake cake = new Cupcake();

            Assert.Equal("🧁", cake.GetName());
            Assert.Equal(1.0m, cake.GetPrice());
        }

        [Fact]
        public void Cookie_Has_Default_Name_And_Price()
        {
            ICake cake = new Cookie();

            Assert.Equal("🍪", cake.GetName());
            Assert.Equal(2.0m, cake.GetPrice());
        }
    }
}