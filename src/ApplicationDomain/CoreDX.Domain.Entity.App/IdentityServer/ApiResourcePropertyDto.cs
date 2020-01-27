using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreDX.Domain.Entity.App.IdentityServer
{
    public class ApiResourcePropertyDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class ApiResourcePropertiesDto
    {
        public int ApiResourcePropertyId { get; set; }

        public int ApiResourceId { get; set; }

        public string ApiResourceName { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public List<ApiResourcePropertyDto> ApiResourceProperties { get; set; } = new List<ApiResourcePropertyDto>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }
}
