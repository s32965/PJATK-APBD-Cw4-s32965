namespace LegacyRenewalApp;

public class SupportFeeCalculator : ISupportFeeCalculator
{
    public decimal Calculate(bool includePremiumSupport, string normalizedPlanCode)
    {
        decimal supportFee = 0m;
        if (includePremiumSupport)
        {
            if (normalizedPlanCode == "START")
            {
                supportFee = 250m;
            }
            else if (normalizedPlanCode == "PRO")
            {
                supportFee = 400m;
            }
            else if (normalizedPlanCode == "ENTERPRISE")
            {
                supportFee = 700m;
            }
        }
        return supportFee;
    }
}