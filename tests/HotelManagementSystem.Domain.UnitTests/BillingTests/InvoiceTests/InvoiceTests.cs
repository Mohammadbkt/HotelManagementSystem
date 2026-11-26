using HotelManagementSystem.Domain.Entities.Billing;
using Xunit;

namespace HotelManagementSystem.Domain.UnitTests.InvoiceTests
{
    public class InvoiceTests
    {
        [Fact]
        public void Create_WithValidStayId_ReturnsDraftInvoiceWithZeroTotals()
        {
            var stayId = Guid.NewGuid();

            var result = Invoice.Create(stayId);

            Assert.False(result.IsError);
            var invoice = result.Value;
            Assert.Equal(stayId, invoice.StayId);
            Assert.Equal(InvoiceStatus.Draft, invoice.Status);
            Assert.Equal(0m, invoice.TotalAmount.Amount);
            Assert.Empty(invoice.LineItems);
        }
    }
}