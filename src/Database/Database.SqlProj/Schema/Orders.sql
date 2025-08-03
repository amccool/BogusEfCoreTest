CREATE TABLE [dbo].[Orders] (
    [Id] INT IDENTITY(1,1) NOT NULL,
    [CustomerId] INT NOT NULL,
    [OrderNumber] NVARCHAR(50) NOT NULL,
    [OrderDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Pending',
    [TotalAmount] DECIMAL(18,2) NOT NULL,
    [ShippingAddress] NVARCHAR(500) NULL,
    [BillingAddress] NVARCHAR(500) NULL,
    [Notes] NVARCHAR(1000) NULL,
    [CreatedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [ModifiedDate] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Orders_Customers] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customers] ([Id])
);
GO;
-- Create unique index on OrderNumber
CREATE UNIQUE NONCLUSTERED INDEX [IX_Orders_OrderNumber] ON [dbo].[Orders] ([OrderNumber] ASC);
GO;
-- Create index on CustomerId for foreign key lookups
CREATE NONCLUSTERED INDEX [IX_Orders_CustomerId] ON [dbo].[Orders] ([CustomerId] ASC);
GO;
-- Create index on OrderDate for date range queries
CREATE NONCLUSTERED INDEX [IX_Orders_OrderDate] ON [dbo].[Orders] ([OrderDate] ASC); 