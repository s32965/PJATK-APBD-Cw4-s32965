using System.Collections.Generic;

namespace LegacyRenewalApp;

public class TaxRateCalculator : ITaxRateCalculator
{
    private static readonly Dictionary<string, decimal> Database = new Dictionary<string, decimal>
    {
        {"Poland", 0.23m},
        {"Germany", 0.19m},
        {"Czech Republic", 0.21m},
        {"Norway", 0.25m}
    };
    
    public decimal Calculate(Customer customer)
    {
        if (Database.ContainsKey(customer.Segment))
        {
            return Database[customer.Segment];
        }
        return 0.20m;
    }
}