using PaymentGateway.Api.Domain.Models;

namespace PaymentGateway.Api.Ports;

public interface IPaymentsRepository
{
    public Guid Add(Payment payment);

    public Payment? Get(Guid id);
}