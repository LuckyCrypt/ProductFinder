namespace ToDoList.Domain.Entities
{
	public class User : Entity
	{
		public string Login { get; set; }
		public string Password { get; set; }
		public string email { get; set; }
		public string firstName { get; set; }

		public User(string login, string password)	
		{
			Login = login;
			Password = password;
			email = string.Empty;
			firstName = string.Empty;
		}
	}
}
