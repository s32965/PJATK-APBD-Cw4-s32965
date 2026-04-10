using System;

namespace LegacyRenewalApp.Exceptions;

public class PlanCodeAbsentException() : Exception($"Plan code is required");