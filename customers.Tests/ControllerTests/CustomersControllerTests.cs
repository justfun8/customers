using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.Controllers;
using customers.DataType;
using customers.Models;
using customers.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace customers.Tests.ControllerTests
{
    public class CustomersControllerTests
    {
        private readonly Mock<ICustomersService> _mockCustomersService;
        private readonly CustomersController _controller;

        public CustomersControllerTests()
        {
            _mockCustomersService = new Mock<ICustomersService>();
            _controller = new CustomersController(_mockCustomersService.Object);
        }

        [Fact]
        public void AddOrUpdate_Should_Add_CurrentScore()
        {
            // Arrange
            var customerId = 1L;
            var score = 100;
            var expectedScore = 100;
            _mockCustomersService
                .Setup(service => service.AddOrUpdate(customerId, score))
                .Returns(expectedScore);

            // Act
            // var result = _controller.AddOrUpdate(customerId, score) as OkObjectResult;

            // // Assert
            // Assert.NotNull(result);
            // var actualScore = (int)result.Value;
            // Assert.Equal(expectedScore, actualScore);
            var actionResult = _controller.AddOrUpdate(customerId, score);

            // 检查 ActionResult 是否成功并包含一个值
            Assert.IsType<OkObjectResult>(actionResult.Result);
            var okResult = actionResult.Result as OkObjectResult;

            Assert.NotNull(okResult);
            var actualScore = okResult.Value as int?;
            Assert.Equal(expectedScore, actualScore);
        }



        [Fact]
        public void AddOrUpdate_Should_Return_BadRequest_On_ArgumentOutOfRangeException()
        {
            // Arrange
            var customerId = 1L;
            var score = -1001; // Invalid score that would throw exception
            _mockCustomersService
                .Setup(service => service.AddOrUpdate(customerId, score))
                .Throws(new ArgumentOutOfRangeException());

            // Act
            var result = _controller.AddOrUpdate(customerId, score);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode); // BadRequest is 400
        }

        [Fact]
        public void Get_Should_Return_OkResult_With_Customers()
        {
            // Arrange
            var start = 1;
            var end = 10;
            var expectedCustomerDtos = new List<CustomerDto>
            {
                new CustomerDto
                {
                    CustomerId = 2,
                    Score = 20,
                    Rank = 4
                },
                new CustomerDto
                {
                    CustomerId = 8000,
                    Score = 20,
                    Rank = 5
                }
            };

            _mockCustomersService
                .Setup(service => service.GetCustomersByRange(start, end))
                .Returns(expectedCustomerDtos);

            // Act
            var result = _controller.Get(start, end);

            // Assert
            var okResult = result.Result as OkObjectResult;
            Assert.NotNull(okResult);
            Assert.Equal(200, okResult.StatusCode);

            var actualCustomerDtos = okResult.Value as List<CustomerDto>;
            Assert.NotNull(actualCustomerDtos);
            Assert.Equal(expectedCustomerDtos.Count, actualCustomerDtos.Count);
        }

        [Fact]
        public void Get_Should_Return_BadRequest_On_ArgumentOutOfRangeException()
        {
            // Arrange
            int? start = -5;
            int? end = 10;

            _mockCustomersService
                .Setup(service => service.GetCustomersByRange(start, end))
                .Throws(new ArgumentOutOfRangeException());

            // Act
            var result = _controller.Get(start, end);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public void Get_Should_Return_InternalServerError_On_Exception()
        {
            // Arrange
            int? start = 1;
            int? end = 10;

            _mockCustomersService
                .Setup(service => service.GetCustomersByRange(start, end))
                .Throws(new Exception());

            // Act
            var result = _controller.Get(start, end);

            // Assert
            var internalServerErrorResult = result.Result as ObjectResult;
            Assert.NotNull(internalServerErrorResult);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
        }

        // }
        [Fact]
        public void GetByID_Should_Return_Ok_With_Customers()
        {
            // Arrange
            int customerId = 5;
            int high = 10;
            int low = 1;
            var expectedCustomers = new List<CustomerDto>
            {
                new CustomerDto
                {
                    CustomerId = 3,
                    Score = 30,
                    Rank = 1
                },
                new CustomerDto
                {
                    CustomerId = 2,
                    Score = 20,
                    Rank = 2
                }
            };

            _mockCustomersService
                .Setup(service => service.GetCustomersAroundCustomerId(customerId, high, low))
                .Returns(expectedCustomers);

            // Act
            var result = _controller.Get(customerId, high, low);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedCustomers = Assert.IsAssignableFrom<List<CustomerDto>>(okResult.Value);
            Assert.Equal(expectedCustomers, returnedCustomers);
        }

        [Fact]
        public void GetByID_Should_Return_NotFound_When_KeyNotFoundException()
        {
            // Arrange
            int customerId = 5;
            int high = 10;
            int low = 1;

            _mockCustomersService
                .Setup(service => service.GetCustomersAroundCustomerId(customerId, high, low))
                .Throws(new KeyNotFoundException("Customer not found"));

            // Act
            var result = _controller.Get(customerId, high, low);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal("Customer not found", notFoundResult.Value);
        }


        [Fact]
        public void GetByID_Should_Return_InternalServerError_On_Exception()
        {
            // Arrange
            int customerId = 5;
            int high = 10;
            int low = 1;

            _mockCustomersService
                .Setup(service => service.GetCustomersAroundCustomerId(customerId, high, low))
                .Throws(new Exception());

            // Act
            var result = _controller.Get(customerId, high, low);

            // Assert
            var internalServerErrorResult = Assert.IsType<ObjectResult>(result.Result);
            Assert.Equal(500, internalServerErrorResult.StatusCode);
        }


    }
}
