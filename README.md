# graphql-core
A wrapper library for GraphQL.NET that provides additional features to ease enterprise integration

[![Build status](https://ci.appveyor.com/api/projects/status/dptn1yq8wc65mmxs?svg=true)](https://ci.appveyor.com/project/Michannne/graphql-core)

GraphQL.NET can be found [here](https://github.com/graphql-dotnet/graphql-dotnet/)

This library is licensed under the [MIT License](https://licenses.nuget.org/MIT)

## Installation

The latest package can be downloaded through NuGet

```
PM> Install-Package GraphQL-Core
```

## Features

This library sits on top of GraphQL.NET to make it easier to integrate pre-existing C# models into a schema. A typical use case is providing a GraphQL interface as an alternative to a REST interface for a pre-existing API.

This library is designed with fluent API design to make it easy to maintain new GraphQL.NET configurations

The latest release supports the following features:
- [x] Types
- [x] Queries
- [x] Query stitching
- [x] Type stitching
- [x] Auto-conversion of models to GraphQL type
- [x] Auto-conversion of C# types to GraphQL type
- [x] Lambda queries
- [ ] Mutations
- [ ] Unions

Supported C# types for auto-conversion:
- [x] Any user model defined with .Type<Model>()
  
Instead of having to add several classes on top of pre-exising models, GraphQL-Core's main entrypoint -- `IGraphQLBuilder`, contains several features to ease integration of GraphQL.NET

### Adding user-defined models
User defined types can be added through the `.Type<TModel>()` method:

```csharp
services.AddGraphQL()
  .Type<Books>()
  .Type<Authors>()
  .Build()
```

This will iterate through the properties on the Type and generate GraphQL Type-nodes for the class, containing proper fields.
Notice the use of the `.Build()` method on the last line. This is necessary to create the nodes, and allows classes that reference other classes to be automatically resolved to the corresponding GraphQL Type

### Queries

Queries can be added through the `.Query<TResult>()` method:

```csharp
...after setting up the DataRepositories
services.AddGraphQL()
  .Type<Books>()
  .Type<Authors>()
  .Query<List<Authors>>(
    () => new Query(){
      Expression = "authors",
      Resolver = (context) => authorRepo.GetAll()
    }
  )
  .Build()
```

The `Query` class supports all parameters that can be passed into the `Field` method from GraphQL.NET, including arguments:

```csharp
services.AddGraphQL()
.Query<Author>(
    () => new Query()
    {
        Expression = "author",
        Args = new QueryArguments(
        new QueryArgument(typeof(long).GetGraphTypeFromType(nullable: false))
        {
            Name = "authorId",
            DefaultValue = 0,
            Description = "The Author to be searched"
        }),
        Resolver = (context) => authorRepo.GetAll().Where(a => a.AuthorId == (long)context.Arguments["authorId"])
    })
.Build()
```

### Query stitching

By default, GraphQL.NET does not support Query stitching, that is, chaining multiple queries in different files into one query. With GraphQL-Core, this behaviour is supported out-of-the-box:

```csharp
IServiceProvider serviceProvider = services.BuildServiceProvider();

services.AddSingleton<AuthorRepository>();
services.AddSingleton<BookRepository>();

AuthorRepository authorRepo = serviceProvider.GetService<AuthorRepository>();
BookRepository bookRepo = serviceProvider.GetService<BookRepository>();
services.AddGraphQL()
  .Type<Books>()
  .Type<Authors>()
  .Query<List<Authors>>(
    () => new Query(){
      Expression = "authors",
      Resolver = (context) => authorRepo.GetAll()
    }
  )
  .Query<List<Books>>(
    () => new Query(){
      Expression = "books",
      Resolver = (context) => bookRepo.GetAll()
    }
  )
  .Build()
```

In this case, `AddGraphQL()` returns an instance of `IGraphQLBuilder`, through which configurations can be chained. An alternative approach is to create seperate files to hold instances of `IGraphQLConfiguration` containing groups of types and queries.

```csharp
  public class LibraryConfiguration : IGraphQLConfiguration
  {
    public AuthorRepository authorRepo { get; set; }
    public BookRepository bookRepo { get; set; }
    
    public LibraryConfiguration(AuthorRepository _authorRepo, BookRepository _bookRepo)
    {
      authorRepo = _authorRepo;
      bookRepo = _bookRepo;
    }
    
    public IGraphQLBuilder Configure(IGraphQLBuilder builder) => builder
      .Type<Books>
      .Type<Authors>
      .Query<List<Authors>>(
        () => new Query(){
          Expression = "authors",
          Resolver = (context) => authorRepo.GetAll()
        }
      )
      .Query<List<Books>>(
        () => new Query(){
          Expression = "books",
          Resolver = (context) => bookRepo.GetAll()
        }
      );
  }
```

With this approach, `.Build()` should not be called, it will be called automatically by the `.AddGraphQL()` middleware

### Type stitching

Type stitching is a useful feature that allows additional fields to be attached to a GraphQL Type node. GraphQL-Core makes this process very simple to do:

```csharp
...after setting up the DataRepositories
services.AddGraphQL()
  .Type<Books>()
  .Type<Authors>()
  .Stitch<Author, List<Books>>(
    expr: "books",
    joinOn: a => a.AuthorId
    joinTo: () => (id) =>
      (context) => bookRepo.GetAll().Where(b => b.AuthorId == (long)id)
  )
  .Query<List<Authors>>(
    () => new Query(){
      Expression = "authors",
      Resolver = (context) => authorRepo.GetAll()
    }
  )
  .Build()
```

The `Author` type will have a property named `books` which, when resolved, returns the list of books for that author. This is very useful in cases where an opaque user-defined model does not have an implied relationship with another user-defined type, and neither type can be changed to add a new property. 

It should be noted this approach is *much* slower than a native join, such as `.Include()`, whicn injects JOIN statements in the SQL queries sent to the database. As such, it is recommended to use this only if absolutely required for usability purposes, such as consumers of the GraphQL API that cannot view the database Ids in order to query the `Books` table seperately, and do not want to make two atomic calls to the API.

### Auto-conversion of models

If you've noticed, nowhere is a seperate class being manually added to hold queries, mutations or types. This is because an internal class generator uses IL to create classes that act as containers for user-defined models at runtime. There are additional extension methods within the library that can convert C# types such as `List<TModel>` into `ListGraphType<GraphType<TModel>>` without intervention by the developer.

In addition, return types of queries can also be specifiec using C# types
