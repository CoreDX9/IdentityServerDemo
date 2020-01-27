using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreDX.Domain.Entity.App.IdentityServer
{
    public class IdentityResourcePropertiesDto
    {
        public int IdentityResourcePropertyId { get; set; }

        public int IdentityResourceId { get; set; }

        public string IdentityResourceName { get; set; }

        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public List<IdentityResourcePropertyDto> IdentityResourceProperties { get; set; } = new List<IdentityResourcePropertyDto>();

        public int TotalCount { get; set; }

        public int PageSize { get; set; }
    }

    public class IdentityResourcePropertyDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
