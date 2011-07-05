ServiceBus
==========

Usage
-----

```csharp
    var bus = ServiceBus.Run(c => c
        .SetServiceLocator(new UnityServiceLocator(unity))
        .SetInputQueue("InputQueue")
        .AddEndpoint("Paralect.ServiceBus.Test.Messages", "SomeQueue")
        .Dispatcher(d => d
            .AddHandlers(Assembly.GetExecutingAssembly())
        )
    );
```