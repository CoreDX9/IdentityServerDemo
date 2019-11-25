using CoreDX.Domain.Entity.Identity;
using System;
using CoreDX.Common.Util.Security;

namespace IdentityServer.Areas.Manage.Models.Users
{
    public class ApplicationUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public Gender? Gender { get; set; }
        public string Email { get; set; }
        public string InsensitiveEmail => Email.HideEmailDetails();
        public string PhoneNumber { get; set; }
        public string InsensitivePhoneNumber => PhoneNumber.HideSensitiveInfo(3, 4);
        public bool EmailConfirmed { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public int CreatorId { get; set; }
        public DateTimeOffset LastModificationTime { get; set; }
        public int LastModificationUserId { get; set; }
    }
}
