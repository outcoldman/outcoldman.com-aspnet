// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------
namespace OutcoldSolutions.Web.Blog.Models.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Transactions;

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

        public LocalDatabaseEntities DataContext
        {
            get
            {
                return this._dataContext;
            }
        }

        public void Dispose()
        {
            this._dataContext.Dispose();
        }

        public IList<NotificationModel> GetNotifications()
        {
            return (from n in this.DataContext.Notifications
                    join c in this.DataContext.Comments on n.CommentID equals c.CommentID
                    join p in this.DataContext.BlogPosts on c.PostID equals p.PostID
                    join s in this.DataContext.CommentSubscriptions on new { c.PostID, email = n.Email.ToLower() }
                        equals new { s.PostID, email = s.Email.ToLower() } into sd
                    from sdd in sd.DefaultIfEmpty()
                    where n.IsSent == false
                    select
                        new NotificationModel
                            {
                                Notification = n, 
                                Comment = c, 
                                Language = p.Language, 
                                SubscribtionId = sdd.SubscriptionID, 
                                Title = p.Title
                            }).ToList();
        }

        public void UpdateNotification(Notification notification)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                notification.IsSent = true;
                this.DataContext.SaveChanges();
                scope.Complete();
            }
        }
    }
}