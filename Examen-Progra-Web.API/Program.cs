using System.Text;
using Examen_Progra_Web.API.Services;
using Examen_Progra_Web.API.Services.Interface;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

try
{
    var firebasePath = Path.Combine(AppContext.BaseDirectory, "Config", "firebase-credentials.json");
    if (File.Exists(firebasePath))
    {
        Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", firebasePath);
        builder.Services.AddSingleton(FirestoreDb.Create("examen-progra-web-1a261"));
    }
    else
    {
        Console.WriteLine("Advertencia: firebase-credentials.json no encontrado. Firestore no estará disponible.");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Error inicializando Firebase: {ex.Message}");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "ClaveSecreta123456789012345678901234567890")),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IJugadoresService, JugadoresService>();
builder.Services.AddSingleton<ITorneosService, TorneosService>();
builder.Services.AddSingleton<IJuegosService, JuegosService>();
builder.Services.AddSingleton<IParticipacionesService, ParticipacionesService>();
builder.Services.AddSingleton<ClasificacionesService>();
builder.Services.AddSingleton<ReportesService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
