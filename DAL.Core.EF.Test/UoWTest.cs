using System;
using System.Linq;
using System.Linq.Expressions;
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

        [TestMethod]
        public void Get_For_A_Valid_Repository_With_Filter_Must_Return_Filtered_Results()
        {
            //
            // Arrange
            //
            var mockedList = Enumerable.Range(1, 10).Select(x =>
            {
                var mockModel = new Mock<IModel>();
                mockModel.Setup(y => y.Id).Returns(x);

                return mockModel.Object;
            }).AsQueryable();

            var mockRepository = new Mock<IRepository<IModel>>();
            mockRepository.Setup(x => x.GetAll()).Returns(mockedList);

            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x => x.GetRepository<IModel>()).Returns(mockRepository.Object);



            var mockedUoW = new UoW(mockContext.Object, mockRepoFactory.Object);
            Expression<Func<IModel, bool>> filterExpression = x => x.Id == 1;

            //
            // Act
            //
            var results = mockedUoW.Get(filterExpression);
            //
            // Assert
            //
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.First());
            Assert.AreEqual(results.First().Id, 1);


        }

        [TestMethod]
        public void Get_For_Invalid_Repository_Must_Return_NULL()
        {
            //
            // Arrange
            //
            var mockedList = Enumerable.Range(1, 10).Select(x =>
            {
                var mockModel = new Mock<IModel>();
                mockModel.Setup(y => y.Id).Returns(x);

                return mockModel.Object;
            }).AsQueryable();

            var mockRepository = new Mock<IRepository<IModel>>();
            mockRepository.Setup(x => x.GetAll()).Returns(mockedList);

            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x => x.GetRepository<IModel>()).Returns<IModel>(null);

            var mockedUoW = new UoW(mockContext.Object, mockRepoFactory.Object);
            Expression<Func<IModel, bool>> filterExpression = x => x.Id == 1;

            //
            // Act
            //
            var results = mockedUoW.Get(filterExpression);
            //
            // Assert
            //
            Assert.IsNull(results);
        }

        [TestMethod]
        public void Get_For_A_NULL_Filter_Must_Return_All()
        {
            //
            // Arrange
            //
            var mockedList = Enumerable.Range(1, 10).Select(x =>
            {
                var mockModel = new Mock<IModel>();
                mockModel.Setup(y => y.Id).Returns(x);

                return mockModel.Object;
            }).AsQueryable();

            var mockRepository = new Mock<IRepository<IModel>>();
            mockRepository.Setup(x => x.GetAll()).Returns(mockedList);

            var mockContext = new Mock<EFDbContext>();
            var mockRepoFactory = new Mock<IRepositoryFactory>();
            mockRepoFactory.Setup(x => x.GetRepository<IModel>()).Returns(mockRepository.Object);



            var mockedUoW = new UoW(mockContext.Object, mockRepoFactory.Object);
            Expression<Func<IModel, bool>> filterExpression = null;

            //
            // Act
            //
            var results = mockedUoW.Get(filterExpression);
            //
            // Assert
            //
            Assert.IsNotNull(results);
            Assert.AreEqual(results.Count(), 10);
            Assert.AreEqual(results.First().Id, 1);


        }
        
    }
}
