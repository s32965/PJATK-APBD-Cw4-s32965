using System;

namespace LegacyRenewalApp.Exceptions;

public class PaymentMethodAbsentException() : Exception($"Payment method is required");