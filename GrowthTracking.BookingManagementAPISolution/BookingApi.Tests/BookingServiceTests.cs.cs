using BookingApi.Application.DTOs;
using BookingApi.Application.Interfaces;
using BookingApi.Application.Services;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BookingApi.Tests
{
    public class BookingServiceTests
    {
        private readonly Mock<IBookingRepository> _bookingRepositoryMock;
        private readonly Mock<IScheduleRepository> _scheduleRepositoryMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly Mock<ILogger<BookingService>> _loggerMock;
        private readonly BookingService _bookingService;

        public BookingServiceTests()
        {
            _bookingRepositoryMock = new Mock<IBookingRepository>();
            _scheduleRepositoryMock = new Mock<IScheduleRepository>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _loggerMock = new Mock<ILogger<BookingService>>();

            _bookingService = new BookingService(
                _bookingRepositoryMock.Object,
                _scheduleRepositoryMock.Object,
                _eventPublisherMock.Object);
        }

        [Fact]
        public async Task CreateBookingAsync_ScheduleNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: null,
                DoctorConfirmationDeadline: null,
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            _scheduleRepositoryMock.Setup(repo => repo.GetScheduleAsync(bookingDto.ScheduleId))
                .ReturnsAsync((ScheduleDTO?)null);

            // Act
            var response = await _bookingService.CreateBookingAsync(bookingDto);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Schedule not found", response.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ScheduleNotAvailable_ReturnsFailureResponse()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: null,
                DoctorConfirmationDeadline: null,
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            var scheduleDto = new ScheduleDTO(
                Id: bookingDto.ScheduleId,
                DoctorId: Guid.NewGuid(),
                StartTime: DateTime.UtcNow,
                EndTime: DateTime.UtcNow.AddHours(1),
                Location: "Test location",
                IsAvailable: false);

            _scheduleRepositoryMock.Setup(repo => repo.GetScheduleAsync(bookingDto.ScheduleId))
                .ReturnsAsync(scheduleDto);

            // Act
            var response = await _bookingService.CreateBookingAsync(bookingDto);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Schedule is not available", response.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_ScheduleConflict_ReturnsFailureResponse()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: null,
                DoctorConfirmationDeadline: null,
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            var scheduleDto = new ScheduleDTO(
                Id: bookingDto.ScheduleId,
                DoctorId: Guid.NewGuid(),
                StartTime: DateTime.UtcNow,
                EndTime: DateTime.UtcNow.AddHours(1),
                Location: "Test location",
                IsAvailable: true);

            _scheduleRepositoryMock.Setup(repo => repo.GetScheduleAsync(bookingDto.ScheduleId))
                .ReturnsAsync(scheduleDto);

            _bookingRepositoryMock.Setup(repo => repo.CheckScheduleConflictAsync(bookingDto.ScheduleId, It.IsAny<Guid>()))
                .ReturnsAsync(true);

            // Act
            var response = await _bookingService.CreateBookingAsync(bookingDto);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Schedule conflict detected", response.Message);
        }

        [Fact]
        public async Task CreateBookingAsync_SuccessfulBooking_PublishesEventAndReturnsSuccess()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: null,
                DoctorConfirmationDeadline: null,
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            var scheduleDto = new ScheduleDTO(
                Id: bookingDto.ScheduleId,
                DoctorId: Guid.NewGuid(),
                StartTime: DateTime.UtcNow,
                EndTime: DateTime.UtcNow.AddHours(1),
                Location: "Test location",
                IsAvailable: true);

            var createdBookingDto = bookingDto with { Id = Guid.NewGuid() };

            _scheduleRepositoryMock.Setup(repo => repo.GetScheduleAsync(bookingDto.ScheduleId))
                .ReturnsAsync(scheduleDto);

            _bookingRepositoryMock.Setup(repo => repo.CheckScheduleConflictAsync(bookingDto.ScheduleId, It.IsAny<Guid>()))
                .ReturnsAsync(false);

            _bookingRepositoryMock.Setup(repo => repo.CreateBookingAsync(It.Is<BookingDTO>(dto =>
                dto.ParentId == bookingDto.ParentId &&
                dto.ChildId == bookingDto.ChildId &&
                dto.ScheduleId == bookingDto.ScheduleId)))
                .Callback<BookingDTO>(dto => dto.Id = createdBookingDto.Id)
                .ReturnsAsync(new Response(true, "Booking created successfully"));

            // Act
            var response = await _bookingService.CreateBookingAsync(bookingDto);

            // Assert
            Assert.True(response.Flag);
            Assert.Equal("Booking created successfully", response.Message);
            _eventPublisherMock.Verify(p => p.PublishBookingCreated(
                createdBookingDto.Id.Value,
                bookingDto.ParentId,
                bookingDto.ChildId,
                scheduleDto.DoctorId), Times.Once());
        }

        [Fact]
        public async Task UpdateBookingAsync_BookingIdMissing_ReturnsFailureResponse()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: DateTime.UtcNow,
                DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            // Act
            var response = await _bookingService.UpdateBookingAsync(bookingDto);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Booking Id is required for update", response.Message);
        }

        [Fact]
        public async Task UpdateBookingAsync_BookingNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var bookingDto = new BookingDTO(
                Id: Guid.NewGuid(),
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: DateTime.UtcNow,
                DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingDto.Id.Value))
                .ReturnsAsync((BookingDTO?)null);

            // Act
            var response = await _bookingService.UpdateBookingAsync(bookingDto);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Booking not found", response.Message);
        }

        [Fact]
        public async Task UpdateBookingAsync_StatusChangedToConfirmed_SetsPaymentDeadline()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var existingBookingDto = new BookingDTO(
                Id: bookingId,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: DateTime.UtcNow,
                DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            var updatedBookingDto = existingBookingDto with { Status = "confirmed" };

            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingId))
                .ReturnsAsync(existingBookingDto);

            _bookingRepositoryMock.Setup(repo => repo.UpdateBookingAsync(It.IsAny<BookingDTO>()))
                .ReturnsAsync(new Response(true, "Booking updated successfully"));

            // Act
            var response = await _bookingService.UpdateBookingAsync(updatedBookingDto);

            // Assert
            Assert.True(response.Flag);
            Assert.Equal("Booking updated successfully", response.Message);
            _bookingRepositoryMock.Verify(repo => repo.UpdateBookingAsync(It.Is<BookingDTO>(dto =>
                dto.PaymentDeadline.HasValue &&
                dto.PaymentDeadline.Value >= DateTime.UtcNow.AddHours(48).AddMinutes(-1) &&
                dto.PaymentDeadline.Value <= DateTime.UtcNow.AddHours(48).AddMinutes(1))), Times.Once());
        }

        [Fact]
        public async Task GetBookingAsync_BookingNotFound_ReturnsNull()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingId))
                .ReturnsAsync((BookingDTO?)null);

            // Act
            var result = await _bookingService.GetBookingAsync(bookingId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetBookingAsync_BookingExists_ReturnsBooking()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var bookingDto = new BookingDTO(
                Id: bookingId,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: DateTime.UtcNow,
                DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingId))
                .ReturnsAsync(bookingDto);

            // Act
            var result = await _bookingService.GetBookingAsync(bookingId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(bookingId, result.Id);
        }

        [Fact]
        public async Task GetBookingsByParentAsync_ReturnsBookings()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            var bookings = new List<BookingDTO>
            {
                new BookingDTO(
                    Id: Guid.NewGuid(),
                    ParentId: parentId,
                    ChildId: Guid.NewGuid(),
                    ScheduleId: Guid.NewGuid(),
                    Status: "pending",
                    BookingDate: DateTime.UtcNow,
                    DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                    PaymentDeadline: null,
                    Notes: "Test booking 1",
                    CancelledBy: null,
                    CancellationTime: null),
                new BookingDTO(
                    Id: Guid.NewGuid(),
                    ParentId: parentId,
                    ChildId: Guid.NewGuid(),
                    ScheduleId: Guid.NewGuid(),
                    Status: "confirmed",
                    BookingDate: DateTime.UtcNow,
                    DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                    PaymentDeadline: DateTime.UtcNow.AddHours(48),
                    Notes: "Test booking 2",
                    CancelledBy: null,
                    CancellationTime: null)
            };

            _bookingRepositoryMock.Setup(repo => repo.GetBookingsByParentAsync(parentId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetBookingsByParentAsync(parentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, booking => Assert.Equal(parentId, booking.ParentId));
        }

        [Fact]
        public async Task GetBookingsByDoctorAsync_ReturnsBookings()
        {
            // Arrange
            var doctorId = Guid.NewGuid();
            var bookings = new List<BookingDTO>
            {
                new BookingDTO(
                    Id: Guid.NewGuid(),
                    ParentId: Guid.NewGuid(),
                    ChildId: Guid.NewGuid(),
                    ScheduleId: Guid.NewGuid(),
                    Status: "pending",
                    BookingDate: DateTime.UtcNow,
                    DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                    PaymentDeadline: null,
                    Notes: "Test booking 1",
                    CancelledBy: null,
                    CancellationTime: null)
            };

            _bookingRepositoryMock.Setup(repo => repo.GetBookingsByDoctorAsync(doctorId))
                .ReturnsAsync(bookings);

            // Act
            var result = await _bookingService.GetBookingsByDoctorAsync(doctorId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task CancelBookingAsync_BookingNotFound_ReturnsFailureResponse()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingId))
                .ReturnsAsync((BookingDTO?)null);

            // Act
            var response = await _bookingService.CancelBookingAsync(bookingId);

            // Assert
            Assert.False(response.Flag);
            Assert.Equal("Booking not found", response.Message);
        }

        [Fact]
        public async Task CancelBookingAsync_SuccessfulCancellation_PublishesEvent()
        {
            // Arrange
            var bookingId = Guid.NewGuid();
            var bookingDto = new BookingDTO(
                Id: bookingId,
                ParentId: Guid.NewGuid(),
                ChildId: Guid.NewGuid(),
                ScheduleId: Guid.NewGuid(),
                Status: "pending",
                BookingDate: DateTime.UtcNow,
                DoctorConfirmationDeadline: DateTime.UtcNow.AddDays(1),
                PaymentDeadline: null,
                Notes: "Test booking",
                CancelledBy: null,
                CancellationTime: null);

            var scheduleDto = new ScheduleDTO(
                Id: bookingDto.ScheduleId,
                DoctorId: Guid.NewGuid(),
                StartTime: DateTime.UtcNow,
                EndTime: DateTime.UtcNow.AddHours(1),
                Location: "Test location",
                IsAvailable: true);

            _bookingRepositoryMock.Setup(repo => repo.GetBookingAsync(bookingId))
                .ReturnsAsync(bookingDto);

            _scheduleRepositoryMock.Setup(repo => repo.GetScheduleAsync(bookingDto.ScheduleId))
                .ReturnsAsync(scheduleDto);

            _bookingRepositoryMock.Setup(repo => repo.CancelBookingAsync(bookingId))
                .ReturnsAsync(new Response(true, "Booking cancelled successfully"));

            // Act
            var response = await _bookingService.CancelBookingAsync(bookingId);

            // Assert
            Assert.True(response.Flag);
            Assert.Equal("Booking cancelled successfully", response.Message);
            _eventPublisherMock.Verify(p => p.PublishBookingCancelled(
                bookingId,
                bookingDto.ParentId,
                bookingDto.ChildId,
                scheduleDto.DoctorId), Times.Once());
        }
    }
}