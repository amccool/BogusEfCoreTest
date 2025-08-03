CREATE TABLE [dbo].[Customers] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [Email] NVARCHAR(255) NOT NULL,
    [Phone] NVARCHAR(21) NULL,
    [Address] NVARCHAR(500) NULL,
    [City] NVARCHAR(100) NULL,
    [State] NVARCHAR(50) NULL,
    [ZipCode] NVARCHAR(20) NULL,
    [Country] NVARCHAR(100) NULL,
    [DateOfBirth] DATE NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO; 
-- Create unique index on email
CREATE UNIQUE NONCLUSTERED INDEX [IX_Customers_Email] ON [dbo].[Customers] ([Email] ASC); 
GO;