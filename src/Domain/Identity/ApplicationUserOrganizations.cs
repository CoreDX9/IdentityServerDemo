using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Identity
{
    public class ApplicationUserOrganization : DomainEntityBase
    {
        public Guid UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
