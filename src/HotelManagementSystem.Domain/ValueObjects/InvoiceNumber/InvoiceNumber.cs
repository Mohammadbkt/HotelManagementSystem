namespace HotelManagementSystem.Domain.ValueObjects.Invoicing;

public sealed record class InvoiceNumber
{
    public string Value { get; }

    private InvoiceNumber(string value)
    {
        Value = value;
    }

    public static InvoiceNumber Generate()
    {
        var datePart = DateTime.UtcNow.ToString("yyyyMMdd");
        var randomPart = Guid.NewGuid().ToString().Substring(0, 8).ToUpperInvariant();

        return new InvoiceNumber($"INV-{datePart}-{randomPart}");
    }

    public static InvoiceNumber Create(string value)
    {
        return new InvoiceNumber(value);
    } 
}
