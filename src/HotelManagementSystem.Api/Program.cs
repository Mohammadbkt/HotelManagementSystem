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


app.UseExceptionHandler();
app.UseStatusCodePages();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.MapScalarApiReference();
}

app.MapPost("/send-email", async (
    IEmailSender emailSender,
    ILogger<Program> logger,
    CancellationToken cancellationToken) =>
{
    try
    {
        await emailSender.SendAsync(
            EmailAddress.Create("2020980156@alumni.yu.edu.jo").Value,
            "SMTP test",
            "<p>This is a test email.</p>",
            cancellationToken);

        return Results.Ok(new { status = "sent" });
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Email test failed");

        return Results.Problem(
            title: "Email sending failed",
            detail: app.Environment.IsDevelopment() ? ex.ToString() : null);
    }
});


app.Run();

