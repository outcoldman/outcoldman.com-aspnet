using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.SqlServerCe;
using System.Linq;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Model.Repositories
{
	[Flags]
	public enum BlogPostLoadFlag
	{
		OnlyMainData = 0,
		LoadAbstraction = 1,
		LoadBody = 2,
		LoadTags = 4,
		FullLoad = LoadAbstraction | LoadBody | LoadTags
	}

	public class BlogPostModel
	{
		public BlogPost BlogPost { get; set; }
		public int CommentsCount { get; set; }
	}

	public class BlogRepository : BaseRepository
	{
		private readonly BlogPostLoadFlag _blogFlag;

		public BlogRepository()
			: this(BlogPostLoadFlag.OnlyMainData)
		{
		}

		public BlogRepository(BlogPostLoadFlag blogFlag)
		{
			_blogFlag = blogFlag;
		}

		#region Entity

		public BlogPost LoadPost(int id)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();
			BlogPost post = objectQuery.FirstOrDefault(x => x.PostID == id);
			return post;
		}

		private ObjectQuery<BlogPost> GetBlogPostsObjectQuery()
		{
			ObjectQuery<BlogPost> objectQuery = DataContext.BlogPosts;
			if (NeedToLoadTags())
			{
				objectQuery = objectQuery.Include("Tags");
			}
			return objectQuery;
		}

		public List<Comment> GetComments(int id)
		{
			return DataContext.Comments.Where(x => x.PostID == id && !x.IsSpam).OrderBy(x => x.Date).ToList();
		}

		public void Save(BlogPost post, string tagsLine)
		{
			IEnumerable<string> tags = tagsLine.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim());

			if (post.PostID > 0)
			{

				List<Tag> tagsToRemove = new List<Tag>();
				foreach (Tag postTag in post.Tags)
				{
					if (tags.Count(x => x.ToLower() == postTag.LowerName) == 0)
					{
						tagsToRemove.Add(postTag);
					}
				}
				foreach (var tag in tagsToRemove)
				{
					post.Tags.Remove(tag);
				}
			}
			else
			{
				int maxPostId = DataContext.BlogPosts.Max(x => x.PostID);
				post.PostID = maxPostId + 1;
				DataContext.BlogPosts.AddObject(post);
			}

			int maxTagId = DataContext.Tags.Max(x => x.TagID);

			foreach (string strTag in tags)
			{
				string tag = strTag.ToLower();

				if (post.Tags.Count(x => x.LowerName == tag) == 0)
				{
					Tag tagDb = DataContext.Tags.SingleOrDefault(x => x.Name.ToLower() == tag);
					if (tagDb == null)
					{
						tagDb = new Tag { Name = strTag, TagID = (++maxTagId) };
						DataContext.Tags.AddObject(tagDb);
					}
					post.Tags.Add(tagDb);
				}
			}

			SubmitChanges();
		}

		#endregion

		#region Admin Works

		/// <summary>
		/// 	Get posts to administration list
		/// </summary>
		/// <param name = "pageIndex"></param>
		/// <param name = "pageSize"></param>
		/// <returns></returns>
		public List<BlogPostModel> GetPosts(int pageIndex, int pageSize)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();

			return objectQuery.OrderByDescending(x => x.Date).Skip((pageIndex - 1) * pageSize).Take(pageSize).Select(
						x => new BlogPostModel {BlogPost = x, CommentsCount = x.Comments.Count}).ToList();
		}

		public int GetPostsCount()
		{
			return DataContext.BlogPosts.Count();
		}

		/// <summary>
		/// 	Get comments to administration list
		/// </summary>
		/// <param name = "pageIndex"></param>
		/// <param name = "pageSize"></param>
		/// <returns></returns>
		public List<Comment> GetComments(int pageIndex, int pageSize)
		{
			return
				DataContext.Comments.OrderByDescending(x => x.Date).Skip((pageIndex - 1)*pageSize).Take(pageSize).ToList();
		}

		public int GetCommentsCount()
		{
			return DataContext.Comments.Count();
		}

		public List<BlogPost> GetLikePosts(int postId)
		{
			return DataContext.ExecuteStoreQuery<BlogPost>(
				@"
SELECT * FROM BlogPost AS b
INNER JOIN 
(
	SELECT   TOP(3) X.PostID
	FROM
	(
		SELECT  t2.PostID, COUNT(*) AS Count, p2.Date
		FROM            BlogPostTag AS t INNER JOIN
								 BlogPostTag AS t2 ON t.TagID = t2.TagID AND t.PostID <> t2.PostID INNER JOIN
								 BlogPost AS p1 ON t.PostID = p1.PostID INNER JOIN
								 BlogPost AS p2 ON t2.PostID = p2.PostID AND p1.Language = p2.Language AND p2.IsPublic = 1 AND p2.Date <= @datetime
		WHERE        (t.PostID = @postID)
		GROUP BY t2.PostID, p2.Date
	)  AS X
	ORDER BY X.Count DESC, X.Date DESC
) AS X on b.PostID = X.PostID

			", new SqlCeParameter { ParameterName = "postID", Value = postId }, new SqlCeParameter { ParameterName = "datetime", Value = DateTime.Now.ToUniversalTime() }).ToList();
		}

		#endregion

		#region Load Posts

		/// <summary>
		/// 	Get last items for home page
		/// </summary>
		/// <param name = "lang"></param>
		/// <param name = "count"></param>
		/// <returns></returns>
		public List<BlogPostModel> GetLast(string lang, int count)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();

			DateTime dateTime = DateTime.Now.ToUniversalTime();
			var result = objectQuery.Where(x => x.Language == lang && x.IsPublic && x.Date <= dateTime)
				.OrderByDescending(x => x.Date).Take(count).Select(
					x => new BlogPostModel {BlogPost = x, CommentsCount = x.Comments.Where(c => !c.IsSpam).Count()}).
				ToList();
			if (NeedToLoadTags())
			{
				foreach (var blogPostModel in result)
				{
					blogPostModel.BlogPost.Tags.Load();
				}
			}
			return result;
		}

		/// <summary>
		/// 	Get 10 last items for rss
		/// </summary>
		/// <param name = "lang"></param>
		/// <param name = "isExternal"></param>
		/// <returns></returns>
		public List<BlogPost> GetRss(string lang, bool isExternal)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();

			DateTime dateTime = DateTime.Now.ToUniversalTime();
			return
				objectQuery.Where(x => x.Language == lang && (!isExternal || x.IsForExternal) && x.IsPublic && x.Date <= dateTime)
							.OrderByDescending(x => x.Date).Take(10).ToList();
		}


		public List<BlogPostModel> GetPosts(int pageIndex, int pageSize, string lang)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();

			DateTime dateTime = DateTime.Now.ToUniversalTime();

			IQueryable<BlogPost> blogPosts = (from post in objectQuery
											  where post.Language == lang && post.IsPublic && post.Date <= dateTime
											  orderby post.Date descending
											  select post).Skip((pageIndex - 1)*pageSize).Take(pageSize);

			var result = blogPosts.Select(x => new BlogPostModel { BlogPost = x, CommentsCount = x.Comments.Where(c => !c.IsSpam).Count() }).
						ToList();

			if (NeedToLoadTags())
			{
				foreach (var blogPost in result)
				{
					blogPost.BlogPost.Tags.Load();
				}
			}
			
			return result;
		}

		public int GetPostsCount(string lang)
		{
			DateTime dateTime = DateTime.Now.ToUniversalTime();
			return DataContext.BlogPosts.Where(x => x.Language == lang && x.IsPublic && x.Date <= dateTime).Count();
		}

		#endregion

		#region Tag Show

		public List<BlogPostModel> GetPostsByTag(int pageIndex, int pageSize, string lang, int tagId)
		{
			var posts = GetPostsByTagQuery(lang, tagId);

			var result = posts.Skip((pageIndex - 1)*pageSize).Take(pageSize).Select(
						x => new BlogPostModel {BlogPost = x, CommentsCount = x.Comments.Where(c => !c.IsSpam).Count()}).ToList();
			if (NeedToLoadTags())
			{
				foreach (var blogPostModel in result)
				{
					blogPostModel.BlogPost.Tags.Load();
				}
			}
			return result;
		}

		public int GetPostsByTagCount(string lang, int tagId)
		{
			var posts = GetPostsByTagQuery(lang, tagId);

			return posts.Count();
		}

		private IQueryable<BlogPost> GetPostsByTagQuery(string lang, int tagId)
		{
			ObjectQuery<BlogPost> objectQuery = GetBlogPostsObjectQuery();

			DateTime universalTime = DateTime.Now.ToUniversalTime();
			return objectQuery.Where(
				x => x.Tags.Any(t => t.TagID == tagId) && x.Language == lang && x.IsPublic && x.Date <= universalTime).
				OrderByDescending(x => x.Date);
		}

		public Tag GetTag(int tagId)
		{
			return DataContext.Tags.SingleOrDefault(x => x.TagID == tagId);
		}

		public List<GetTopTagsResult> GetTopTags(string language)
		{
			List<GetTopTagsResult> result = new List<GetTopTagsResult>();

			float step;

			using (SqlCeConnection connection = new SqlCeConnection(ConfigurationUtil.GetConnectionString("LocalDatabase").ConnectionString))
			{
				connection.Open();

				using (SqlCeCommand command = new SqlCeCommand(@"
select top 1 x.Step from 
(
SELECT        CAST(COUNT(*) AS float) / CAST(10 AS float) AS Step, COUNT(*)  as Count
FROM            BlogPostTag AS t INNER JOIN
						 BlogPost AS p ON t.PostID = p.PostID
WHERE        (p.Language = @language)
GROUP BY t.TagID
) AS x
ORDER BY x.Count DESC
", connection))
				{
					command.Parameters.AddWithValue("@language", language);
					step = command.ExecuteScalar().To<float>();
				}

				using (SqlCeCommand command = new SqlCeCommand(@"
select * from
(
select top 30 * from 
(
	select  t.TagID, t.Name, cast(round( (cast(count(*) as float) / @onestep + .49999), 0 ) as int) as TagType, count(*) as Count
	from Tag t 
		inner join BlogPostTag bt on t.TagID = bt.TagID
		inner join BlogPost  p on bt.PostID = p.PostID and p.[Language] = @language
	group by t.TagID, t.Name
) as x
order by x.Count desc
) as y
order by y.Name

", connection))
				{
					command.Parameters.AddWithValue("@language", language);
					command.Parameters.AddWithValue("@onestep", step);
					SqlCeDataReader sqlCeDataReader = command.ExecuteReader();
					while(sqlCeDataReader.Read())
					{
						GetTopTagsResult r = new GetTopTagsResult();
						r.TagID = sqlCeDataReader.GetSqlInt32(0).Value;
						r.Name = sqlCeDataReader.GetSqlString(1).Value;
						r.TagType = sqlCeDataReader.GetSqlInt32(2).Value;
						result.Add(r);
					}
				}
			}



			return result;
		}

		#endregion

		#region Comments

		public bool CheckComment(Comment comment)
		{
			var comments = DataContext.Comments.Where(x =>
									   x.PostID == comment.PostID & x.UserAgent == comment.UserAgent &
									   x.UserEmail == comment.UserEmail &
									   x.UserIP == comment.UserIP & x.UserWeb == comment.UserWeb).Select(x => x.Body).AsEnumerable();

			return !comments.Contains(comment.Body);
		}

		private static object _locker = new object();

		public void AddComment(Comment comment, CommentSubscription subscription)
		{
			lock (_locker)
			{
				int maxCommentId = DataContext.Comments.Max(x => x.CommentID);
				comment.CommentID = ++maxCommentId;
				DataContext.Comments.AddObject(comment);
				if (subscription != null)
					DataContext.CommentSubscriptions.AddObject(subscription);
				DataContext.SaveChanges();
				SetNotifications(comment);
			}
		}

		public void SetNotifications(Comment comment)
		{
			using (SqlCeConnection connection = new SqlCeConnection(ConfigurationUtil.GetConnectionString("LocalDatabase").ConnectionString))
			{
				connection.Open();

				using (
					SqlCeCommand command =
						new SqlCeCommand(
							string.Format(@"
insert into Notification(CommentID, Email)
select x.CommentID, x.Email
	from 
	(
		select i.CommentID, s.Email
		from CommentSubscription s
			inner join Comment i on s.PostID = i.PostID
				and i.UserEmail <> s.Email
		where i.CommentID = @CommentID and i.IsSpam = 0
		union 
		select {1} as CommentID , '{0}' as Email
	) as x
", ConfigurationUtil.MeEmail, comment.CommentID),	connection))
				{
					command.Parameters.AddWithValue("@commentID", comment.CommentID);
					command.ExecuteNonQuery();
				}
			}
		}

		public Comment LoadComment(int id)
		{
			return DataContext.Comments.SingleOrDefault(x => x.CommentID == id);
		}

		public bool CheckSubscriptionExist(int postID, string email)
		{
			return
				DataContext.CommentSubscriptions.Count(x => x.PostID == postID && x.Email.ToLower() == email.ToLower()) > 0;
		}

		public CommentSubscription LoadSubscription(Guid id)
		{
			return DataContext.CommentSubscriptions.SingleOrDefault(x => x.SubscriptionID == id);
		}

		public UnsubscribeModel LoadUnsubscribeModel(Guid id)
		{
			return
				DataContext.CommentSubscriptions.Where(x => x.SubscriptionID == id).Select(
					s => new UnsubscribeModel {BlogTitle = s.BlogPost.Title, PostId = s.PostID}).SingleOrDefault();
		}

		public void DeleteSubscribtion(CommentSubscription commentSubscription)
		{
			DataContext.CommentSubscriptions.DeleteObject(commentSubscription);
			DataContext.SaveChanges();
		}

		#endregion

		#region SiteMap

		public List<SiteMapBlogItem> GetModelForSiteMap()
		{
			DateTime dateTime = DateTime.Now.ToUniversalTime();

			return
				DataContext.BlogPosts.Where(x => x.IsPublic && x.Date <= dateTime).OrderByDescending(x => x.Date).Select(
					x => new SiteMapBlogItem {BlogId = x.PostID, Language = x.Language}).ToList();
		}

		#endregion

		#region Helpers


		private bool NeedToLoadTags()
		{
			return (_blogFlag & BlogPostLoadFlag.LoadTags) == BlogPostLoadFlag.LoadTags;
		}

		#endregion
	}

	public class GetTopTagsResult
	{
		public int TagID { get; set; }
		public string Name { get; set; }
		public int TagType { get; set; }
	}
}