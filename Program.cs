using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "v1";
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(version, new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WordParserApi",
        Version = version,
        Description = "API for generating Legal Act XML structure from Word Documents.",
        // Contact = new Microsoft.OpenApi.Models.OpenApiContact
        // {
        //     Name = "Your Name",
        //     Email = "your.email@example.com",
        //     Url = new Uri("https://yourwebsite.com")
        // }
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
