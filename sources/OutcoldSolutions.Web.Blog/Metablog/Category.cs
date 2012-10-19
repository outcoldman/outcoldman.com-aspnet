// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract]
    public class Category
    {
        [DataMember(Name = "categoryId")]
        public string CategoryId { get; set; }

        [DataMember(Name = "categoryName")]
        public string CategoryName { get; set; }
    }
}