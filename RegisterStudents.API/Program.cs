using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RegisterStudents.API;
using RegisterStudents.API.Configurations;
using RegisterStudents.API.Data;
using RegisterStudents.API.Models;
using RegisterStudents.API.Services;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Leer configuración JWT
var jwtSettingsSection = builder.Configuration.GetSection("JwtConfiguration");
builder.Services.Configure<JwtConfiguration>(jwtSettingsSection);
var jwtSettings = jwtSettingsSection.Get<JwtConfiguration>();
if (jwtSettings == null)
{
    throw new InvalidOperationException("JWT configuration is missing or invalid.");
}
// Agregar autenticación JWT con validación de roles
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero,

        // 👇 Configura cuál claim representa el rol del usuario
        RoleClaimType = ClaimTypes.Role
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Token inválido: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token válido para: " + context.Principal.Identity.Name);
            return Task.CompletedTask;
        }
    };
});

// Servicios
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<ITeacherService, TeacherService>();
builder.Services.AddScoped<ISubjectService, SubjectService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers()
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(System.Text.Unicode.UnicodeRanges.All);
});
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "RegisterStudents API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa el token JWT en este formato: Bearer {tu_token}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseCors("AllowAll");


//example users
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Crear rol Student si no existe
    var studentRole = context.Roles.FirstOrDefault(r => r.Name == "Student");
    if (studentRole == null)
    {
        studentRole = new Role { Name = "Student" };
        context.Roles.Add(studentRole);
        context.SaveChanges();
    }

    // Crear rol Admin si no existe
    var adminRole = context.Roles.FirstOrDefault(r => r.Name == "Admin");
    if (adminRole == null)
    {
        adminRole = new Role { Name = "Admin" };
        context.Roles.Add(adminRole);
        context.SaveChanges();
    }

    // Crear usuario estudiante si no existe
    if (!context.Users.Any(u => u.Email == "paola@email.com"))
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("miContraseñaSegura123");

        var studentUser = new User
        {
            FirstName = "Paola",
            LastName = "Gómez",
            Email = "paola@email.com",
            RoleId = studentRole.Id,
            PasswordHash = hashedPassword
        };

        context.Users.Add(studentUser);
        context.SaveChanges();

        var studentRecord = new Student
        {
            FirstName = "Paola",
            LastName = "Gómez",
            Email = "paola@email.com",
            Credits = 0,          // O el valor inicial que corresponda
            TeacherId = 1         // Debe ser un Id válido de un profesor existente
        };

        context.Students.Add(studentRecord);
        context.SaveChanges();
    }

    // Crear usuario admin si no existe
    if (!context.Users.Any(u => u.Email == "admin@email.com"))
    {
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword("AdminPassword123!");

        var adminUser = new User
        {
            FirstName = "Admin",
            LastName = "User",
            Email = "admin@email.com",
            RoleId = adminRole.Id,
            PasswordHash = hashedPassword
        };

        context.Users.Add(adminUser);
        context.SaveChanges();
    }
}


// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Welcome to RegisterStudents API!");

app.Run();
