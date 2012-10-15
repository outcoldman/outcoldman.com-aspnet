using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Data;

namespace PersonalWeb.Web.Providers
{
	/// <summary>
	/// http://sqlcemembership.codeplex.com/
	/// </summary>
	public class SqlCeMembershipUtils
	{

		private static readonly object _lock = new object();
		public static void CreateDatabaseIfRequired(string connection)
		{
			string dataDirectory = AppDomain.CurrentDomain.GetData("DataDirectory") as string;

			string physConnectionString = connection.Replace("|DataDirectory|", dataDirectory);

			string sdfPath = string.Empty;
			lock (_lock)
			{
				using (var testConn = new SqlCeConnection(physConnectionString))
				{
					sdfPath = testConn.Database;
				}
				if (string.IsNullOrWhiteSpace(sdfPath))
					return;

				if (!System.IO.File.Exists(sdfPath))
				{
					//OK, try to create the database file
					using (var engine = new SqlCeEngine(connection))
					{
						engine.CreateDatabase();
					}
				}
				ValidateDatabase(connection);
			}
		}

		private static void ValidateDatabase(string connection)
		{
			using (var conn = new SqlCeConnection(connection))
			{
				conn.Open();
				IList<string> tables = GetTableNames(conn);
				using (var cmd = new SqlCeCommand())
				{
					cmd.Connection = conn;

					if (!tables.Contains("aspnet_Users"))
						CreateUsers(cmd);
				}
			}
		}



		private static void CreateUsers(SqlCeCommand cmd)
		{
			cmd.CommandText =
				@"CREATE TABLE [aspnet_Users] (
						  PKID uniqueidentifier NOT NULL default NEWID() PRIMARY KEY,
						  Username nvarchar(255)  NOT NULL,
						  ApplicationName nvarchar(100) NOT NULL,
						  Email nvarchar(100)  NOT NULL,
						  Comment nvarchar(255)  NULL,
						  Password nvarchar(128)  NOT NULL,
						  PasswordQuestion nvarchar(255) NULL,
						  PasswordAnswer nvarchar(255)  NULL,
						  IsApproved bit NULL,
						  LastActivityDate datetime NULL,
						  LastLoginDate datetime NULL,
						  LastPasswordChangedDate datetime NULL,
						  CreationDate datetime NULL,
						  IsOnLine bit  NULL,
						  IsLockedOut bit  NULL,
						  LastLockedOutDate datetime NULL,
						  FailedPasswordAttemptCount int NULL,
						  FailedPasswordAttemptWindowStart datetime NULL,
						  FailedPasswordAnswerAttemptCount int NULL,
						  FailedPasswordAnswerAttemptWindowStart datetime NULL
						);
						";
			cmd.ExecuteNonQuery();

			cmd.CommandText =
				@"CREATE UNIQUE INDEX idxUser ON [aspnet_Users] ( Username, ApplicationName);";
			cmd.ExecuteNonQuery();
		}

		private static IList<string> GetTableNames(SqlCeConnection testConn)
		{
			var dt = testConn.GetSchema("Tables");
			IList<string> tables = new List<string>();
			foreach (DataRow r in dt.Rows)
			{
				tables.Add(r["TABLE_NAME"].ToString());
			}
			return tables;
		}
	}
}
