using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

builder.Services.Configure<DatqOptions>(builder.Configuration.GetSection("Datq"));
builder.Services.Configure<AuditOptions>(builder.Configuration.GetSection("Audit"));

string? queueType = builder.Configuration["Queue:Type"];

switch (queueType)
{
    case "Datq":
        builder.Services.AddSingleton<IMessageQueue, DatqQueue>();
        break;
    case "Sim":
        builder.Services.AddSingleton<IMessageQueue, SimulatorQueue>();
        break;
    default:
        throw new InvalidOperationException($"Unknown Queue:Type '{queueType}'. Use 'Datq' or 'Sim'.");
}

builder.Services.AddSingleton<IAudit, FileAudit>();
builder.Services.AddSingleton<Session>();
builder.Services.AddSingleton<MessageSender>();

var app = builder.Build();

var session = app.Services.GetRequiredService<Session>();
var sender = app.Services.GetRequiredService<MessageSender>();

Console.WriteLine($"State: {session.State.Name}, sent: {sender.Send(new byte[] { 1, 2, 3 })}");

session.TransitionTo(new ConnectingState());
session.TransitionTo(new LoggingInState());
session.TransitionTo(new ActiveState());

Console.WriteLine($"State: {session.State.Name}, sent: {sender.Send(new byte[] { 1, 2, 3 })}");