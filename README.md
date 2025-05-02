[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/6vQaPKZ5)
# Assignment 10: Data Persistence with Data Mapper, Proxy, and Command

In this assignment, you will enhance your infrastructure simulator by handling database persistence and improving behavioral design through pattern implementation. This work will allow you to:

1.	Separate persistence and domain logic using the Data Mapper pattern.
2.	Intercept server operations using the Proxy pattern to ensure database consistency.
3.	Encapsulate operations using the Command pattern to add flexibility and support Undo/Redo functionality.

By the end of this assignment, you will have a persistent server infrastructure model that supports flexible command-based operations and cleanly separates business logic from data access.


## Part 1: Handling Persistence with Data Mapper


The Data Mapper pattern acts as a layer of separation between in-memory objects and the database. Instead of letting domain objects persist themselves, a mapper handles the data access logic and conversion between objects and database rows.

You’ll implement a ServerDataMapper to manage IServer instances’ storage and retrieval. This ensures that your IServer logic remains decoupled from database operations, leading to cleaner code and better testability.

### 1. Update the Server with a Unique Identifier

If we want to persist Servers, we need to have the identifier on the IServer so we can relate it to the DbServer in the database.

- Add the Id to the IServer

```csharp
Guid Id { get; set; }
```

- Update your IServerBuilder to support setting the Id.
- Ensure ServerBuilder assigns the Id correctly.
- Fix any errors resulting from the interface change.


### 2. Create the IServerDataMapper Interface 

Create your interface using GetAll, Get, Insert, and Remove.

```csharp
public interface IServerDataMapper
{
    List<IServer> GetAll();
    IServer? Get(Guid id);
    void Insert(IServer server);
    void Remove(IServer server);
}
```

### 3. Implement ServerDataMapper

- Inject IUnitOfWork and ICapabilityFactory via the constructor.

- Implement GetAll by retrieving DbServer items and transforming them into IServer using IServerBuilder.

- Implement remaining methods:
  - Get(Guid id)
  - Insert(IServer server)
  - Remove(IServer server) – be sure to retrieve the DbServer before deleting.
		
- Register ServerDataMapper as a singleton.


<br>

### 🏁  Commit Your Changes
<br><br><br><br>


## Part 2: Persisting Through a Proxy

The Proxy pattern provides a surrogate or placeholder for another object to control access to it. It can be used for lazy initialization, logging, or, as in this case, persistence.

You’ll create a ClusterProxy that wraps the original Cluster and intercepts changes to its server list. Any call to AddServeror RemoveServer will be forwarded to the proxy, which also performs the database operation via the ServerDataMapper.

This pattern lets you transparently handle database operations without modifying the original Cluster logic.

### 1. Extract Server List Management

- Create a new interface IServerList.

- Move relevant methods from ICluster into IServerList.

```csharp
public interface IServerList
  {
      List<IServer> Servers {get;}
      void AddServer(IServer server);
      void RemoveServer(IServer server);
  }
```

- Update ICluster to inherit both IServer and IServerList:

```csharp
public interface ICluster : IServer, IServerList {}

```

### 2. Create the proxy

- Create the ListServerProxy class which implements the IListServer

- Hold a reference to the real Cluster and the IServerDataMapper.

- Implement AddServer and RemoveServer:
  - Persist to DB using the ServerDataMapper.
  - Forward the call to the real cluster.



<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 3: Encapsulating Behavior with Command Pattern

The Command pattern encapsulates an operation (such as “Add Server”) as an object. This allows you to store, queue, and execute operations later, like undo or redo them.


You’ll implement AddServerCommand and RemoveServerCommand to encapsulate these changes. These commands will use the ClusterProxy to ensure persistence is handled automatically. You’ll also lay the groundwork for undo/redo functionality.



### 1. Create ICommand Interface

- Declare core command actions:

```csharp
void Do();
void Undo();
void Redo();
```


### 2. Implement AddServerCommand

- Create the class that implements ICommand

- Accept IListServer, IServer, and IServerDataMapper via constructor.

  - IListServer -> The Cluster we want to operate

  - IServer -> The Server we want to Add

  - IServerDataMapper -> For persistence purposes



- Wrap the IListServer in a ListServerProxy 
		
- Implement:
	- Do() → calls AddServer
	- Undo() → calls RemoveServer
	- Redo() → calls Do() again


### 3. Implement RemoveServerCommand

- Follow the same steps as AddServerCommand, but invert the logic:
  - Do() → calls RemoveServer
  - Undo() → calls AddServer

- Apply steps similar to the RemoveServerCommand as you did on the AddServerCommand.

<br>

### 🏁  Commit Your Changes
<br><br><br><br>


## Part 4: Creating the Command Manager

The Command Manager (also called an Invoker in Command Pattern terminology) is responsible for executing commands, keeping a history of executed commands, and managing undo/redo.


You’ll build a CommandManager that tracks executed commands, enabling users to step forward or back through server modifications.

### 1. Define ICommandManager Interface

- Create the interface with the following methods definition:

```csharp
bool HasUndo { get; }
bool HasRedo { get; }

void Execute(ICommand command);
void Undo();
void Redo();
```

### 2. Implement your CommandManager

- Use a list to track all executed commands.
	
- Keep a position pointer to manage undo/redo stack.

- In Execute, add command to the list and advance the pointer.

  - If it has redo, you'll need to clean the stack ahead:

```csharp
if (HasRedo)
{
  Commands.RemoveRange(Position + 1, Commands.Count - 1);
}
```

- In Undo, call Undo() on the current command and update the pointer.

```csharp
public void Undo()
{
    if(HasUndo)
    {
        Position--;
        Commands[Position].Undo();
        
    }
}
```

- In Redo, call Redo() on the next command and update pointer.

```csharp
public void Redo()
{
    if(HasRedo)
    
        Commands[Position].Redo();
        Position++;
    }
}
```

- In HasUndo, return if Position is grater or equal to 0.

- In HasRedo, reurn if Position is less than the last element.




### 3. Resgister as Singleton

Register your Command Manager as Singleton in the Program.cs file.

### 4. Update your Interface Mediator to execute the commands

- Pass the CommandManager in the constructor and hold it in a class property.

- Pass the ServerDataMapper in the constructor and hold it in a class property.

- Update the AddServer implementation to use the Command:

```csharp
public void AddServer(IServer server)
{
    switch (server.ServerType)
    {
        case ServerType.CDN:
        case ServerType.LoadBalancer:
            AddServerCommand addServerCommand = new AddServerCommand(Gateway, server, Mapper);
            CommandManager.Execute(addServerCommand);
            break;
        case ServerType.Cache:
        case ServerType.Server:
            addServerCommand = new AddServerCommand(Processors, server, Mapper);
            CommandManager.Execute(addServerCommand);
            break;
        
    }
}
```



<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 5: Unit Tests

- Fix your unit tests

- Generate the unit tests fpr the command manager. Select the CommandManager file and write `/tests` in the copilot prompt.

  - Verify if Do, Redo, Undo are performed has expected



<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Final Reminder

⚠️ Don’t Forget: Push your code to this assignment’s remote repository once you have completed all parts of the assignment. This exercise focuses on the behavioral and data access design patterns essential to building resilient and scalable systems.

By completing this assignment, you will:

- **Data Mapper** keeps your business logic clean.
- **Proxy** quietly enforces side effects.
- **Command** empowers flexible user-driven operations.

These principles are foundational for maintainable enterprise applications and systems architecture. Practice them and don’t hesitate to ask for guidance. 🚀

