using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToDoList.Domain.Entities;

namespace ToDoList.Domain
{
	public class ToDoListContext : DbContext
	{
		public DbSet<User> Users { get; set; }

		public DbSet<TaskApp> Tasks { get; set; }

		public ToDoListContext(DbContextOptions options) : base(options) 
		{
			
		}

	}
}
