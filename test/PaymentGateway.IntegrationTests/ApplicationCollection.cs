namespace PaymentGateway.IntegrationTests;

[CollectionDefinition(Name)]
public sealed class ApplicationCollection : ICollectionFixture<ApplicationFixture>
{
    public const string Name = "PaymentGateway server collection";
}