[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/1iXIjRpu)
# Assignment 11: UI Display with Adapter, Iterator & Visitor

In this assignment, we will enhance the infrastructure simulator's front end by wiring domain objects to the Blazor UI. We'll practise three design patterns: Adapter, Iterator, and Visitor.

Completing the exercise will let us:

- Adapt a domain model so the UI can bind directly to it (Adapter).

- Traverse composite structures safely (Iterator).

- Aggregate information over a heterogeneous structure (Visitor).

- Round everything off with Razor component updates, undo/redo buttons and total‑cost display.

## Part 1: Displaying our servers with Adapter

The Adapter pattern converts the interface of one class into another interface that clients expect. It is most useful when we reuse an existing class whose API does not match the one required by a consuming layer (UI, external library, test harness, etc.).

Our UI needs a simple model with a Name, an Icon and a Color that reflects the server's current state.

1. Create the `IServerInfo` with these properties.

```csharp
public interface IServerInfo
{
    public string Name { get; }
    public string ImageUrl { get; }
    public string StatusColor { get; }
}
```

2. Implement ServerInfoAdapter. It implements IServerInfo, receives an IServer in its constructor and stores it in a read‑only field/property.

3. Use the server type to return the name and icon.
Use the ones in this assignment or pick any from the web.

**Note**: Since .NET 8, we can use switch expressions instead of the traditional switch statements.
This is a good opportunity to apply a switch statement, here is an example:
```csharp
public string Name => Server.ServerType switch
        {
            ServerType.Server => "Server",
            ServerType.Cache => "Cache",
            ServerType.LoadBalancer => "Load Balancer",
            ServerType.CDN => "CDN",
            _ => throw new NotImplementedException()
        };

````


4. Calculate the status color based on its status.
Idle -> Gray
Normal -> Blue
Overloaded -> Orange
Failed -> Red

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 2: Navigating with Iterator

The Iterator pattern provides a uniform way to traverse elements of an aggregate object (a collection, tree, composite, …) without exposing its internal representation.

A cluster may contain individual servers and nested clusters, and we need to list all leaf servers, even those inside nested clusters. An iterator will give us a flat walk over the composite.



1. Create an IServerIterator with two properties: HasNext and Next, where HasNext is a boolean and Next returns an IServer.

2. Implement ServerIterator

- Create the ServerIterator class implementing IServerIterator

- Create a property to hold a list of IServer

- On the constructor, it will need to receive a Cluster and call a recursive method to retrieve the servers and store them in the list of servers. Here is an example of the recursive GetServers

```csharp
private List<IServer> GetServers(ICluster cluster) {
    var servers = new List<IServer>();
    
    cluster.Servers.ForEach(server => {
        if (server is Cluster) {
            servers.AddRange(GetServers((ICluster)server));
        }
        else {
            servers.Add(server);
        }
    });

    return servers;
}
```

- Create an integer property to indicate the index positions. You can name it `Position`.

3. Implement `HasNext` by comparing the `Position` with the Count of the List of servers.

4. Implement the `Next` by returning the `Server` at the current `Position` and incrementing it.

5. Add a property on the `IInfrastructureMediator` to return a new `Iterator`

On the IInfrastructureMediator
```csharp
IServerIterator CreateServerIterator();
```

And on the InfrastructureMediator:

```csharp
public IServerIterator CreateServerIterator()
{
    return new ServerIterator(Gateway);
}
```


<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 3: Load the Servers from the database

When loading the page, we need to retrieve the data from the database to display the existing servers.
We will return the clusters loaded with respective servers on the Server factory.

1. Update the `IServerFactory` and `ServerFactory` to support the different clusters:

```csharp
    public virtual ICluster CreateGatewayCluster();
    public virtual ICluster CreateProcessorsCluster();
```

2. Update the `ServerFactory` constructor to receive an `IServerDataMapper` on the constructor

3. Update the creation of the cluster's methods to receive the servers from the database and filter out the desired servers.
Remember, Gateway receives CDN and LoadBalancer. Processors receive Cache and Server.

**Note**: You may need a public `Servers` setter in `IServerList` and its implementers (`Cluster`, `ServerListProxy`).


<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 4: Implement the visitor to get a total cost

The Visitor pattern allows algorithms to be separated from the objects on which they operate. By moving the computation into a visitor, you can add new operations without changing the element classes.

Every IServer has a Capability with an associated cost. UI needs the aggregate cost of the entire infrastructure. 
Instead of adding a GetTotalCost() method to each server (violating SRP), we implement a CostCalculator visitor.
All servers implement Accept(IServerVisitor) which forwards the call to the visitor’s Visit(this).



1. Create an IServerVisitor 

- IServerVisitor will have a void Visit(IServer server)

2. Create an IServerAcceptVisit

- IServerAcceptVisit will have an accept method:

```csharp
void Accept(ISserverVisitor visitor);
```

3. Extent the IServer from IServerAcceptVisit

4. Add the implementation on the BaseServer.

```csharp
public void Accept(IServerVisitor visitor)
{
    visitor.Visit(this);
}
```

5. Create a CostCalculator implementing the IServerVisitor

- Create a class property TotalCost

```csharp
public int TotalCost { get; private set; }
```

- implement the Visit by incrementing the TotalCost with the Server's capability cost.

```csharp
public void Visit(IServer server)
{
    TotalCost += server.Capability.Cost;
}
```



6. In the IInfrastructureMediator create a TotalCost property.

7. Implement it in the InfrastructureMediator:
```csharp
public int TotalCost
    {
        get
        {
            IServerIterator serverIterator = CreateServerIterator();
            CostCalculator costCalculator = new CostCalculator();
            
            while(serverIterator.HasNext())
                serverIterator.Next().Accept(costCalculator);
            
            return costCalculator.TotalCost;
        }
    }
```

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 5: Update the razor components

Back to our web interface and  start bringing visibility to our Infrastructure simulator.

If you try to run the project as it is you might find the following issue:
```
System.AggregateException: Some services are not able to be constructed
```

This error is caused by Dependency Injection dependencies. 
Blazor will not start because scoped services are injected into singletons causing a lifetime mismatch. 
If a singleton service depends on a scoped service, then the singleton might accidentally share the state from one user’s circuit with others, breaking isolation and introducing bugs.


1. We need to update some of the singletons. The InfraSimContext is a Scoped and is injected into IInfrastriuctureMediator. 
So this last one cannot be a Singleton, but a scoped, as the others that depend on it:

```csharp
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddDbContext<InfraSimContext>();
builder.Services.AddSingleton<ICapabilityFactory, CapabilityFactory>();
builder.Services.AddScoped<IServerFactory, ServerFactory>();

builder.Services.AddScoped<IServerDataMapper, ServerDataMapper>();
builder.Services.AddSingleton<ICommandManager, CommandManager>();
builder.Services.AddScoped<IInfrastructureMediator, InfrastructureMediator>();
```

2. Update Actions.razor

- Inject the IServerFactory
```csharp
@inject IServerFactory ServerFactory
```

- Change the `AddPressed` to receive an IServer

```csharp
public EventCallback<IServer> AddPressed { get; set; }
```

- Create an Add Button for each kind of servers, and call the AddPressed with an instance for the specific server from the server factory:
```csharp
<div class="div-container">
    <button class="round-button green-button" @onclick="@StartPressed">Start</button>
</div>
<div class="div-container">
    <button class="round-button grey-button" @onclick="() => AddPressed.InvokeAsync(ServerFactory.CreateCDN())">Add CDN</button>
    <button class="round-button grey-button" @onclick="() => AddPressed.InvokeAsync(ServerFactory.CreateLoadBalancer())">Add LB</button>
</div>
<div class="div-container">
    <button class="round-button grey-button" @onclick="() => AddPressed.InvokeAsync(ServerFactory.CreateServer())">Add Server</button>
    <button class="round-button grey-button" @onclick="() => AddPressed.InvokeAsync(ServerFactory.CreateCache())">Add Cache</button>
</div>
```

3. Update Index/Home 

- Inject the InfrastructureMediator

```csharp
@inject IInfrastructureMediator InfrastructureMediator
```

- Maintain a local list of servers:

```csharp
List<IServer> Servers = new List<IServer>();
```


- Create a RefreshUI method to reload the ServerIterator, fill the list of servers from the iterator and call the StateHasChanged

```csharp

private void RefreshUI(){
    Servers = new List<IServer>();
    IServerIterator iterator = InfrastructureMediator.CreateServerIterator();
    while(iterator.HasNext){
        Servers.Add(iterator.Next);
    }
    StateHasChanged();
}
```

- Update the AddServer to receive the server as an argument, call the infrastructure mediator, and call the refresh UI

```csharp 
protected void AddServer(IServer server){
    InfrastructureMediator.AddServer(server);
    RefreshUI();
}
```

- Update the `OnInitialize` method to call the RefreshUI:

```csharp
    protected override void OnInitialized(){
        UserCounter.OnCounterChanged += StateHasChanged;
        RefreshUI();
    }
```



4. Update Server.razor

- Create a parameter to receive an IServerInfo
@code {
    [Parameter]
    public IServerInfo? ServerInfo { get; set; } 
}

- Create a new div to add the status element

```html
<div class="div-container">
    <img class="img-size" src=@ServerInfo?.ImageUrl />
    <div>
        <div class="status @ServerInfo?.StatusColor">&nbsp;</div>
        <span>@ServerInfo?.Name</span>
    </div>
    
</div>
```

- Create the following styles
```csharp
.status{
    width: 20px;
    height: 20px;
    border-radius: 10px;
}
.blue{
    background-color: blue;
}
.gray{
    background-color: gray;
}
.red{
    background-color: red;
}
.orange{
    background-color: orange;
}
```

- In the Home/Index, update the servers panel to pass the ServerInfo

```csharp
<div class="servers-panel">
    @foreach (IServer server in Servers){
        <Server ServerInfo="@(new ServerInfoAdapter(server))" />
    }
    
</div>
```

- Drop server icons into `wwwroot/images`



5. Display the infrastructure total cost

- On the Index/Home component add a span below the users' count with a reference for the value:

```csharp

<span>€ @InfrastructureMediator.TotalCost</span>
```

6. Add the Undo and Redo Options in the `Actions.razor`

- inject the CommandManager

```csharp
@inject ICommandManager CommandManager
```

- Add the new buttons

```csharp
<div class="div-container">
    
    <button disabled="@UndoDisabled" class="round-button grey-button" @onclick="Undo">Undo</button>
    <button disabled="@RedoDisabled" class="round-button grey-button" @onclick="Redo">Redo</button>
</div>
```

- Create a parameter to pass the refresh page action:

```csharp
[Parameter]
public EventCallback Refresh { get; set; }
```

- Implement the Undo/Redo logic:
```csharp
   void Redo()
    {
        CommandManager.Redo();
        Refresh.InvokeAsync();
    }
    void Undo()
    {
        CommandManager.Undo();
        Refresh.InvokeAsync();
    }
```

- Create the Visibility properties for the buttons:

```csharp
public bool UndoDisabled => !CommandManager.HasUndo;
public bool RedoDisabled => !CommandManager.HasRedo;
```

- In the Index/Home component, pass the RefreshUI to the Actions' Refresh property

```csharp
<Actions StartPressed="StartIncrementing" AddPressed="AddServer" Refresh="RefreshUI" />
```


7. Smoke Tests

- Add each server type.
- Reload the browser—the servers should still be there.
- Verify Undo and Redo stack.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>




## Final Reminder

⚠️  Don't Forget: Push your code to this assignment's remote repository once you have completed all parts of the assignment. This exercise focuses on gathering the elements for being represented on the UI.

By finishing Assignment 11 you will have:

- **Adapter** to decouple domain objects from UI needs.
- **Iterator** to flatten composite structures safely.
- **Visitor** to aggregate metrics (cost!) across heterogeneous nodes.

These patterns keep the infrastructure simulator maintainable and extensible. Enjoy and reach out for help whenever you need it. 🚀

