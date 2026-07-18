using System.ComponentModel.DataAnnotations;

namespace Shop.Models.Cabinet
{
    public class ProfileViewModel
    {
        [Display(Name = "Логин")]
        public string UserName { get; set; } = "";

        [Display(Name = "Имя")]
        public string? FirstName { get; set; }

        [EmailAddress(ErrorMessage = "Некорректный e-mail")]
        [Display(Name = "E-mail")]
        public string? Email { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Введите текущий пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Текущий пароль")]
        public string OldPassword { get; set; } = "";

        [Required(ErrorMessage = "Введите новый пароль")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Минимум 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль")]
        public string NewPassword { get; set; } = "";

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Повторите новый пароль")]
        public string ConfirmPassword { get; set; } = "";
    }
}
