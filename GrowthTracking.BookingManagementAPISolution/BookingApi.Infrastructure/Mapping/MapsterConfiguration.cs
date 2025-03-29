using BookingApi.Application.DTOs;
using BookingApi.Domain.Entities;
using Mapster;

namespace BookingApi.Infrastructure.Mapping
{
    public static class MapsterConfiguration
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Booking, BookingDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<BookingDTO, Booking>.NewConfig().MapToConstructor(true);

            TypeAdapterConfig<Schedule, ScheduleDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<ScheduleDTO, Schedule>.NewConfig().MapToConstructor(true);

            TypeAdapterConfig<Consultation, ConsultationDTO>.NewConfig().MapToConstructor(true);
            TypeAdapterConfig<ConsultationDTO, Consultation>.NewConfig().MapToConstructor(true);
        }
    }
}