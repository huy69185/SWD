using System;
using System.Threading.Tasks;

namespace BookingApi.Application.Interfaces
{
    public interface IPaymentServiceClient
    {
        Task<bool> HasSuccessfulTransactionAsync(Guid bookingId);
    }
}