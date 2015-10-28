using System;
using System.Data.Entity;
using System.Linq;
using DAL.Core.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace DAL.Core.EF.Test
{
    [TestClass]
    public class GenericRepositoryTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_If_Passed_Context_Is_Null_Must_Throw_Exception()
        {
            //
            // Arrange, Act, Assert
            //
            var repository = new GenericRepository<IModel>(null);
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void Constructor_InValid_DbSet_Must_Throw_Exception()
        {
            //
            // Arrange
            //
            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns<IModel>(null);
            //
            // Act, Assert
            //
            var repository = new GenericRepository<IModel>(mockContext.Object);
        }

        [TestMethod]
        public void GetAll_Must_Return_All()
        {
            //
            // Arrange
            //

            var models = Enumerable.Range(1, 10).Select(x =>
            {
                var mockModel = new Mock<IModel>();
                mockModel.Setup(y => y.Id).Returns(x);

                return mockModel.Object;

            }).AsQueryable();

            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.As<IQueryable<IModel>>().Setup(m => m.Provider).Returns(models.Provider);
            mockDbSet.As<IQueryable<IModel>>().Setup(m => m.Expression).Returns(models.Expression);
            mockDbSet.As<IQueryable<IModel>>().Setup(m => m.ElementType).Returns(models.ElementType);
            mockDbSet.As<IQueryable<IModel>>().Setup(m => m.GetEnumerator()).Returns(models.GetEnumerator());

            var mockContext = new Mock<EFDbContext>();

            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            var results = repository.GetAll();
            //
            // Assert
            //
            Assert.IsNotNull(results);
            Assert.IsNotNull(results.First());
            Assert.AreEqual(10, results.Count());
            Assert.AreEqual(1, results.First().Id);
        }

        [TestMethod]
        public void Add_Valid_Object_Must_Add()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            mockModel.Setup(x => x.Id).Returns(1);

            var mockDbSet = new Mock<DbSet<IModel>>();
            var mockContext = new Mock<EFDbContext>();

            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Add(mockModel.Object);
            //
            // Assert
            //
            mockDbSet.Verify(x => x.Add(It.IsAny<IModel>()), Times.Once);
        }

        [TestMethod]
        public void Add_NULL_Object_Must_Not_Add()
        {
            //
            // Arrange
            //
            var mockDbSet = new Mock<DbSet<IModel>>();
            var mockContext = new Mock<EFDbContext>();

            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Add(null);
            //
            // Assert
            //
            mockDbSet.Verify(x => x.Add(It.IsAny<IModel>()), Times.Never);
        }

        [TestMethod]
        public void Delete_Valid_Object_Must_Delete()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return mockModel.Object;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>())).Callback<object, RecordState>((obj, state) =>
            {
                return;
            });

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(mockModel.Object);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Once);
        }

        [TestMethod]
        public void Delete_NULL_Object_Must_NOT_Delete()
        {
            //
            // Arrange
            //
            var mockDbSet = new Mock<DbSet<IModel>>();

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(null);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Never);
        }

        [TestMethod]
        public void Delete_UnExisting_Object_Must_NOT_Delete()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return null;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(mockModel.Object);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Never);
        }

        [TestMethod]
        public void Delete_UnExisting_Id_Must_Not_Delete()
        {
            //
            // Arrange
            //
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return null;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(999);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Never);
        }

        [TestMethod]
        public void Delete_Existing_Id_MUST_Delete()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return mockModel.Object;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>())).Callback<object, RecordState>((obj, state) =>
            {
                return;
            });

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(1);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Once);
        }

        [TestMethod]
        public void Update_Valid_Object_Must_Update()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return mockModel.Object;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);
            mockContext.Setup(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>())).Callback<object, RecordState>((obj, state) =>
            {
                return;
            });

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Update(mockModel.Object);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Once);
        }

        [TestMethod] 
        public void Update_NULL_Object_Must_NOT_Update()
        {
            //
            // Arrange
            //
            var mockDbSet = new Mock<DbSet<IModel>>();

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Update(null);
            //
            // Assert
            //
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Never);
        }

        [TestMethod]
        public void Update_UnExisting_Object_Must_NOT_Update()
        {
            //
            // Arrange
            //
            var mockModel = new Mock<IModel>();
            var mockDbSet = new Mock<DbSet<IModel>>();
            mockDbSet.Setup(x => x.Find(It.IsAny<object[]>())).Returns<object[]>(x =>
            {
                return null;
            });

            var mockContext = new Mock<EFDbContext>();
            mockContext.Setup(x => x.Set<IModel>()).Returns(mockDbSet.Object);

            var repository = new GenericRepository<IModel>(mockContext.Object);
            //
            // Act
            //
            repository.Delete(mockModel.Object);
            //
            // Assert
            //
            mockDbSet.Verify(x => x.Find(It.IsAny<object[]>()), Times.Once);
            mockContext.Verify(x => x.ChangeState(It.IsAny<object>(), It.IsAny<RecordState>()), Times.Never);
        }

    }
}
