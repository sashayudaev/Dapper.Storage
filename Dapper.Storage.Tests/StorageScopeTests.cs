using Dapper.Storage.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dapper.Storage.Tests
{
	[TestClass]
	public class StorageScopeTests
	{
		[TestMethod]
		public void SingleStorageScope_SameTransactionLevel_ShouldControlLevel()
		{
			var resource = Mock.Of<IStorageResource>();
			var scope = new StorageScope(resource);

			scope.Dispose();

			Assert.AreEqual(resource.TransactionLevel, 0);
		}

		[TestMethod]
		public void MultipleStorageScope_SameTransactionLevel_ShouldControlLevel()
		{
			var resource = Mock.Of<IStorageResource>();
			var first = new StorageScope(resource);
			var second = new StorageScope(resource);
			var third = new StorageScope(resource);


			first.Dispose();
			var twoLevels = resource.TransactionLevel; 
			second.Dispose();
			var oneLevel = resource.TransactionLevel;
			third.Dispose();
			var noLevels = resource.TransactionLevel;

			Assert.AreEqual(noLevels, 0);
			Assert.AreEqual(oneLevel, 1);
			Assert.AreEqual(twoLevels, 2);
		}
	}
}
