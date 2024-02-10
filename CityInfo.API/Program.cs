using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Net;
using System.Text;

namespace CityInfo.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();

			var builder = WebApplication.CreateBuilder(args);
			//builder.Logging.ClearProviders();
			//builder.Logging.AddConsole();

			builder.Host.UseSerilog();

			// Add services to the container.

			builder.Services.AddControllers(options =>
			{
				options.ReturnHttpNotAcceptable = true;
			}).AddNewtonsoftJson()
			.AddXmlDataContractSerializerFormatters();



			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
			builder.Services.AddTransient<IMailService, LocalMailService>();
#else
			builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

			builder.Services.AddSingleton<CitiesDataStore>();

			builder.Services.AddDbContext<CityInfoContext>(
				options => options.UseSqlite(
					builder.Configuration["ConnectionStrings:DefaultConnection"]));

			builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			builder.Services.AddAuthentication("Bearer")
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new()
					{
						ValidateIssuer = true,
						ValidateAudience = true,
						ValidateIssuerSigningKey = true,
						ValidIssuer = builder.Configuration["Authentication:Issuer"],
						ValidAudience = builder.Configuration["Authentication:Audience"],
						IssuerSigningKey = new SymmetricSecurityKey(
							Encoding.ASCII.GetBytes(builder.Configuration["Authentication:SecretForKey"]))
					};
				});

			// For this code watch 8th video of the section 9
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("MustBeFromAntwerp", policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireClaim("city", "Antwerp"); ;
				});
			});

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});

			app.Run();
		}
	}
}
