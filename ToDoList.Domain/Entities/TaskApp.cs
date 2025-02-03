using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoList.Domain.Entities
{
	public class TaskApp:Entity
	{
		public string Name { get; set; }

		public bool IsCompleted { get; set; }

		public DateTime ExpiredDate { get; set; }

		public int UserID { get; set; }

		[ForeignKey("UserID")]

		public User User { get; set; }
			
	}	
}
