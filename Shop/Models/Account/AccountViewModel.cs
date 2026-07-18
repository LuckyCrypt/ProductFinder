using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Account
{
	public class AccountViewModel
	{
		public LoginViewModel? LoginViewModel { get; set; }

		public RegisterViewModel? RegisterViewModel { get; set; }
	}

	public class LoginViewModel
	{
		[Required(ErrorMessage = "Данное поле обязательно")]
		public required string Login { get; set; }

		[Required(ErrorMessage = "Данное поле обязательно")]
		public required string Password { get; set; }
	}

	public class RegisterViewModel
	{
		[Required(ErrorMessage = "Данное поле обязательно")]
		public required string Login { get; set; }

		[Display(Name = "Имя")]
		public string? FirstName { get; set; }

		[EmailAddress(ErrorMessage = "Некорректный e-mail")]
		[Display(Name = "E-mail")]
		public string? Email { get; set; }

		[Required(ErrorMessage = "Данное поле обязательно")]
		public required string Password { get; set; }

		[Required(ErrorMessage = "Данное поле обязательно")]
		[Compare("Password", ErrorMessage = "Пароли не совпадают")]
		public required string RepeatPassword { get; set; }
	}
}
