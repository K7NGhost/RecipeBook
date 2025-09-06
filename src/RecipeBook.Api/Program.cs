using RecipeBook.ServiceLibrary.Data;
using RecipeBook.ServiceLibrary.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Read client origin from config / env (override in appsettings or environment)
var clientOrigin = builder.Configuration["ClientOrigin"]
                   ?? Environment.GetEnvironmentVariable("CLIENT_ORIGIN")
                   ?? "https://localhost:61395";

// Build a small list that includes the http OR https variant so local dev works either way
var clientOrigins = new List<string> { clientOrigin };
if (clientOrigin.StartsWith("https://"))
{
    clientOrigins.Add("http://" + clientOrigin.Substring("https://".Length));
}
else if (clientOrigin.StartsWith("http://"))
{
    clientOrigins.Add("https://" + clientOrigin.Substring("http://".Length));
}

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// CORS: allow only the client origin(s)
builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientOnly", policy =>
    {
        policy.WithOrigins(clientOrigins.ToArray())
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// DI for service library
builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IRecipeRepository, RecipeRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// Apply the CORS policy (must come before UseAuthorization / MapControllers)
app.UseCors("ClientOnly");

app.UseAuthorization();

app.MapControllers();

app.Run();
