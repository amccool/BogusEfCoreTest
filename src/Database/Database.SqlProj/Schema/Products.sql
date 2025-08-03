CREATE TABLE [dbo].[Products] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [Name] NVARCHAR(200) NOT NULL,
    [Description] NVARCHAR(1000) NULL,
    [Price] DECIMAL(18,2) NOT NULL,
    [Category] NVARCHAR(100) NOT NULL,
    [SKU] NVARCHAR(50) NOT NULL,
    [StockQuantity] INT NOT NULL DEFAULT 0,
    [IsActive] BIT NOT NULL DEFAULT 1,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([Id] ASC)
);
GO;
-- Create unique index on SKU
CREATE UNIQUE NONCLUSTERED INDEX [IX_Products_SKU] ON [dbo].[Products] ([SKU] ASC);
GO;
-- Create index on category for filtering
CREATE NONCLUSTERED INDEX [IX_Products_Category] ON [dbo].[Products] ([Category] ASC); 
GO;