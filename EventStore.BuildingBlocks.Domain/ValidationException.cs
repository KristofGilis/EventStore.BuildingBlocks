﻿namespace EventStore.BuildingBlocks.Domain;
public class ValidationException : Exception
{
    public ValidationException(string? message) : base(message)
    {
    }
}
