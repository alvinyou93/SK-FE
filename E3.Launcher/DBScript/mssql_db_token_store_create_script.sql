
/* Token 저장소 */
CREATE TABLE [dbo].[E3_TOKEN_STORE] (
	[USER_ID] [varchar](100) NOT NULL,  /* 사용자계정 */
	[TOKEN] [varchar](max) NOT NULL,  /* Token */
	[REG_DATE] [datetime] NOT NULL,  /* 등록일시 */
	[CUR_DATE] [datetime] /* 현재일시 */
)
GO

/* Token 저장소 기본키 */
ALTER TABLE [dbo].[E3_TOKEN_STORE]
	ADD
		CONSTRAINT [PK_E3_TOKEN_STORE]
		PRIMARY KEY NONCLUSTERED (
			[USER_ID] ASC
		)
GO

/* Token 저장소 인덱스 */
CREATE NONCLUSTERED INDEX [IX_E3_TOKEN_STORE] ON [dbo].[E3_TOKEN_STORE] (
	[TOKEN] ASC
)
GO

/* Token 저장소 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'Token 저장소', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE'
GO

/* 사용자계정 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'사용자계정
', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'COLUMN', @level2name=N'USER_ID'
GO

/* Token */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'Token
', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'COLUMN', @level2name=N'TOKEN'
GO

/* 등록일시 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'등록일시

', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'COLUMN', @level2name=N'REG_DATE'
GO

/* 현재일시 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'현재일시
', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'COLUMN', @level2name=N'CUR_DATE'
GO

/* Token 저장소 기본키 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'Token 저장소 기본키', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'CONSTRAINT', @level2name=N'PK_E3_TOKEN_STORE'
GO

/* Token 저장소 기본키 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'Token 저장소 기본키', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'INDEX', @level2name=N'PK_E3_TOKEN_STORE'
GO

/* Token 저장소 인덱스 */
EXEC sp_addextendedproperty 
	@name=N'MS_Description', @value=N'Token 저장소 인덱스', 
	@level0type=N'SCHEMA', @level0name=N'dbo', 
	@level1type=N'TABLE', @level1name=N'E3_TOKEN_STORE', 
	@level2type=N'INDEX', @level2name=N'IX_E3_TOKEN_STORE'
GO