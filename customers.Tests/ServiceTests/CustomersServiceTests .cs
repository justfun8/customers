using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.DataType;
using customers.Models;
using customers.Services;
using Moq;
using Xunit;

namespace customers.Tests.ServiceTests;

public class CustomersServiceTests
{
    private readonly Mock<IThreadSafeSkipList> _mockSkipList;
    private readonly CustomersService _service;

    public CustomersServiceTests()
    {
        _mockSkipList = new Mock<IThreadSafeSkipList>();
        _service = new CustomersService(_mockSkipList.Object);
    }

    [Fact]
    public void AddOrUpdate_WithValidScoreAndCustomerId_ShouldReturnExpectedValue()
    {
        // Arrange
        long customerId = 1;
        int score = 100;
        int expectedValue = 100;
        _mockSkipList.Setup(sl => sl.AddOrUpdate(customerId, score)).Returns(expectedValue);

        // Act
        var result = _service.AddOrUpdate(customerId, score);

        // Assert
        Assert.Equal(expectedValue, result);
    }

    [Fact]
    public void GetCustomersByRange_ValidRange_ShouldReturnCorrectCustomers()
    {
        // Arrange
        int start = 1,
            end = 2; 
        var customersInRange = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = 1, Score = 1000,Rank=1 },
            new CustomerDto { CustomerId = 2, Score = 200 ,Rank=2},
        };

        _mockSkipList
            .Setup(m => m.GetCustomersByRange(It.IsAny<int?>(), It.IsAny<int?>()))
            .Returns(customersInRange);

        // Act
        var result = _service.GetCustomersByRange(start, end);

        // Assert
        Assert.Equal(customersInRange.Count, result.Count);
        for (int i = 0; i < customersInRange.Count; i++)
        {
            Assert.Equal(customersInRange[i].CustomerId, result[i].CustomerId);
            Assert.Equal(customersInRange[i].Score, result[i].Score);
            Assert.Equal(customersInRange[i].Rank, result[i].Rank);
        }
    }

    [Theory]
    [InlineData(1L, 0, 10)]
    [InlineData(5L, 3, 4)]
    public void GetCustomersAroundCustomerId_ShouldReturnCorrectCustomers(long customerId, int high, int low)
    {
        // Arrange
        var expectedCustomers = new List<CustomerDto>
        {
            new CustomerDto { CustomerId = customerId, Score = 100, Rank = 1 },
        };

        _mockSkipList.Setup(m => m.GetCustomersAroundCustomerId(customerId, high, low))
            .Returns(expectedCustomers);

        // Act
        List<CustomerDto> actualCustomers = _service.GetCustomersAroundCustomerId(customerId, high, low);

        // Assert
        Assert.NotNull(actualCustomers);
        Assert.Equal(expectedCustomers.Count, actualCustomers.Count);
        for (int i = 0; i < expectedCustomers.Count; i++)
        {
            Assert.Equal(expectedCustomers[i].CustomerId, actualCustomers[i].CustomerId);
            Assert.Equal(expectedCustomers[i].Score, actualCustomers[i].Score);
            Assert.Equal(expectedCustomers[i].Rank, actualCustomers[i].Rank);
        }
    }
}

