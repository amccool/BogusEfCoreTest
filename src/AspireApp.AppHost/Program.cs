using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = DistributedApplication.CreateBuilder(args);

// Add SQL Server resource
var sqlServer = builder.AddSqlServer("sqlserver")
    .WithLifetime(ContainerLifetime.Persistent);

var storeDb = sqlServer.AddDatabase("store");
                 
// Add SSDT Project deployment using Community Toolkit
var sqlProject = builder.AddSqlProject<Projects.Database_SqlProj>("sqlproject")
   .WithReference(storeDb)
   .WaitFor(storeDb);
    
    // .WithConfigureDacDeployOptions(options =>
    // {
    //     options.CreateNewDatabase = true;
    //     options.BlockOnPossibleDataLoss = false;
    //     options.DropObjectsNotInSource = true;
    //     options.IgnorePermissions = true;
    //     options.IgnoreRoleMembership = true;
    //     options.IgnoreLoginSids = true;
    //     options.IgnoreExtendedProperties = true;
    //     options.IgnoreDdlTriggerOrder = true;
    //     options.IgnoreDdlTriggerState = true;
    //     options.IgnoreDefaultSchema = true;
    //     options.IgnoreSemicolonBetweenStatements = true;
    //     options.IgnoreWhitespace = true;
    //     options.IgnoreKeywordCasing = true;
    //     options.IgnoreAnsiNulls = true;
    //     options.IgnoreFileSize = true;
    //     options.IgnoreFillFactor = true;
    //     options.IgnoreIndexPadding = true;
    //     options.IgnoreTableOptions = true;
    //     options.IgnoreLockHintsOnIndexes = true;
    //     options.IgnoreUserSettingsObjects = true;
    //     options.IgnoreIndexOptions = true;
    //     options.IgnoreTablePartitionOptions = true;
    //     options.IgnoreColumnOrder = true;
    //     options.IgnoreComments = true;
    //     options.AllowIncompatiblePlatform = true;
    //     options.BackupDatabaseBeforeChanges = false;
    //     options.BlockWhenDriftDetected = false;
    //     options.CommentOutSetVarDeclarations = true;
    //     options.CompareUsingTargetCollation = false;
    //     options.DeployDatabaseInSingleUserMode = false;
    //     options.DisableAndReenableDdlTriggers = true;
    //     options.DoNotAlterChangeDataCaptureObjects = true;
    //     options.DoNotAlterReplicatedObjects = true;
    //     options.DropConstraintsNotInSource = true;
    //     options.DropDmlTriggersNotInSource = true;
    //     options.DropExtendedPropertiesNotInSource = true;
    //     options.DropIndexesNotInSource = true;
    //     options.DropPermissionsNotInSource = true;
    //     options.DropRoleMembersNotInSource = true;
    //     options.DropStatisticsNotInSource = true;
    //     options.GenerateSmartDefaults = true;
    //     options.IgnoreAuthorizer = true;
    //     options.IgnoreColumnCollation = true;
    //     options.IgnoreCryptographicProviderFilePath = true;
    //     options.IgnoreDmlTriggerOrder = true;
    //     options.IgnoreDmlTriggerState = true;
    //     options.IgnoreFullTextCatalogFilePath = true;
    //     options.IgnoreIdentitySeed = true;
    //     options.IgnoreNotForReplication = true;
    //     options.IgnoreRouteLifetime = true;
    //     options.IgnoreWithNocheckOnCheckConstraints = true;
    //     options.IgnoreWithNocheckOnForeignKeys = true;
    //     options.IncludeCompositeObjects = true;
    //     options.IncludeTransactionalScripts = false;
    //     options.NoAlterStatementsToChangeClrTypes = true;
    //     options.PopulateFilesOnFileGroups = true;
    //     options.RegisterDataTierApplication = true;
    //     options.RestoreSequenceCurrentValue = false;
    //     options.ScriptDatabaseCollation = false;
    //     options.ScriptDatabaseCompatibility = false;
    //     options.ScriptDatabaseOptions = true;
    //     options.ScriptDeployStateChecks = false;
    //     options.ScriptFileSize = false;
    //     options.ScriptNewConstraintValidation = true;
    //     options.ScriptRefreshModule = true;
    //     options.TreatVerificationErrorsAsWarnings = false;
    //     options.UnmodifiableObjectWarnings = true;
    //     options.VerifyCollationCompatibility = true;
    //     options.VerifyDeployment = true;
    // });

// Add Database Seeding Service (waits for SQL project deployment)
var seedData = builder.AddProject<Projects.SeedData_Generator>("databaseseeder")
    .WithReference(storeDb)
    .WaitForCompletion(sqlProject);

// Add API Service with database connection
var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithReference(storeDb)
    .WaitForCompletion(seedData);

// Add Web Frontend
builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
