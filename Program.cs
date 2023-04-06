using ApiKeyAuthentication.Authentication;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => 
{
    // Auth method #2 using filters
    //opt.Filters.Add<ApiKeyAuthFilter>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options => 
{
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "The API Key to access the API.",
        In = ParameterLocation.Header,
        Name = "x-api-key",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "ApiKeyScheme" //Bearer
    });

    var scheme = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "ApiKey"
        },
        In = ParameterLocation.Header
    };
    var requirement = new OpenApiSecurityRequirement
    {
        { scheme, new List<string>() }
    };
    options.AddSecurityRequirement(requirement);

    // options.AddSecurityRequirement(new OpenApiSecurityRequirement
    // {
    //     {
    //         new OpenApiSecurityScheme
    //         {
    //             Name = "Bearer",
    //             In = ParameterLocation.Header,
    //             Reference = new OpenApiReference
    //             {
    //                 Id = "Bearer",
    //                 Type = ReferenceType.SecurityScheme
    //             }
    //         },
    //         new List<string>()
    //     }
    // });
});

// Auth method #3 using attributes
builder.Services.AddScoped<ApiKeyAuthFilter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Auth method #1 using global middleware
//app.UseMiddleware<ApiKeyAuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
