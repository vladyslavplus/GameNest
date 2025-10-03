using FluentValidation;
using GameNest.ReviewsService.Api.Middlewares;
using GameNest.ReviewsService.Application.Behaviors;
using GameNest.ReviewsService.Application.Commands.CommentCommands.AddReply;
using GameNest.ReviewsService.Application.Services;
using GameNest.ReviewsService.Application.Validators.Comments;
using GameNest.ReviewsService.Domain.Interfaces.Repositories;
using GameNest.ReviewsService.Domain.Interfaces.Services;
using GameNest.ReviewsService.Infrastructure.Mongo.Configuration;
using GameNest.ReviewsService.Infrastructure.Mongo.Context;
using GameNest.ReviewsService.Infrastructure.Mongo.HealthChecks;
using GameNest.ReviewsService.Infrastructure.Mongo.Indexes;
using GameNest.ReviewsService.Infrastructure.Mongo.Seeding;
using GameNest.ReviewsService.Infrastructure.Mongo.UOW;
using GameNest.ReviewsService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddMemoryCache();

builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDbSettings"));

var aspireConn = builder.Configuration.GetConnectionString("mongodb");
if (!string.IsNullOrEmpty(aspireConn))
{
    builder.Services.PostConfigure<MongoDbSettings>(options =>
    {
        options.ConnectionString = aspireConn;
    });
}

builder.Services.PostConfigure<MongoDbSettings>(options =>
{
    if (string.IsNullOrEmpty(options.ConnectionString))
        throw new InvalidOperationException("MongoDB ConnectionString is required");
    if (string.IsNullOrEmpty(options.DatabaseName))
        throw new InvalidOperationException("MongoDB DatabaseName is required");
});

builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddSingleton<IIndexCreationService, MongoIndexCreationService>();
builder.Services.AddHealthChecks()
    .AddCheck<MongoHealthCheck>("mongodb");

builder.Services.AddScoped<IUnitOfWork>(provider =>
{
    var context = provider.GetRequiredService<MongoDbContext>();
    return new MongoUnitOfWork(context.Database);
});

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<IMediaRepository, MediaRepository>();
builder.Services.AddScoped<IReviewRepository, ReviewRepository>();

builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IMediaService, MediaService>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddSingleton<IDataSeeder, DatabaseSeeder>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(typeof(AddReplyCommandHandler).Assembly);
    cfg.AddOpenBehavior(typeof(ExceptionHandlingBehavior<,>));
    cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
    cfg.AddOpenBehavior(typeof(PerformanceBehavior<,>));
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

builder.Services.AddValidatorsFromAssembly(typeof(GetCommentsQueryValidator).Assembly);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(cfg => { }, AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health");

using (var scope = app.Services.CreateScope())
{
    var indexService = scope.ServiceProvider.GetRequiredService<IIndexCreationService>();
    await indexService.CreateIndexesAsync();

    var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
    await seeder.SeedAsync();
}

await app.RunAsync();
