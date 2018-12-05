namespace Domain.Identity
{
    public class ApplicationRoleView : DomainTreeEntityViewBase<string, ApplicationRoleView, string>
    {
        /// <summary>Gets or sets the name for this role.</summary>
        public string Name { get; set; }

        /// <summary>Gets or sets the normalized name for this role.</summary>
        public string NormalizedName { get; set; }

        /// <summary>
        /// A random value that should change whenever a role is persisted to the store
        /// </summary>
        public string ConcurrencyStamp { get; set; }

        public string Description { get; set; }
    }
}
