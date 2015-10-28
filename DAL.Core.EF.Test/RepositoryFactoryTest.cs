using System;
using System.Data.Entity;
using DAL.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAL.Core.EF.Test
{
    [TestClass]
    public class RepositoryFactoryTest
    {
        [TestMethod]
        public void Constructor_Context_Cannot_Be_Null()
        {
            //
            // Arrange, Act
            //
            var mockContext = new Mock<EFDbContext>();
            var repositoryFactory = new RepositoryFactory(mockContext.Object);
            //
            // Assert
            //
            Assert.IsNotNull(repositoryFactory.Context);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void If_Passed_Context_Is_Null_Must_Throw_Exception()
        {
            //
            // Arrange, Act, Assert
            //
            var repositoryFactory = new RepositoryFactory(null);
        }

        [TestMethod]
        public void Must_Return_Repository_When_Registered()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            var mockRepository = new Mock<IRepository<IModel>>();

            var repositoryFactory = new RepositoryFactory(mockContext.Object);
            repositoryFactory.SetCustomRepo(mockRepository.Object);

            //
            // Act
            //
            var result = repositoryFactory.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNotNull(result);

        }

        [TestMethod]
        public void Must_Return_Repository_For_Valid_Entity_Regardless_It_Was_Registered()
        {
            //
            // Arrange
            //
            var mockDbSet = new Mock<DbSet<IModel>>();
            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repositoryFacotory = new RepositoryFactory(mockContext.Object);
            //
            // Act
            //
            var result = repositoryFacotory.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNotNull(result);

        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Must_Return_NULL_For_Invalid_Entity_If_It_Is_Not_In_Context()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            var repositoryFacotory = new RepositoryFactory(mockContext.Object);
            //
            // Act
            //
            var result = repositoryFacotory.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNotNull(result);

        }

    }
}
