using PursuitSim_v2.Core.UrbanSiege;

namespace PursuitSim_v2.ConsoleApp.Engine;

public static class UrbanSiegeBatchRunner
{
    public static List<UrbanSiegeRunResult> Run(int runs)
    {
        var results = new List<UrbanSiegeRunResult>();

        for (int i = 0; i < runs; i++)
        {
            var s = UrbanSiegeFactory.Create();

            var engine = new UrbanSiegeEngine(s);
            engine.Run();

            int attackersAlive = s.Attackers.Soldiers.Count(sol => !sol.IsKO);
            int defendersAlive = s.Defenders.SelectMany(t => t.Soldiers).Count(sol => !sol.IsKO);

            bool attackersWin = attackersAlive >= s.VictoryThreshold;

            string resultText = attackersWin ? "ATTACKERS WIN" : "DEFENDERS HOLD";

            results.Add(new UrbanSiegeRunResult
            {
                RunIndex = i + 1,
                AttackersWin = attackersWin,
                AliveAttackers = attackersAlive,
                AliveDefenders = defendersAlive,
                ResultText = resultText
            });
        }

        return results;
    }
}
