using HotelManagementSystem.Api;
using HotelManagementSystem.Application.Common.Interfaces;
using HotelManagementSystem.Domain.ValueObjects.Email;
using HotelManagementSystem.Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApi(builder.Configuration);

var app = builder.Build();

await app.InitialiseDatabaseAsync(
    migrate: app.Environment.IsDevelopment());

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}


app.UseHttpsRedirection();

app.MapControllers();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/send-email", async (IEmailSender emailSender, CancellationToken cancellationToken ) =>
{
    await emailSender.SendAsync(EmailAddress.Create("barakatmoe@proton.me").Value, "test", "test", cancellationToken );
} 
);

app.Run();

