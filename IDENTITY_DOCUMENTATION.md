# Identity (Authentication & Authorization) Documentation

This document provides a comprehensive overview of all Identity-related code and packages in the BookLendingSystem solution.

---

## Table of Contents
1. [NuGet Packages](#nuget-packages)
2. [Identity Models](#identity-models)
3. [Services](#services)
4. [Controllers](#controllers)
5. [DTOs (Data Transfer Objects)](#dtos-data-transfer-objects)
6. [Database Context](#database-context)
7. [Seed Data](#seed-data)
8. [Configuration](#configuration)
9. [Authorization Policies](#authorization-policies)
10. [JWT Settings](#jwt-settings)
11. [Hangfire Authorization](#hangfire-authorization)

---

## NuGet Packages

### Infrastructure Project (`BookLendingSystem.Infrastructure.csproj`)
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
```
- **Purpose**: Provides ASP.NET Core Identity framework with Entity Framework Core integration
- **Features**: User management, role management, password hashing, claims, tokens

### API Project (`BookLendingSystem.Api.csproj`)
```xml
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
```
- **Purpose**: JWT (JSON Web Token) bearer authentication middleware
- **Features**: Token validation, bearer token authentication scheme

---

## Identity Models

### ApplicationUser
**Location**: `/BookLendingSystem/Infrastructure/Identity/ApplicationUser.cs`

```csharp
using Microsoft.AspNetCore.Identity;

namespace BookLendingSystem.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        
    }
}
```

**Description**: 
- Extends `IdentityUser` from ASP.NET Core Identity
- Inherits properties: Id, UserName, Email, PasswordHash, SecurityStamp, etc.
- Can be extended with custom properties for user profile data

---

## Services

### 1. ITokenService Interface
**Location**: `/BookLendingSystem/Application/Interfaces/ITokenService.cs`

```csharp
namespace BookLendingSystem.Application.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateJwtTokenAsync(string userId);
    }
}
```

### 2. TokenService Implementation
**Location**: `/BookLendingSystem/Infrastructure/Services/TokenService.cs`

```csharp
public class TokenService : ITokenService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public async Task<string> GenerateJwtTokenAsync(string userId)
    {
        // Retrieves user
        // Creates claims (NameIdentifier, Email, Role claims)
        // Generates JWT token with configured settings
        // Returns signed JWT token string
    }
}
```

**Features**:
- Generates JWT tokens for authenticated users
- Includes user ID, email, and role claims
- Uses HS256 algorithm for signing
- Token expiration configured via appsettings

### 3. ICurrentUserService Interface
**Location**: `/BookLendingSystem/Application/Interfaces/ICurrentUserService.cs`

```csharp
namespace BookLendingSystem.Application.Interfaces
{
    public interface ICurrentUserService
    {
        string UserId { get; }
        bool IsAuthenticated { get; }
        bool IsInRole(string role);
    }
}
```

### 4. CurrentUserService Implementation
**Location**: `/BookLendingSystem/Infrastructure/Services/CurrentUserService.cs`

```csharp
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    public bool IsInRole(string role) => _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
}
```

**Features**:
- Provides access to current authenticated user information
- Extracts user ID from JWT claims
- Checks authentication status
- Role-based authorization support

---

## Controllers

### AuthController
**Location**: `/BookLendingSystem/Api/Controllers/AuthController.cs`

**Endpoints**:

#### 1. Register User
```csharp
[HttpPost("register")]
[AllowAnonymous]
public async Task<IActionResult> Register([FromBody] RegisterDTO model)
```
- Creates new user account
- Assigns "Member" role by default
- Validates email format and password strength

#### 2. Login User
```csharp
[HttpPost("login")]
[AllowAnonymous]
public async Task<IActionResult> Login([FromBody] LoginDTO model)
```
- Authenticates user credentials
- Returns JWT token on success
- Returns user email and roles

**Dependencies**:
- `UserManager<ApplicationUser>`
- `SignInManager<ApplicationUser>`
- `ITokenService`

### BaseApiController
**Location**: `/BookLendingSystem/Api/Controllers/BaseApiController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    protected string CurrentUserId => User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
}
```

**Purpose**: 
- Base class for all API controllers
- Provides convenient access to current user ID via claims

---

## DTOs (Data Transfer Objects)

### 1. RegisterDTO
**Location**: `/BookLendingSystem/Application/DTOs/RegisterDTO.cs`

```csharp
public class RegisterDTO
{
    [Required(ErrorMessage ="Email is required.")]
    [EmailAddress(ErrorMessage ="Invalid email format.")]
    [StringLength(100, ErrorMessage ="Email cannot exceed 100 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage ="Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{6,}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character")]
    public string Password { get; set; }
}
```

**Validation Rules**:
- Email: Required, valid format, max 100 characters
- Password: Required, 6-100 characters, must contain uppercase, lowercase, digit, and special character

### 2. LoginDTO
**Location**: `/BookLendingSystem/Application/DTOs/LoginDTO.cs`

```csharp
public class LoginDTO
{
    [Required(ErrorMessage ="Email is required.")]
    [EmailAddress(ErrorMessage ="Invalid email format.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    public string Password { get; set; }
}
```

### 3. AuthResponseDTO
**Location**: `/BookLendingSystem/Application/DTOs/AuthResponseDTO.cs`

```csharp
public class AuthResponseDTO
{
    public string Token { get; set; }
    public string Email { get; set; }
    public IEnumerable<string> Roles { get; set; }
}
```

**Purpose**: Response object returned after successful login

---

## Database Context

### ApplicationDbContext
**Location**: `/BookLendingSystem/Infrastructure/Persistence/ApplicationDbContext.cs`

```csharp
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Loan> Loans { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // Loan entity has foreign key to ApplicationUser
        builder.Entity<Loan>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(l => l.UserId)
            .IsRequired();
        
        // ... other configurations
    }
}
```

**Identity Tables Created**:
- AspNetUsers
- AspNetRoles
- AspNetUserRoles
- AspNetUserClaims
- AspNetUserLogins
- AspNetUserTokens
- AspNetRoleClaims

---

## Seed Data

### 1. RoleSeed
**Location**: `/BookLendingSystem/Infrastructure/Persistence/Seeds/RoleSeed.cs`

```csharp
public static class RoleSeed
{
    public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        if (!await roleManager.RoleExistsAsync("Member"))
        {
            await roleManager.CreateAsync(new IdentityRole("Member"));
        }
    }
}
```

**Roles Created**:
- **Admin**: Full system access, can manage books
- **Member**: Can borrow and return books, view their loans

### 2. UserSeed
**Location**: `/BookLendingSystem/Infrastructure/Persistence/Seeds/UserSeed.cs`

```csharp
public static class UserSeed
{
    public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
    {
        const string adminEmail = "admin@booklending.com";
        const string adminPassword = "Admin@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}
```

**Default Admin Account**:
- **Email**: admin@booklending.com
- **Password**: Admin@123
- **Role**: Admin

### 3. DatabaseSeeder
**Location**: `/BookLendingSystem/Infrastructure/Persistence/Seeds/DatabaseSeeder.cs`

```csharp
public class DatabaseSeeder
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<DatabaseSeeder> _logger;

    public async Task SeedAsync()
    {
        await RoleSeed.SeedAsync(_roleManager);
        await UserSeed.SeedAdminAsync(_userManager);
    }
}
```

**Purpose**: Orchestrates seeding of roles and admin user during application startup

---

## Configuration

### Dependency Injection (Infrastructure Layer)
**Location**: `/BookLendingSystem/Infrastructure/DependencyInjection.cs`

```csharp
public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
{
    // Database Context
    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

    // Identity Configuration
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.User.RequireUniqueEmail = true;

        // Password requirements
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

    // Services
    services.AddScoped<ICurrentUserService, CurrentUserService>();
    services.AddScoped<ITokenService, TokenService>();
    services.AddScoped<DatabaseSeeder>();
    
    // HttpContextAccessor for accessing user context
    services.AddHttpContextAccessor();

    return services;
}
```

### JWT Authentication Configuration (API Layer)
**Location**: `/BookLendingSystem/Api/Program.cs`

```csharp
// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]));

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
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = key
    };
});
```

**Authentication Middleware Order**:
```csharp
app.UseAuthentication();  // Must come before UseAuthorization
app.UseAuthorization();
```

---

## Authorization Policies

**Location**: `/BookLendingSystem/Api/Program.cs`

```csharp
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("MemberPolicy", policy => policy.RequireRole("Member"));
});
```

### Usage in Controllers

#### BookController
**Location**: `/BookLendingSystem/Api/Controllers/BookController.cs`

```csharp
[Authorize]  // Requires authentication for all actions
public class BookController : BaseApiController
{
    [AllowAnonymous]  // Public access
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetAll()
    
    [AllowAnonymous]  // Public access
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetById(Guid id)
    
    [Authorize(Roles = "Admin")]  // Admin only
    [HttpPost]
    public async Task<ActionResult<BookDto>> Create([FromBody] CreateBookDto dto)
    
    [Authorize(Roles = "Admin")]  // Admin only
    [HttpPut("{id}")]
    public async Task<ActionResult<BookDto>> Update(Guid id, [FromBody] UpdateBookDto dto)
    
    [Authorize(Roles = "Admin")]  // Admin only
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
}
```

#### LoanController
**Location**: `/BookLendingSystem/Api/Controllers/LoanController.cs`

```csharp
[Authorize]  // Requires authentication for all actions
public class LoanController : BaseApiController
{
    [Authorize(Roles = "Member")]  // Member only
    [HttpPost("borrow")]
    public async Task<ActionResult<LoanDto>> Borrow([FromBody] BorrowBookDto dto)
    
    [Authorize(Roles = "Member")]  // Member only
    [HttpPost("return/{loanId}")]
    public async Task<ActionResult<LoanDto>> Return(Guid loanId)
    
    [Authorize(Roles = "Member")]  // Member only
    [HttpGet("my-loans")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetUserLoans()
    
    [Authorize(Roles = "Admin")]  // Admin only
    [HttpGet("all")]
    public async Task<ActionResult<IEnumerable<LoanDto>>> GetAllLoans()
}
```

---

## JWT Settings

**Location**: `/BookLendingSystem/Api/appsettings.json`

```json
{
  "JwtSettings": {
    "Secret": "ThisIsASecretKeyThatMustBeAtLeast32CharactersLongForSecurity",
    "Issuer": "BookLendingSystem",
    "Audience": "BookLendingSystemUsers",
    "ExpiryMinutes": 60
  }
}
```

**Configuration Details**:
- **Secret**: Symmetric key used for signing tokens (minimum 32 characters)
- **Issuer**: Identifies who issued the token
- **Audience**: Identifies who the token is intended for
- **ExpiryMinutes**: Token validity period (60 minutes = 1 hour)

**Security Note**: The secret key should be stored securely (e.g., Azure Key Vault, AWS Secrets Manager) in production environments.

---

## Hangfire Authorization

**Location**: `/BookLendingSystem/Api/HangfireAuthorizationFilter.cs`

```csharp
public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
{
    private readonly IWebHostEnvironment _env;

    public HangfireAuthorizationFilter(IWebHostEnvironment env)
    {
        _env = env;
    }
    
    public bool Authorize(DashboardContext context)
    {
        var httpContext = context.GetHttpContext();

        // Allow anonymous access in development
        if (_env.IsDevelopment())
            return true;

        // Require authenticated Admin user in production
        return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Admin");
    }
}
```

**Purpose**: 
- Secures Hangfire dashboard (/hangfire endpoint)
- Development: Allows anonymous access
- Production: Requires authenticated Admin role

**Configuration in Program.cs**:
```csharp
builder.Services.AddSingleton<HangfireAuthorizationFilter>();

app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization = new[] { app.Services.GetRequiredService<HangfireAuthorizationFilter>() }
});
```

---

## Swagger/OpenAPI JWT Configuration

**Location**: `/BookLendingSystem/Api/Program.cs`

```csharp
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Book Lending System API", Version = "v1" });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\""
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
            new string[] {}
        }
    });
});
```

**Purpose**: Enables JWT authentication testing in Swagger UI with "Authorize" button

---

## Database Seeding During Startup

**Location**: `/BookLendingSystem/Api/Program.cs`

```csharp
// Database Migration and Seeding
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();

        // Apply migrations
        context.Database.Migrate();

        // Seed roles and admin user
        var seeder = services.GetRequiredService<DatabaseSeeder>();
        await seeder.SeedAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
    }
}
```

**Purpose**: 
- Automatically applies database migrations on startup
- Seeds default roles (Admin, Member)
- Creates default admin account

---

## Summary

### Key Features
1. **ASP.NET Core Identity**: User and role management
2. **JWT Authentication**: Stateless token-based authentication
3. **Role-Based Authorization**: Admin and Member roles with different permissions
4. **Password Policies**: Enforced complexity requirements
5. **Seed Data**: Default admin account and roles
6. **Swagger Integration**: JWT authentication support in API documentation
7. **Hangfire Security**: Dashboard protected by Admin role
8. **Current User Service**: Access to authenticated user information

### Authentication Flow
1. User registers via `/api/auth/register` (assigned Member role)
2. User logs in via `/api/auth/login` with email and password
3. Server validates credentials and generates JWT token
4. Client includes token in `Authorization: Bearer {token}` header
5. Server validates token on each request
6. Authorization checks role claims for protected endpoints

### Authorization Matrix

| Endpoint | Anonymous | Member | Admin |
|----------|-----------|--------|-------|
| GET /api/book | ✅ | ✅ | ✅ |
| GET /api/book/{id} | ✅ | ✅ | ✅ |
| POST /api/book | ❌ | ❌ | ✅ |
| PUT /api/book/{id} | ❌ | ❌ | ✅ |
| DELETE /api/book/{id} | ❌ | ❌ | ✅ |
| POST /api/loan/borrow | ❌ | ✅ | ❌ |
| POST /api/loan/return/{id} | ❌ | ✅ | ❌ |
| GET /api/loan/my-loans | ❌ | ✅ | ❌ |
| GET /api/loan/all | ❌ | ❌ | ✅ |
| POST /api/auth/register | ✅ | ✅ | ✅ |
| POST /api/auth/login | ✅ | ✅ | ✅ |

---

## Files Reference

### Complete File List

**Identity Models:**
- `/BookLendingSystem/Infrastructure/Identity/ApplicationUser.cs`

**Services:**
- `/BookLendingSystem/Application/Interfaces/ITokenService.cs`
- `/BookLendingSystem/Application/Interfaces/ICurrentUserService.cs`
- `/BookLendingSystem/Infrastructure/Services/TokenService.cs`
- `/BookLendingSystem/Infrastructure/Services/CurrentUserService.cs`

**Controllers:**
- `/BookLendingSystem/Api/Controllers/AuthController.cs`
- `/BookLendingSystem/Api/Controllers/BaseApiController.cs`
- `/BookLendingSystem/Api/Controllers/BookController.cs` (Authorization attributes)
- `/BookLendingSystem/Api/Controllers/LoanController.cs` (Authorization attributes)

**DTOs:**
- `/BookLendingSystem/Application/DTOs/RegisterDTO.cs`
- `/BookLendingSystem/Application/DTOs/LoginDTO.cs`
- `/BookLendingSystem/Application/DTOs/AuthResponseDTO.cs`

**Database:**
- `/BookLendingSystem/Infrastructure/Persistence/ApplicationDbContext.cs`

**Seed Data:**
- `/BookLendingSystem/Infrastructure/Persistence/Seeds/RoleSeed.cs`
- `/BookLendingSystem/Infrastructure/Persistence/Seeds/UserSeed.cs`
- `/BookLendingSystem/Infrastructure/Persistence/Seeds/DatabaseSeeder.cs`

**Configuration:**
- `/BookLendingSystem/Infrastructure/DependencyInjection.cs`
- `/BookLendingSystem/Api/Program.cs`
- `/BookLendingSystem/Api/appsettings.json` (JwtSettings section)

**Hangfire:**
- `/BookLendingSystem/Api/HangfireAuthorizationFilter.cs`

**Project Files:**
- `/BookLendingSystem/Infrastructure/BookLendingSystem.Infrastructure.csproj`
- `/BookLendingSystem/Api/BookLendingSystem.Api.csproj`

---

*Last Updated: 2025-12-10*
