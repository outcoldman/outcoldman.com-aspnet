// --------------------------------------------------------------------------------------------------------------------
// Outcold Solutions (http://outcoldman.ru)
// --------------------------------------------------------------------------------------------------------------------

namespace OutcoldSolutions.Web.Blog.Models.Repositories
{
    using System;
    using System.Diagnostics;
    using System.Transactions;

    /// <summary>
    /// 	Default(Base) ModelDataContext Repository
    /// </summary>
    public class BaseRepository : IDisposable
    {
        public BaseRepository()
            : this(new LocalDatabaseEntities())
        {
        }

        public BaseRepository(LocalDatabaseEntities dataContext)
        {
            if (dataContext == null)
            {
                throw new ArgumentNullException("dataContext");
            }

            this.DataContext = dataContext;
            this.DataContext.ContextOptions.LazyLoadingEnabled = false;
            this.DataContext.ContextOptions.ProxyCreationEnabled = false;
        }

        #region Model Data Context

        /// <summary>
        /// 	Active DataContext
        /// </summary>
        public LocalDatabaseEntities DataContext { get; private set; }

        public void Dispose()
        {
            if (this.DataContext != null)
            {
                this.DataContext.Dispose();
            }
        }

        #endregion

        #region protected members

        protected void SubmitChanges()
        {
            try
            {
                using (TransactionScope scope = new TransactionScope())
                {
                    this.DataContext.SaveChanges();
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion
    }
}