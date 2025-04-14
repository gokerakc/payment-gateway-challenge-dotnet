using PaymentGateway.Api.Domain.Models;
using PaymentGateway.Api.Ports.Storage;

namespace PaymentGateway.Api.Infrastructure.Storage;

public class PaymentsRepository : IPaymentsRepository
{
    private readonly Dictionary<Guid,Payment> _payments = [];
    
    public Guid Add(Payment payment)
    {
        var id = Guid.NewGuid();
        payment.Id = id;
        _payments.Add(id, payment);
        return id;
    }

    public Payment? Get(Guid id)
    {
        _payments.TryGetValue(id, out Payment? payment);
        return payment;
    }
}