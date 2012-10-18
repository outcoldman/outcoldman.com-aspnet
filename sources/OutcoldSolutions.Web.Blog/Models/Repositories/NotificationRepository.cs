using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;

namespace PersonalWeb.Model.Repositories
{
    using OutcoldSolutions.Web.Blog.Models;

    public class NotificationModel
    {
        public Notification Notification { get; set; }
        public Comment Comment { get; set; }
        public string Title { get; set; }
        public string Language { get; set; }
        public Guid? SubscribtionId { get; set; }
    }

    public class NotificationRepository : IDisposable
    {
        private readonly LocalDatabaseEntities _dataContext = new LocalDatabaseEntities();

        public LocalDatabaseEntities DataContext { get { return _dataContext; } }

        public IList<NotificationModel> GetNotifications()
        {
            return (from n in DataContext.Notifications
                    join c in DataContext.Comments on n.CommentID equals c.CommentID
                    join p in DataContext.BlogPosts on c.PostID equals p.PostID
                    join s in DataContext.CommentSubscriptions on new { c.PostID, email = n.Email.ToLower() } equals new { s.PostID, email = s.Email.ToLower() } into sd
                    from sdd in sd.DefaultIfEmpty()
                    where n.IsSent == false
                    select new NotificationModel { Notification = n, Comment = c, Language = p.Language, SubscribtionId = sdd.SubscriptionID, Title = p.Title }).ToList();
        }

        public void UpdateNotification(Notification notification)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                notification.IsSent = true;
                DataContext.SaveChanges();
                scope.Complete();
            }
        }

        public void Dispose()
        {
            _dataContext.Dispose();
        }
    }
}
