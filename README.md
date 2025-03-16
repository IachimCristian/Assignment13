[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/qpVdAqjX)
# Assignment 4: Traffic Routing with Unit Tests

In this assignment, you will create a simple simulation of an infrastructure built with servers that handle requests and a traffic routing component that distributes these requests. By the end of this assignment, you should be able to:
1.	Define interfaces to model a server and a traffic routing system.
2.	Implement a traffic routing class that distributes requests equally among servers.
3.	Create a unit tests project and use Moq to verify your implementation.



Our classwork will build upon the project you concluded last week, as we work to improve the capabilities of our Infrastructure Simulator.



## 1. Create the Core Interfaces and Classes

This assignment focuses on building a basic model that simulates an infrastructure comprised of servers and a traffic routing mechanism.

We begin by defining clear interfaces to outline the system’s responsibilities:
-  An interface named `IServer` is created with a method `HandleRequests(int requestsCount)`, which ensures that each server can process a given number of requests. 
- An `ITrafficRouting` interface is established with a method `RouterTraffic(int)`, designed to determine how incoming requests are allocated among the available servers.

![Layout](Images/Structure.png)

### 🏁  Commit Your Changes

## 2. **Implementing Traffic Routinf Logic**  

At this stage, the rules for traffic routing are straightforward:
- CalculateRequests:

Will return the total number of requests.

- ObtainServers:

Return the total list of servers.

- SendRequestsToServer:

Divide the total requests equally among the servers.

**Tip**: For 100 requests and 3 servers, each server should get 33 requests, and one server should handle 34 requests (to account for the remainder).

- RouteTraffic:

Should call the SendRequestsToServer with the defined rules:

```csharp
        int requests = CalculateRequests(requestsCount);
        List<IServer> servers = ObtainServers();
        SendRequestsToServers(requests, servers);
```



### 🏁  Commit Your Changes



## 3. Set Up the Unit Tests Project

At this stage is time to test your code and see if it works as expected. On opposite on how you have been testing until now, you don't have a way to test using the web interface, because we don't have a flow from the web interface with your TrafficRouting code.

We can do it by using Unit Tests. Unit tests are small tests that check the correctness of individual parts of your code, typically functions or methods.  

1. Verify your folder structure 

Before creating a tests structure, please verify if you have the following folder structure. If not, arrange as presented below:

```
\
| - InfraSim    <- Folder for InfraSim project
| - InfraSim.sln  <- File of the Solution

```

2. Create a new Tests Project

To create a test project, you’ll need to run the following command where your sln file is present.
```bash
dotnet new xunit -n InfraSim.Tests
```

Verify if your folder's structure is the following:

```
\
| - InfraSim    <- Folder for InfraSim project
| - InfraSim.Test. <-Folder for the InfraSim Test Project
| - InfraSim.sln  <- File of the Solution

```

3. Add your project to the solution:

``` bash
dotnet sln add InfraSim.Tests/InfraSim.Tests.csproj
```

Your sln file should now have reference for both projects.

4. Make sure it builds

```bash
dotnet build
```

5. Run the tests

```bash
dotnet test
```

It should say 1 test passed.

### 🏁  Commit Your Changes

## 4. Write Your First Unit Test

1. **Create a new test class**

Create your TrafficRoutingTests.cs in your tests project.

2. **Create your new test method**

```csharp
[Fact] 
 public void TestRequestCount_ShouldReturnCorrectRequestCount()
```

Note that `Fact` attribute indicates to the Xunit Framework that this is a Unit Test.

3. **Reference your InfraSim Project**

If you instantiate the `TrafficRouting`, you get an error! To access the `InfraSim` project in the test project, you must add a reference to the main project.

```bash
dotnet add InfraSim.Tests/InfraSim.Tests.csproj reference InfraSim/InfraSim.csproj
```

Verify if your `InfraSim.Tests.csproj` has a reference to the `InfraSim` project.


4. **Create your Unit Test**

Providing an example of your first test:

```csharp
    TrafficRouting trafficRouting = new TrafficRouting(new List<IServer>());
    Assert.Equal(100, trafficRouting.CalculateRequests(100));
```

The `Assert` will verify if the specific expectations are valid. If they aren't, the test will fail and will show in the test report.


### 🏁  Commit Your Changes


## 5. Enhance Test Coverage Using Moq

You will need to test all the methods of the TrafficRouting. 
For some of the tests, you’ll need IServer instances, which do not exist. You can achieve this using Moq.

1. **Add Moq to your Test Project**

Please add Moq to your Test project by running the following command. Go to your tests project and perform the following command:

```bash
dotnet add package Moq
```

2. **Create a Mock instance**

Now you can create a Mock instance of IServer as the following:

```csharp
Mock<IServer> server1 = new Mock<IServer>();
```

3. **Test your ObtainServers method**

Now, you can verify if `ObtainServers` are returning the expected servers.

4. **Test your Server distribution**

At last, check if the logic for server distribution is as expected.


**Tip** You can create a Verify on a mock server and check if a method is called with an expected parameter:

```csharp
mockServer1.Verify(s => s.HandleRequests(expectedRequestsPerServer1), Times.Once);
```

### 🏁  Commit Your Changes

## Final Reminder

**⚠️ Don't Forget:** Push your code to this assignment remote repository once you have completed all parts of the assignment. This assignment is designed to help you understand interface design, class implementation, and unit testing with mocking.

Good luck, and enjoy building your Traffic Routing Infrastructure! Use these guidelines to structure your solution, and feel free to experiment and ask questions as you work through the assignment.