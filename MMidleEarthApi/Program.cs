using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiddleEarth.Core.Repositories;
using MiddleEarth.Core.Services;
using MiddleEarth.Core.UnitOfWorks;
using MiddleEarth.Repository;
using MiddleEarth.Repository.Repositories;
using MiddleEarth.Repository.UnitOfWorks;
using MiddleEarth.Service.Mapping;
using MiddleEarth.Service.Services;
using MiddleEarth.Service.Validations;
using MMidleEarthApi.Filters;
using MMidleEarthApi.Milddlewares;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers(opt => opt.Filters.Add(new ValidateFilterAttribute())).AddFluentValidation(
    x=>x.RegisterValidatorsFromAssemblyContaining<CharacterDtoValidator>());

builder.Services.Configure<ApiBehaviorOptions>(opt =>
{
    opt.SuppressModelStateInvalidFilter = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IService<>), typeof(Service<>));

builder.Services.AddScoped<ICharacterRepository, CharacterRepository>();
builder.Services.AddScoped<ICharacterService, CharacterService>();

builder.Services.AddScoped<IRaceRepository, RaceRepository>();
builder.Services.AddScoped<IRaceService, RaceService>();

builder.Services.AddScoped<IRealmRepository, RealmRepository>();
builder.Services.AddScoped<IRealmService, RealmService>();

builder.Services.AddScoped<IWeaponRepository, WeaponRepository>();
builder.Services.AddScoped<IWeaponService, WeaponService>();

builder.Services.AddAutoMapper(typeof(MapProfile));

builder.Services.AddDbContext<AppDbContext>(x =>
{
    x.UseSqlServer(builder.Configuration.GetConnectionString("db"), opt =>
    {
        opt.MigrationsAssembly(Assembly.GetAssembly(typeof(AppDbContext)).GetName().Name);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomException();

app.UseAuthorization();

app.MapControllers();

app.Run();
