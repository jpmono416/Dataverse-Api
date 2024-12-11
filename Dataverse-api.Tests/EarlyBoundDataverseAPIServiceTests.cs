using System;
using System.Collections.Generic;
using Dataverse_api.Entities;
using Dataverse_api.Service;
using Dataverse_api.Util;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using Moq;
using Xunit;

namespace Dataverse_api.Tests
{
    public class EarlyBoundDataverseApiServiceTests
    {
        private readonly Mock<IOrganizationService> _mockService;
        private static void updateAccountAction(Account account) => account.Telephone1 = "098-765-4321";
        private readonly List<Guid> _createdAccounts = [];

        public EarlyBoundDataverseApiServiceTests()
        {
            // Setup
            Utils.LoadEnvVariables();
            
            _mockService = new Mock<IOrganizationService>();
            EarlyBoundDataverseApiService.Initialize(_mockService.Object);
        }
    
        // Destructor
        ~EarlyBoundDataverseApiServiceTests()
        {
            // Teardown
            foreach (var accountId in _createdAccounts) EarlyBoundDataverseApiService.DeleteEntity<Account>(accountId);
        }

        [Fact]
        public void CreateEntity_ShouldUseMockService()
        {
            // Arrange
            var expectedId = Guid.NewGuid();

            _mockService.Setup(s => s.Create(It.IsAny<Entity>())).Returns(expectedId);

            var account = new Account { Name = "Test Account" };

            // Act
            var result = EarlyBoundDataverseApiService.CreateEntity(account);
            _createdAccounts.Add(result);

            // Assert
            Assert.Equal(expectedId, result);
            _mockService.Verify(s => s.Create(It.IsAny<Entity>()), Times.Once);
        }
        
        [Fact]
        public void CreateEntity_ShouldReturnNewGuid()
        {
            // Arrange
            var account = new Account { Name = "Test Account" };
            var expectedId = Guid.NewGuid();

            _mockService.Setup(s => s.Create(It.IsAny<Entity>())).Returns(expectedId);

            // Act
            var result = EarlyBoundDataverseApiService.CreateEntity(account);
            _createdAccounts.Add(result);
            
            // Assert
            Assert.Equal(expectedId, result);
            _mockService.Verify(s => s.Create(It.IsAny<Entity>()), Times.Once);
        }

        [Fact]
        public void GetEntityById_ShouldReturnCorrectEntity()
        {
            // Arrange
            var accountId = Guid.NewGuid();
            var expectedAccount = new Account { Id = accountId, Name = "Test Account" };

            _mockService.Setup(s => s.Retrieve("account", accountId, It.IsAny<ColumnSet>()))
                .Returns(expectedAccount);

            // Act
            var result = EarlyBoundDataverseApiService.GetEntityById<Account>(accountId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedAccount.Id, result.Id);
            Assert.Equal(expectedAccount.Name, result.Name);
            _mockService.Verify(s => s.Retrieve("account", accountId, It.IsAny<ColumnSet>()), Times.Once);
        }

        [Fact]
        public void GetAllEntities_ShouldReturnEntityList()
        {
            // Arrange
            var accounts = new List<Entity>
            {
                new Account { Id = Guid.NewGuid(), Name = "Account 1" },
                new Account { Id = Guid.NewGuid(), Name = "Account 2" }
            };

            var entityCollection = new EntityCollection(accounts);

            _mockService.Setup(s => s.RetrieveMultiple(It.IsAny<QueryExpression>())).Returns(entityCollection);

            // Act
            var result = EarlyBoundDataverseApiService.GetAllEntities<Account>();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Collection(result,
                account => Assert.Equal("Account 1", account.Name),
                account => Assert.Equal("Account 2", account.Name));
            _mockService.Verify(s => s.RetrieveMultiple(It.IsAny<QueryExpression>()), Times.Once);
        }

        [Fact]
        public void UpdateEntity_ShouldInvokeServiceUpdate()
        {
            // Arrange
            var account = new Account { Id = Guid.NewGuid(), Name = "Updated Account" };

            // Act
            
            EarlyBoundDataverseApiService.UpdateEntity(account, updateAccountAction);

            // Assert
            _mockService.Verify(s => s.Update(It.IsAny<Entity>()), Times.Once);
        }

        [Fact]
        public void DeleteEntity_ShouldInvokeServiceDelete()
        {
            // Arrange
            var accountId = Guid.NewGuid();

            // Act
            EarlyBoundDataverseApiService.DeleteEntity<Account>(accountId);

            // Assert
            _mockService.Verify(s => s.Delete("account", accountId), Times.Once);
        }
    }
}
