﻿namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.Runtime.Serialization;

    [DataContract]
	public class MediaObjectInfo
	{
		[DataMember(Name = "url")]
		public string Url;
	}
}