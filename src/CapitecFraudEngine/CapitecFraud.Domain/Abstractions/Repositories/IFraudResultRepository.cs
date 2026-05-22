using CapitecFraud.Domain.Entities;

namespace CapitecFraud.Domain.Abstractions.Repositories;

public interface IFraudResultRepository
{
    Task AddAsync(FraudResult result);
}