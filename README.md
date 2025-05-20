[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/Vmb41GP6)
# Assigment 12 Infrastructure Simulator Conclusion with Strategy, Visitor, Observer and Chain of Responsibility

In this assignment, we will close our cycle on our infrastructure simulator. We’ll practice four more design patterns: **Strategy**, **Visitor**, **Observer**, and **Chain of Responsibility**.

Completing the exercise will let us:

* Validate the infrastructure based on different strategies (Strategy).
* Calculate if the infrastructure is operational (Visitor).
* React to user changes (Observer).
* Route traffic efficiently through our system (Chain of Responsibility).



## Part 1: Validating if the Infrastructure is Ok with Strategy

We need to check if our infrastructure can continue receiving more users and requests to ensure it is ok. The validation will be different if it is a Server or a Cluster.

We define different validation strategies (e.g., for servers, gateways, processors) and assign them to each component. This way, each type of infrastructure node can have its own validation logic without modifying the node classes.

The Strategy pattern allows the selection of an algorithm's behavior at runtime. By encapsulating each validation rule separately, it promotes the open/closed principle.

1. Create `IValidatorStrategy` with a validate method, that returns a boolean.
```csharp
public interface IValidatorStrategy
{
    bool Validate(IServer server);
}
```

2. Create `ServerValidator` implementeing the `IValidatorStrategy`

- Return true if `Server.State` is not `FailedState`.

3. Create the `GatewayValidator` implementing the `IValidatorStrategy`

- Return true if the gateway has **at least one CDN** and **one LoadBalancer**.

4. Create `ProcessorsValidator` implementing `IValidatorStrategy`

- Return true if the cluster has **at least one Cache** and **one Server**.

5. Add a `Validator` property of type `IValidatorStrategy` to `IServer` and implement it in `BaseServer`.

6. Modify the `Server` and `Cluster` constructors to accept and store an `IValidatorStrategy`.

7. Update the builder to accept a strategy, setting the default to `ServerValidator`.

8. Update `IServerFactory` and `ServerFactory` to pass the appropriate validator when creating servers or clusters.



<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 2: Using Visitor to Calculate Infrastructure Health

The Visitor pattern lets you perform operations across a structure of objects without modifying the classes on which it operates. It separates data structures from behavior.

We use the same visitor structure from the last assignment to evaluate each server's health. This allows us to decouple the validation logic from the server structure and compute the centralized infrastructure state.

1. Create `StatusCalculator` implementing `IServerVisitor`.

2. Add a `bool IsOK { get; private set; } = true` property.

3. In the `Visit` method, update IsOK:

```csharp
IsOk = IsOk && server.Validator.Validate(server);
```

4. Add an `IsOK` property in `IInfrastructureMediator`.

5. Implement `IsOK` in `InfrastructureMediator`.

- Use a similar approach to the TotalCost

- Use the iterator for the servers

- Check the GatewayCluster and ProcessorCluster

6. In `UserCounter`, cancel the increment if the infrastructure is not ok:

- Add a private `bool Canceled` field.

- In the increment loop, check `Canceled` and break if true.

- Reset `Canceled` at the start of incrementing.

- Add a `Cancel()` method to set `Canceled = true`.

7. In `Home/Index`, call `InfrastructureMediator.IsOK` inside `OnCounterChanged`. If false, call `UserCounter.Cancel()`.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 3: Observing User Count to Generate Traffic

We’ll notify components when the user count changes using the **Observer pattern**.

The Observer pattern defines a one-to-many dependency between objects so that its dependents are notified automatically when one changes state.

We use observers to allow components, like the infrastructure mediator, to react to user count and changes in real-time, enabling dynamic updates and traffic simulation.

1. Create `IObserver` with:

```csharp 
void Update(int users);
```

2. Create `ISubject` with:

```csharp
void RegisterObserver(IObserver observer);
void RemoveObserver(IObserver observer);
void NotifyObservers();
```

3. Have `UserCounter` implement `ISubject`:
- Create a List<IObserver>
- Register by adding an observer to the list
- Remove by removing from taht list
- Notify by calling update on each of the elements of the list

4. Call `NotifyObservers()` on every user increment step.

5. Make `IInfrastructureMediator` extend `IObserver`.

6. Implement `Update(int users)` in `InfrastructureMediator` (leave the body empty for now).

7. In `Home/Index`, register the mediator in `OnInitialized`:
```csharp
UserCounter.RegisterObserver(InfrastructureMediator);
```


### 🏁  Commit Your Changes
<br><br><br><br>

## Part 4: Distributing Traffic with Chain of Responsibility

We’ll simulate traffic delivery with the **Chain of Responsibility** pattern.

This pattern allows passing requests along a chain of handlers, where each handler decides to process the request, and pass it to the next handler.

We use it to route traffic dynamically through our system, CDNs, Load Balancers, Caches, and Servers, each handling as much traffic as possible.

Using the defined TrafficRouting we will call it sequentially in the following order:

    1. CDNTrafficRouting
    2. FullTrafficRouting (LoadBalancer)
    3. CacheTrafficRouting
    4. FullTrafficRouting (Server)


1. Create `ITrafficDelivery`:
```csharp
    void SetNext(ITrafficDelivery nextHandler);
    void DeliverRequests(long requestCount);
```

2. Create abstract `TrafficDelivery` implementing `ITrafficDelivery`.
* Add a `protected ITrafficDelivery? NextHandler`.
* Implement `SetNext()` to store the next handler.
* Leave `DeliverRequests()` abstract.

3. Have `TrafficRouting` extend `TrafficDelivery`:
```csharp
public abstract class TrafficRouting : TrafficDelivery, ITrafficRouting
```

4. Implement `DeliverRequests` like this:

```csharp

public override void DeliverRequests(long requestCount)
{
    RouteTraffic(requestCount);
    long remainingRequests = requestCount - CalculateRequests(requestCount);
    if (remainingRequests > 0)
    {
        NextHandler?.DeliverRequests(remainingRequests);
    } else {
        NextHandler?.DeliverRequests(requestCount);
    }
}
```

5. Ensure all request counters use `long` to avoid overflow.

6. In `InfrastructureMediator`, create a `GetDeliveryChain` method:

- Create your delivery chain:
```csharp
ITrafficDelivery CDNDeliveryChain = new CDNTrafficRouting(Gateway.Servers);
ITrafficDelivery LBDeliveryChain = new FullTrafficRouting(Gateway.Servers, ServerType.LoadBalancer);
ITrafficDelivery CacheDeliveryChain = new CacheTrafficRouting(Processors.Servers);
ITrafficDelivery ServerDeliveryChain = new FullTrafficRouting(Processors.Servers, ServerType.Server);
```

- Set the chain order
```csharp
CDNDeliveryChain.SetNext(LBDeliveryChain);
LBDeliveryChain.SetNext(CacheDeliveryChain);
CacheDeliveryChain.SetNext(ServerDeliveryChain);        
```

- return the top element in the chain
```csharp
return CDNDeliveryChain;
```

6. Implement the `Update` method in `InfrastructureMediator`. Let's assume that each user generate 4 requests.

* Multiply `users * 4` to get `requestCount`.
* Get the chain via `GetDeliveryChain()`.
* Call `DeliverRequests(requestCount)`.

<br>

### 🏁 Commit Your Changes

<br><br><br><br>

## Part 5: Play with it

1. Try to reach **200,000 users**.

2. In a file `Resolution.txt`, record:

   * The maximum user count reached.
   * The final total cost.


<br>

### 🏁 Commit Your Changes

<br><br><br><br>

## Final Reminder

⚠️ Don’t Forget: Push your code to this assignment’s remote repository once you have completed all parts of the assignment. 

By completing this assignment you will have:

* **Strategy** to handle validation logic for servers.
* **Visitor** to evaluate if the infrastructure is operational.
* **Observer** to react to dynamic changes.
* **Chain of Responsibility** to simulate traffic flow.

These patterns will ensure our infrastructure simulator remains robust and scalable. Great job and, keep building! 🚀
