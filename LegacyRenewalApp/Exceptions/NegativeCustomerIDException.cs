using System;

namespace LegacyRenewalApp.Exceptions;

public class NegativeCustomerIDException() : Exception($"Customer id must be positive");