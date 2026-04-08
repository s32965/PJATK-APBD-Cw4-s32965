namespace LegacyRenewalApp;

public interface ISupportFeeCalculator
{
    public decimal Calculate(bool includePremiumSupport, string normalizedPlanCode);
}