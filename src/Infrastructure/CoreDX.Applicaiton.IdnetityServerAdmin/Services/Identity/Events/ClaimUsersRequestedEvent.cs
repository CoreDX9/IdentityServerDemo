using Skoruba.AuditLogging.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Services.Identity.Events
{
    public class ClaimUsersRequestedEvent<TUsersDto> : AuditEvent
    {
        public TUsersDto Users { get; set; }

        public ClaimUsersRequestedEvent(TUsersDto users)
        {
            Users = users;
        }
    }
}
