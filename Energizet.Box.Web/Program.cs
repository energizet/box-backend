using Energizet.Box.Di;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddConfiguration(builder.Configuration);
builder.Services.AddDi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(policyBuilder =>
{
	policyBuilder
		.WithOrigins(
			"https://energizet.ru",
			"http://localhost:3000"
		)
		.AllowAnyHeader()
		.AllowAnyMethod();
});
app.UseStaticFiles();
app.MapFallbackToFile("index.html");

app.UseAuthorization();

app.MapControllers();

app.Run();