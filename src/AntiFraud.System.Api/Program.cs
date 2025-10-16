using AntiFraud.System.Api.Controllers;
using AntiFraud.System.Application.DependencyInjections;
using AntiFraud.System.Infrastructure.Context;
using AntiFraud.System.Infrastructure.DependencyInjections;
using AntiFraud.System.Infrastructure.Observability;
using AntiFraud.System.Infrastructure.Observability.Settings;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

#region Configuração de Observabilidade (Tracing, Logs, Métricas)
// Interceptação automática de métodos para rastreamento (Tracing)
builder.AddAntiFraudMethodTracingInterceptor(
    includeNamespaces: ["Masstransit", "FluentValidation", "AntiFraud"],
    excludeNamespaces: ["Swagger", "Swashbuckle"]);

// Configuração de OTLP (OpenTelemetry) para rastreamento e logs
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

// --- Registro de Serviços ---

// Serviço de inicialização para ambiente de desenvolvimento
builder.Services.AddCustomHostedService();

// 1. Registra os serviços das nossas camadas (Application e Infrastructure)
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// 2. Adiciona os serviços do ASP.NET Core
//    CORREÇÃO: Adicionada a configuração para converter Enums em Strings no JSON.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });


// 3. Configuração correta do Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

#region Aplicar Migrações Automaticamente
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
    app.Logger.LogInformation("Migrações do banco de dados aplicadas com sucesso");
}
#endregion

// --- Configuração do Pipeline HTTP ---


app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#region Ciclo de Vida da Aplicação
// Logs para eventos de ciclo de vida: Inicialização, Parada e Interrupção

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
