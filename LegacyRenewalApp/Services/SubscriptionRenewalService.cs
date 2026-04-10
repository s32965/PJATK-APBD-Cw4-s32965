using System;
using LegacyRenewalApp.Exceptions;
using LegacyRenewalApp.Validators;

namespace LegacyRenewalApp
{
    public class SubscriptionRenewalService
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IDiscountCalculator _discountCalculator;
        private readonly ISupportFeeCalculator _supportFeeCalculator;
        private readonly IPaymentFeeCalculator _paymentFeeCalculator;
        private readonly ITaxRateCalculator _taxRateCalculator;
        private readonly IRenewalRequestValidator _renewalRequestValidator;
        
        // Default for LegacyRenewalAppConsumer
        public SubscriptionRenewalService() : this(
            new CustomerRepository(), 
            new SubscriptionPlanRepository(), 
            new DiscountCalculator(),
            new SupportFeeCalculator(),
            new PaymentFeeCalculator(),
            new TaxRateCalculator(),
            new RenewalRequestValidator()) {}

        public SubscriptionRenewalService(
            ICustomerRepository customerRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IDiscountCalculator discountCalculator,
            ISupportFeeCalculator supportFeeCalculator,
            IPaymentFeeCalculator paymentFeeCalculator,
            ITaxRateCalculator taxRateCalculator,
            IRenewalRequestValidator renewalRequestValidator)
        {
            _customerRepository = customerRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _discountCalculator = discountCalculator;
            _supportFeeCalculator = supportFeeCalculator;
            _paymentFeeCalculator = paymentFeeCalculator;
            _taxRateCalculator = taxRateCalculator;
            _renewalRequestValidator = renewalRequestValidator;
        }
        
        public RenewalInvoice CreateRenewalInvoice(
            int customerId,
            string planCode,
            int seatCount,
            string paymentMethod,
            bool includePremiumSupport,
            bool useLoyaltyPoints)
        {
            _renewalRequestValidator.Validate(customerId, planCode, seatCount, paymentMethod);

            string normalizedPlanCode = planCode.Trim().ToUpperInvariant();
            string normalizedPaymentMethod = paymentMethod.Trim().ToUpperInvariant();

            var customer = _customerRepository.GetById(customerId);
            var plan = _subscriptionPlanRepository.GetByCode(normalizedPlanCode);

            if (!customer.IsActive)
            {
                throw new InvalidOperationException("Inactive customers cannot renew subscriptions");
            }

            decimal baseAmount = (plan.MonthlyPricePerSeat * seatCount * 12m) + plan.SetupFee;
            var discountResult =
                _discountCalculator.CalculateDiscount(customer, plan, baseAmount, seatCount, useLoyaltyPoints);
            var discountAmount = discountResult.Amount;
            var notes = discountResult.Notes;

            decimal subtotalAfterDiscount = baseAmount - discountAmount;
            if (subtotalAfterDiscount < 300m)
            {
                subtotalAfterDiscount = 300m;
                notes += "minimum discounted subtotal applied; ";
            }

            decimal supportFee = _supportFeeCalculator.Calculate(includePremiumSupport, normalizedPlanCode);
            if (includePremiumSupport)
            {
                notes += "premium support included; ";
            }

            var paymentFeeResult =
                _paymentFeeCalculator.Calculate(normalizedPaymentMethod, subtotalAfterDiscount, supportFee, notes);
            decimal paymentFee = paymentFeeResult.PaymentFee;

            decimal taxRate = _taxRateCalculator.Calculate(customer);
            

            decimal taxBase = subtotalAfterDiscount + supportFee + paymentFee;
            decimal taxAmount = taxBase * taxRate;
            decimal finalAmount = taxBase + taxAmount;

            if (finalAmount < 500m)
            {
                finalAmount = 500m;
                notes += "minimum invoice amount applied; ";
            }

            var invoice = new RenewalInvoice
            {
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{customerId}-{normalizedPlanCode}",
                CustomerName = customer.FullName,
                PlanCode = normalizedPlanCode,
                PaymentMethod = normalizedPaymentMethod,
                SeatCount = seatCount,
                BaseAmount = Math.Round(baseAmount, 2, MidpointRounding.AwayFromZero),
                DiscountAmount = Math.Round(discountAmount, 2, MidpointRounding.AwayFromZero),
                SupportFee = Math.Round(supportFee, 2, MidpointRounding.AwayFromZero),
                PaymentFee = Math.Round(paymentFee, 2, MidpointRounding.AwayFromZero),
                TaxAmount = Math.Round(taxAmount, 2, MidpointRounding.AwayFromZero),
                FinalAmount = Math.Round(finalAmount, 2, MidpointRounding.AwayFromZero),
                Notes = notes.Trim(),
                GeneratedAt = DateTime.UtcNow
            };

            LegacyBillingGateway.SaveInvoice(invoice);

            if (!string.IsNullOrWhiteSpace(customer.Email))
            {
                string subject = "Subscription renewal invoice";
                string body =
                    $"Hello {customer.FullName}, your renewal for plan {normalizedPlanCode} " +
                    $"has been prepared. Final amount: {invoice.FinalAmount:F2}.";

                LegacyBillingGateway.SendEmail(customer.Email, subject, body);
            }

            return invoice;
        }
    }
}
