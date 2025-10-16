using AntiFraud.System.Api.Controllers;
using AntiFraud.System.Application.DependencyInjections;
using AntiFraud.System.Infrastructure.Context;
using AntiFraud.System.Infrastructure.DependencyInjections;
using AntiFraud.System.Infrastructure.Observability;
using AntiFraud.System.Infrastructure.Observability.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configura��o de Observabilidade (Tracing, Logs, M�tricas)
// Intercepta��o autom�tica de m�todos para rastreamento (Tracing)
builder.AddAntiFraudMethodTracingInterceptor(
    includeNamespaces: ["Masstransit", "FluentValidation", "AntiFraud"],
    excludeNamespaces: ["Swagger", "Swashbuckle"]);

// Configura��o de OTLP (OpenTelemetry) para rastreamento e logs
var otlpSettings = new OtlpSettings(builder.Environment.ApplicationName)
{
    Endpoint = builder.Configuration.GetConnectionString("Otlp") ?? throw new NullReferenceException("Otlp"),
    Attributes = new Dictionary<string, object>
    {
        ["deployment.environment"] = builder.Environment.EnvironmentName.ToLowerInvariant(),
    },
    MeterNames = [
        "Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Server.Kestrel",
        "Microsoft.AspNetCore.Http.Connections",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.AspNetCore.Diagnostics",
        "Microsoft.AspNetCore.RateLimiting"
    ],
    TracingSourceNames = ["AntiFraud*", "Masstransit"]
};

// Integra Serilog e Observabilidade via AntiFraud
builder.AddAntiFraudSerilogOtlp((context, service, logger) => logger.AddAntiFraudSerilogOtlp(otlpSettings));
builder.Services.AddAntiFraudObservabilityOtlp(otlpSettings);
#endregion

// --- Registro de Servi�os ---

// Servi�o de inicializa��o para ambiente de desenvolvimento
builder.Services.AddCustomHostedService();

// 1. Registra os servi�os das nossas camadas (Application e Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Adiciona os servi�os do ASP.NET Core
//    CORRE��O: Adicionada a configura��o para converter Enums em Strings no JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// 3. Configura��o correta do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Aplicar Migra��es Automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("Migra��es do banco de dados aplicadas com sucesso");
}
#endregion

// --- Configura��o do Pipeline HTTP ---


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#region Ciclo de Vida da Aplica��o
// Logs para eventos de ciclo de vida: Inicializa��o, Parada e Interrup��o

var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
lifetime.ApplicationStarted.Register(() => app.Logger.LogInformation("[Start:WebApi]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopping.Register(() => app.Logger.LogInformation("[Stopping:WebApi]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
lifetime.ApplicationStopped.Register(() => app.Logger.LogInformation("[Stopped:WebApi]-[{Environment}] - [{Application}]", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName));
#endregion

try
{
    await app.RunAsync().ConfigureAwait(false);
}
catch (Exception ex)
{
    app.Logger.LogCritical("[Error: WebApi]-[{Environment}] - [{Application}] Erro: {Error}", app.Environment.EnvironmentName, AppDomain.CurrentDomain.FriendlyName, ex.Message);
    throw;
}
