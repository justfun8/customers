using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using customers.DataType;
using customers.Models;
using Xunit;
using Xunit.Abstractions;



namespace customers.Tests.DataTypeTests;

public class ThreadSafeSkipListTests
{
    [Fact]
    public void GetCustomersByRange_ShouldReturnCorrectCustomers_Concurrently()
    {
        // Arrange
        var skipList = new ThreadSafeSkipList();
        var threads = new List<Thread>();
        int numberOfOperations = 4; 

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
            },
            new CustomerDto
            {
                CustomerId = 1,
                Score = 10,
                Rank = 3
            }
        };

        for (int i = 1; i < numberOfOperations; i++)
        {
            int id = i; 
            int score = i * 10; 

            var thread = new Thread(() => AddCustomerData(skipList, id, score));
            threads.Add(thread);
            thread.Start();
        }
        foreach (var thread in threads)
        {
            thread.Join();
        }

        threads.Clear();

        int startRank = 1;
        int endRank = 4;

        List<CustomerDto> actualCustomers = null;

        for (int i = 0; i < numberOfOperations; i++)
        {
            var thread = new Thread(() =>
            {
                actualCustomers = skipList.GetCustomersByRange(startRank, endRank);
            });
            threads.Add(thread);
            thread.Start();
        }

        foreach (var thread in threads)
        {
            thread.Join();
        }

        // Assert
        AssertCustomerListsEqual(expectedCustomers, actualCustomers);
    }


    private void AssertCustomerListsEqual(
        List<CustomerDto> expectedCustomers,
        List<CustomerDto> actualCustomers
    )
    {

        for (int i = 0; i < actualCustomers.Count; i++)
        {
            Assert.Equal(expectedCustomers[i].CustomerId, actualCustomers[i].CustomerId);
            Assert.Equal(expectedCustomers[i].Score, actualCustomers[i].Score);
            Assert.Equal(expectedCustomers[i].Rank, actualCustomers[i].Rank);
        }
    }

    void AddCustomerData(ThreadSafeSkipList list, int id, int score)
    {
        list.AddOrUpdate(id, score);
    }
}

