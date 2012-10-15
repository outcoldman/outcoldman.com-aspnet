using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PersonalWeb.Model;
using PersonalWeb.Model.Repositories;

namespace PersonalWeb.Tests.Model
{
	[TestFixture]
	public class CommentRepositorySpec
	{
		// ReSharper disable InconsistentNaming
		[Test]
		public void Work_with_comment()
		{
			using(BaseRepository repository = new BaseRepository())
			{
				Comment comment = new Comment {Date = DateTime.Now, PostID = 150, Body = "Comment", UserIP = "127.0.0.1"};
				repository.DataContext.Comments.AddObject(comment);
				repository.DataContext.SaveChanges();
				comment.Body = "Comment 2";
				repository.DataContext.SaveChanges();
				repository.DataContext.Comments.DeleteObject(comment);
			}
		}
		// ReSharper restore InconsistentNaming
	}
}
