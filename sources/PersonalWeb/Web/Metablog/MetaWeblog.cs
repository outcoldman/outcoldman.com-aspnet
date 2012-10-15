using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Web.Security;
using log4net;
using PersonalWeb.Core.Util;
using PersonalWeb.Model;
using PersonalWeb.Model.Repositories;

namespace PersonalWeb.Web.Metablog
{
	public class MetaWeblog : IMetaWeblog
	{
		private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

		public string CreatePost(string blogid, string username, string password,
		                         Post post, bool publish)
		{
			ValidateUser(username, password);

			try
			{
				BlogPost blogPost = new BlogPost
				{
					Language = blogid,
					Date = DateTime.Now.ToUniversalTime()
				};

				UpdatePost(blogPost, post, publish);

				using (BlogRepository blogRepository = new BlogRepository())
				{
					blogRepository.Save(blogPost, String.Join(",", post.Categories));
				}

				return blogPost.PostID.ToString();
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw;
			}
		}

		public bool UpdatePost(string postid, string username, string password,
		                       Post post, bool publish)
		{
			ValidateUser(username, password);

			try
			{
				using (BlogRepository blogRepository = new BlogRepository())
				{
					BlogPost blogPost = blogRepository.LoadPost(postid.To<int>());
					UpdatePost(blogPost, post, publish);
					blogRepository.Save(blogPost, String.Join(",", post.Categories));
				}
				return true;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
			
		}

		public Post GetPost(string postid, string username, string password)
		{
			ValidateUser(username, password);
			try
			{
				using (BlogRepository blogRepository = new BlogRepository())
				{
					BlogPost blogPost = blogRepository.LoadPost(postid.To<int>());

					return BlogPostCast(blogPost);
				}
			}
			catch(Exception e)
			{
				Log.Error(e);
				throw;
			}
		}

		public CategoryInfo[] GetCategories(string blogid, string username, string password)
		{
			ValidateUser(username, password);
			try
			{
				using (BlogRepository blogRepository = new BlogRepository())
				{
					List<CategoryInfo> categoryInfos =
						blogRepository.DataContext.Tags.AsEnumerable()
							.Select(x => new CategoryInfo { Title = x.Name, Categoryid = x.TagID.ToString() }).
							ToList();
					return categoryInfos.ToArray();
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw;
			}
		}

		public Post[] GetRecentPosts(string blogid, string username, string password,
		                             int numberOfPosts)
		{
			ValidateUser(username, password);
			try
			{
				List<Post> posts = new List<Post>();

				using (BlogRepository blogRepository = new BlogRepository())
				{
					List<BlogPostModel> list = blogRepository.GetLast(blogid, numberOfPosts);

					posts.AddRange(list.Select(post => BlogPostCast(post.BlogPost)));

					return posts.ToArray();
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw;
			}
		}

		public MediaObjectInfo NewMediaObject(string blogid, string username, string password,
		                                      MediaObject mediaObject)
		{
			ValidateUser(username, password);

			try
			{
				DateTime dateTime = DateTime.Today;
				string sole = Path.Combine(dateTime.ToString("yyyy"), dateTime.ToString("MM"), dateTime.ToString("dd"));
				string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Library", sole);

				if (!Directory.Exists(imagePath))
				{
					Log.DebugFormat("create directory {0}", imagePath);
					Directory.CreateDirectory(imagePath);
				}

				
				string mediaObjectName = Path.GetFileName(mediaObject.Name);

				string filePath = Path.Combine(imagePath, mediaObjectName);

				int index = 0;
				while (File.Exists(filePath))
				{
					filePath = Path.Combine(imagePath,
											string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(mediaObjectName), index++,
														  Path.GetExtension(mediaObjectName)));
				}

				Log.DebugFormat("create file {0}", filePath);

				using (FileStream fileStream = new FileStream(filePath
															  , FileMode.CreateNew, FileAccess.ReadWrite))
				{
					fileStream.Write(mediaObject.Bits, 0, mediaObject.Bits.Length);
				}

				MediaObjectInfo objectInfo = new MediaObjectInfo
				{
					Url =
						string.Format("{0}/Library/{1}/{2}", ConfigurationUtil.SiteUrl, sole,
									  Path.GetFileName(filePath))
				};
				return objectInfo;
			}
			catch (Exception e)
			{
				Log.Error(e);
				throw;
			}
		}

		public bool DeletePost(string key, string postid, string username, string password, bool publish)
		{
			ValidateUser(username, password);

			Log.DebugFormat("Delete post {0}", postid);

			try
			{
				using (BlogRepository blogRepository = new BlogRepository())
				{
					BlogPost blogPost = blogRepository.LoadPost(postid.To<int>());
					if (blogPost != null)
					{
						blogRepository.DataContext.BlogPosts.DeleteObject(blogPost);
						blogRepository.DataContext.SaveChanges();
						return true;
					}
				}
				return false;
			}
			catch(Exception e)
			{
				Log.Error(e);
				return false;
			}
		}

		public BlogInfo[] GetUsersBlogs(string key, string username, string password)
		{
			ValidateUser(username, password);

			List<BlogInfo> infoList = new List<BlogInfo>
			                          	{
			                          		new BlogInfo
			                          			{
			                          				BlogId = "ru",
			                          				BlogName = string.Format("{0} {1}", ConfigurationUtil.Me, "ru"),
			                          				Url = string.Format("{0}/{1}/blog/index", ConfigurationUtil.SiteUrl, "ru")
			                          			},
			                          		new BlogInfo
			                          			{
			                          				BlogId = "en",
			                          				BlogName = string.Format("{0} {1}", ConfigurationUtil.Me, "en"),
			                          				Url = string.Format("{0}/{1}/blog/index", ConfigurationUtil.SiteUrl, "en")
			                          			}
			                          	};

			return infoList.ToArray();
		}

		public UserInfo GetUserInfo(string key, string username, string password)
		{
			ValidateUser(username, password);
			UserInfo info = new UserInfo
			                	{
			                		Email = ConfigurationUtil.MeEmail,
			                		Nickname = ConfigurationUtil.Me,
			                		Url = ConfigurationUtil.SiteUrl
			                	};
			return info;
		}

		private static void ValidateUser(string username, string password)
		{
			bool isValid = Membership.ValidateUser(username, password);
			if (!isValid)
			{
				string message = string.Format("User is not valid : {0}", username);
				Log.Error(message);
				throw new AuthenticationException(message);
			}
			Log.Debug("User valid");
		}

		private static string GetBlogUrl(BlogPost blogPost)
		{
			return string.Format("{0}/{1}/blog/show/{2}", ConfigurationUtil.SiteUrl, blogPost.Language,
			                     blogPost.PostID);
		}

		private static Post BlogPostCast(BlogPost blogPost)
		{
			return new Post
			       	{
			       		Postid = blogPost.PostID,
			       		Categories = blogPost.Tags.Select(x => x.Name).ToArray(),
			       		DateCreated = blogPost.Date,
			       		DateCreatedGmt = blogPost.Date,
			       		Description = blogPost.HtmlText,
			       		Permalink = GetBlogUrl(blogPost),
			       		Title = blogPost.Title,
			       		Userid = ConfigurationUtil.Me,
			       		Publish = blogPost.IsPublic
			       	};
		}

		private static void UpdatePost(BlogPost blogPost, Post post, bool publish)
		{
			blogPost.Title = post.Title;
			blogPost.HtmlText = post.Description;
			blogPost.IsPublic = publish;
			blogPost.HtmlAbstraction = post.Description;
			blogPost.IsForExternal = true;

			const string splitTag = "<hr />";

			int indexOf = post.Description.IndexOf(splitTag);
			if (indexOf > 0)
			{
				blogPost.HtmlAbstraction = post.Description.Substring(0, indexOf);
				blogPost.HtmlText = post.Description.Replace(splitTag, string.Empty);
			}
		}
	}
}