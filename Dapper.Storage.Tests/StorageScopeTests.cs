using Dapper.Storage.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Dapper.Storage.Tests
{
	[TestClass]
	public class StorageScopeTests
	{
		public IStorageResource Resource { get; private set; }

		public StorageScope NewScope =>
			new StorageScope(Resource);

		[TestInitialize]
		public void Setup()
		{
			Resource = Mock.Of<IStorageResource>();
		}

		[TestMethod]
		public void SingleStorageScope_SameTransactionLevel_ShouldControlLevel()
		{
			var scope = NewScope;

			scope.Dispose();

			Assert.AreEqual(Resource.TransactionLevel, 0);
		}

		[TestMethod]
		public void MultipleStorageScope_SameTransactionLevel_ShouldControlLevel()
		{
			var first = NewScope;
			var second = NewScope;
			var third = NewScope;

			first.Dispose();
			var twoLevels = Resource.TransactionLevel; 
			second.Dispose();
			var oneLevel = Resource.TransactionLevel;
			third.Dispose();
			var noLevels = Resource.TransactionLevel;

			Assert.AreEqual(noLevels, 0);
			Assert.AreEqual(oneLevel, 1);
			Assert.AreEqual(twoLevels, 2);
		}
	}
}
