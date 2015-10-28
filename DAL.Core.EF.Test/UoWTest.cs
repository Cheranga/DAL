using System;
using DAL.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAL.Core.EF.Test
{
    [TestClass]
    public class UoWTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Must_Fail_When_Valid_Context_Invalid_Factory()
        {
            //
            // Arrange, Act
            //
            var mockContext = new Mock<EFDbContext>();
            var uow = new UoW(mockContext.Object, null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_Must_Fail_When_Valid_RepositoryFactory_Invalid_Context()
        {
            //
            // Arrange, Act
            //
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            var uow = new UoW(null, mockRepoFactory.Object);
        }

        [TestMethod]
        public void Constructor_Must_Accept_Valid_Context_And_Factory()
        {
            //
            // Arrange, Act
            //
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Assert
            //
            Assert.IsNotNull(uow.Context);
            Assert.IsNotNull(uow.repositoryFactory);
        }

        [TestMethod]
        public void For_A_Valid_RegisteredType_Must_Return_Repository()
        {
            //
            // Arrange
            //
            var mockRepository = new Mock<IRepository<IModel>>();
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x=>x.GetRepository<IModel>()).Returns(mockRepository.Object);
            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Act
            //
            var result = uow.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void For_An_Invalid_Type_Must_Return_NULL_Repository()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x => x.GetRepository<IModel>()).Returns((IRepository<IModel>) null);
            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Act
            //
            var result = uow.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNull(result);
        }

        [TestMethod]
        public void For_A_Valid_Type_Must_Return_Repository_With_UoW()
        {
            //
            // Arrange
            //
            var mockRepository = new Mock<IRepository<IModel>>();
            
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x => x.GetRepository<IModel>()).Returns(mockRepository.Object);
            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Act
            //
            var result = uow.GetRepository<IModel>();
            //
            // Assert
            //
            Assert.IsNotNull(result);
            mockRepository.VerifySet(x=>x.UoW = It.IsAny<IUoW>(), Times.Once());
        }

        [TestMethod]
        public void When_Passed_An_Action_To_Commit_That_Action_Must_Execute()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            var mockAction = new Mock<Action>();

            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Act
            //
            uow.Commit(mockAction.Object);
            //
            // Assert
            //
            mockAction.Verify(x => x(), Times.Once);
        }

        [TestMethod]
        public void When_Exception_Occured_Must_Return_DataResult_With_Exception()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            Action action = () =>
            {
                throw new Exception();
            };

            var uow = new UoW(mockContext.Object, mockRepoFactory.Object);
            //
            // Act
            //
            var result = uow.Commit(action);
            //
            // Assert
            //
            mockContext.Verify(x=>x.SaveChanges(), Times.Never);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Exception);
        }
        
    }
}
