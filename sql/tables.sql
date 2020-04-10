
/****** Object:  Table [dbo].[BondoraCart]    Script Date: 04/09/2020 19:49:53 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BondoraCart](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[InventoryId] [int] NULL,
	[Days] [int] NULL,
	[Status] [int] NULL,
	[Token] [nvarchar](300) NULL,
 CONSTRAINT [PK_Bondora_Cart] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BondoraCustomer](
	[CustomerId] [int] IDENTITY(1,1) NOT NULL,
	[Username] [nvarchar](50) NULL,
	[LoyaltyPoints] [int] NULL,
 CONSTRAINT [PK_Bondora_Customers] PRIMARY KEY CLUSTERED 
(
	[CustomerId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BondoraInventory](
	[InventoryId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[TypeId] [int] NULL,
 CONSTRAINT [PK_Bondora_Inventory] PRIMARY KEY CLUSTERED 
(
	[InventoryId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BondoraInventoryTypes](
	[TypeId] [int] IDENTITY(1,1) NOT NULL,
	[TypeName] [nvarchar](50) NULL,
 CONSTRAINT [PK_Bondora_Inventory_Types] PRIMARY KEY CLUSTERED 
(
	[TypeId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[BondoraOrder](
	[OrderId] [int] IDENTITY(1,1) NOT NULL,
	[Token] [nvarchar](300) NULL,
	[DateOrder] [datetime] NULL,
	[CustomerId] [int] NULL,
 CONSTRAINT [PK_Bondora_Order] PRIMARY KEY CLUSTERED 
(
	[OrderId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


INSERT INTO BondoraInventory (Name, TypeId) VALUES ('Caterpillar bulldozer', 1);

INSERT INTO BondoraInventory (Name, TypeId) VALUES ('KamAZ truck', 2);

INSERT INTO BondoraInventory (Name, TypeId) VALUES ('Komatsu crane', 1);

INSERT INTO BondoraInventory (Name, TypeId) VALUES ('Volvo steamroller', 2);

INSERT INTO BondoraInventory (Name, TypeId) VALUES ('Bosch jackhammer', 3);


