﻿using Microsoft.Xrm.Sdk;

namespace Dataverse_api.Util;

/// <summary>
/// This class describes an App Command, which is a command that the user can input to interact with the app
/// It is composed by an action (create, delete, exit...), an optional entity (Account, Contact, Case) and ID
/// </summary>
public class Command
{
    public required string Action { get; init; }
    public Type? EntityType { get; init; } // Must extend Entity
    public Guid? Id { get; init; }
}