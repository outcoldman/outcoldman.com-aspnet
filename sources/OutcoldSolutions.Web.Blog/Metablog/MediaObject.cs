namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract]
	public class MediaObject
	{
		[DataMember(Name = "name")]
		public string Name { get; set; }

		[DataMember(Name = "type")]
		public string Type { get; set; }

		[DataMember(Name = "bits")]
		public byte[] Bits { get; set; }
	}
}