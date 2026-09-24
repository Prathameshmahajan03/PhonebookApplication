using PhonebookApplication.Repositories;

namespace PhonebookApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddProblemDetails();

            builder.Services.AddScoped<IContactRepository, ContactRepository>();

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            app.UseExceptionHandler();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Serve Vue application from wwwroot
            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseAuthorization();

            // API endpoints
            app.MapControllers();

            // Vue SPA fallback
            app.MapFallbackToFile("index.html");

            app.Run();
        }
    }
}