using LegacyRenewalApp.Exceptions;

namespace LegacyRenewalApp.Validators;

public class RenewalRequestValidator : IRenewalRequestValidator
{
    public void Validate(int customerId, string planCode, int seatCount, string paymentMethod)
    {
        if (customerId <= 0)
        {
            throw new NegativeCustomerIDException();
        }

        if (string.IsNullOrWhiteSpace(planCode))
        {
            throw new PlanCodeAbsentException();
        }

        if (seatCount <= 0)
        {
            throw new SeatCountNegativeException();
        }

        if (string.IsNullOrWhiteSpace(paymentMethod))
        {
            throw new PaymentMethodAbsentException();
        }
    }
}