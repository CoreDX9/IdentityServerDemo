using System.Threading.Tasks;
using AutoMapper;
using CoreDX.Applicaiton.IdnetityServerAdmin.Mappers.Identity;
using Skoruba.AuditLogging.Services;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Dtos.Grant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Events.PersistedGrant;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Mappers;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Resources;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Identity.Services.Interfaces;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Shared.ExceptionHandling;
using Skoruba.IdentityServer4.Admin.EntityFramework.Identity.Repositories.Interfaces;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Services.Identity
{
    public class PersistedGrantAspNetIdentityService : IPersistedGrantAspNetIdentityService
    {
        protected readonly IPersistedGrantAspNetIdentityRepository PersistedGrantAspNetIdentityRepository;
        protected readonly IPersistedGrantAspNetIdentityServiceResources PersistedGrantAspNetIdentityServiceResources;
        protected readonly IAuditEventLogger AuditEventLogger;
        protected readonly IMapper Mapper;

        public PersistedGrantAspNetIdentityService(IPersistedGrantAspNetIdentityRepository persistedGrantAspNetIdentityRepository,
            IPersistedGrantAspNetIdentityServiceResources persistedGrantAspNetIdentityServiceResources,
            IAuditEventLogger auditEventLogger,
            IMapper mapper)
        {
            PersistedGrantAspNetIdentityRepository = persistedGrantAspNetIdentityRepository;
            PersistedGrantAspNetIdentityServiceResources = persistedGrantAspNetIdentityServiceResources;
            AuditEventLogger = auditEventLogger;
            Mapper = mapper;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUsersAsync(search, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel(Mapper);

            await AuditEventLogger.LogEventAsync(new PersistedGrantsIdentityByUsersRequestedEvent(persistedGrantsDto));

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(subjectId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

            var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
            var persistedGrantsDto = pagedList.ToModel(Mapper);

            await AuditEventLogger.LogEventAsync(new PersistedGrantsIdentityByUserRequestedEvent(persistedGrantsDto));

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantDto> GetPersistedGrantAsync(string key)
        {
            var persistedGrant = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantAsync(key);
            if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
            var persistedGrantDto = persistedGrant.ToModel(Mapper);

            await AuditEventLogger.LogEventAsync(new PersistedGrantIdentityRequestedEvent(persistedGrantDto));

            return persistedGrantDto;
        }

        public virtual async Task<int> DeletePersistedGrantAsync(string key)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantAsync(key);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);

            var deleted = await PersistedGrantAspNetIdentityRepository.DeletePersistedGrantAsync(key);

            await AuditEventLogger.LogEventAsync(new PersistedGrantIdentityDeletedEvent(key));

            return deleted;
        }

        public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(userId);
            if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, userId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);

            var deleted = await PersistedGrantAspNetIdentityRepository.DeletePersistedGrantsAsync(userId);

            await AuditEventLogger.LogEventAsync(new PersistedGrantsIdentityDeletedEvent(userId));

            return deleted;
        }
    }
}
