using System.ComponentModel.DataAnnotations;
using FluentValidation;
using IdentityServer.HttpHandlerBase;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Pages.FluentValidationDemo
{
    [Authorize]
    public class IndexModel : PageModelBase
    {
        public class A
        {
            [Display(Name = "是")]
            public string S { get; set; }
            [Display(Name = "他")]
            public string T { get; set; }
            [Display(Name = "吧")]
            public B B { get; set; }
            [Display(Name = "店铺")]
            public D Dp { get; set; }

            public class D
            {
                [Display(Name = "额")]
                public string E { get; set; }
            }
        }

        public class B
        {
            [Display(Name = "从")]
            public string C { get; set; }
        }

        public class AValidator : AbstractValidator<A>
        {
            public AValidator()
            {
                RuleFor(x => x.S).NotNull();
                RuleFor(x => x.B).NotNull();
                RuleFor(x => x.B.C).NotNull().When(x => x.B != null).WithLanguageManagedMessage("'{PropertyName}' 是必填的");
                RuleFor(x => x.Dp.E).NotNull().When(x => x.Dp != null).WithLanguageManagedMessage("'{PropertyName}' 是必填的");
                RuleFor(x => x.T).Equal(a => a.S);
                RuleFor(x => x.T).Must(t => t?.Length > 1).WithLanguageManagedMessage("'{PropertyName}' 需要至少2个字");
            }
        }

        [BindProperty]
        public A Input { get; set; }

        public void OnGet()
        {

        }

        public void OnPost()
        {

        }
    }
}