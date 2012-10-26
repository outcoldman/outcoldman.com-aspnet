// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.com)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Metablog
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Security.Authentication;
    using System.Web.Security;

    using OutcoldSolutions.Web.Blog.Core.Util;
    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    using WebMatrix.WebData;

    public class MetaWeblog : IMetaWeblog
    {
        public string CreatePost(string blogid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);

            try
            {
                BlogPost blogPost = new BlogPost { Language = blogid, Date = DateTime.Now.ToUniversalTime() };

                UpdatePost(blogPost, post, publish);

                using (BlogRepository blogRepository = new BlogRepository())
                {
                    blogRepository.Save(blogPost, string.Join(",", post.Categories));
                }

                return blogPost.PostID.ToString();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        public bool UpdatePost(string postid, string username, string password, Post post, bool publish)
        {
            ValidateUser(username, password);

            try
            {
                using (BlogRepository blogRepository = new BlogRepository())
                {
                    BlogPost blogPost = blogRepository.LoadPost(postid.To<int>());
                    UpdatePost(blogPost, post, publish);
                    blogRepository.Save(blogPost, string.Join(",", post.Categories));
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
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
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
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
                            .Select(x => new CategoryInfo { Title = x.Name, Categoryid = x.TagID.ToString() })
                            .ToList();
                    return categoryInfos.ToArray();
                }
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        public Post[] GetRecentPosts(string blogid, string username, string password, int numberOfPosts)
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
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        public MediaObjectInfo NewMediaObject(string blogid, string username, string password, MediaObject mediaObject)
        {
            ValidateUser(username, password);

            try
            {
                DateTime dateTime = DateTime.Today;
                string sole = Path.Combine(dateTime.ToString("yyyy"), dateTime.ToString("MM"), dateTime.ToString("dd"));
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Library", sole);

                if (!Directory.Exists(imagePath))
                {
                    Trace.TraceInformation("create directory {0}", imagePath);
                    Directory.CreateDirectory(imagePath);
                }

                string mediaObjectName = Path.GetFileName(mediaObject.Name);

                string filePath = Path.Combine(imagePath, mediaObjectName);

                int index = 0;
                while (File.Exists(filePath))
                {
                    filePath = Path.Combine(
                        imagePath, 
                        string.Format("{0}_{1}{2}", Path.GetFileNameWithoutExtension(mediaObjectName), index++, Path.GetExtension(mediaObjectName)));
                }

                Trace.TraceInformation("create file {0}", filePath);

                using (FileStream fileStream = new FileStream(filePath, FileMode.CreateNew, FileAccess.ReadWrite))
                {
                    fileStream.Write(mediaObject.Bits, 0, mediaObject.Bits.Length);
                }

                MediaObjectInfo objectInfo = new MediaObjectInfo
                    {
                        Url = string.Format("/Library/{0}/{1}", sole.Replace(Path.DirectorySeparatorChar, '/'), Path.GetFileName(filePath))
                    };
                return objectInfo;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw;
            }
        }

        public bool DeletePost(string key, string postid, string username, string password, bool publish)
        {
            ValidateUser(username, password);

            Trace.TraceInformation("Delete post {0}", postid);

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
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
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
                            BlogName =
                                string.Format(
                                    "{0} {1}", 
                                    ConfigurationUtil.AuthorUsername, 
                                    "ru"), 
                            Url =
                                string.Format(
                                    "{0}/{1}/blog/index", 
                                    ConfigurationUtil.SiteUrl, 
                                    "ru")
                        }, 
                    new BlogInfo
                        {
                            BlogId = "en", 
                            BlogName =
                                string.Format(
                                    "{0} {1}", 
                                    ConfigurationUtil.AuthorUsername, 
                                    "en"), 
                            Url =
                                string.Format(
                                    "{0}/{1}/blog/index", 
                                    ConfigurationUtil.SiteUrl, 
                                    "en")
                        }
                };

            return infoList.ToArray();
        }

        public UserInfo GetUserInfo(string key, string username, string password)
        {
            ValidateUser(username, password);
            UserInfo info = new UserInfo
                {
                    Email = ConfigurationUtil.AuthorEmail, 
                    Nickname = ConfigurationUtil.AuthorUsername, 
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
                Trace.TraceError(message);
                throw new AuthenticationException(message);
            }

            Trace.TraceInformation("User valid");
        }

        private static string GetBlogUrl(BlogPost blogPost)
        {
            return string.Format("{0}/{1}/blog/show/{2}", ConfigurationUtil.SiteUrl, blogPost.Language, blogPost.PostID);
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
                    Userid = ConfigurationUtil.AuthorUsername, 
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