using ChildApi.Application.DTOs;
using ChildApi.Application.Interfaces;
using ChildApi.Application.Messaging;
using ChildApi.Application.Services;
using ChildApi.Presentation.Controllers;
using GrowthTracking.ShareLibrary.Response;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ChildApi.Tests
{
    public class ChildControllerTests
    {
        private readonly Mock<IChildRepository> _childRepositoryMock;
        private readonly Mock<IParentIdCache> _parentIdCacheMock;
        private readonly Mock<IEventPublisher> _eventPublisherMock;
        private readonly ChildController _controller;

        public ChildControllerTests()
        {
            _childRepositoryMock = new Mock<IChildRepository>();
            _parentIdCacheMock = new Mock<IParentIdCache>();
            _eventPublisherMock = new Mock<IEventPublisher>();
            _controller = new ChildController(_childRepositoryMock.Object, _parentIdCacheMock.Object, _eventPublisherMock.Object);
        }

        #region CreateChild Tests
        [Fact]
        public async Task CreateChild_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var parentId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                FullName: "Test Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            _parentIdCacheMock.SetupGet(x => x.ParentId).Returns(parentId);
            var updatedChildDto = childDto with { ParentId = parentId, Id = childId };
            _childRepositoryMock.Setup(x => x.CreateChildAsync(It.Is<ChildDTO>(dto => dto.ParentId == parentId && dto.FullName == childDto.FullName)))
                .ReturnsAsync((new Response(true, "Child created successfully"), childId));
            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync(updatedChildDto);

            // Act
            var result = await _controller.CreateChild(childDto);

            // Assert
            var okResult = result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Child created successfully", apiResponse.Message);
            _eventPublisherMock.Verify(x => x.PublishChildCreated(childId, parentId, childDto.FullName), Times.Once());
        }

        [Fact]
        public async Task CreateChild_Should_Return_InternalServerError_When_Failed()
        {
            // Arrange
            var childDto = new ChildDTO(
                Id: null,
                ParentId: Guid.NewGuid(),
                FullName: "Test Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            var parentId = Guid.NewGuid();
            _parentIdCacheMock.SetupGet(x => x.ParentId).Returns(parentId);
            _childRepositoryMock.Setup(x => x.CreateChildAsync(It.IsAny<ChildDTO>()))
                .ReturnsAsync((new Response(false, "Failed to create child"), (Guid?)null));

            // Act
            var result = await _controller.CreateChild(childDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            var apiResponse = Assert.IsType<ApiResponse>(statusCodeResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Failed to create child", apiResponse.Message);
            _eventPublisherMock.Verify(x => x.PublishChildCreated(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<string>()), Times.Never());
        }

        #endregion

        #region GetChild Tests

        [Fact]
        public async Task GetChild_Should_Return_Ok_When_Child_Found()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: childId,
                ParentId: Guid.NewGuid(),
                FullName: "Test Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync(childDto);

            // Act
            var result = await _controller.GetChild(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(childDto, apiResponse.Data);
        }

        [Fact]
        public async Task GetChild_Should_Return_NotFound_When_Child_Not_Found()
        {
            // Arrange
            var childId = Guid.NewGuid();
            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync((ChildDTO?)null);

            // Act
            var result = await _controller.GetChild(childId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Child not found", apiResponse.Message);
        }

        #endregion

        #region UpdateChild Tests

        [Fact]
        public async Task UpdateChild_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: childId,
                ParentId: Guid.NewGuid(),
                FullName: "Updated Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            _childRepositoryMock.Setup(x => x.UpdateChildAsync(It.Is<ChildDTO>(dto => dto.Id == childId)))
                .ReturnsAsync(new Response(true, "Child updated successfully"));

            // Act
            var result = await _controller.UpdateChild(childId, childDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Child updated successfully", apiResponse.Message);
        }

        [Fact]
        public async Task UpdateChild_Should_Return_NotFound_When_Failed()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: childId,
                ParentId: Guid.NewGuid(),
                FullName: "Updated Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            _childRepositoryMock.Setup(x => x.UpdateChildAsync(It.IsAny<ChildDTO>()))
                .ReturnsAsync(new Response(false, "Child not found"));

            // Act
            var result = await _controller.UpdateChild(childId, childDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Child not found", apiResponse.Message);
        }

        #endregion

        #region DeleteChild Tests

        [Fact]
        public async Task DeleteChild_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var childId = Guid.NewGuid();
            _childRepositoryMock.Setup(x => x.DeleteChildAsync(childId))
                .ReturnsAsync(new Response(true, "Child deleted successfully"));

            // Act
            var result = await _controller.DeleteChild(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal("Child deleted successfully", apiResponse.Message);
        }

        [Fact]
        public async Task DeleteChild_Should_Return_NotFound_When_Failed()
        {
            // Arrange
            var childId = Guid.NewGuid();
            _childRepositoryMock.Setup(x => x.DeleteChildAsync(childId))
                .ReturnsAsync(new Response(false, "Child not found"));

            // Act
            var result = await _controller.DeleteChild(childId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Child not found", apiResponse.Message);
        }

        #endregion

        #region GetChildrenByParent Tests

        [Fact]
        public async Task GetChildrenByParent_Should_Return_Ok_With_Children()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            var children = new List<ChildDTO>
            {
                new ChildDTO(Guid.NewGuid(), parentId, "Child 1", DateTime.Now.AddYears(-1), "Male", 3.5m, 50m, null),
                new ChildDTO(Guid.NewGuid(), parentId, "Child 2", DateTime.Now.AddYears(-2), "Female", 3.0m, 48m, null)
            }.AsEnumerable(); // Ensure the return type is IEnumerable<ChildDTO>

            _childRepositoryMock.Setup(x => x.GetChildrenByParentAsync(parentId))
                .ReturnsAsync(children);

            // Act
            var result = await _controller.GetChildrenByParent(parentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDTO>>(apiResponse.Data);
            Assert.Equal(2, returnedChildren.Count());
        }

        [Fact]
        public async Task GetChildrenByParent_Should_Return_Ok_With_Empty_List_When_No_Children()
        {
            // Arrange
            var parentId = Guid.NewGuid();
            _childRepositoryMock.Setup(x => x.GetChildrenByParentAsync(parentId))
                .ReturnsAsync(Enumerable.Empty<ChildDTO>()); // Use Enumerable.Empty to match IEnumerable<ChildDTO>

            // Act
            var result = await _controller.GetChildrenByParent(parentId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            var returnedChildren = Assert.IsAssignableFrom<IEnumerable<ChildDTO>>(apiResponse.Data);
            Assert.Empty(returnedChildren);
        }

        #endregion

        #region GetChildBMI Tests

        [Fact]
        public async Task GetChildBMI_Should_Return_Ok_When_Child_Found()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: childId,
                ParentId: Guid.NewGuid(),
                FullName: "Test Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 50m,
                AvatarUrl: null
            );

            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync(childDto);
            _childRepositoryMock.Setup(x => x.CalculateBMI(childDto))
                .Returns((decimal?)14m); // Use decimal? to match the updated interface

            // Act
            var result = await _controller.GetChildBMI(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);

            var bmiData = Assert.IsType<BmiResult>(apiResponse.Data);
            Assert.Equal(14m, bmiData.BMI);
        }

        [Fact]
        public async Task GetChildBMI_Should_Return_NotFound_When_Child_Not_Found()
        {
            // Arrange
            var childId = Guid.NewGuid();
            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync((ChildDTO?)null);

            // Act
            var result = await _controller.GetChildBMI(childId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(notFoundResult.Value);
            Assert.False(apiResponse.Success);
            Assert.Equal("Child not found", apiResponse.Message);
        }

        [Fact]
        public async Task GetChildBMI_Should_Return_Ok_With_Null_BMI_When_Calculation_Fails()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var childDto = new ChildDTO(
                Id: childId,
                ParentId: Guid.NewGuid(),
                FullName: "Test Child",
                DateOfBirth: DateTime.Now.AddYears(-1),
                Gender: "Male",
                BirthWeight: 3.5m,
                BirthHeight: 0m, // This should cause CalculateBMI to return null
                AvatarUrl: null
            );

            _childRepositoryMock.Setup(x => x.GetChildAsync(childId))
                .ReturnsAsync(childDto);
            _childRepositoryMock.Setup(x => x.CalculateBMI(childDto))
                .Returns((decimal?)null); // Simulate the case where BMI cannot be calculated

            // Act
            var result = await _controller.GetChildBMI(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);

            var bmiData = Assert.IsType<BmiResult>(apiResponse.Data);
            Assert.Null(bmiData.BMI); // BMI should be null when calculation fails
        }

        #endregion

        #region GetGrowthAnalysis Tests

        [Fact]
        public async Task GetGrowthAnalysis_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var growthAnalysis = new GrowthAnalysis
            {
                ChildId = childId,
                BMI = 14m,
                Warning = "No issues detected"
            };

            _childRepositoryMock.Setup(x => x.AnalyzeGrowthAsync(childId))
                .ReturnsAsync(growthAnalysis);

            // Act
            var result = await _controller.GetGrowthAnalysis(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            Assert.Equal(growthAnalysis, apiResponse.Data);
        }

        [Fact]
        public async Task GetGrowthAnalysis_Should_Return_Ok_With_Warning_When_Issues_Detected()
        {
            // Arrange
            var childId = Guid.NewGuid();
            var growthAnalysis = new GrowthAnalysis
            {
                ChildId = childId,
                BMI = 10m,
                Warning = "Warning: Child may be underweight"
            };

            _childRepositoryMock.Setup(x => x.AnalyzeGrowthAsync(childId))
                .ReturnsAsync(growthAnalysis);

            // Act
            var result = await _controller.GetGrowthAnalysis(childId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var apiResponse = Assert.IsType<ApiResponse>(okResult.Value);
            Assert.True(apiResponse.Success);
            var returnedAnalysis = Assert.IsType<GrowthAnalysis>(apiResponse.Data);
            Assert.Equal("Warning: Child may be underweight", returnedAnalysis.Warning);
        }

        #endregion
    }
}