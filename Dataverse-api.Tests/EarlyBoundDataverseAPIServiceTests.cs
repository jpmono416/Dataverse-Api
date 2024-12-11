using System.ServiceModel;
using Dataverse_api.Entities;
using Dataverse_api.Service;
using Dataverse_api.Util;
using Microsoft.Xrm.Sdk;
using Xunit;

namespace Dataverse_api.Tests;

[Collection("Service unit tests")]
public class EarlyBoundDataverseAPIServiceTests
{
    private List<Guid> _createdAccounts = [];
    private List<Guid> _createdContacts = [];
    private List<Guid> _createdCases = [];
    
    // Constructor
    public EarlyBoundDataverseAPIServiceTests()
    {
        // Setup
        Utils.LoadEnvVariables();
    }
    
    // Destructor
    ~EarlyBoundDataverseAPIServiceTests()
    {
        // Teardown
        foreach (var accountId in _createdAccounts) EarlyBoundDataverseApiService.DeleteEntity<Account>(accountId);
        foreach (var contactId in _createdContacts) EarlyBoundDataverseApiService.DeleteEntity<Contact>(contactId);
        foreach (var caseId in _createdCases) EarlyBoundDataverseApiService.DeleteEntity<Incident>(caseId);
    }
    
    [Fact]
    public void CreateEntityShouldReturnEntityGuid()
    {
        // Arrange
        Account account = new()
        {
            Name = "Test Corp",
            EMailAddress1 = "testcontact@acme.com",
            Telephone1 = "123-456-7890"
        };
        
        // Act
        Guid id = EarlyBoundDataverseApiService.CreateEntity(account);
        
        // Assert
        Assert.NotEqual(Guid.Empty, id);
        
        _createdAccounts.Add(id);
    }
    
    [Fact]
    public void CreateEntityWithNullEntityShouldThrowException()
    {
        // Act & Assert
        Assert.Throws<FaultException<OrganizationServiceFault>>(() => EarlyBoundDataverseApiService.CreateEntity<Account>(null));
    }

    [Fact]
    public void CreateEntityWithMinimalFieldsShouldReturnEntityGuid()
    {
        // Arrange
        Account account = new()
        {
            Name = "Minimal Corp"
        };

        // Act
        Guid id = EarlyBoundDataverseApiService.CreateEntity(account);

        // Assert
        Assert.NotEqual(Guid.Empty, id);

        _createdAccounts.Add(id);
    }

    [Fact]
    public void CreateEntityWithSpecialCharactersInNameShouldReturnEntityGuid()
    {
        // Arrange
        Account account = new()
        {
            Name = "Test Corp!@#$%^&*()",
            EMailAddress1 = "testcontact@acme.com",
            Telephone1 = "123-456-7890"
        };

        // Act
        Guid id = EarlyBoundDataverseApiService.CreateEntity(account);

        // Assert
        Assert.NotEqual(Guid.Empty, id);

        _createdAccounts.Add(id);
    }
}