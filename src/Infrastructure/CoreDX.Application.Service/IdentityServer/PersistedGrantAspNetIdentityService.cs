using AutoMapper;
using CoreDX.Domain.Entity.App.IdentityServer;
using CoreDX.Domain.Repository.App.IdentityServer;
using CoreDX.Domain.Service.App.IdentityServer;
using System;
using System.Threading.Tasks;

namespace CoreDX.Application.Service.IdentityServer
{
    public class PersistedGrantAspNetIdentityService : IPersistedGrantAspNetIdentityService
    {
        protected readonly IPersistedGrantAspNetIdentityRepository PersistedGrantAspNetIdentityRepository;
        protected readonly IMapper _mapper;

        public PersistedGrantAspNetIdentityService(IPersistedGrantAspNetIdentityRepository persistedGrantAspNetIdentityRepository, IMapper mapper)
        {
            PersistedGrantAspNetIdentityRepository = persistedGrantAspNetIdentityRepository;
            _mapper = mapper;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUsersAsync(string search, int page = 1, int pageSize = 10)
        {
            var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUsersAsync(search, page, pageSize);
            var persistedGrantsDto = _mapper.Map<PersistedGrantsDto>(pagedList);

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantsDto> GetPersistedGrantsByUserAsync(string subjectId, int page = 1, int pageSize = 10)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(subjectId);
            //if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, subjectId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);
            if (!exists) throw new Exception(string.Format("{0}"));

            var pagedList = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantsByUserAsync(subjectId, page, pageSize);
            var persistedGrantsDto = _mapper.Map<PersistedGrantsDto>(pagedList);

            return persistedGrantsDto;
        }

        public virtual async Task<PersistedGrantDto> GetPersistedGrantAsync(string key)
        {
            var persistedGrant = await PersistedGrantAspNetIdentityRepository.GetPersistedGrantAsync(key);
            //if (persistedGrant == null) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
            if (persistedGrant == null) throw new Exception(string.Format("{0}"));
            var persistedGrantDto = _mapper.Map<PersistedGrantDto>(persistedGrant);

            return persistedGrantDto;
        }

        public virtual async Task<int> DeletePersistedGrantAsync(string key)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantAsync(key);
            //if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description, key), PersistedGrantAspNetIdentityServiceResources.PersistedGrantDoesNotExist().Description);
            if (!exists) throw new Exception(string.Format("{0}"));

            var deleted = await PersistedGrantAspNetIdentityRepository.DeletePersistedGrantAsync(key);

            return deleted;
        }

        public virtual async Task<int> DeletePersistedGrantsAsync(string userId)
        {
            var exists = await PersistedGrantAspNetIdentityRepository.ExistsPersistedGrantsAsync(userId);
            //if (!exists) throw new UserFriendlyErrorPageException(string.Format(PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description, userId), PersistedGrantAspNetIdentityServiceResources.PersistedGrantWithSubjectIdDoesNotExist().Description);
            if (!exists) throw new Exception(string.Format("{0}"));

            var deleted = await PersistedGrantAspNetIdentityRepository.DeletePersistedGrantsAsync(userId);

            return deleted;
        }
    }
}
