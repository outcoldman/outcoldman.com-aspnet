using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;
using System.Data;
using System.Data.SqlServerCe;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Web.Security;

namespace PersonalWeb.Web.Providers
{
	/// <summary>
	/// http://sqlcemembership.codeplex.com/
	/// </summary>
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "Ce")]
	public sealed class SqlCeMembershipProvider : MembershipProvider
	{
		private const string encryptionKey = "AE09F72BA97CBBB5EEAAFF";

		private int newPasswordLength = 8;
		private string eventSource = "SqlCeMembershipProvider";
		private string eventLog = "Application";
		private string exceptionMessage = "An exception occurred. Please check the Event Log.";
		private string connectionString;

		//
		// If false, exceptions are thrown to the caller. If true,
		// exceptions are written to the event log.
		//

		private bool pWriteExceptionsToEventLog;

		public bool WriteExceptionsToEventLog
		{
			get { return pWriteExceptionsToEventLog; }
			set { pWriteExceptionsToEventLog = value; }
		}


		//
		// System.Configuration.Provider.ProviderBase.Initialize Method
		//

		public override void Initialize(string name, NameValueCollection config)
		{
			//
			// Initialize values from web.config.
			//

			if (config == null)
				throw new ArgumentNullException("config");

			if (name == null || name.Length == 0)
				name = "SqlCeMembershipProvider";

			if (String.IsNullOrEmpty(config["description"]))
			{
				config.Remove("description");
				config.Add("description", "SqlCe Membership provider");
			}

			// Initialize the abstract base class.
			base.Initialize(name, config);

			pApplicationName = GetConfigValue(config["applicationName"],
											System.Web.Hosting.HostingEnvironment.ApplicationVirtualPath);
			pMaxInvalidPasswordAttempts = Convert.ToInt32(GetConfigValue(config["maxInvalidPasswordAttempts"], "5"), CultureInfo.InvariantCulture);
			pPasswordAttemptWindow = Convert.ToInt32(GetConfigValue(config["passwordAttemptWindow"], "10"), CultureInfo.InvariantCulture);
			pMinRequiredNonAlphanumericCharacters = Convert.ToInt32(GetConfigValue(config["minRequiredNonAlphanumericCharacters"], "1"), CultureInfo.InvariantCulture);
			pMinRequiredPasswordLength = Convert.ToInt32(GetConfigValue(config["minRequiredPasswordLength"], "7"), CultureInfo.InvariantCulture);
			pPasswordStrengthRegularExpression = Convert.ToString(GetConfigValue(config["passwordStrengthRegularExpression"], ""), CultureInfo.InvariantCulture);
			pEnablePasswordReset = Convert.ToBoolean(GetConfigValue(config["enablePasswordReset"], "true"), CultureInfo.InvariantCulture);
			pEnablePasswordRetrieval = Convert.ToBoolean(GetConfigValue(config["enablePasswordRetrieval"], "true"), CultureInfo.InvariantCulture);
			pRequiresQuestionAndAnswer = Convert.ToBoolean(GetConfigValue(config["requiresQuestionAndAnswer"], "false"), CultureInfo.InvariantCulture);
			pRequiresUniqueEmail = Convert.ToBoolean(GetConfigValue(config["requiresUniqueEmail"], "true"), CultureInfo.InvariantCulture);
			pWriteExceptionsToEventLog = Convert.ToBoolean(GetConfigValue(config["writeExceptionsToEventLog"], "true"), CultureInfo.InvariantCulture);

			string temp_format = config["passwordFormat"];
			if (temp_format == null)
			{
				temp_format = "Hashed";
			}

			switch (temp_format)
			{
				case "Hashed":
					pPasswordFormat = MembershipPasswordFormat.Hashed;
					break;
				case "Encrypted":
					pPasswordFormat = MembershipPasswordFormat.Encrypted;
					break;
				case "Clear":
					pPasswordFormat = MembershipPasswordFormat.Clear;
					break;
				default:
					throw new ProviderException("Password format not supported.");
			}

			//
			// Initialize SqlCeConnection.
			//

			ConnectionStringSettings ConnectionStringSettings =
			  ConfigurationManager.ConnectionStrings[config["connectionStringName"]];

			if (ConnectionStringSettings == null || string.IsNullOrWhiteSpace(ConnectionStringSettings.ConnectionString))
			{
				throw new ProviderException("Connection string cannot be blank.");
			}

			connectionString = ConnectionStringSettings.ConnectionString;

			SqlCeMembershipUtils.CreateDatabaseIfRequired(connectionString);
		}



		//
		// A helper function to retrieve config values from the configuration file.
		//

		private string GetConfigValue(string configValue, string defaultValue)
		{
			if (String.IsNullOrEmpty(configValue))
				return defaultValue;

			return configValue;
		}


		//
		// System.Web.Security.MembershipProvider properties.
		//


		private string pApplicationName;
		private bool pEnablePasswordReset;
		private bool pEnablePasswordRetrieval;
		private bool pRequiresQuestionAndAnswer;
		private bool pRequiresUniqueEmail;
		private int pMaxInvalidPasswordAttempts;
		private int pPasswordAttemptWindow;
		private MembershipPasswordFormat pPasswordFormat;

		public override string ApplicationName
		{
			get { return pApplicationName; }
			set { pApplicationName = value; }
		}

		public override bool EnablePasswordReset
		{
			get { return pEnablePasswordReset; }
		}


		public override bool EnablePasswordRetrieval
		{
			get { return pEnablePasswordRetrieval; }
		}


		public override bool RequiresQuestionAndAnswer
		{
			get { return pRequiresQuestionAndAnswer; }
		}


		public override bool RequiresUniqueEmail
		{
			get { return pRequiresUniqueEmail; }
		}


		public override int MaxInvalidPasswordAttempts
		{
			get { return pMaxInvalidPasswordAttempts; }
		}


		public override int PasswordAttemptWindow
		{
			get { return pPasswordAttemptWindow; }
		}


		public override MembershipPasswordFormat PasswordFormat
		{
			get { return pPasswordFormat; }
		}

		private int pMinRequiredNonAlphanumericCharacters;

		public override int MinRequiredNonAlphanumericCharacters
		{
			get { return pMinRequiredNonAlphanumericCharacters; }
		}

		private int pMinRequiredPasswordLength;

		public override int MinRequiredPasswordLength
		{
			get { return pMinRequiredPasswordLength; }
		}

		private string pPasswordStrengthRegularExpression;

		public override string PasswordStrengthRegularExpression
		{
			get { return pPasswordStrengthRegularExpression; }
		}

		//
		// System.Web.Security.MembershipProvider methods.
		//

		//
		// MembershipProvider.ChangePassword
		//

		public override bool ChangePassword(string username, string oldPassword, string newPassword)
		{
			if (!ValidateUser(username, oldPassword))
				return false;


			ValidatePasswordEventArgs args =
			  new ValidatePasswordEventArgs(username, newPassword, true);

			OnValidatingPassword(args);

			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Change password canceled due to new password validation failure.");

			int rowsAffected = 0;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
						" SET Password = @Password, LastPasswordChangedDate = @LastPasswordChangedDate " +
						" WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = EncodePassword(newPassword);
					cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						rowsAffected = cmd.ExecuteNonQuery();
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "ChangePassword");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			if (rowsAffected > 0)
			{
				return true;
			}

			return false;
		}



		//
		// MembershipProvider.ChangePasswordQuestionAndAnswer
		//

		public override bool ChangePasswordQuestionAndAnswer(string username,
					  string password,
					  string newPasswordQuestion,
					  string newPasswordAnswer)
		{

			int rowsAffected = 0;

			if (!ValidateUser(username, password))
				return false;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
						" SET PasswordQuestion = @Question, PasswordAnswer = @Answer" +
						" WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Question", SqlDbType.NVarChar, 255).Value = newPasswordQuestion;
					cmd.Parameters.Add("@Answer", SqlDbType.NVarChar, 255).Value = EncodePassword(newPasswordAnswer);
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						rowsAffected = cmd.ExecuteNonQuery();
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "ChangePasswordQuestionAndAnswer");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			if (rowsAffected > 0)
			{
				return true;
			}

			return false;
		}



		//
		// MembershipProvider.CreateUser
		//

		public override MembershipUser CreateUser(string username,
				 string password,
				 string email,
				 string passwordQuestion,
				 string passwordAnswer,
				 bool isApproved,
				 object providerUserKey,
				 out MembershipCreateStatus status)
		{
			ValidatePasswordEventArgs args =
			  new ValidatePasswordEventArgs(username, password, true);

			OnValidatingPassword(args);

			if (args.Cancel)
			{
				status = MembershipCreateStatus.InvalidPassword;
				return null;
			}

			if (RequiresUniqueEmail && !string.IsNullOrWhiteSpace(GetUserNameByEmail(email)))
			{
				status = MembershipCreateStatus.DuplicateEmail;
				return null;
			}

			MembershipUser u = GetUser(username, false);

			if (u == null)
			{
				DateTime createDate = DateTime.Now;

				if (providerUserKey == null)
				{
					providerUserKey = Guid.NewGuid();
				}
				else
				{
					if (!(providerUserKey is Guid))
					{
						status = MembershipCreateStatus.InvalidProviderUserKey;
						return null;
					}
				}

				passwordQuestion = string.IsNullOrWhiteSpace(passwordQuestion) ? string.Empty : passwordQuestion;
				passwordAnswer = string.IsNullOrWhiteSpace(passwordAnswer) ? string.Empty : passwordAnswer;

				using (SqlCeConnection conn = new SqlCeConnection(connectionString))
				{
					using (SqlCeCommand cmd = new SqlCeCommand("INSERT INTO [aspnet_Users] " +
						  " (PKID, Username, Password, Email, PasswordQuestion, " +
						  " PasswordAnswer, IsApproved," +
						  " Comment, CreationDate, LastPasswordChangedDate, LastActivityDate," +
						  " ApplicationName, IsLockedOut, LastLockedOutDate," +
						  " FailedPasswordAttemptCount, FailedPasswordAttemptWindowStart, " +
						  " FailedPasswordAnswerAttemptCount, FailedPasswordAnswerAttemptWindowStart)" +
						  " Values(@PKID, @Username, @Password, @Email, @PasswordQuestion, " +
						  " @PasswordAnswer, @IsApproved, @Comment, @CreationDate, @LastPasswordChangedDate, " +
						  " @LastActivityDate, @ApplicationName, @IsLockedOut, @LastLockedOutDate, " +
						  " @FailedPasswordAttemptCount, @FailedPasswordAttemptWindowStart, " +
						  " @FailedPasswordAnswerAttemptCount, @FailedPasswordAnswerAttemptWindowStart)", conn))
					{

						cmd.Parameters.Add("@PKID", SqlDbType.UniqueIdentifier).Value = (Guid)providerUserKey;
						cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
						cmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = EncodePassword(password);
						cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 128).Value = email;
						cmd.Parameters.Add("@PasswordQuestion", SqlDbType.NVarChar, 255).Value = passwordQuestion;
						cmd.Parameters.Add("@PasswordAnswer", SqlDbType.NVarChar, 255).Value = EncodePassword(passwordAnswer);
						cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = isApproved;
						cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = "";
						cmd.Parameters.Add("@CreationDate", SqlDbType.DateTime).Value = createDate;
						cmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = createDate;
						cmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = createDate;
						cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;
						cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = false;
						cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = createDate;
						cmd.Parameters.Add("@FailedPasswordAttemptCount", SqlDbType.Int).Value = 0;
						cmd.Parameters.Add("@FailedPasswordAttemptWindowStart", SqlDbType.DateTime).Value = createDate;
						cmd.Parameters.Add("@FailedPasswordAnswerAttemptCount", SqlDbType.Int).Value = 0;
						cmd.Parameters.Add("@FailedPasswordAnswerAttemptWindowStart", SqlDbType.DateTime).Value = createDate;

						try
						{
							conn.Open();

							int recAdded = cmd.ExecuteNonQuery();

							if (recAdded > 0)
							{
								status = MembershipCreateStatus.Success;
							}
							else
							{
								status = MembershipCreateStatus.UserRejected;
							}
						}
						catch (SqlCeException e)
						{
							if (WriteExceptionsToEventLog)
							{
								WriteToEventLog(e, "CreateUser");
							}

							status = MembershipCreateStatus.ProviderError;
						}
					}
				}
				return GetUser(username, false);
			}
			else
			{
				status = MembershipCreateStatus.DuplicateUserName;
			}

			return null;
		}



		//
		// MembershipProvider.DeleteUser
		//

		public override bool DeleteUser(string username, bool deleteAllRelatedData)
		{
			int rowsAffected = 0;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("DELETE FROM [aspnet_Users] " +
						" WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						rowsAffected = cmd.ExecuteNonQuery();

						if (deleteAllRelatedData)
						{
							//TODO - Process commands to delete all data for the user in the database.
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "DeleteUser");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			if (rowsAffected > 0)
				return true;

			return false;
		}

		//
		// MembershipProvider.GetAllUsers
		//

		public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection users = new MembershipUserCollection();

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Count(*) FROM [aspnet_Users] " +
												  "WHERE ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;

					totalRecords = 0;

					try
					{
						conn.Open();
						totalRecords = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);

						if (totalRecords <= 0) { return users; }

						cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
								 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
								 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
								 " FROM [aspnet_Users] " +
								 " WHERE ApplicationName = @ApplicationName " +
								 " ORDER BY Username Asc";

						using (var reader = cmd.ExecuteReader())
						{
							int counter = 0;
							int startIndex = pageSize * pageIndex;
							int endIndex = startIndex + pageSize - 1;

							while (reader.Read())
							{
								if (counter >= startIndex)
								{
									MembershipUser u = GetUserFromReader(reader);
									users.Add(u);
								}

								if (counter >= endIndex) { cmd.Cancel(); }

								counter++;
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetAllUsers");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return users;
		}


		//
		// MembershipProvider.GetNumberOfUsersOnline
		//

		public override int GetNumberOfUsersOnline()
		{
			int numOnline = 0;
			TimeSpan onlineSpan = new TimeSpan(0, System.Web.Security.Membership.UserIsOnlineTimeWindow, 0);
			DateTime compareTime = DateTime.Now.Subtract(onlineSpan);

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Count(*) FROM [aspnet_Users] " +
						" WHERE LastActivityDate > @LastActivityDate AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@CompareDate", SqlDbType.DateTime).Value = compareTime;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						numOnline = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetNumberOfUsersOnline");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return numOnline;
		}



		//
		// MembershipProvider.GetPassword
		//

		public override string GetPassword(string username, string answer)
		{
			if (!EnablePasswordRetrieval)
			{
				throw new ProviderException("Password Retrieval Not Enabled.");
			}

			if (PasswordFormat == MembershipPasswordFormat.Hashed)
			{
				throw new ProviderException("Cannot retrieve Hashed passwords.");
			}

			string password = "";
			string passwordAnswer = "";

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Password, PasswordAnswer, IsLockedOut FROM [aspnet_Users] " +
					  " WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();
						using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
						{
							if (reader.Read())
							{
								if (reader.GetBoolean(2))
									throw new MembershipPasswordException("The supplied user is locked out.");
								password = reader.GetString(0);
								passwordAnswer = reader.GetString(1);
							}
							else
							{
								throw new MembershipPasswordException("The supplied user name is not found.");
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetPassword");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}

			if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
			{
				UpdateFailureCount(username, "passwordAnswer");

				throw new MembershipPasswordException("Incorrect password answer.");
			}


			if (PasswordFormat == MembershipPasswordFormat.Encrypted)
			{
				password = UnEncodePassword(password);
			}

			return password;
		}



		//
		// MembershipProvider.GetUser(string, bool)
		//

		public override MembershipUser GetUser(string username, bool userIsOnline)
		{
			MembershipUser u = null;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT PKID, Username, Email, PasswordQuestion," +
					 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
					 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
					 " FROM [aspnet_Users] WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{

					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						using (var reader = cmd.ExecuteReader())
						{

							if (reader.Read())
							{
								u = GetUserFromReader(reader);

								if (userIsOnline)
								{
									using (SqlCeCommand updateCmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
											  "SET LastActivityDate = @LastActivityDate " +
											  "WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
									{

										updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
										updateCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
										updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

										updateCmd.ExecuteNonQuery();
									}
								}
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetUser(String, Boolean)");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}

			return u;
		}


		//
		// MembershipProvider.GetUser(object, bool)
		//

		public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
		{
			MembershipUser u = null;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT PKID, Username, Email, PasswordQuestion," +
					  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
					  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate" +
					  " FROM [aspnet_Users] WHERE PKID = @PKID", conn))
				{
					cmd.Parameters.Add("@PKID", SqlDbType.UniqueIdentifier).Value = (Guid)providerUserKey;

					try
					{
						conn.Open();

						using (var reader = cmd.ExecuteReader())
						{
							if (reader.Read())
							{
								u = GetUserFromReader(reader);

								if (userIsOnline)
								{
									using (SqlCeCommand updateCmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
											  "SET LastActivityDate = @LastActivityDate " +
											  "WHERE PKID = @PKID", conn))
									{
										updateCmd.Parameters.Add("@LastActivityDate", SqlDbType.DateTime).Value = DateTime.Now;
										updateCmd.Parameters.Add("@PKID", SqlDbType.UniqueIdentifier).Value = (Guid)providerUserKey;

										updateCmd.ExecuteNonQuery();
									}
								}
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetUser(Object, Boolean)");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return u;
		}


		//
		// GetUserFromReader
		//    A helper function that takes the current row from the SqlCeDataReader
		// and hydrates a MembershipUser from the values. Called by the 
		// MembershipUser.GetUser implementation.
		//

		private MembershipUser GetUserFromReader(SqlCeDataReader reader)
		{
			if (string.IsNullOrWhiteSpace(reader.GetString(1))) return null;
			object providerUserKey = null;
			string strGooid = Guid.NewGuid().ToString();

			if (reader.GetValue(0).ToString().Length > 0)
				providerUserKey = new Guid(reader.GetValue(0).ToString());
			else
				providerUserKey = new Guid(strGooid);

			string username = reader.GetString(1);

			string email = reader.GetString(2);

			string passwordQuestion = reader.IsDBNull(3) ? string.Empty : reader.GetString(3);

			string comment = reader.IsDBNull(4) ? string.Empty : reader.GetString(4);

			bool tmpApproved = (reader.GetValue(5) == null);
			bool isApproved = false;
			if (tmpApproved)
				isApproved = reader.GetBoolean(5);

			bool tmpLockedOut = (reader.GetValue(6) == null);
			bool isLockedOut = false;
			if (tmpLockedOut)
				isLockedOut = reader.GetBoolean(6);

			DateTime creationDate = reader.IsDBNull(7) ? DateTime.Now : reader.GetDateTime(7);

			DateTime lastLoginDate = reader.IsDBNull(8) ? DateTime.Now : reader.GetDateTime(8);

			DateTime lastActivityDate = reader.IsDBNull(9) ? DateTime.Now : reader.GetDateTime(9);

			DateTime lastPasswordChangedDate = reader.IsDBNull(10) ? DateTime.Now : reader.GetDateTime(10);

			DateTime lastLockedOutDate = reader.IsDBNull(11) ? DateTime.Now : reader.GetDateTime(11);

			MembershipUser u = new MembershipUser(this.Name,
												  username,
												  providerUserKey,
												  email,
												  passwordQuestion,
												  comment,
												  isApproved,
												  isLockedOut,
												  creationDate,
												  lastLoginDate,
												  lastActivityDate,
												  lastPasswordChangedDate,
												  lastLockedOutDate);

			return u;
		}


		//
		// MembershipProvider.UnlockUser
		//

		public override bool UnlockUser(string userName)
		{
			int rowsAffected = 0;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
												  " SET IsLockedOut = False, LastLockedOutDate = @LastLockedOutDate " +
												  " WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{

					cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = userName;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						rowsAffected = cmd.ExecuteNonQuery();
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "UnlockUser");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			if (rowsAffected > 0)
				return true;

			return false;
		}


		//
		// MembershipProvider.GetUserNameByEmail
		//

		public override string GetUserNameByEmail(string email)
		{
			string username = "";

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Username" +
					  " FROM [aspnet_Users] WHERE Email = @Email AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 128).Value = email;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();
						object o = cmd.ExecuteScalar();
						if (o != null) username = Convert.ToString(o, CultureInfo.InvariantCulture);
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "GetUserNameByEmail");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}

			if (username == null)
				username = "";

			return username;
		}




		//
		// MembershipProvider.ResetPassword
		//

		public override string ResetPassword(string username, string answer)
		{
			if (!EnablePasswordReset)
			{
				throw new NotSupportedException("Password reset is not enabled.");
			}

			if (answer == null && RequiresQuestionAndAnswer)
			{
				UpdateFailureCount(username, "passwordAnswer");

				throw new ProviderException("Password answer required for password reset.");
			}

			string newPassword =
			  System.Web.Security.Membership.GeneratePassword(newPasswordLength, MinRequiredNonAlphanumericCharacters);

			ValidatePasswordEventArgs args =
			  new ValidatePasswordEventArgs(username, newPassword, true);

			OnValidatingPassword(args);

			if (args.Cancel)
				if (args.FailureInformation != null)
					throw args.FailureInformation;
				else
					throw new MembershipPasswordException("Reset password canceled due to password validation failure.");

			int rowsAffected = 0;
			string passwordAnswer = "";

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT PasswordAnswer, IsLockedOut FROM [aspnet_Users] " +
					  " WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
						{
							if (reader.Read())
							{
								//object val = reader.GetValue(1);

								if (Convert.ToBoolean(reader.GetValue(1)))
									throw new MembershipPasswordException("The supplied user is locked out.");

								passwordAnswer = reader.GetString(0);
							}
							else
							{
								throw new MembershipPasswordException("The supplied user name is not found.");
							}
							if (RequiresQuestionAndAnswer && !CheckPassword(answer, passwordAnswer))
							{
								UpdateFailureCount(username, "passwordAnswer");

								throw new MembershipPasswordException("Incorrect password answer.");
							}
						}

						using (SqlCeCommand updateCmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
							" SET Password = @Password, LastPasswordChangedDate = @LastPasswordChangedDate" +
							" WHERE Username = @Username AND ApplicationName = @ApplicationName AND IsLockedOut = 0", conn))
						{

							updateCmd.Parameters.Add("@Password", SqlDbType.NVarChar, 255).Value = EncodePassword(newPassword);
							updateCmd.Parameters.Add("@LastPasswordChangedDate", SqlDbType.DateTime).Value = DateTime.Now;
							updateCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
							updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

							rowsAffected = updateCmd.ExecuteNonQuery();
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "ResetPassword");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
				if (rowsAffected > 0)
				{
					return newPassword;
				}
				else
				{
					throw new MembershipPasswordException("User not found, or user is locked out. Password not Reset.");
				}
			}
		}


		//
		// MembershipProvider.UpdateUser
		//

		public override void UpdateUser(MembershipUser user)
		{
			if (user == null)
				throw new ArgumentNullException("user");
			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("UPDATE [aspnet_Users] " +
						" SET Email = @Email, Comment = @Comment," +
						" IsApproved = @IsApproved" +
						" WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@Email", SqlDbType.NVarChar, 128).Value = user.Email;
					cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = user.Comment;
					cmd.Parameters.Add("@IsApproved", SqlDbType.Bit).Value = user.IsApproved;
					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = user.UserName;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();

						cmd.ExecuteNonQuery();
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "UpdateUser");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
		}

		//
		// MembershipProvider.ValidateUser
		//

		public override bool ValidateUser(string username, string password)
		{
			bool isValid = false;

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Password, IsApproved FROM [aspnet_Users] " +
						" WHERE Username = @Username AND ApplicationName = @ApplicationName AND IsLockedOut = 0", conn))
				{

					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					bool isApproved = false;
					string pwd = "";

					try
					{
						conn.Open();

						using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
						{
							if (reader.Read())
							{
								pwd = reader.GetString(0);
								int iApp = Convert.ToInt32(reader.GetValue(1));
								if (iApp == 1) isApproved = true;
							}
							else
							{
								return false;
							}
						}

						if (CheckPassword(password, pwd))
						{
							if (isApproved)
							{
								isValid = true;

								using (SqlCeCommand updateCmd = new SqlCeCommand("UPDATE [aspnet_Users] SET LastLoginDate = @LastLoginDate" +
																		" WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
								{

									updateCmd.Parameters.Add("@LastLoginDate", SqlDbType.DateTime).Value = DateTime.Now;
									updateCmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
									updateCmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

									updateCmd.ExecuteNonQuery();
								}
							}
						}
						else
						{
							UpdateFailureCount(username, "password");
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "ValidateUser");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return isValid;
		}


		//
		// UpdateFailureCount
		//   A helper method that performs the checks and updates associated with
		// password failure tracking.
		//

		private void UpdateFailureCount(string username, string failureType)
		{
			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT FailedPasswordAttemptCount, " +
												   "  FailedPasswordAttemptWindowStart, " +
												   "  FailedPasswordAnswerAttemptCount, " +
												   "  FailedPasswordAnswerAttemptWindowStart " +
												   "  FROM [aspnet_Users] " +
												   "  WHERE Username = @Username AND ApplicationName = @ApplicationName", conn))
				{

					cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					DateTime windowStart = new DateTime();
					int failureCount = 0;

					try
					{
						conn.Open();

						using (var reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
						{

							if (reader.Read())
							{
								if (failureType == "password")
								{
									failureCount = reader.GetInt32(0);
									try
									{
										windowStart = reader.GetDateTime(1);
									}
									catch
									{
										windowStart = DateTime.Now;
									}
								}

								if (failureType == "passwordAnswer")
								{
									failureCount = reader.GetInt32(2);
									windowStart = reader.GetDateTime(3);
								}
							}
						}

						DateTime windowEnd = windowStart.AddMinutes(PasswordAttemptWindow);

						if (failureCount == 0 || DateTime.Now > windowEnd)
						{
							// First password failure or outside of PasswordAttemptWindow. 
							// Start a new password failure count from 1 and a new window starting now.

							if (failureType == "password")
								cmd.CommandText = "UPDATE [aspnet_Users] " +
												  "  SET FailedPasswordAttemptCount = @Count, " +
												  "      FailedPasswordAttemptWindowStart = @WindowStart " +
												  "  WHERE Username = @Username AND ApplicationName = @ApplicationName";

							if (failureType == "passwordAnswer")
								cmd.CommandText = "UPDATE [aspnet_Users] " +
												  "  SET FailedPasswordAnswerAttemptCount = @Count, " +
												  "      FailedPasswordAnswerAttemptWindowStart = @WindowStart " +
												  "  WHERE Username = @Username AND ApplicationName = @ApplicationName";

							cmd.Parameters.Clear();

							cmd.Parameters.Add("@Count", SqlDbType.Int).Value = 1;
							cmd.Parameters.Add("@WindowStart", SqlDbType.DateTime).Value = DateTime.Now;
							cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
							cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

							if (cmd.ExecuteNonQuery() < 0)
								throw new ProviderException("Unable to update failure count and window start.");
						}
						else
						{
							if (failureCount++ >= MaxInvalidPasswordAttempts)
							{
								// Password attempts have exceeded the failure threshold. Lock out
								// the user.

								cmd.CommandText = "UPDATE [aspnet_Users] " +
												  "  SET IsLockedOut = @IsLockedOut, LastLockedOutDate = @LastLockedOutDate " +
												  "  WHERE Username = @Username AND ApplicationName = @ApplicationName";

								cmd.Parameters.Clear();

								cmd.Parameters.Add("@IsLockedOut", SqlDbType.Bit).Value = true;
								cmd.Parameters.Add("@LastLockedOutDate", SqlDbType.DateTime).Value = DateTime.Now;
								cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
								cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

								if (cmd.ExecuteNonQuery() < 0)
									throw new ProviderException("Unable to lock out user.");
							}
							else
							{
								// Password attempts have not exceeded the failure threshold. Update
								// the failure counts. Leave the window the same.

								if (failureType == "password")
									cmd.CommandText = "UPDATE [aspnet_Users] " +
													  "  SET FailedPasswordAttemptCount = @Count" +
													  "  WHERE Username = @Username AND ApplicationName = @ApplicationName";

								if (failureType == "passwordAnswer")
									cmd.CommandText = "UPDATE [aspnet_Users] " +
													  "  SET FailedPasswordAnswerAttemptCount = @Count" +
													  "  WHERE Username = @Username AND ApplicationName = @ApplicationName";

								cmd.Parameters.Clear();

								cmd.Parameters.Add("@Count", SqlDbType.Int).Value = failureCount;
								cmd.Parameters.Add("@Username", SqlDbType.NVarChar, 255).Value = username;
								cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

								if (cmd.ExecuteNonQuery() < 0)
									throw new ProviderException("Unable to update failure count.");
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "UpdateFailureCount");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
		}

		//
		// CheckPassword
		//   Compares password values based on the MembershipPasswordFormat.
		//

		private bool CheckPassword(string password, string dbpassword)
		{
			string pass1 = password;
			string pass2 = dbpassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Encrypted:
					pass2 = UnEncodePassword(dbpassword);
					break;
				case MembershipPasswordFormat.Hashed:
					pass1 = EncodePassword(password);
					break;
				default:
					break;
			}

			if (pass1 == pass2)
			{
				return true;
			}

			return false;
		}


		//
		// EncodePassword
		//   Encrypts, Hashes, or leaves the password clear based on the PasswordFormat.
		//

		private string EncodePassword(string password)
		{
			if (password == null) password = "";
			string encodedPassword = password;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					encodedPassword =
					  Convert.ToBase64String(EncryptPassword(Encoding.Unicode.GetBytes(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					using (HMACSHA1 hash = new HMACSHA1())
					{
						hash.Key = HexToByte(encryptionKey);
						encodedPassword =
						  Convert.ToBase64String(hash.ComputeHash(Encoding.Unicode.GetBytes(password)));
					}
					break;
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return encodedPassword;
		}


		//
		// UnEncodePassword
		//   Decrypts or leaves the password clear based on the PasswordFormat.
		//

		private string UnEncodePassword(string encodedPassword)
		{
			string password = encodedPassword;

			switch (PasswordFormat)
			{
				case MembershipPasswordFormat.Clear:
					break;
				case MembershipPasswordFormat.Encrypted:
					password =
					  Encoding.Unicode.GetString(DecryptPassword(Convert.FromBase64String(password)));
					break;
				case MembershipPasswordFormat.Hashed:
					throw new ProviderException("Cannot decode a hashed password.");
				default:
					throw new ProviderException("Unsupported password format.");
			}

			return password;
		}

		//
		// HexToByte
		//   Converts a hexadecimal string to a byte array. Used to convert encryption
		// key values from the configuration.
		//

		private byte[] HexToByte(string hexString)
		{
			byte[] returnBytes = new byte[hexString.Length / 2];
			for (int i = 0; i < returnBytes.Length; i++)
				returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			return returnBytes;
		}


		//
		// MembershipProvider.FindUsersByName
		//

		public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection users = new MembershipUserCollection();

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Count(*) FROM [aspnet_Users] " +
						  "WHERE Username LIKE @UsernameSearch AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@UsernameSearch", SqlDbType.NVarChar, 255).Value = usernameToMatch;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = pApplicationName;

					try
					{
						conn.Open();
						totalRecords = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);

						if (totalRecords <= 0) { return users; }

						cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
						  " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
						  " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
						  " FROM [aspnet_Users] " +
						  " WHERE Username LIKE @UsernameSearch AND ApplicationName = @ApplicationName " +
						  " ORDER BY Username Asc";

						using (var reader = cmd.ExecuteReader())
						{
							int counter = 0;
							int startIndex = pageSize * pageIndex;
							int endIndex = startIndex + pageSize - 1;

							while (reader.Read())
							{
								if (counter >= startIndex)
								{
									MembershipUser u = GetUserFromReader(reader);
									users.Add(u);
								}

								if (counter >= endIndex) { cmd.Cancel(); }

								counter++;
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "FindUsersByName");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return users;
		}

		//
		// MembershipProvider.FindUsersByEmail
		//

		public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
		{
			MembershipUserCollection users = new MembershipUserCollection();

			using (SqlCeConnection conn = new SqlCeConnection(connectionString))
			{
				using (SqlCeCommand cmd = new SqlCeCommand("SELECT Count(*) FROM [aspnet_Users] " +
												  "WHERE Email LIKE @EmailSearch AND ApplicationName = @ApplicationName", conn))
				{
					cmd.Parameters.Add("@EmailSearch", SqlDbType.NVarChar, 255).Value = emailToMatch;
					cmd.Parameters.Add("@ApplicationName", SqlDbType.NVarChar, 255).Value = ApplicationName;

					totalRecords = 0;

					try
					{
						conn.Open();
						totalRecords = Convert.ToInt32(cmd.ExecuteScalar(), CultureInfo.InvariantCulture);

						if (totalRecords <= 0) { return users; }

						cmd.CommandText = "SELECT PKID, Username, Email, PasswordQuestion," +
								 " Comment, IsApproved, IsLockedOut, CreationDate, LastLoginDate," +
								 " LastActivityDate, LastPasswordChangedDate, LastLockedOutDate " +
								 " FROM [aspnet_Users] " +
								 " WHERE Email LIKE @EmailSearch AND ApplicationName = @ApplicationName " +
								 " ORDER BY Username Asc";

						using (var reader = cmd.ExecuteReader())
						{
							int counter = 0;
							int startIndex = pageSize * pageIndex;
							int endIndex = startIndex + pageSize - 1;

							while (reader.Read())
							{
								if (counter >= startIndex)
								{
									MembershipUser u = GetUserFromReader(reader);
									users.Add(u);
								}

								if (counter >= endIndex) { cmd.Cancel(); }

								counter++;
							}
						}
					}
					catch (SqlCeException e)
					{
						if (WriteExceptionsToEventLog)
						{
							WriteToEventLog(e, "FindUsersByEmail");

							throw new ProviderException(exceptionMessage);
						}
						else
						{
							throw;
						}
					}
				}
			}
			return users;
		}

		//
		// WriteToEventLog
		//   A helper function that writes exception detail to the event log. Exceptions
		// are written to the event log as a security measure to avoid private database
		// details from being returned to the browser. If a method does not return a status
		// or boolean indicating the action succeeded or failed, a generic exception is also 
		// thrown by the caller.
		//

		private void WriteToEventLog(Exception e, string action)
		{
			using (EventLog log = new EventLog())
			{
				log.Source = eventSource;
				log.Log = eventLog;

				string message = "An exception occurred communicating with the data source.\n\n";
				message += "Action: " + action + "\n\n";
				message += "Exception: " + e.ToString();
			}
		}
	}
}