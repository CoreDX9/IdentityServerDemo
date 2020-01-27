using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace CoreDX.Domain.Entity.App.IdentityServer
{
	public class ApiResourceDto
	{
		public ApiResourceDto()
		{
			UserClaims = new List<string>();
		}

		public int Id { get; set; }

		[Required]
		public string Name { get; set; }

		public string DisplayName { get; set; }

		public string Description { get; set; }

		public bool Enabled { get; set; } = true;

		public List<string> UserClaims { get; set; }

		public string UserClaimsItems { get; set; }
	}
}
