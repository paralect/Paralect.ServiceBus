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



Another example:

```csharp
    var bus = ServiceBus.Run(c => c
        .SetServiceLocator(new StructureMapServiceLocator(container))
        .MemorySynchronousTransport()
        .SetName("Main Service Bus")
        .SetInputQueue("App.Server")
        .AddEndpoint(type => type.FullName.EndsWith("Event"), "App.Server")
        .AddEndpoint(type => type.FullName.EndsWith("Command"), "App.Server")
        .Dispatcher(d => d
            .AddHandlers(typeof(UserAR).Assembly)
            .AddHandlers(typeof(TopicAR).Assembly)
            .AddHandlers(typeof(AuthenticationService).Assembly)
            .AddHandlers(typeof(AccountDocument).Assembly)
            .AddHandlers(typeof(CreateUserCommand).Assembly)
            .AddHandlers(typeof(PermissionDocument).Assembly)
            .SetOrder(typeof(UserDocumentEventHandler), 
                      typeof(TopicDocumentEventHandler),
                      typeof(CommentDocumentEventHandler))
        )
    );
```    