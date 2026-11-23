using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
builder.Services.Configure<DatqOptions>(builder.Configuration.GetSection("Datq"));
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

var app = builder.Build();

var queue = app.Services.GetRequiredService<IMessageQueue>();
bool ok = queue.Send(new byte[] { 1, 2, 3 });

Console.WriteLine($"Sent: {ok}");