using System;
using System.Collections.Generic;
using System.Text;

namespace CoreDX.Domain.Entity.App.IdentityServer
{
	public class SelectItemDto
	{
		public SelectItemDto(string id, string text)
		{
			Id = id;
			Text = text;
		}

		public string Id { get; set; }

		public string Text { get; set; }
	}
}
