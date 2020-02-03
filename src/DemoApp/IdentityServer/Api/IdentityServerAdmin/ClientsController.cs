using AutoMapper;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Dtos.Clients;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.ExceptionHandling;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Mappers;
using CoreDX.Applicaiton.IdnetityServerAdmin.Api.Resources;
using CoreDX.Applicaiton.IdnetityServerAdmin.Configuration.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Dtos.Configuration;
using Skoruba.IdentityServer4.Admin.BusinessLogic.Services.Interfaces;
using System.Threading.Tasks;

namespace IdentityServer.Admin.Api.Controllers
{
    [Route("api/identityserveradmin/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    [TypeFilter(typeof(ControllerExceptionFilterAttribute))]
    [Produces("application/json", "application/problem+json")]
    [Authorize(Policy = AuthorizationConsts.AdministrationPolicy)]
    public class ClientsController : ControllerBase
    {
        private readonly IClientService _clientService;
        private readonly IApiErrorResources _errorResources;
        private readonly IMapper _mapper;

        public ClientsController(IClientService clientService, IApiErrorResources errorResources, IMapper mapper)
        {
            _clientService = clientService;
            _errorResources = errorResources;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<ClientsApiDto>> Get(string searchText, int page = 1, int pageSize = 10)
        {
            var clientsDto = await _clientService.GetClientsAsync(searchText, page, pageSize);
            var clientsApiDto = clientsDto.ToClientApiModel<ClientsApiDto>(_mapper);

            return Ok(clientsApiDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClientApiDto>> Get(int id)
        {
            var clientDto = await _clientService.GetClientAsync(id);
            var clientApiDto = clientDto.ToClientApiModel<ClientApiDto>(_mapper);

            return Ok(clientApiDto);
        }

        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Post([FromBody]ClientApiDto client)
        {
            var clientDto = client.ToClientApiModel<ClientDto>(_mapper);

            if (!clientDto.Id.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var id = await _clientService.AddClientAsync(clientDto);
            client.Id = id;

            return CreatedAtAction(nameof(Get), new { id }, client);
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody]ClientApiDto client)
        {
            var clientDto = client.ToClientApiModel<ClientDto>(_mapper);

            await _clientService.GetClientAsync(clientDto.Id);
            await _clientService.UpdateClientAsync(clientDto);

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var clientDto = new ClientDto { Id = id };

            await _clientService.GetClientAsync(clientDto.Id);
            await _clientService.RemoveClientAsync(clientDto);

            return Ok();
        }

        [HttpPost("Clone")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostClientClone([FromBody]ClientCloneApiDto client)
        {
            var clientCloneDto = client.ToClientApiModel<ClientCloneDto>(_mapper);

            var originalClient = await _clientService.GetClientAsync(clientCloneDto.Id);
            var id = await _clientService.CloneClientAsync(clientCloneDto);
            originalClient.Id = id;

            return CreatedAtAction(nameof(Get), new { id }, originalClient);
        }

        [HttpGet("{id}/Secrets")]
        public async Task<ActionResult<ClientSecretsApiDto>> GetSecrets(int id, int page = 1, int pageSize = 10)
        {
            var clientSecretsDto = await _clientService.GetClientSecretsAsync(id, page, pageSize);
            var clientSecretsApiDto = clientSecretsDto.ToClientApiModel<ClientSecretsApiDto>(_mapper);

            return Ok(clientSecretsApiDto);
        }

        [HttpGet("Secrets/{secretId}")]
        public async Task<ActionResult<ClientSecretApiDto>> GetSecret(int secretId)
        {
            var clientSecretsDto = await _clientService.GetClientSecretAsync(secretId);
            var clientSecretDto = clientSecretsDto.ToClientApiModel<ClientSecretApiDto>(_mapper);

            return Ok(clientSecretDto);
        }

        [HttpPost("{id}/Secrets")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostSecret(int id, [FromBody]ClientSecretApiDto clientSecretApi)
        {
            var secretsDto = clientSecretApi.ToClientApiModel<ClientSecretsDto>(_mapper);
            secretsDto.ClientId = id;

            if (!secretsDto.ClientSecretId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var secretId = await _clientService.AddClientSecretAsync(secretsDto);
            clientSecretApi.Id = secretId;

            return CreatedAtAction(nameof(GetSecret), new { secretId }, clientSecretApi);
        }

        [HttpDelete("Secrets/{secretId}")]
        public async Task<IActionResult> DeleteSecret(int secretId)
        {
            var clientSecret = new ClientSecretsDto { ClientSecretId = secretId };

            await _clientService.GetClientSecretAsync(clientSecret.ClientSecretId);
            await _clientService.DeleteClientSecretAsync(clientSecret);

            return Ok();
        }

        [HttpGet("{id}/Properties")]
        public async Task<ActionResult<ClientPropertiesApiDto>> GetProperties(int id, int page = 1, int pageSize = 10)
        {
            var clientPropertiesDto = await _clientService.GetClientPropertiesAsync(id, page, pageSize);
            var clientPropertiesApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertiesApiDto>(_mapper);

            return Ok(clientPropertiesApiDto);
        }

        [HttpGet("Properties/{propertyId}")]
        public async Task<ActionResult<ClientPropertyApiDto>> GetProperty(int propertyId)
        {
            var clientPropertiesDto = await _clientService.GetClientPropertyAsync(propertyId);
            var clientPropertyApiDto = clientPropertiesDto.ToClientApiModel<ClientPropertyApiDto>(_mapper);

            return Ok(clientPropertyApiDto);
        }

        [HttpPost("{id}/Properties")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostProperty(int id, [FromBody]ClientPropertyApiDto clientPropertyApi)
        {
            var clientPropertiesDto = clientPropertyApi.ToClientApiModel<ClientPropertiesDto>(_mapper);
            clientPropertiesDto.ClientId = id;

            if (!clientPropertiesDto.ClientPropertyId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var propertyId = await _clientService.AddClientPropertyAsync(clientPropertiesDto);
            clientPropertyApi.Id = propertyId;

            return CreatedAtAction(nameof(GetProperty), new { propertyId }, clientPropertyApi);
        }

        [HttpDelete("Properties/{propertyId}")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            var clientProperty = new ClientPropertiesDto { ClientPropertyId = propertyId };

            await _clientService.GetClientPropertyAsync(clientProperty.ClientPropertyId);
            await _clientService.DeleteClientPropertyAsync(clientProperty);

            return Ok();
        }

        [HttpGet("{id}/Claims")]
        public async Task<ActionResult<ClientClaimsApiDto>> GetClaims(int id, int page = 1, int pageSize = 10)
        {
            var clientClaimsDto = await _clientService.GetClientClaimsAsync(id, page, pageSize);
            var clientClaimsApiDto = clientClaimsDto.ToClientApiModel<ClientClaimsApiDto>(_mapper);

            return Ok(clientClaimsApiDto);
        }

        [HttpGet("Claims/{claimId}")]
        public async Task<ActionResult<ClientClaimApiDto>> GetClaim(int claimId)
        {
            var clientClaimsDto = await _clientService.GetClientClaimAsync(claimId);
            var clientClaimApiDto = clientClaimsDto.ToClientApiModel<ClientClaimApiDto>(_mapper);

            return Ok(clientClaimApiDto);
        }

        [HttpPost("{id}/Claims")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostClaim(int id, [FromBody]ClientClaimApiDto clientClaimApiDto)
        {
            var clientClaimsDto = clientClaimApiDto.ToClientApiModel<ClientClaimsDto>(_mapper);
            clientClaimsDto.ClientId = id;

            if (!clientClaimsDto.ClientClaimId.Equals(default))
            {
                return BadRequest(_errorResources.CannotSetId());
            }

            var claimId = await _clientService.AddClientClaimAsync(clientClaimsDto);
            clientClaimApiDto.Id = claimId;

            return CreatedAtAction(nameof(GetClaim), new { claimId }, clientClaimApiDto);
        }

        [HttpDelete("Claims/{claimId}")]
        public async Task<IActionResult> DeleteClaim(int claimId)
        {
            var clientClaimsDto = new ClientClaimsDto { ClientClaimId = claimId };

            await _clientService.GetClientClaimAsync(claimId);
            await _clientService.DeleteClientClaimAsync(clientClaimsDto);

            return Ok();
        }
    }
}





