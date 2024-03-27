using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.DataType;
using customers.Models;
using Xunit;
using Xunit.Abstractions;

namespace customers.Tests.DataTypeTests;

public class SkipListTests
{

    private readonly ITestOutputHelper output;
    private readonly ThreadSafeSkipList skipList; 


    public SkipListTests(ITestOutputHelper output)
    {
        this.output = output;
        this.skipList = new ThreadSafeSkipList(); 
        skipList.AddOrUpdate(1, 10);
        skipList.AddOrUpdate(4, 40);
        skipList.AddOrUpdate(1, 10);
        skipList.AddOrUpdate(2, 20);
        skipList.AddOrUpdate(8000, 20);
        skipList.AddOrUpdate(3, 30);
    }

    private void AssertCustomerListsEqual(
        List<CustomerDto> expectedCustomers,
        List<CustomerDto> actualCustomers
    )
    {

        for (int i = 0; i < actualCustomers.Count; i++)
        {
            // AssertCustomerDtoEqual(expectedCustomers[i], actualCustomers[i]);
            Assert.Equal(expectedCustomers[i].CustomerId, actualCustomers[i].CustomerId);
            Assert.Equal(expectedCustomers[i].Score, actualCustomers[i].Score);
            Assert.Equal(expectedCustomers[i].Rank, actualCustomers[i].Rank);
        }
    }

    // private void AssertCustomerDtoEqual(CustomerDto expected, CustomerDto actual)
    // {
    //     output.WriteLine(expected.CustomerId + " " + actual.CustomerId);
    //     output.WriteLine(expected.Score + " " + actual.Score);
    //     output.WriteLine(expected.Rank + " " + actual.Rank);

    //     Assert.Equal(expected.CustomerId, actual.CustomerId);
    //     Assert.Equal(expected.Score, actual.Score);
    //     Assert.Equal(expected.Rank, actual.Rank);
    // }

    [Fact]
    public void GetCustomersByRange_ShouldReturnCorrectCustomers()
    {
        List<CustomerDto> expectedCustomers = new List<CustomerDto>
        {
            new CustomerDto
            {
                CustomerId = 4,
                Score = 40,
                Rank = 1
            },
            new CustomerDto
            {
                CustomerId = 3,
                Score = 30,
                Rank = 2
            },
            new CustomerDto
            {
                CustomerId = 1,
                Score = 20,
                Rank = 3
            }
        };
        int startRank = 1;
        int endRank = 3;
        // Act
        List<CustomerDto> actualCustomers = skipList.GetCustomersByRange(startRank, endRank);
        // Assert
        AssertCustomerListsEqual(expectedCustomers, actualCustomers);
    }

    [Fact]
    public void GetCustomersByRange_endOutofBounds_ShouldReturnCorrectCustomers()
    {
        List<CustomerDto> expectedCustomers = new List<CustomerDto>
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
        int startRank = 4;
        int endRank = 30;
        // Act
        List<CustomerDto> actualCustomers = skipList.GetCustomersByRange(startRank, endRank);
        // Assert
        AssertCustomerListsEqual(expectedCustomers, actualCustomers);
    }

    [Fact]
    public void GetCustomersAroundCustomerId_ValidId_ShouldReturnCorrectCustomers()
    {
        // Arrange
        List<CustomerDto> expectedCustomers = new List<CustomerDto>
        {
            new CustomerDto
            {
                CustomerId = 1,
                Score = 20,
                Rank = 3
            },
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
        var targetCustomerId = 2;
        var high = 1;
        var low = 1;
        // Act
        var actualCustomers = skipList.GetCustomersAroundCustomerId(targetCustomerId, high, low);
        foreach (var customer in actualCustomers)
        {
            output.WriteLine($"ID: {customer.CustomerId}, Score: {customer.Score},{customer.Rank}");
        }

        // Assert
        AssertCustomerListsEqual(expectedCustomers, actualCustomers);
    }

    [Fact]
    public void GetCustomersAroundCustomerId_ValidId_endOutofBounds_ShouldReturnCorrectCustomers()
    {
        // Arrange
        List<CustomerDto> expectedCustomers = new List<CustomerDto>
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
        var targetCustomerId = 8000;
        var high = 1;
        var low = 1;
        // Act
        var actualCustomers = skipList.GetCustomersAroundCustomerId(targetCustomerId, high, low);
        foreach (var customer in actualCustomers)
        {
            output.WriteLine($"ID: {customer.CustomerId}, Score: {customer.Score},{customer.Rank}");
        }

        // Assert
        AssertCustomerListsEqual(expectedCustomers, actualCustomers);
    }
}
