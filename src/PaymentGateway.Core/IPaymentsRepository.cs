using PaymentGateway.Core.Models;

namespace PaymentGateway.Core;

public interface IPaymentsRepository
{
    public Guid Add(Payment payment);

    public Payment? Get(Guid id);
}