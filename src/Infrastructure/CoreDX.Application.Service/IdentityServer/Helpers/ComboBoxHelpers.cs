using System.Collections.Generic;
using System.Text.Json;

namespace CoreDX.Application.Service.IdentityServer.Helpers
{
	public static class ComboBoxHelpers
	{
		public static void PopulateValuesToList(string jsonValues, List<string> list)
		{
			if (string.IsNullOrEmpty(jsonValues)) return;

			var listValues = JsonSerializer.Deserialize<List<string>>(jsonValues);
			if (listValues == null) return;

			list.AddRange(listValues);
		}
	}
}
