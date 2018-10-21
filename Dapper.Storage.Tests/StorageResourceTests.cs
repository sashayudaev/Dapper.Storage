using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Dapper.Storage.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dapper.Storage.Tests
{
	[TestClass]
	public class StorageResourceTests
	{
		public ILifetimeScope LifetimeScope { get; private set; }

		[TestInitialize]
		public void Setup()
		{
			var mock = new Mock<ILifetimeScope>();
			mock.Setup(lf => lf.BeginLifetimeScope())
				.Returns(mock.Object);

			LifetimeScope = mock.Object;
		}

		[TestMethod]
		public void Query_WithoutTransaction_ShouldNotHaveTransaction()
		{
			var resource = new StorageResource(LifetimeScope);

			Assert.IsFalse(resource.HasTransaction);
		}

		[TestMethod]
		public void Query_InTransaction_ShouldHaveTransaction()
		{
			var resource = new StorageResource(LifetimeScope);

			bool hasTransaction;
			using (var transaction = resource.Begin())
			{
				hasTransaction = resource.HasTransaction;
			}

			Assert.IsTrue(hasTransaction);
		}

		[TestMethod]
		public void MultipleTransaction_EndTransaction_NoTransactionLevel()
		{
			var resource = new StorageResource(LifetimeScope);

			var first = resource.Begin();
			var second = resource.Begin();
			var third = resource.Begin();

			var threeLevels = resource.TransactionLevel;
			third.Dispose();
			var twoLevels = resource.TransactionLevel;
			second.Dispose();
			var oneLevel = resource.TransactionLevel;
			first.Dispose();
			var noLevels = resource.TransactionLevel;

			Assert.AreEqual(threeLevels, 3);
			Assert.AreEqual(twoLevels, 2);
			Assert.AreEqual(oneLevel, 1);
			Assert.AreEqual(noLevels, 0);
		}
	}
}
