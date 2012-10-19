// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Suites.Model
{
    using System;

    using NUnit.Framework;

    using OutcoldSolutions.Web.Blog.Models;
    using OutcoldSolutions.Web.Blog.Models.Repositories;

    [TestFixture]
    public class CommentRepositorySpec
    {
        [Test]
        public void Work_With_Comment()
        {
            using (BaseRepository repository = new BaseRepository())
            {
                Comment comment = new Comment
                    {
                        Date = DateTime.Now, 
                        PostID = 150, 
                        Body = "Comment", 
                        UserIP = "127.0.0.1"
                    };
                repository.DataContext.Comments.AddObject(comment);
                repository.DataContext.SaveChanges();
                comment.Body = "Comment 2";
                repository.DataContext.SaveChanges();
                repository.DataContext.Comments.DeleteObject(comment);
            }
        }
    }
}