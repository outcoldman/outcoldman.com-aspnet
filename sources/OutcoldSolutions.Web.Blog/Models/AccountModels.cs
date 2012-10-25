// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models
{
    using System.Data.Entity;

    internal class DatabaseContext : DbContext
    {
        public DatabaseContext()
            : base("LocalDatabase")
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}
