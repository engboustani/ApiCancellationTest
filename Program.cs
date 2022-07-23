var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
app.UseStaticFiles();

app.MapGet("/cancellable", async (ILogger<Program> logger, CancellationToken cancellationToken) =>
    {
        var result = await Task.Run(() =>
        {
            for (int i = 0; i < 20; i++)
            {
                // If cancellation token requested with aborting the request
                if (cancellationToken.IsCancellationRequested)
                    return new StatusDTO("Canceled");

                Thread.Sleep(1000);
                logger.LogInformation($"Step number #{i}");
            }
            return new StatusDTO("Done");
        }, cancellationToken);
        return result;
    })
.WithName("GetCancellable");

app.Run();

public record StatusDTO(string Message);