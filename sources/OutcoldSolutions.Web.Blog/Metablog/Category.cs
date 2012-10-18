using System.Runtime.Serialization;

namespace PersonalWeb.Web.Metablog
{
	[DataContract]
	public class Category
	{
		[DataMember(Name = "categoryId")]
		public string CategoryId { get; set; }

		[DataMember(Name = "categoryName")]
		public string CategoryName { get; set; }
	}
}