using AutoMapper;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log;
using Skoruba.IdentityServer4.Admin.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.EntityFramework.Extensions.Common;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Mappers
{
    public static class LogMappers
    {
        //internal static IMapper Mapper { get; }

        //static LogMappers()
        //{
        //    Mapper = new MapperConfiguration(cfg => cfg.AddProfile<LogMapperProfile>())
        //        .CreateMapper();
        //}

        public static LogDto ToModel(this Log log, IMapper mapper)
        {
            return mapper.Map<LogDto>(log);
        }

        public static LogsDto ToModel(this PagedList<Log> logs, IMapper mapper)
        {
            return mapper.Map<LogsDto>(logs);
        }

        public static AuditLogsDto ToModel<TAuditLog>(this PagedList<TAuditLog> auditLogs, IMapper mapper)
            where TAuditLog : AuditLog
        {
            return mapper.Map<AuditLogsDto>(auditLogs);
        }

        public static AuditLogDto ToModel(this AuditLog auditLog, IMapper mapper)
        {
            return mapper.Map<AuditLogDto>(auditLog);
        }

        public static Log ToEntity(this LogDto log, IMapper mapper)
        {
            return mapper.Map<Log>(log);
        }
    }
}
