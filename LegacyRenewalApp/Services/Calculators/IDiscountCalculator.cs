namespace LegacyRenewalApp;

public interface IDiscountCalculator
{
    public DiscountResult CalculateDiscount(
        Customer customer, 
        SubscriptionPlan plan, 
        decimal baseAmount,
        int seatCount,
        bool useLoyaltyPoints);
}