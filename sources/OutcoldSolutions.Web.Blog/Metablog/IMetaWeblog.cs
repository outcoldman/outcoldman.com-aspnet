namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System.ServiceModel;

    [ServiceContract(Namespace = "http://www.xmlrpc.com/metaWeblogApi")]
	public interface IMetaWeblog
	{
		[OperationContract(Action = "metaWeblog.newPost")]
		string CreatePost(string blogid, string username, string password, Post post, bool publish);

		[OperationContract(Action = "metaWeblog.editPost")]
		bool UpdatePost(string postid, string username, string password, Post post, bool publish);

		[OperationContract(Action = "metaWeblog.getPost")]
		Post GetPost(string postid, string username, string password);

		[OperationContract(Action = "metaWeblog.getCategories")]
		CategoryInfo[] GetCategories(string blogid, string username, string password);

		[OperationContract(Action = "metaWeblog.getRecentPosts")]
		Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts);

		[OperationContract(Action = "metaWeblog.newMediaObject")]
		MediaObjectInfo NewMediaObject(string blogid, string username, string password,
		                               MediaObject mediaObject);

		[OperationContract(Action = "blogger.deletePost")]
		bool DeletePost(string key, string postid, string username, string password, bool publish);

		[OperationContract(Action = "blogger.getUsersBlogs")]
		BlogInfo[] GetUsersBlogs(string appKey, string username, string password);

		[OperationContract(Action = "blogger.getUserInfo")]
		UserInfo GetUserInfo(string key, string username, string password);
	}
}