using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports.Storage;

namespace PaymentGateway.Api.Infrastructure.Storage;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid,Payment> _payments = [];
    
    public Guid Add(Payment payment)
    {
        _payments.Add(payment.Id, payment);
        return payment.Id;
    }

    public Payment? Get(Guid id)
    {
        _payments.TryGetValue(id, out Payment? payment);
        return payment;
    }
}