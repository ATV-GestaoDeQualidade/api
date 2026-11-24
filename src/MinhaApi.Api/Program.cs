using MinhaApi.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddApiDependencies();

var app = builder.Build();

app.UseExceptionHandler();
app.UseStatusCodePages();

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
