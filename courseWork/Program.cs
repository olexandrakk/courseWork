using courseWork.BLL.Profiles;
using courseWork.BLL.Services;
using courseWork.BLL.Services.Interfaces;
using courseWork.DAL.DBContext;
using courseWork.DAL.Repository;
using Microsoft.EntityFrameworkCore;
using ShopAPI.DAL.Repository;
using System;

var builder = WebApplication.CreateBuilder(args);

// AutoMapper
builder.Services.AddAutoMapper(typeof(AuthorProfile));

// Add services to the container.

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

//Service

builder.Services.AddScoped<IAuthorService, AuthorService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IBookStoreService, BookStoreService>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPublisherService, PublisherService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IUserService, UserService>();

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
