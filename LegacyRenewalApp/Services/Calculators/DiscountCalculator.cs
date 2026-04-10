using System.Collections.Generic;
using System.Linq;

namespace LegacyRenewalApp;

public class DiscountCalculator : IDiscountCalculator
{
    private static readonly Dictionary<string, List<object>> StandardDiscountDatabase = new Dictionary<string, List<object>>
    {
        {"Silver", new List<object> {0.05m, "silver discount; "}},
        {"Gold", new List<object> {0.10m, "gold discount; "}},
        {"Platinum", new List<object> {0.15m, "platinum discount; "}},
        {"Education", new List<object> {0.20m, "education discount; "}}
    };

    private static readonly Dictionary<int, List<object>> YearsWithCompanyDiscountDatabase = new Dictionary<int, List<object>>
    {
        { 5, new List<object> { 0.07m, "long-term loyalty discount; " } },
        { 2, new List<object> { 0.03m, "basic loyalty discount; " } },
    };

    private static readonly Dictionary<int, List<object>> SeatCountDiscountDatabase = new Dictionary<int, List<object>>
    {
        { 50, new List<object> { 0.12m, "large team discount; "} },
        { 20, new List<object> { 0.08m, "medium team discount; "} },
        { 10, new List<object> { 0.04m, "small team discount; "} }
    };
    
    public DiscountResult CalculateDiscount(
        Customer customer,
        SubscriptionPlan plan,
        decimal baseAmount,
        int seatCount,
        bool useLoyaltyPoints)
    {
        decimal discountAmount = 0m;
        string notes = string.Empty;
        int loyaltyPointsTreshold = 200;

        if (StandardDiscountDatabase.ContainsKey(customer.Segment))
        {
            discountAmount += baseAmount * (decimal)StandardDiscountDatabase[customer.Segment][0];
            notes += StandardDiscountDatabase[customer.Segment][1];
        }
        
        var validYears = YearsWithCompanyDiscountDatabase.Keys.Where(k => k <= customer.YearsWithCompany);
        if (validYears.Any())
        {
            int year = validYears.Max();
            discountAmount += baseAmount * (decimal)YearsWithCompanyDiscountDatabase[year][0];
            notes += YearsWithCompanyDiscountDatabase[year][1];
        }

        var validSeatCounts = SeatCountDiscountDatabase.Keys.Where(k => k <= seatCount);
        if (validSeatCounts.Any())
        {
            int sCount = validSeatCounts.Max(); 
            discountAmount += baseAmount * (decimal)SeatCountDiscountDatabase[sCount][0];
            notes += SeatCountDiscountDatabase[sCount][1];
        }

        if (useLoyaltyPoints && customer.LoyaltyPoints > 0)
        {
            int pointsToUse = customer.LoyaltyPoints > loyaltyPointsTreshold ? loyaltyPointsTreshold : customer.LoyaltyPoints;
            discountAmount += pointsToUse;
            notes += $"loyalty points used: {pointsToUse}; ";
        }
        
        return new DiscountResult { Amount = discountAmount, Notes = notes };    
    }
}