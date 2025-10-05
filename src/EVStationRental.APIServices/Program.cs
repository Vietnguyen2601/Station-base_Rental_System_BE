using EVStationRental.Repositories.UnitOfWork;
using EVStationRental.Services.InternalServices.IServices.IAccountServices;
using EVStationRental.Services.InternalServices.Services.AccountServices;

var builder = WebApplication.CreateBuilder(args);

// ??ng k� Services 
builder.Services.AddScoped<IAccountService, AccountService>();


// ??ng k� UnitOfWork v� c�c Repository li�n quan
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<UnitOfWork>();


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
