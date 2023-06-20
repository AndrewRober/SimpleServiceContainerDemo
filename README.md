# Simple Dependency Injection Framework Demo

This repository demonstrates a basic implementation of a dependency injection (DI) framework called Simple Service Container (SSC). DI frameworks help manage dependencies between components, promote loose coupling, and enhance the testability of your code.

## How the DI Framework Works

The Simple Service Container provides a lightweight DI solution, allowing you to register and resolve dependencies. It supports three registration types:

1. **Transient**: A new instance is created each time the service is requested.
2. **Singleton**: A single shared instance is created and reused for the lifetime of the container.
3. **Scoped**: A new instance is created once per scope.

The container resolves dependencies by creating instances of the requested types and their associated dependencies.

## Usage

To use the Simple Service Container, follow these steps:

1. Create an instance of the `SimpleServiceContainer` class.
2. Register the dependencies using the `RegisterTransient`, `RegisterSingleton`, or `RegisterScoped` methods, as appropriate.
3. Create instances of the registered types using the `CreateInstance` method or retrieve instances within a scope using the `GetService` method.

## Example

In the demo, we have a set of sample classes (A, B, C, D, E, and F) with various relationships. The `Program` class demonstrates how to register these dependencies in the container and create instances of the types.

```csharp
public class A : BaseEntity, IA
{
    public IB B { get; }

    public A(IB b)
    {
        B = b;
    }
}

public class B : BaseEntity, IB
{
    public IC C { get; }
    public ID D { get; }

    public B(IC c, ID d)
    {
        C = c;
        D = d;
    }
}

public class C : BaseEntity, IC { }

public class D : BaseEntity, ID { }

public class E : BaseEntity { }

public class F : BaseEntity { }
```

```csharp
static void Main(string[] args)
        {
            var container = new SimpleServiceContainer();

            container.RegisterTransient<ID, D>();
            container.RegisterTransient<IC, C>();
            container.RegisterTransient<IB, B>();
            container.RegisterTransient<IA, A>();

            container.RegisterSingleton<E>();

            container.RegisterScoped<F>();

            var a1 = container.CreateInstance<IA>();
            var a2 = container.CreateInstance<IA>();

            var e1 = container.CreateInstance<E>();
            var e2 = container.CreateInstance<E>();

            Console.WriteLine($"[Singleton] e1 {e1}");
            Console.WriteLine($"[Singleton] e2 {e2}");
            Console.WriteLine();
            Console.WriteLine($"[Transient] a1 {a1}");
            Console.WriteLine($"[Transient] a2 {a2}");

            Console.WriteLine();
            using (var scope1 = container.CreateScope())
            {
                Console.WriteLine("-----=====Scope 1 begins=====-----");
                var f1 = scope1.GetService(typeof(F));
                var f2 = scope1.GetService(typeof(F));

                Console.WriteLine($"[Scoped] F1 {f1}");
                Console.WriteLine($"[Scoped] F2 {f2}");
                Console.WriteLine("-----=====Scope 1  ends=====-----");
            }

            Console.WriteLine();

            using (var scope1 = container.CreateScope())
            {
                Console.WriteLine("-----=====Scope 2 begins=====-----");
                var f1 = scope1.GetService(typeof(F));
                var f2 = scope1.GetService(typeof(F));

                Console.WriteLine($"[Scoped] F1 {f1}");
                Console.WriteLine($"[Scoped] F2 {f2}");
                Console.WriteLine("-----=====Scope 2  ends=====-----");
            }
```

### The output of the example demonstrates the different behavior of transient, singleton, and scoped instances:

**Singleton:**\
`e1 43abfe81-371c-45cb-bde1-68d950558d8d`\
`e2 43abfe81-371c-45cb-bde1-68d950558d8d`

**Transient:**\
`a1 b880fbb7-0bfd-4c4e-91ed-e8369a0816e4`\
`a2 dbfabf33-59cc-42b8-bc54-c4bc79d69aec`

**Scope 1 begins:**\
**Scoped:**\
`F1 13edb850-5fb6-40e4-bbcf-6f1af2600275`\
`F2 13edb850-5fb6-40e4-bbcf-6f1af2600275`\
**Scope 1 ends**

**Scope 2 begins:**\
**Scoped:**\
`F1 b19713ae-1004-4b15-b6cf-d2fdebfeaad1`\
`F2 b19713ae-1004-4b15-b6cf-d2fdebfeaad1`\
**Scope 2 ends**
