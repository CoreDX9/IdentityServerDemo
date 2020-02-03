using System;
using System.Threading.Tasks;
using AutoMapper;
using CoreDX.Applicaiton.IdnetityServerAdmin.Mappers;
using Skoruba.AuditLogging.EntityFramework.Entities;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Log;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.EntityFramework.Repositories.Interfaces;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Services
{
    public class AuditLogService<TAuditLog> : IAuditLogService
        where TAuditLog : AuditLog
    {
        protected readonly IAuditLogRepository<TAuditLog> AuditLogRepository;
        protected readonly IMapper Mapper;

        public AuditLogService(IAuditLogRepository<TAuditLog> auditLogRepository, IMapper mapper)
        {
            AuditLogRepository = auditLogRepository;
            Mapper = mapper;
        }

        public async Task<AuditLogsDto> GetAsync(AuditLogFilterDto filters)
        {
            var pagedList = await AuditLogRepository.GetAsync(filters.Event, filters.Source, filters.Category, filters.Created, filters.SubjectIdentifier, filters.SubjectName, filters.Page, filters.PageSize);
            var auditLogsDto = pagedList.ToModel(Mapper);

            return auditLogsDto;
        }

        public virtual async Task DeleteLogsOlderThanAsync(DateTime deleteOlderThan)
        {
            await AuditLogRepository.DeleteLogsOlderThanAsync(deleteOlderThan);
        }
    }
}
