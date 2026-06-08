using Microsoft.EntityFrameworkCore;
using Shop.Domain.Entities;
using Shop.Models;

namespace Shop.Domain
{
	public class DBContext : DbContext
	{
		public DbSet<TaskApp> Tasks { get; set; }
		public DbSet<DataRentApart> apartmentsavito { get; set; }
		public DbSet<User> Users { get; set; }
		public DbSet<Shop.Domain.Entities.Phone> Phones { get; set; }
		public DBContext(DbContextOptions options) : base(options)
		{

		}
	}
}
