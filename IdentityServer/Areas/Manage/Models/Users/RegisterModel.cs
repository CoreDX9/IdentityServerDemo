using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Areas.Manage.Models.Users
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "电子邮件地址是必填的。")]
        [EmailAddress]
        [Display(Name = "电子邮件")]
        public string Email { get; set; }

        [Required(ErrorMessage = "用户名是必填的。")]
        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "密码是必填的。")]
        [StringLength(100, ErrorMessage = "密码需要至少{0}个字，至多{1}个字。", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "密码")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "确认密码")]
        [Compare("Password", ErrorMessage = "密码必须和确认密码一致。")]
        public string ConfirmPassword { get; set; }
    }
}
