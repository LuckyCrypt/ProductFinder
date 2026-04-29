using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Entities
{
	public class TaskApp:Entity
	{
		public required string Name { get; set; }

		public bool IsCompleted { get; set; }

		public DateTime ExpiredDate { get; set; }

		public int UserID { get; set; }

		[ForeignKey("UserID")]

		public required User User { get; set; }
			
	}	
}
