using HRAnalytics.BL;
using HRAnalytics.BL.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IJobBL, JobBL>();
builder.Services.AddScoped<ICandidateBL, CandidateBL>();

builder.Services.AddScoped<ICandidateScore, CandidateScoreBL>();
builder.Services.AddScoped<ISolverBL, SolverBL>();
builder.Services.AddScoped<ITopsis, Topsis>();

builder.Services.AddApplicationInsightsTelemetry(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]);



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
