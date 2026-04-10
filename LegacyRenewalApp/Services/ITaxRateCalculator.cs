namespace LegacyRenewalApp;

public interface ITaxRateCalculator
{
    public decimal Calculate(Customer customer);
}