using PursuitSim_v2.Core.Common;
using PursuitSim_v2.Core.DroneSim.Models;
using PursuitSim_v2.Core.DroneSim.Models.Mines;


namespace PursuitSim_v2.Core.DroneSim
{
    public static class Scenarios
    {
        public static Scenario PlainWithHedges()
        {
            var s = new Scenario { Name = "Plain + Hedges" };

            for (int y = 150; y <= 900; y += 150)
                s.Obstacles.Add(new Rect(0, y - 1, 1000, 2));

            s.AccessYs = Enumerable.Range(1, 9).Select(i => i * 100.0).ToList();

            s.MainPath = new PolyPath([new Vec2(500, 0), new Vec2(500, 1000)]);
            s.AltPathA = new PolyPath([new Vec2(500, 250), new Vec2(300, 250), new Vec2(300, 700), new Vec2(500, 700)]);
            s.AltPathB = new PolyPath([new Vec2(500, 400), new Vec2(700, 400), new Vec2(700, 850), new Vec2(500, 850)]);

            s.Mines =
            [
                new Mine { Pos = new Vec2(300, 200) },
            new Mine { Pos = new Vec2(600, 450) },
            new Mine { Pos = new Vec2(800, 700) }
            ];


            return s;
        }

        public static Scenario UrbanGrid()
        {
            var s = new Scenario { Name = "Urban Grid" };

            for (int x = 0; x < 1000; x += 60)
                for (int y = 0; y < 1000; y += 60)
                    s.Obstacles.Add(new Rect(x + 5, y + 5, 50, 50));

            s.AccessYs = Enumerable.Range(1, 9).Select(i => i * 100.0).ToList();

            s.MainPath = new PolyPath([new Vec2(500, 0), new Vec2(500, 1000)]);
            s.AltPathA = new PolyPath([new Vec2(500, 300), new Vec2(420, 300), new Vec2(420, 700), new Vec2(500, 700)]);
            s.AltPathB = new PolyPath([new Vec2(500, 450), new Vec2(580, 450), new Vec2(580, 850), new Vec2(500, 850)]);

            s.Mines =
            [
                new Mine { Pos = new Vec2(300, 200) },
            new Mine { Pos = new Vec2(600, 450) },
            new Mine { Pos = new Vec2(800, 700) }
            ];


            return s;
        }

        public static Scenario MixedClearingFinal()
        {
            var s = new Scenario { Name = "Mixed: Cover then Clearing" };

            for (int x = 0; x < 1000; x += 80)
                for (int y = 0; y < 600; y += 80)
                    s.Obstacles.Add(new Rect(x + 10, y + 10, 50, 50));

            s.Obstacles.Add(new Rect(450, 620, 100, 20));
            s.Obstacles.Add(new Rect(300, 720, 120, 20));

            s.AccessYs = [200, 300, 400, 500, 650, 700, 800, 900];

            s.MainPath = new PolyPath([new Vec2(500, 0), new Vec2(500, 1000)]);
            s.AltPathA = new PolyPath([new Vec2(500, 250), new Vec2(320, 250), new Vec2(320, 700), new Vec2(500, 700)]);
            s.AltPathB = new PolyPath([new Vec2(500, 400), new Vec2(680, 400), new Vec2(680, 850), new Vec2(500, 850)]);

            s.Mines =
            [
                new Mine { Pos = new Vec2(300, 200) },
            new Mine { Pos = new Vec2(600, 450) },
            new Mine { Pos = new Vec2(800, 700) }
            ];


            return s;
        }
    }

}
