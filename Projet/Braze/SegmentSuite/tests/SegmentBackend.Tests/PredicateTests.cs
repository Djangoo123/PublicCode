
using SegmentBackend.Models;
using SegmentBackend.Services;
using Xunit;

public class PredicateTests
{
    private static readonly string[] item = new []{"FR","NL"};

    [Fact]
    public void AndOrBetween_Builds()
    {
        var def = new SegmentDefinition
        {
            Name = "t",
            Root = new RuleGroup
            {
                Type = "and",
                Children =
                [
                    new Rule { Kind="attribute", Field="country", Operator="in", Value= item },
                    new RuleGroup
                    {
                        Type = "or",
                        Children =
                        [
                            new Rule { Kind="metric", Field="orders_count", Operator="gte", Value=5 },
                            new Rule { Kind="metric", Field="total_spend", Operator="between", Value=new { min=100, max=300 } }
                        ]
                    }
                ]
            }
        };

        var expr = SegmentToExpression.BuildPredicate(def.Root);
        Assert.NotNull(expr);
    }
}
