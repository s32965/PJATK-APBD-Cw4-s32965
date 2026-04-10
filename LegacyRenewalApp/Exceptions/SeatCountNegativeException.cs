using System;

namespace LegacyRenewalApp.Exceptions;

public class SeatCountNegativeException() : Exception($"Seat count must be positive");   