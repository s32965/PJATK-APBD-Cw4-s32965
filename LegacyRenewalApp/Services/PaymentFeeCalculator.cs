using System;
using System.Collections.Generic;

namespace LegacyRenewalApp;

public class PaymentFeeCalculator : IPaymentFeeCalculator
{
    private static readonly Dictionary<string, List<object>> PaymentFeeDatabase = new Dictionary<string, List<object>>
    {
        { "CARD", new List<object> {0.02m, "card payment fee; "} },
        { "BANK_TRANSFER", new List<object> {0.01m, "bank transfer fee; "} },
        { "PAYPAL", new List<object> {0.035m, "paypal fee; "} },
        { "INVOICE", new List<object> {0m, "invoice payment; "} }
    };
    
    public PaymentFeeResult Calculate(
        string normalizedPaymentMethod, 
        decimal subtotalAfterDiscount, 
        decimal supportFee,
        string notes)
    {
        decimal paymentFee = 0m;
        
        if (PaymentFeeDatabase.ContainsKey(normalizedPaymentMethod))
        {
            paymentFee = (subtotalAfterDiscount + supportFee) * (decimal) PaymentFeeDatabase[normalizedPaymentMethod][0];
            notes += PaymentFeeDatabase[normalizedPaymentMethod][1];
        }
        else
        {
            throw new ArgumentException("Unsupported payment method");
        }
        
        return new PaymentFeeResult { PaymentFee = paymentFee, Notes = notes };
    }
}