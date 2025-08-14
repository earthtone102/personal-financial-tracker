using Microsoft.EntityFrameworkCore;
using PersonalFinancialTracker.Data;
using PersonalFinancialTracker.Repository;
using PersonalFinancialTracker.Helper;
using AutoMapper;
using Serilog;

namespace PersonalFinancialTracker
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            #region auto mapper configuration
            var mapperConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new ApplicationMapper());
            });
            IMapper mapper = mapperConfig.CreateMapper();
            #endregion
            #region inject a service
            builder.Services.AddSingleton(mapper);
            builder.Services.AddTransient<OperationHelper>();
            builder.Services.AddTransient<ITransactionRepository, TransactionRepository>();
            builder.Services.AddTransient<ICategoryRepository, CategoryRepository>();
            builder.Services.AddTransient<IAccountRepository, AccountRepository>();
            #endregion

            //builder.Services.AddControllers();

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
            });
            //enabled cors
            builder.Services.AddCors(option =>
            {
                option.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
            builder.Services.AddDbContext<TransactionContext>(opt =>
            {
                opt.UseNpgsql(builder.Configuration.GetSection("ConnectionStrings")["PersonalFinancialTrackerDB"]!).EnableSensitiveDataLogging();
            });

            var logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog(logger);

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddHttpLogging(o => { });



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthorization();
            app.UseHttpLogging();
            app.MapControllers();

            app.Run();
        }
    }
}
