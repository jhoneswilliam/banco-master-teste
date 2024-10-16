var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Api", Version = "v1" });
    c.OperationFilter<SwaggerFileOperationFilter>();
});

builder.Services.AddSingleton(new MapperConfiguration(mc =>
{
    mc.AddProfile(new AutoMapperDto());
}).CreateMapper());

builder.Services.AddScoped<ITravelCostFileRepository, TravelCostFileRepository>();
builder.Services.AddScoped<IHeuristicService, HeuristicService>();
builder.Services.AddScoped<ITravelService, TravelService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
