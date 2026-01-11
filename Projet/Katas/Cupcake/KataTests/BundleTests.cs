using Kata_Cupcake.Core.Models;
using Kata_Cupcake.Core.Services;

namespace KataTests
{
    public class BundleTests
    {
        [Fact]
        public void Bundle_With_One_Cake()
        {
            var bundle = new Bundle();
            bundle.Add(new Cupcake());

            Assert.Equal(0.9m, bundle.GetPrice());
        }

        [Fact]
        public void Bundle_With_Multiple_Cakes()
        {
            var bundle = new Bundle();
            bundle.Add(new Cupcake());
            bundle.Add(new Cookie());

            Assert.Equal(2.7m, bundle.GetPrice());
        }

        [Fact]
        public void Bundle_Of_Bundles()
        {
            var innerBundle = new Bundle();
            innerBundle.Add(new Cupcake());
            innerBundle.Add(new Cookie());

            var outerBundle = new Bundle();
            outerBundle.Add(innerBundle);
            outerBundle.Add(new Cupcake());

            Assert.Equal(((1 + 2) * 0.9m + 1) * 0.9m, outerBundle.GetPrice());
        }
    }
}
