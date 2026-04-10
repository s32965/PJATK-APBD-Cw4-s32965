using System.Collections.Generic;

namespace LegacyRenewalApp;

public class SupportFeeCalculator : ISupportFeeCalculator
{
    private static readonly Dictionary<string, decimal> SupportFeeDatabase = new Dictionary<string, decimal>
    {
        { "START", 250m },
        { "PRO", 400m },
        { "ENTERPRISE", 700m },
    };
    
    public decimal Calculate(bool includePremiumSupport, string normalizedPlanCode)
    {
        decimal supportFee = 0m;
        if (includePremiumSupport)
        {
            if (SupportFeeDatabase.ContainsKey(normalizedPlanCode))
            {
                supportFee = SupportFeeDatabase[normalizedPlanCode];    
            }
        }
        return supportFee;
    }
}