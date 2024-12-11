﻿using System.Reflection;
using Dataverse_api.Entities;
using Dataverse_api.Util;
using Dataverse_api.View;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace Dataverse_api.Service;

/// <summary>
/// Class to interact with the Dataverse API for managing Accounts, Contacts, and Cases.
/// It uses the Early Bound classes generated by the CLI Pac tools.
/// It is the main class for interacting with the Dataverse API.
/// It makes use of templating to allow for the flexible creation, retrieval, updating, and deletion of entities.
/// </summary>
public static class EarlyBoundDataverseApiService
{
    // This can be changed to use an OAuth connection string from the Constants class
    private static IOrganizationService _service;

    static EarlyBoundDataverseApiService()
    {
        // Default behavior
        _service = Utils.GetOrganizationService(Constants.ClientSecretConnectionString);
    }
    
    public static void Initialize(IOrganizationService service)
    {
        // Dependency injection (for tests or alternative configurations)
        _service = service;
    }
    
    /// <summary>
    /// Creates a new entity of the specified type in Dataverse.
    /// </summary>
    /// <typeparam name="T">The type of the entity to create. Must inherit from <see cref="Entity"/>.</typeparam>
    /// <param name="entity">The entity to create.</param>
    /// <returns>The unique identifier (ID) of the created entity.</returns>
    public static Guid CreateEntity<T>(T entity) where T : Entity => _service.Create(entity);

    /// <summary>
    /// Retrieves an entity by its ID from Dataverse.
    /// </summary>
    /// <typeparam name="T">The type of the entity to retrieve. Must inherit from <see cref="Entity"/>.</typeparam>
    /// <param name="entityId">The unique identifier (ID) of the entity to retrieve.</param>
    /// <returns>The retrieved entity, cast to the specified type.</returns>
    public static T GetEntityById<T>(Guid entityId) where T : Entity => 
        _service.Retrieve(typeof(T).Name.ToLower(), entityId, new ColumnSet(true)).ToEntity<T>();

    public static List<T> GetAllEntities<T>() where T : Entity =>
        _service.RetrieveMultiple(new QueryExpression(typeof(T).Name.ToLower()){ ColumnSet = new ColumnSet(true) })
            .Entities.Select(e => e.ToEntity<T>()).ToList();

    /// <summary>
    /// Updates an existing entity in Dataverse using a custom update action.
    /// </summary>
    /// <typeparam name="T">The type of the entity to update. Must inherit from <see cref="Entity"/>.</typeparam>
    /// <param name="entity">The entity to update.</param>
    /// <param name="updateAction">The action to perform updates on the entity.</param>
    public static void UpdateEntity<T>(T entity, Action<T> updateAction) where T : Entity
    {
        updateAction(entity);
        _service.Update(entity);
    }

    /// <summary>
    /// Deletes an entity from Dataverse by its ID.
    /// </summary>
    /// <typeparam name="T">The type of the entity to delete. Must inherit from <see cref="Entity"/>.</typeparam>
    /// <param name="entityId">The unique identifier (ID) of the entity to delete.</param>
    public static void DeleteEntity<T>(Guid entityId) where T : Entity => _service.Delete(typeof(T).Name.ToLower(), entityId);

}