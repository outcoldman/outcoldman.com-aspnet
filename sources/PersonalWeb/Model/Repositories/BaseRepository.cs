using System;
using System.Transactions;
using PersonalWeb.Core.Util;

namespace PersonalWeb.Model.Repositories
{
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
			if (dataContext == null) throw new ArgumentNullException("dataContext");

			DataContext = dataContext;
			DataContext.ContextOptions.LazyLoadingEnabled = false;
			DataContext.ContextOptions.ProxyCreationEnabled = false;
		}

		#region Model Data Context

		/// <summary>
		/// 	Active DataContext
		/// </summary>
		public LocalDatabaseEntities DataContext { get; private set; }

		public void Dispose()
		{
			if (DataContext != null)
				DataContext.Dispose();
		}

		#endregion

		#region protected members

		protected void SubmitChanges()
		{
			try
			{
				using (TransactionScope scope = new TransactionScope())
				{
					DataContext.SaveChanges();
					scope.Complete();
				}
			}
			catch (Exception ex)
			{
				Log4NetHelper.Log.Error(ex);
			}
		}

		#endregion
	}
}