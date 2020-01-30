namespace CoreDX.Domain.Repository.App.IdentityServer.Helpers
{
	public class SelectItem
	{
		public SelectItem(string id, string text)
		{
			Id = id;
			Text = text;
		}

		public string Id { get; set; }

		public string Text { get; set; }
	}
}
