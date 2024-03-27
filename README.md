## Api Project
updates with additional interfaces and modified naming conventions. Adjustments were made to the GetCustomersAroundCustomerId method. 
## Unit Testing
Focused on the Skip List class. Mocks used for service and controller layers.
## Performance Testing with JMeter

* ID Table: Prevents "not found" errors.
* Boundary Checks: Negative start or end returns empty list, no exceptions.

### Load Testing

Steady performance up to 300 threads.( reportt3 folder )

### Stress Testing

* Concurrency Thread Group with increasing load.
* 100 threads: Stable with minimal errors. ( reportc1 folder )
* 500 threads: Increased errors, likely local windows machine limitations.( reportc5 folder )
* Tested on Linux and entry-level servers; local machine outperformed.

### Next Steps

* Implement CPU and memory monitoring.
* Error:Non HTTP response code.  Is the reason local machine limitations? What can I do?
* Comments are welcome. 
