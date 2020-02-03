using CoreDX.Applicaiton.IdnetityServerAdmin.Api.ExceptionHandling;

namespace CoreDX.Applicaiton.IdnetityServerAdmin.Api.Resources
{
    public class ApiErrorResources : IApiErrorResources
    {
        public virtual ApiError CannotSetId()
        {
            return new ApiError
            {
                Code = nameof(CannotSetId),
                Description = ApiErrorResource.CannotSetId
            };
        }
    }
}





