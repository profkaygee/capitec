using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Application.Abstractions;

public interface IFraudResultRepository
{
    Task SaveAsync(FraudResult result);
}