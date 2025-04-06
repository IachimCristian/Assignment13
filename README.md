[![Review Assignment Due Date](https://classroom.github.com/assets/deadline-readme-button-22041afd0340ce965d47ae6ef1cefeee28c7c493a6346c4f15d667ab976d596c.svg)](https://classroom.github.com/a/ftQ_uu23)
# Assignment 9: Using Entity Framework, Repository, and Unit of Work with Generics

In this assignment, we will use Entity Framework for data persistence with SQLite. We will implement the **Repository** and **Unit of Work** patterns to manage database operations efficiently, while utilizing **generics** to make the repository system flexible and reusable.

---


## Part 1: Install Entity Framework and Set Up `DbContext`

We'll use Entity Framework for data persistence. To learn more about it, please check [here](https://learn.microsoft.com/en-us/aspnet/entity-framework).

### 1. Install Required Packages

We need to install some NuGet packages to work with EF and SQLite.
Run the following commands in the terminal:

```bash
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Tools
```


### 2. Define a Base Model

A base model ensures that all our database entities (tables) have common properties like an ID, a creation date, and an update date.

#### Define the `IDbItem` Interface:
```csharp
public interface IDbItem {
    Guid Id { get; set; }
    DateTime Created { get; set; }
    DateTime Updated { get; set; }
}
```

#### Implement `DbItem` Abstract Class:
```csharp
public abstract class DbItem : IDbItem {
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime Updated { get; set; } = DateTime.UtcNow;
}
```

This ensures that all database entities automatically get an ID and timestamps.


#### Create `DbServer` Entity:
```csharp
public class DbServer : DbItem {
    public ServerType ServerType { get; set; }
}
```


### 3. Create Your `InfraSimContext`

Entity Framework uses a class called DbContext to interact with the database.


- Create a class `InfraSimContext` and extend it from `DbContext`


- Add a property for storing the servers:

```csharp
public DbSet<DbServer> DbServers { get; set; }
```

- Override the `OnConfiguring` method to add your SQLite configuration

```charp
protected override void OnConfiguring(DbContextOptionsBuilder options)
{
options.UseSqlite("Data Source=InfraSim.db");
}
```

- Register teh context on `Program.cs` as the DbContext:

```csharp
builder.Services.AddDbContext<InfraSimContext>();
```

### 4. Apply Database Migrations


We need to create and apply migrations to set up the database schema.

- Install EF tools:
```bash
dotnet tool install --global dotnet-ef
```

- Create the migration
```bash
dotnet ef migrations add InitialCreate
```

- Apply the migration to create the database
```bash
dotnet ef database update
```

- Check if a Migrations folder appears in your project and if InfraSim.db is created.


<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 2: Create a Repository to Manage Database Entities

A repository is a class that handles data operations, so we don’t need to interact directly with the database in multiple places.

Instead of creating a repository for each entity, we will use generics to create a reusable repository. More information on generics can be found [here](https://learn.microsoft.com/en-us/dotnet/standard/generics/).

### 1. Create an `IRepository` Interface

- The declaration of a generic in the interface should be done as following, defining the generic entity as a DbItem:

```csharp
public interface IRepository<TEntity> where TEntity : DbItem{

}
```

- Add the following methods on the interface:
```csharp
    
    List<TEntity> GetAll();
    TEntity? Get(Guid id);
    void Insert(TEntity item);
    void Update(TEntity item);
    void Delete(TEntity item);
```

### 2. Implement the `Repository` Class

  
- The class should be created identifying the generic:

```csharp
public class Repository<TEntity>: IRepository<TEntity> where TEntity: DbItem
```

- Your Repository needs the db context to perform database operations. You need to pass the InfraSimContext within the constructor and hold it in a class variable.


- Implement the methods defined on the interface

  - GetAll 

  ```csharp
  return Context.Set<TEntity>().ToList();
  ```  
  
  - Get

  ```csharp
  return Context.Set<TEntity>()?.FirstOrDefault( x => x.Id == id );
  ```
  - Insert  
  
  ```csharp
  Context.Set<TEntity>().Add(item);
  ```  
  
  - Update  
  
  ```csharp
  Context.Entry(item).State = EntityState.Modified;
  ```  
  
  - Delete  
  
  ```csharp
  Context.Remove(item);
  ```

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 3: Implement a Generic Repository Factory

### 1. Create an `IRepositoryFactory` Interface

- Create a regular interface

```csharp
public interface IRepositoryFactory
```

- Define the generic in the method declaration. `TEntity` should be from an element that is storeable in our database:

```csharp
IRepository<TEntity> Create<TEntity>() where TEntity:DbItem
```

### 2. Implement the `RepositoryFactory` Class

- In order to handle the database entities, we need the context `InfraSimContext`. Pass it on constructor and hold it as a property

- Implement the create Method:

  ```csharp
  return new Repository<TEntity>(Context);
  ```

### 3. Register as Singleton for Dependency Injection

```csharp
builder.Services.AddSingleton<IRepositoryFactory, RepositoryFactory>();
```

<br>

### 🏁  Commit Your Changes
<br><br><br><br>


## Part 4: Interact with Database with Unit Of Work

The Unit of Work Pattern ensures that multiple database operations are treated as a single transaction.

### 1. Create an `IUnitOfWork` interface 

```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<TEntity>? GetRepository<TEntity>() where TEntity : DbItem;

    void Begin();
    void Commit();
    void Rollback();

    void SaveChanges();
}
```

### 2. Create the `UnitOfWork` class implementing the `IUnitOfWork`.

- You need a property for handling with the transactions:

```csharp
private IDbContextTransaction? Transaction { get; set; }
```

- Inject `RepositoryFactory` and `InfraSimContext` in the constructor:

```csharp
private IRepositoryFactory<DbItem> RepositoryFactory { get; set; }
private InfraSimContext Context { get; set; }

public UnitOfWork(InfraSimContext context, IRepositoryFactory<DbItem> repositoryFactory)
{
    Context = context;
    RepositoryFactory = repositoryFactory;
}
````

### 3. Implement the database operation methods in `UnitOfWork`

- Begin:
```csharp
Transaction = Context.Database.BeginTransaction();
```

- Commit:
```csharp
Transaction?.Commit();
Transaction = null;
```

- Rollback:
```csharp
Trasaction?.Rollback();
Transaction = null;
```

- Save Changes:
```csharp
Context.SaveChanges();
```

### 4. Hold references to repositories and create them on demand

- Create a dictionary property in `UnitOfWork`:

```csharp
private IDictionary<Type, IRepository<DbItem>> Repositories { get; } = new Dictionary<Type, IRepository<DbItem>>();
```

- Implement `GetRepository`, checking if the repository exists in the dictionary, and creating it if not:

```csharp
public IRepository<TEntity> GetRepository<TEntity>() where TEntity : DbItem
{
    if (!Repositories.ContainsKey(typeof(TEntity)))
    {
        Repositories[typeof(TEntity)] = RepositoryFactory.Create<TEntity>();
    }

    return (IRepository<TEntity>)Repositories[typeof(TEntity)];
}
```

<br>

### 🏁  Commit Your Changes
<br><br><br><br>

## Part 5: Verify with Unit Tests

We need to test two things: if a successful transaction is processed and if a failed transaction does not change the database.

### 1. Create a MemoryInfraSimContext class
- To perform tests without modifying the actual database, create an in-memory context by extending `InfraSimContext`:


```csharp
protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        SqliteConnection connection = new("DataSource=:memory:");
        connection.Open();

        options.UseSqlite(connection);
    }
```


### 2. Create the DatabaseTest class

**Important note:** Each unit test runs into a new instance. Meaning, before each unit test, the constructor will be called.

Use the constructor to perform the preparation for the test:

```csharp
public class DataBaseTest
{
    DbServer Server;
    InfraSimContext Context;
    IRepositoryFactory Factory;
    IUnitOfWork UnitOfWork;
    IRepository<DbServer> ServerRepository;
    public DataBaseTest()
    {
        

        Server = new DbServer {
            Id = Guid.NewGuid(),
            ServerType = ServerType.Server
        };
    
        Context = new MemoryInfraSimContext();
        Context.Database.EnsureCreated();
        
        Factory = new RepositoryFactory(Context);
        UnitOfWork = new UnitOfWork(Context, Factory);

        ServerRepository = UnitOfWork.GetRepository<DbServer>();


        UnitOfWork.Begin();
        ServerRepository.Insert(Server);
        
        UnitOfWork.SaveChanges();
    }
}
```

### 3. Implement the success transaction test

The constructor will prepare the context, create the repository and start the transaction.
Ensure in case of commit the server is stored.

```csharp
[Fact]
public void WhenAddingServersInDatabase_TheyAreStoredIfSuccess()
{
    
    UnitOfWork.Commit();

    var servers = ServerRepository.GetAll();
    Assert.Single(servers);
}
```

### 4. Implement the failing transaction test

The constructor will prepare the context, create the repository and start the transaction.
Ensure in case of rollback the server is not stored.

```csharp
[Fact]
public void WhenAddingServersInDatabase_TheyAreNotStoredIfFailed()
{
    UnitOfWork.Rollback();

    var servers = ServerRepository.GetAll();
    Assert.Empty(servers);
}
```

<br>

### 🏁  Commit Your Changes

<br><br><br><br>

# Final Reminder
⚠️ Don’t forget to push your code to the assignment repository once all parts are complete. This assignment is designed to create a database layer using Repository and UnitOfWork.

Good luck, and enjoy building your database structure with Entity Framework, Repository and UnitOfWork using generics!