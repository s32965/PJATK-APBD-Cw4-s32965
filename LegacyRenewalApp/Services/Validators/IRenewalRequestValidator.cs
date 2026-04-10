namespace LegacyRenewalApp.Validators;

public interface IRenewalRequestValidator
{
    public void Validate(int customerId, string planCode, int seatCount, string paymentMethod);
}