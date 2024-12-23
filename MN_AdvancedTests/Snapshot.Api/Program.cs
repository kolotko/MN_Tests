using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Snapshot.Api.Dto;
using Snapshot.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<CustomerService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/customers", ([FromBody]CustomerRequestDto request, CustomerService customerService) =>
    {
        var customer = customerService.CreateCustomer(request);
        return TypedResults.CreatedAtRoute(request, "GetCustomers", new { id = customer.Id });
    })
    .WithName("GetCustomers")
    .WithOpenApi();

app.MapGet("customers/{id:int}", (int id) =>
    {
        return TypedResults.Ok(id);
    })
    .WithOpenApi();

app.Run();