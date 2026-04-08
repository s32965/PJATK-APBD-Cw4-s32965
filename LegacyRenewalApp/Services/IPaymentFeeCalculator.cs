namespace LegacyRenewalApp;

public interface IPaymentFeeCalculator
{
    public PaymentFeeResult Calculate(
        string normalizedPaymentMethod, 
        decimal subtotalAfterDiscount, 
        decimal supportFee,
        string notes);
}