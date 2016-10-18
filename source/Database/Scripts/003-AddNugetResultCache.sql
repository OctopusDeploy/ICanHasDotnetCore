CREATE TABLE [dbo].[NugetResultCache](
	[Id] [nvarchar](300) NOT NULL,
	[Version] [nvarchar](150) NOT NULL,
	[SupportType] [nvarchar](20) NOT NULL,
	[ProjectUrl] [nvarchar](1000) NULL,
	[Dependencies] [nvarchar](max) NOT NULL,
	[Frameworks] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_NugetResultCache] PRIMARY KEY CLUSTERED 
(
	[Id] ASC,
	[Version] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO