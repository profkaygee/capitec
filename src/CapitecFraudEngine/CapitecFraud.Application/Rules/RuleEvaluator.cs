using System.Reflection;
using CapitecFraud.Application.Models;
using CapitecFraud.Domain.Abstractions.Rules;
using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Rules;

public class RuleEvaluator : IRuleEvaluator
{
    public bool Evaluate(RuleConfig rule, FraudEvaluationContext context)
    {
        var prop = context.GetType().GetProperty(rule.Field, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop == null)
            return false;

        var actual = prop.GetValue(context);
        if (actual == null)
            return false;

        return rule.Operator switch
        {
            ">" => Compare(actual, rule.Value, (a, b) => a > b),
            "<" => Compare(actual, rule.Value, (a, b) => a < b),
            "=" => actual.ToString()?.ToLower() == rule.Value.ToLower(),
            "IN" => rule.Value.Split(',').Contains(actual.ToString(), StringComparer.OrdinalIgnoreCase),
            _ => false
        };
    }

    private bool Compare(object actual, string value, Func<double, double, bool> op)
    {
        if (!double.TryParse(actual.ToString(), out var a)) return false;
        if (!double.TryParse(value, out var b)) return false;

        return op(a, b);
    }
}