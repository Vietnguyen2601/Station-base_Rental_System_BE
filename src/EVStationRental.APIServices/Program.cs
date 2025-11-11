using EVStationRental.Common.Middleware.Validation;
using EVStationRental.Common.Enums.EnumModel;
using EVStationRental.Repositories.DBContext;
using EVStationRental.Repositories.IRepositories;
using EVStationRental.Repositories.Repositories;
using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.InternalServices.IServices.IAccountServices;
using EVStationRental.Services.InternalServices.IServices.IAuthServices;
using EVStationRental.Services.InternalServices.IServices.IVehicleServices;
using EVStationRental.Services.InternalServices.IServices.IStationServices;
using EVStationRental.Services.InternalServices.IServices.IPromotionServices;
using EVStationRental.Services.InternalServices.IServices.IReportServices;
using EVStationRental.Services.InternalServices.IServices.IOrderServices;
using EVStationRental.Services.InternalServices.IServices.IPaymentServices;
using EVStationRental.Services.InternalServices.IServices.IWalletServices;
using EVStationRental.Services.InternalServices.IServices.IDashboardServices;
using EVStationRental.Services.InternalServices.Services.AccountServices;
using EVStationRental.Services.InternalServices.Services.AuthServices;
using EVStationRental.Services.InternalServices.Services.VehicleServices;
using EVStationRental.Services.InternalServices.Services.StationServices;
using EVStationRental.Services.InternalServices.Services.PromotionServices;
using EVStationRental.Services.InternalServices.Services.ReportServices;
using EVStationRental.Services.InternalServices.Services.OrderServices;
using EVStationRental.Services.InternalServices.Services.PaymentServices;
using EVStationRental.Services.InternalServices.Services.PaymentServices;
using EVStationRental.Services.InternalServices.Services.WalletServices;
using EVStationRental.Services.InternalServices.Services.DashboardServices;
using EVStationRental.Services.ExternalService.IServices;
using EVStationRental.Services.ExternalService.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Security.Claims;
using System.Text;
using EVStationRental.Services.InternalServices.IServices.IFeedbackServices;
using EVStationRental.Services.InternalServices.Services.FeedbackServices;
using EVStationRental.Services.InternalServices.IServices.IDamageReportServices;
using EVStationRental.Services.InternalServices.Services.DamageReportServices;

var builder = WebApplication.CreateBuilder(args);

// IMPORTANT: Reset and Map PostgreSQL Enums
try
{
    // Reset any previous mappings
    Npgsql.NpgsqlConnection.GlobalTypeMapper.Reset();
    
    // Map enums with proper names
    Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<EVStationRental.Common.Enums.EnumModel.OrderStatus>("order_status", 
        nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
    Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<EVStationRental.Common.Enums.EnumModel.DamageLevelEnum>("damage_level_enum",
        nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
    Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<EVStationRental.Common.Enums.EnumModel.VehicleStatus>("vehicle_status",
        nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
    Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<EVStationRental.Common.Enums.EnumModel.PaymentType>("payment_type_enum",
        nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
    Npgsql.NpgsqlConnection.GlobalTypeMapper.MapEnum<EVStationRental.Common.Enums.EnumModel.TransactionType>("transaction_type_enum",
        nameTranslator: new Npgsql.NameTranslation.NpgsqlNullNameTranslator());
}
catch (Exception ex)
{
    Console.WriteLine($"[ENUM MAPPING ERROR] {ex.Message}");
}

// Đăng ký Services 
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IAuthService, AuthService>();
//vehicle
builder.Services.AddScoped<IVehicleService, VehicleService>();
builder.Services.AddScoped<IVehicleRepository, VehicleRepository>();
//Vehicle Model
builder.Services.AddScoped<IVehicleModelRepository, VehicleModelRepository>();
builder.Services.AddScoped<IVehicleModelService, VehicleModelService>();
//Vehicle Type
builder.Services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
builder.Services.AddScoped<IVehicleTypeServices, VehicleTypeServices>();
//station
builder.Services.AddScoped<IStationService, StationService>();
builder.Services.AddScoped<IStationRepository, StationRepository>();
//promotion
builder.Services.AddScoped<IPromotionService, PromotionService>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
//report
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
//damage report
builder.Services.AddScoped<IDamageReportRepository, DamageReportRepository>();
builder.Services.AddScoped<IDamageReportService, DamageReportService>();
//order
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
//feedback
builder.Services.AddScoped<IFeedbackRepository, FeedbackRepository>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
//payment
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
builder.Services.AddScoped<IVNPayService, VNPayService>();
builder.Services.AddScoped<DatabasePaymentService>(); // Add database payment service
//wallet
builder.Services.AddScoped<IWalletRepository, WalletRepository>();
builder.Services.AddScoped<IWalletService, WalletService>();
//dashboard
builder.Services.AddScoped<IDashboardService, DashboardService>();

// Đăng ký UnitOfWork và các Repository liên quan
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UnitOfWork>();
builder.Services.AddDbContext<ElectricVehicleDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<IAuthRepository, AuthRepository>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "EVStationRental API", Version = "v1" });
    var securityScheme = new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    };
    c.AddSecurityDefinition("Bearer", securityScheme);
    var securityRequirement = new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
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
            new string[] {}
        }
    };
    c.AddSecurityRequirement(securityRequirement);
});

// JWT Authentication
var jwtSection = builder.Configuration.GetSection("Jwt");
var secret = jwtSection["Secret"];
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnChallenge = context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    message = "Bạn không có quyền truy cập."
                });

                return context.Response.WriteAsync(result);
            },
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"[JWT ERROR] {context.Exception}");
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Request validation (JSON/content-type/size)
app.UseCors("AllowAll");
app.UseRouting();
app.UseRequestValidation();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
