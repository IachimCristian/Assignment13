[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/BNj4BeP0)
# Assignment 8: Builder, Factory Method, Mediator, Singleton and Dependency Injection

In this assignment, you will extend your previous infrastructure simulation by designing and implementing a flexible and modular server infrastructure. This work will give you the opportunity to:

- Use the Builder Pattern for Server Creation

- Applying Singleton and Dependency Injection

- Leveraging the Factory Method Pattern

- Starting with the Mediator Pattern for Infrastructure Management

By the end of this assignment, you will have a well structure and testable code to create your servers and shape your infrastructure.

## Part 1: Create and build your server class.

- Create your Server class and extend from the BaseServer.

As you can see there is a few arguments that needs to be passed. You can simplify its creation with the Builder Pattern.


- Create the IServerBuilder with the methods to hold paramenters for the server.
  - Type
  - Capability
  - State

  
- Implement ServerBuilder that:
  - Stores default values for ServerType, ServerCapability, and ServerState.
  - Implements the builder methods, returning itself (this) to allow method chaining.
  - Includes a Build() method to instantiate and return a Server object.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 2: Use Singleton and Dependency Injection


We will obtain server capabilities from CapabilitiesFactory. .NET provides a simple way to create singletons.

- Register IServerCapability as a singleton in Program.cs.

```csharp
builder.Services.AddSingleton<IServerCapability, ServerCapability>();
```



<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 3: Implement Factory Method for Server and Cluster Creation

- Create your `IServerFactory` with different methods for each `ServerType` 

- Create the concrete class `ServerFactory` implementing IServerFactory
  
- Use your previous Builder to generate the expected servers

- The creation of the several Servers instance are very similar. Create a private method to avoid duplicated code.

**Tip:** Inject the IServerCapability in order to optain the desired capability.

The injection is performed within the constructor.
Example:

```csharp
    public ServerFactory(ICapabilityFactory capabilityFactory)
    {
        CapabilityFactory = capabilityFactory;
    }
```

**Hint:** Cluster will have a different creation. 

- Register the new factory as singleton.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

# Part 4: Implement the Infrastructure Mediator Pattern

The infrastructure must handle multiple aspects, like adding servers and processing clients requests. We'll use the Mediator Pattern.

- Create the IInfrastructureMediator and the InfrastructureMediator class.

- Implement InfrastructureMediator to manage server additions

- Create the initial structure in the constructor, representing the two clusters from previous assignment.

![Cluster](Images/Cluster.png)

```csharp
public InfrastructureMediator(ServerFactory serverFactory)
{
    Gateway = serverFactory.CreateCluster();
    Processors = serverFactory.CreateCluster();
    Gateway.AddServer(Processors);
}
```

- Handle server addition following the representation from the last assignment. 

```csharp
public void AddServer(IServer server)
{
    switch (server.ServerType)
    {
        case ServerType.CDN:
        case ServerType.LoadBalancer:
            Gateway.AddServer(server);
            break;
        case ServerType.Cache:
        case ServerType.Server:
            Processors.AddServer(server);
            break;
        
    }`
}
```

- Register it as singleton for Dependency Injection usage.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 5: Write Unit Tests

1. Create the tests for the Server Builder

**Tip** 
1. With the file selected, enter `/test` on your copilot prompt
2. Select ApplyEdit
3. Save in your InfraSim.Tests project
4. You might need to fix some parts of the generated code.

2. Verify on your Infraastructur Mediator, if the servers are being added to the right Cluster.


**Tip** Use the following prompt on copilot: `/tests creating a Moq for the ClusterFactory`

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

# Final Reminder

⚠️ Don’t Forget: Push your code to this assignment’s remote repository once you have completed all parts of the assignment. This exercise is designed to strengthen your understanding of key software architecture principles, including the Builder, Factory Method, Singleton, and Mediator design patterns.

By completing this assignment, you will enhance your skills in modular system design, dependency injection, and unit testing. all essential for building scalable and maintainable software

Good luck, and enjoy building your server infrastructure system! Follow the guidelines, experiment with different implementations, and don’t hesitate to ask questions as you refine your solution. 🚀