using CityInfo.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace CityInfo.API.DbContexts
{
	public class CityInfoContext : DbContext
	{
		public DbSet<City> Cities { get; set; } = null!;
		public DbSet<PointOfInterest> PointsOfInterests { get; set; } = null!;
		
		public CityInfoContext(DbContextOptions<CityInfoContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<City>()
				.HasData(
				new City("New York City")
				{
					Id = 1,
					Descripion = "The one with big park."
				},
				new City("Antwerp")
				{
					Id = 2,
					Descripion = "The one with the cathedral that was never really finished."
				},
				new City("Paris")
				{
					Id = 3,
					Descripion = "The one with that big tower."
				});

			modelBuilder.Entity<PointOfInterest>()
				.HasData(
				new PointOfInterest("Central Park")
				{
					Id = 1,
					CityId = 1,
					Description = "The most visited urban park in the USA."
				},
				new PointOfInterest("Empire State Building")
				{
					Id = 2,
					CityId = 1,
					Description = "A 102-story skyscraper located in Midtown Manhattan."
				},
				new PointOfInterest("Cathedral")
				{
					Id = 3,
					CityId = 2,
					Description = "A Gothic style cathedral."
				},
				new PointOfInterest("Antwerp Central Station")
				{
					Id = 4,
					CityId = 2,
					Description = "The finest example of railway architecture in Belgium."
				},
				new PointOfInterest("Eifel Tower")
				{
					Id = 5,
					CityId = 3,
					Description = "A wrought iron tower on the Champ de Mars."
				},
				new PointOfInterest("The Louver")
				{
					Id = 6,
					CityId = 3,
					Description = "The world's largest museum."
				});

			base.OnModelCreating(modelBuilder);
		}
	}
}
