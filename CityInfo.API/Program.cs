using CityInfo.API.DbContexts;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Net;
using System.Reflection;
using System.Text;

namespace CityInfo.API
{
	public class Program
	{
		public static void Main(string[] args)
		{
			// This creates configurations(specifications like where to write) and
			// ("writes" information to the console and to the file located in logs folder)
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Debug()
				.WriteTo.Console()
				.WriteTo.File("logs/cityinfo.txt", rollingInterval: RollingInterval.Day)
				.CreateLogger();

			var builder = WebApplication.CreateBuilder(args);
			//builder.Logging.ClearProviders();
			//builder.Logging.AddConsole();

			// This enables Logging(writing information about any actions to the console)
			builder.Host.UseSerilog();

			// Add services to the container.

			// This enables specifications what kind of response will be returned for example: application/json, application/xml...
			builder.Services.AddControllers(options =>
			{
				options.ReturnHttpNotAcceptable = true;
			}).AddNewtonsoftJson()
			.AddXmlDataContractSerializerFormatters();



			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(setupAction =>
			{
				var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

				setupAction.IncludeXmlComments(xmlCommentsFullPath);

				// These lines of code enables Authentication
				setupAction.AddSecurityDefinition("CityInfoApiBearerAuth", new OpenApiSecurityScheme()
				{
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					Description = "Input a valid token to access this API"
				});

				setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "CityInfoApiBearerAuth"
							}
						}, new List<string>()
					}
				});
			});

			// Not sure about it but
			// This enables sharing files through api
			builder.Services.AddSingleton<FileExtensionContentTypeProvider>();

#if DEBUG
			builder.Services.AddTransient<IMailService, LocalMailService>();
#else
			builder.Services.AddTransient<IMailService, CloudMailService>();
#endif

			// This connects to the database
			builder.Services.AddDbContext<CityInfoContext>(
				options => options.UseSqlite(
					builder.Configuration["ConnectionStrings:DefaultConnection"]));

			// This enables of making an injection
			builder.Services.AddScoped<ICityInfoRepository, CityInfoRepository>();

			// This enables AutoMapper. (See folder Profiles)
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			// This enables Authentication
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
			// This enables Authorization
			builder.Services.AddAuthorization(options =>
			{
				options.AddPolicy("MustBeFromAntwerp", policy =>
				{
					policy.RequireAuthenticatedUser();
					policy.RequireClaim("city", "Antwerp"); ;
				});
			});

			// This enables API versioning and sets default version(1.0) in case if it was not specified when request was executed
			builder.Services.AddApiVersioning(setupAction =>
			{
				setupAction.AssumeDefaultVersionWhenUnspecified = true;
				setupAction.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
				setupAction.ReportApiVersions = true;
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
