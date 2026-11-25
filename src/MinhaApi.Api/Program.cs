using MinhaApi.Api.Extensions;
using MinhaApi.Infraestrutura.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDependencies(builder.Configuration);

var app = builder.Build();
await app.Services.MigrateDatabaseAsync();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
