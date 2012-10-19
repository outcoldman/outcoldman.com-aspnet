// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models
{
    partial class Tag
    {
        public string LowerName
        {
            get
            {
                return this.Name.ToLower();
            }
        }
    }
}