namespace LegacyRenewalApp;

public class TaxRateCalculator : ITaxRateCalculator
{
    public decimal Calculate(Customer customer)
    {
        if (customer.Country == "Poland")
        {
            return 0.23m;
        }
        else if (customer.Country == "Germany")
        {
            return 0.19m;
        }
        else if (customer.Country == "Czech Republic")
        {
            return 0.21m;
        }
        else if (customer.Country == "Norway")
        {
            return 0.25m;
        }
        return 0.20m;
    }
}