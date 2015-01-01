USE [showmanagement_DEV]
GO

/****** Object:  Table [dbo].[ShowDownloads]    Script Date: 12/23/2014 17:18:31 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[ShowDownloads](
	[ShowDownloadId] [int] IDENTITY(1,1) NOT NULL,
	[OriginalPath] [nvarchar](max) NULL,
	[CurrentPath] [nvarchar](max) NULL
 CONSTRAINT [PK_dbo.ShowDownloads] PRIMARY KEY CLUSTERED 
(
	[ShowDownloadId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
)

GO