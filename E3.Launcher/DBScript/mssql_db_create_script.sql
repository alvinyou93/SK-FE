USE [e3]
GO
/****** Object:  User [e3cmid]    Script Date: 2021-08-31 오후 1:34:15 ******/
CREATE USER [e3cmid] FOR LOGIN [e3cmid] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  DatabaseRole [cmid_roll]    Script Date: 2021-08-31 오후 1:34:15 ******/
CREATE ROLE [cmid_roll]
GO
ALTER ROLE [cmid_roll] ADD MEMBER [e3cmid]
GO
/****** Object:  UserDefinedFunction [dbo].[FN_MASK_TELNO]    Script Date: 2021-08-31 오후 1:34:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- 함수생성
CREATE FUNCTION [dbo].[FN_MASK_TELNO](@STR VARCHAR(50))
RETURNS VARCHAR(50)
AS
     BEGIN
         DECLARE @RTNVALUE VARCHAR(50), @STRLEN INT, @POS INT, @VSTR VARCHAR(50);
         SET @VSTR = REPLACE(REPLACE(@STR, ' ', ''), '-', '');
         SET @STRLEN = LEN(@VSTR);
         IF @STRLEN < 7
             BEGIN
                 SET @RTNVALUE = @VSTR;
         END;
             ELSE
             IF @STRLEN = 7
                 BEGIN
                     SET @RTNVALUE = '***-' + RIGHT(@VSTR, 4);
             END;
                 ELSE
                 IF @STRLEN = 8
                     BEGIN
                         SET @RTNVALUE = '****-' + RIGHT(@VSTR, 4);
                 END;
                     ELSE
                     IF @STRLEN = 9
                         BEGIN
                             SET @RTNVALUE = '02-****-' + RIGHT(@VSTR, 4);
                     END;
                         ELSE
                         IF @STRLEN = 10
                             BEGIN
                                 IF LEFT(@VSTR, 2) = '02'
                                     BEGIN
                                         SET @RTNVALUE = LEFT(@VSTR, 2) + '-****-' + RIGHT(@VSTR, 4);
                                 END;
                                     ELSE
                                     BEGIN
                                         SET @RTNVALUE = LEFT(@VSTR, 3) + '-***-' + RIGHT(@VSTR, 4);
                                 END;
                         END;
                             ELSE
                             IF @STRLEN = 11
                                 BEGIN
                                     SET @RTNVALUE = LEFT(@VSTR, 3) + '-****-' + RIGHT(@VSTR, 4);
                             END;
         RETURN @RTNVALUE;
     END;
GO
/****** Object:  UserDefinedFunction [dbo].[func_getNumeric]    Script Date: 2021-08-31 오후 1:34:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[func_getNumeric] (@strAlphaNumeric VARCHAR(256)) 
returns VARCHAR(256) 
AS 
  BEGIN 
      DECLARE @intAlpha INT 

      SET @intAlpha = Patindex('%[^0-9]%', @strAlphaNumeric) 

      BEGIN 
          WHILE @intAlpha > 0 
            BEGIN 
                SET @strAlphaNumeric = Stuff(@strAlphaNumeric, @intAlpha, 1, '') 
                SET @intAlpha = Patindex('%[^0-9]%', @strAlphaNumeric) 
            END 
      END 

      RETURN Isnull(@strAlphaNumeric, 0) 
  END 

GO
/****** Object:  UserDefinedFunction [dbo].[F_GET_TB_INFO]    Script Date: 2021-08-31 오후 1:34:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[F_GET_TB_INFO] 
(	
	@TB_NM          NVARCHAR(100)
)
RETURNS TABLE 
AS
/******************************************************************************
   NAME     : F_GET_TB_INFO
   PURPOSE  : 테이블 컬럼 정보 조회
   PARAMETER:   
				TB_NM			:	테이블명
				
   RETURN   :
                테이블 코멘트 및 타입정보
   REVISIONS:
   Ver.       Date        Author           Description
   ---------  ----------  ---------------  ------------------------------------
   1.0        2021-07-06  황인태            1. 최초 작성
******************************************************************************/
RETURN 
(
	WITH TB_INFO AS (
	SELECT  DISTINCT
	-- ROW_NUMBER() OVER (ORDER BY A.ORDINAL_POSITION) R_NUM
	A.ORDINAL_POSITION,
	A.TABLE_NAME,
	C.VALUE AS TABLE_COMMENT,
	A.COLUMN_NAME, A.DATA_TYPE,
	CASE WHEN D.TABLE_NAME IS NULL THEN '' ELSE 'PK' END PK,
	ISNULL(CAST(A.CHARACTER_MAXIMUM_LENGTH AS VARCHAR),  
	CAST(A.NUMERIC_PRECISION AS VARCHAR) + ',' +
	CAST(A.NUMERIC_SCALE AS VARCHAR)) AS COLUMN_LENGTH,
	A.COLUMN_DEFAULT, A.IS_NULLABLE,
	B.VALUE AS COLUM_COMMENT
	FROM INFORMATION_SCHEMA.COLUMNS A
	LEFT OUTER JOIN SYS.EXTENDED_PROPERTIES B ON B.MAJOR_ID = OBJECT_ID(A.TABLE_NAME) AND A.ORDINAL_POSITION = B.MINOR_ID and class_desc = 'OBJECT_OR_COLUMN'
	LEFT OUTER JOIN (SELECT OBJECT_ID(OBJNAME) AS TABLE_ID, VALUE FROM ::FN_LISTEXTENDEDPROPERTY(NULL, 'USER','DBO','TABLE',NULL, NULL, NULL)) C ON OBJECT_ID(A.TABLE_NAME) = C.TABLE_ID
	LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE D ON A.TABLE_NAME = D.TABLE_NAME AND A.COLUMN_NAME = D.COLUMN_NAME
	WHERE A.TABLE_NAME = @TB_NM
	)
	SELECT ROW_NUMBER() OVER (ORDER BY A.ORDINAL_POSITION) R_NUM
	, CASE WHEN ORDINAL_POSITION = 1 THEN TABLE_NAME ELSE '' END TABLE_NAME
	, CASE WHEN ORDINAL_POSITION = 1 THEN TABLE_COMMENT ELSE '' END TABLE_COMMENT
	, COLUMN_NAME
	, PK
	, DATA_TYPE
	, ISNULL(COLUMN_LENGTH, '') COLUMN_LENGTH
	, COLUMN_DEFAULT
	, IS_NULLABLE
	, COLUM_COMMENT
	FROM TB_INFO A
	-- ORDER BY A.TABLE_NAME, R_NUM
)
GO
/****** Object:  Table [dbo].[E3_ALLOW_IP_ADDRESS]    Script Date: 2021-08-31 오후 1:34:15 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_ALLOW_IP_ADDRESS](
	[IDX] [int] NOT NULL,
	[FROMIP] [varchar](16) NULL,
	[TOIP] [varchar](16) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [PK_E3_ALLOW_IP_ADDRESS] PRIMARY KEY CLUSTERED 
(
	[IDX] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_AUTH]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_AUTH](
	[AUTHORITY_ID] [varchar](100) NOT NULL,
	[AUTHORITY_NAME] [varchar](100) NULL,
	[AUTHORITY_DESC] [varchar](2100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[PLANT_ID] [varchar](100) NULL,
 CONSTRAINT [PK_E3_AUTH] PRIMARY KEY CLUSTERED 
(
	[AUTHORITY_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_AUTH_MENU_BTN_MAPPING]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_AUTH_MENU_BTN_MAPPING](
	[AUTHORITY_ID] [varchar](100) NOT NULL,
	[MENU_ID] [varchar](100) NOT NULL,
	[BTN_ID] [varchar](100) NOT NULL,
	[USE_YN] [varchar](1) NOT NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
 CONSTRAINT [PK_E3_AUTH_MENU_BTN_MAPPING] PRIMARY KEY CLUSTERED 
(
	[AUTHORITY_ID] ASC,
	[MENU_ID] ASC,
	[BTN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_AUTH_MENU_MAPPING]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_AUTH_MENU_MAPPING](
	[AUTHORITY_ID] [varchar](100) NOT NULL,
	[MENU_ID] [varchar](100) NOT NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
 CONSTRAINT [PK_E3_AUTH_MENU_MAPPING] PRIMARY KEY CLUSTERED 
(
	[AUTHORITY_ID] ASC,
	[MENU_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_BARCODEPRINTER]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_BARCODEPRINTER](
	[PLANT_ID] [varchar](100) NULL,
	[MACADDR] [varchar](32) NOT NULL,
	[IPADDR] [varchar](64) NOT NULL,
	[SERVICEPORT] [varchar](8) NOT NULL,
	[MODELINFO] [varchar](100) NULL,
	[LABELTYPE] [varchar](100) NULL,
	[LOCATIONINFO] [varchar](100) NULL,
	[DESCRIPTION] [varchar](256) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [E3_BARCODEPRINTER_PK] PRIMARY KEY CLUSTERED 
(
	[MACADDR] ASC,
	[IPADDR] ASC,
	[SERVICEPORT] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_CFG_BUTTON]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_CFG_BUTTON](
	[BTN_ID] [varchar](100) NOT NULL,
	[BTN_NAME] [varchar](100) NOT NULL,
	[BTN_TYPE] [varchar](100) NOT NULL,
	[BTN_EVENT] [varchar](100) NULL,
	[BTN_SERVICE] [varchar](100) NULL,
	[BTN_IMAGE] [varchar](100) NULL,
	[USE_YN] [varchar](1) NOT NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [PK_E3_CFG_BUTTON] PRIMARY KEY CLUSTERED 
(
	[BTN_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_CODE]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_CODE](
	[CODE_CLASS_ID] [varchar](100) NULL,
	[CODE_ID] [varchar](100) NOT NULL,
	[CODE_NAME] [varchar](100) NOT NULL,
	[PARENT_ID] [varchar](100) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[DESCRIPTION] [varchar](100) NULL,
	[DISPLAY_SEQ] [float] NULL,
	[TEMP_01] [varchar](100) NULL,
	[TEMP_02] [varchar](100) NULL,
	[TEMP_03] [varchar](100) NULL,
	[TEMP_04] [varchar](100) NULL,
	[TEMP_05] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[LANGUAGE_ID] [varchar](20) NOT NULL,
	[PLANT_ID] [varchar](100) NOT NULL,
 CONSTRAINT [E3_CODE_PK] PRIMARY KEY CLUSTERED 
(
	[CODE_ID] ASC,
	[PLANT_ID] ASC,
	[LANGUAGE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_CUSTOMER]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_CUSTOMER](
	[CUSTOMER_ID] [varchar](100) NOT NULL,
	[CUSTOMER_TYPE] [varchar](100) NULL,
	[CUSTOMER_NAME] [varchar](100) NULL,
	[ADDRESS1] [varchar](100) NULL,
	[ADDRESS2] [varchar](100) NULL,
	[ETC] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[POSTAL_CODE] [varchar](100) NULL,
	[PLANT_ID] [varchar](100) NULL,
	[DEPART_ID] [varchar](100) NULL,
	[CUSTOMER_OWNER] [varchar](100) NULL,
	[CUSTOMER_PHONE] [varchar](100) NULL,
 CONSTRAINT [TB_CUSTOMER_PK] PRIMARY KEY CLUSTERED 
(
	[CUSTOMER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_LOG]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_LOG](
	[LOG_ID] [varchar](100) NOT NULL,
	[LOG_TYPE] [varchar](40) NULL,
	[PROGRAM_ID] [varchar](100) NULL,
	[ERROR_MESSAGE] [varchar](1000) NULL,
	[USER_ID] [varchar](100) NULL,
	[IPADDRESS] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
 CONSTRAINT [E3_LOG_PK] PRIMARY KEY CLUSTERED 
(
	[LOG_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_MENU]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_MENU](
	[MENU_ID] [varchar](100) NOT NULL,
	[MENU_NAME] [varchar](100) NOT NULL,
	[PARENT_ID] [varchar](100) NULL,
	[LVL] [smallint] NULL,
	[MENU_DESCRIPTION] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[PROGRAM_ID] [varchar](100) NULL,
	[ORD] [int] NULL,
 CONSTRAINT [E3_MENU_PK] PRIMARY KEY CLUSTERED 
(
	[MENU_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_MENU_FAVORITE]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_MENU_FAVORITE](
	[PLANT_ID] [varchar](100) NULL,
	[USER_ID] [varchar](100) NULL,
	[MENU_ID] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_MESSAGE]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_MESSAGE](
	[MESSAGE_ID] [varchar](6) NOT NULL,
	[LANGUAGE_ID] [varchar](10) NOT NULL,
	[MESSAGE_TYPE] [varchar](1) NULL,
	[MESSAGE] [varchar](1200) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [E3_MESSAGE_PK] PRIMARY KEY CLUSTERED 
(
	[MESSAGE_ID] ASC,
	[LANGUAGE_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_PGM]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_PGM](
	[PLANT_ID] [varchar](100) NULL,
	[PROGRAM_ID] [varchar](100) NOT NULL,
	[PROGRAM_NAME] [varchar](100) NULL,
	[PROGRAM_PATH] [varchar](100) NULL,
	[PROGRAM_TYPE] [varchar](100) NULL,
	[PROGRAM_DESC] [varchar](200) NULL,
	[ASSEMBLY_NAME] [varchar](100) NULL,
	[NAMESPACE_NAME] [varchar](100) NULL,
	[CLASS_NAME] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [E3_PGM_PK] PRIMARY KEY CLUSTERED 
(
	[PROGRAM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_TEST]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_TEST](
	[ID] [varchar](100) NOT NULL,
	[NAME] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [E3_TEAT_PK] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_USER]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_USER](
	[USER_ID] [varchar](100) NOT NULL,
	[USER_NAME] [varchar](100) NULL,
	[USER_PASSWORD] [varchar](256) NULL,
	[USER_EMAIL] [varchar](100) NULL,
	[LOGIN_ID] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[EXPIRY_DATE] [datetime] NULL,
	[PLANT_ID] [varchar](100) NULL,
	[PASSWORD_CHANGE_YN] [varchar](1) NULL,
	[PASSWORD_CHANGE_DATE] [datetime] NULL,
	[FAIL_CNT] [smallint] NULL,
	[DEPART_ID] [varchar](100) NULL,
 CONSTRAINT [E3_USER_PK] PRIMARY KEY CLUSTERED 
(
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_USER_AUTH_MAPPING]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_USER_AUTH_MAPPING](
	[USER_ID] [varchar](100) NOT NULL,
	[AUTHORITY_ID] [varchar](100) NOT NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[ID] [varchar](100) NULL,
 CONSTRAINT [E3_USER_AUTH_MAPPING_PK] PRIMARY KEY CLUSTERED 
(
	[USER_ID] ASC,
	[AUTHORITY_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_USER_CHANGE_LOG]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_USER_CHANGE_LOG](
	[LOG_ID] [varchar](100) NOT NULL,
	[USER_ID] [varchar](100) NOT NULL,
	[USER_NAME] [varchar](100) NULL,
	[USER_PASSWORD] [varchar](256) NULL,
	[USER_EMAIL] [varchar](100) NULL,
	[LOGIN_ID] [varchar](100) NULL,
	[USE_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
	[EXPIRY_DATE] [datetime] NULL,
	[PLANT_ID] [varchar](100) NULL,
	[PASSWORD_CHANGE_YN] [varchar](1) NULL,
	[PASSWORD_CHANGE_DATE] [datetime] NULL,
	[FAIL_CNT] [smallint] NULL,
 CONSTRAINT [E3_USER_CHANGE_LOG_PK] PRIMARY KEY CLUSTERED 
(
	[LOG_ID] ASC,
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[E3_USER_PASSWORD_HIS]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[E3_USER_PASSWORD_HIS](
	[REG_DATE] [datetime] NOT NULL,
	[USER_ID] [varchar](100) NOT NULL,
	[OLD_PASSWORD] [varchar](256) NOT NULL,
 CONSTRAINT [PK_E3_USER_PASSWORD_HIS] PRIMARY KEY CLUSTERED 
(
	[REG_DATE] ASC,
	[USER_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_ATTC_DOC_MAN]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_ATTC_DOC_MAN](
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[ACCP_GB] [varchar](100) NOT NULL,
	[FILE_TYPE] [varchar](100) NOT NULL,
	[FILE_NAME] [varchar](100) NULL,
	[USE_YN] [varchar](100) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_ATTC_DOC_MAN] PRIMARY KEY NONCLUSTERED 
(
	[PLANT_ID] ASC,
	[TA_ID] DESC,
	[ACCP_GB] ASC,
	[FILE_TYPE] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_ATTC_FILE]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_ATTC_FILE](
	[FILE_ID] [int] IDENTITY(1,1) NOT NULL,
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[ACCP_GB] [varchar](100) NOT NULL,
	[EMP_ID] [int] NOT NULL,
	[FILE_TYPE] [varchar](100) NOT NULL,
	[REV] [int] NOT NULL,
	[UPLOAD_FILE] [text] NULL,
	[REAL_FILE_NM] [varchar](300) NULL,
	[REAL_FILE_DT] [varchar](20) NULL,
	[REG_DT] [datetime] NULL,
	[FILE_APPR_YN] [varchar](100) NULL,
	[FILE_APPR_DT] [datetime] NULL,
	[LST_YN] [varchar](1) NULL,
 CONSTRAINT [PK_SE_TA_ATTC_FILE] PRIMARY KEY CLUSTERED 
(
	[FILE_ID] ASC,
	[PLANT_ID] ASC,
	[TA_ID] ASC,
	[ACCP_GB] ASC,
	[EMP_ID] ASC,
	[FILE_TYPE] ASC,
	[REV] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_CMP]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_CMP](
	[CMP_ID] [int] IDENTITY(1,1) NOT NULL,
	[CMP_NM] [varchar](100) NOT NULL,
	[CMP_NO] [varchar](100) NOT NULL,
	[MNGR_NM] [varchar](100) NOT NULL,
	[MNGR_TEL_NO] [varchar](100) NULL,
	[MNGR_EMAIL] [varchar](100) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_CMP] PRIMARY KEY CLUSTERED 
(
	[CMP_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_EDU]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_EDU](
	[PLANT_ID] [varchar](10) NOT NULL,
	[EDU_MTRL_ID] [int] NOT NULL,
	[L_CLOUD_FILE_ID] [varchar](100) NULL,
	[EDU_MTRL_TYPE] [varchar](100) NOT NULL,
	[EDU_MTRL_NM] [varchar](100) NOT NULL,
	[ORDR] [int] NULL,
	[FILE_PATH] [varchar](200) NOT NULL,
	[USE_YN] [varchar](1) NOT NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
	[FILE_GB] [varchar](100) NULL,
 CONSTRAINT [PK_SE_TA_EDU] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[EDU_MTRL_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_EMP]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_EMP](
	[EMP_ID] [int] IDENTITY(1,1) NOT NULL,
	[PLANT_ID] [varchar](10) NOT NULL,
	[CRTIC_NO] [int] NOT NULL,
	[ACCESS_GB] [varchar](10) NOT NULL,
	[SAFE_EDU_CMPLT_YN] [varchar](1) NULL,
	[SAFE_EDU_CMPLT_DT] [datetime] NULL,
	[PASS_YN] [varchar](1) NULL,
	[PASS_SCORE] [int] NULL,
	[PASS_DT] [datetime] NULL,
	[PRIVACY_CONSENT_DT] [datetime] NULL,
	[SMS_SND_DT] [datetime] NULL,
	[BLOCK_YN] [varchar](1) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
	[ID_PHOTO] [text] NULL,
	[BP_HIGHT] [int] NULL,
	[BP_LOW] [int] NULL,
	[ACCESS_POSSIBLE_YN] [varchar](100) NULL,
	[ACCESS_POSSIBLE_DT] [date] NULL,
	[RMK] [varchar](2000) NULL,
	[ADDR] [varchar](500) NULL,
	[BIRTH_DT] [varchar](20) NULL,
	[OPTIONDATAS] [varchar](max) NULL,
	[EMP_TEL_NO] [varchar](15) NULL,
	[EMP_NM] [varchar](100) NULL,
	[FILE_SMS_SEND] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_EMP] PRIMARY KEY CLUSTERED 
(
	[EMP_ID] ASC,
	[PLANT_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_EMP_EXCEL]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_EMP_EXCEL](
	[PLANT_ID] [varchar](10) NOT NULL,
	[FACT_CD] [varchar](100) NULL,
	[TA_CODE] [varchar](100) NOT NULL,
	[TA_YEAR] [varchar](100) NULL,
	[UPLOAD_SEQ] [int] NOT NULL,
	[TA_SUB] [varchar](200) NOT NULL,
	[REL_DEPT_CD] [varchar](100) NOT NULL,
	[EMP_ID] [int] NOT NULL,
	[CMP_ID] [int] NOT NULL,
	[EMP_NM] [varchar](100) NOT NULL,
	[EMP_TEL_NO] [varchar](15) NOT NULL,
	[ACCESS_GB] [varchar](10) NOT NULL,
	[RMK] [varchar](16) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_EMP_EXCEL] PRIMARY KEY CLUSTERED 
(
	[UPLOAD_SEQ] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_EXAM]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_EXAM](
	[PLANT_ID] [varchar](10) NOT NULL,
	[EXAM_ID] [int] NOT NULL,
	[EXAM_TYPE] [varchar](100) NOT NULL,
	[EXAM_SENTENCE] [varchar](500) NOT NULL,
	[ANSWER_1] [varchar](100) NULL,
	[ANSWER_2] [varchar](100) NULL,
	[ANSWER_3] [varchar](100) NULL,
	[ANSWER_4] [varchar](100) NULL,
	[RIGHT_ANSWER] [int] NULL,
	[USE_YN] [varchar](1) NOT NULL,
	[ORDR] [int] NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_EXAM] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[EXAM_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_FACEDETECT]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_FACEDETECT](
	[PLANT_ID] [varchar](100) NOT NULL,
	[FACT_CD] [varchar](100) NOT NULL,
	[SERIALNO] [varchar](50) NOT NULL,
	[IPADDR] [varchar](32) NULL,
	[LOCATION] [varchar](256) NULL,
	[RMK] [varchar](2000) NULL,
	[USE_YN] [varchar](100) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_FACEDETECT] PRIMARY KEY NONCLUSTERED 
(
	[PLANT_ID] ASC,
	[FACT_CD] ASC,
	[SERIALNO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_LOG]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_LOG](
	[event_index] [int] NOT NULL,
	[event_date] [datetime] NOT NULL,
	[event_result] [varchar](200) NOT NULL,
	[user_id] [varchar](200) NOT NULL,
	[user_name] [varchar](200) NULL,
	[door_name] [varchar](200) NULL,
	[direction] [varchar](200) NULL,
	[device_ip] [varchar](200) NULL,
	[device_name] [varchar](200) NULL,
	[device_sn] [varchar](200) NOT NULL,
	[auth_type] [varchar](200) NULL,
 CONSTRAINT [PK_SE_TA_LOG] PRIMARY KEY CLUSTERED 
(
	[event_index] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SE_TA_QUIZ]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SE_TA_QUIZ](
	[QUIZ_ID] [varchar](100) NOT NULL,
	[PLANT_ID] [varchar](10) NOT NULL,
	[EDU_MTRL_ID] [int] NOT NULL,
	[SDDN_QUIZ_ORDR] [int] NOT NULL,
	[QUIZ_SENTENCE] [varchar](200) NULL,
	[ANSWER_1] [varchar](100) NULL,
	[ANSWER_2] [varchar](100) NULL,
	[ANSWER_3] [varchar](100) NULL,
	[ANSWER_4] [varchar](100) NULL,
	[RIGHT_ANSWER] [int] NULL,
	[USE_YN] [varchar](1) NOT NULL,
	[SDDN_QUIZ_TIME] [int] NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DT] [datetime] NULL,
	[UPT_ID] [varchar](100) NULL,
	[UPT_DT] [datetime] NULL,
 CONSTRAINT [PK_SE_TA_QUIZ] PRIMARY KEY CLUSTERED 
(
	[QUIZ_ID] ASC,
	[PLANT_ID] ASC,
	[EDU_MTRL_ID] ASC,
	[SDDN_QUIZ_ORDR] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WK_FACILITY_INFO]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WK_FACILITY_INFO](
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[FACILITY_KND] [varchar](100) NOT NULL,
	[PROCESS_CD] [varchar](100) NOT NULL,
	[FACILITY_NO] [varchar](100) NOT NULL,
	[FACILITY_NM] [varchar](2000) NULL,
	[RMK] [varchar](2000) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [WK_FACILITY_INFO_PK] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[TA_ID] ASC,
	[FACILITY_KND] ASC,
	[PROCESS_CD] ASC,
	[FACILITY_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WK_FACILITY_STEP]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WK_FACILITY_STEP](
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[FACILITY_KND] [varchar](100) NOT NULL,
	[STEP1_CD] [varchar](100) NULL,
	[STEP2_CD] [varchar](100) NULL,
	[STEP3_CD] [varchar](100) NULL,
	[STEP4_CD] [varchar](100) NULL,
	[STEP5_CD] [varchar](100) NULL,
	[STEP6_CD] [varchar](100) NULL,
	[STEP7_CD] [varchar](100) NULL,
	[STEP8_CD] [varchar](100) NULL,
	[STEP9_CD] [varchar](100) NULL,
	[RMK] [varchar](2000) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [WK_FACILITY_STEP_PK] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[TA_ID] ASC,
	[FACILITY_KND] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WK_FACILTY_INFO]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WK_FACILTY_INFO](
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[FACILTY_KND] [varchar](100) NOT NULL,
	[PROCESS_CD] [varchar](100) NOT NULL,
	[FACILTY_NO] [varchar](100) NOT NULL,
	[FACILTY_NM] [varchar](2000) NULL,
	[RMK] [varchar](2000) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [WK_FACILTY_INFO_PK] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[TA_ID] ASC,
	[FACILTY_KND] ASC,
	[PROCESS_CD] ASC,
	[FACILTY_NO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WK_JOB_AUTH_MAPPING]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WK_JOB_AUTH_MAPPING](
	[PLANT_ID] [varchar](100) NOT NULL,
	[JOB_ID] [varchar](100) NOT NULL,
	[AUTHORITY_ID] [varchar](100) NOT NULL,
	[RMK] [varchar](2000) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [WK_JOB_AUTH_MAPPING_PK] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[JOB_ID] ASC,
	[AUTHORITY_ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[WK_SIGN_SHEET]    Script Date: 2021-08-31 오후 1:34:16 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WK_SIGN_SHEET](
	[PLANT_ID] [varchar](100) NOT NULL,
	[TA_ID] [varchar](100) NOT NULL,
	[FACILITY_KND] [varchar](100) NOT NULL,
	[PROCESS_CD] [varchar](100) NOT NULL,
	[FACILITY_NO] [varchar](100) NOT NULL,
	[STEP_CD] [varchar](100) NOT NULL,
	[WK_STATUS] [varchar](100) NULL,
	[MNGR_ID] [varchar](100) NULL,
	[COMP_DT] [varchar](8) NULL,
	[RMK] [varchar](2000) NULL,
	[REG_ID] [varchar](100) NULL,
	[REG_DATE] [datetime] NULL,
	[MOD_ID] [varchar](100) NULL,
	[MOD_DATE] [datetime] NULL,
 CONSTRAINT [WK_SIGN_SHEET_PK] PRIMARY KEY CLUSTERED 
(
	[PLANT_ID] ASC,
	[TA_ID] ASC,
	[FACILITY_KND] ASC,
	[PROCESS_CD] ASC,
	[FACILITY_NO] ASC,
	[STEP_CD] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[E3_ALLOW_IP_ADDRESS] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_AUTH_MENU_BTN_MAPPING] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[E3_AUTH_MENU_BTN_MAPPING] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_BARCODEPRINTER] ADD  DEFAULT (NULL) FOR [PLANT_ID]
GO
ALTER TABLE [dbo].[E3_BARCODEPRINTER] ADD  DEFAULT ((8888)) FOR [SERVICEPORT]
GO
ALTER TABLE [dbo].[E3_BARCODEPRINTER] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_CFG_BUTTON] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[E3_CFG_BUTTON] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_CODE] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_CODE] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[E3_CODE] ADD  DEFAULT ('KO') FOR [LANGUAGE_ID]
GO
ALTER TABLE [dbo].[E3_CODE] ADD  DEFAULT ('') FOR [PLANT_ID]
GO
ALTER TABLE [dbo].[E3_CUSTOMER] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[E3_CUSTOMER] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[E3_LOG] ADD  DEFAULT ('ACTIVITY_LOG') FOR [LOG_TYPE]
GO
ALTER TABLE [dbo].[E3_LOG] ADD  DEFAULT ('-1') FOR [PROGRAM_ID]
GO
ALTER TABLE [dbo].[E3_LOG] ADD  DEFAULT ('-1') FOR [ERROR_MESSAGE]
GO
ALTER TABLE [dbo].[E3_LOG] ADD  DEFAULT ('-1') FOR [USER_ID]
GO
ALTER TABLE [dbo].[E3_USER_PASSWORD_HIS] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[SE_TA_CMP] ADD  DEFAULT (getdate()) FOR [REG_DT]
GO
ALTER TABLE [dbo].[SE_TA_CMP] ADD  DEFAULT (getdate()) FOR [UPT_DT]
GO
ALTER TABLE [dbo].[SE_TA_EDU] ADD  DEFAULT (getdate()) FOR [REG_DT]
GO
ALTER TABLE [dbo].[SE_TA_EDU] ADD  DEFAULT (getdate()) FOR [UPT_DT]
GO
ALTER TABLE [dbo].[SE_TA_EMP] ADD  DEFAULT (getdate()) FOR [REG_DT]
GO
ALTER TABLE [dbo].[SE_TA_EMP] ADD  DEFAULT (getdate()) FOR [UPT_DT]
GO
ALTER TABLE [dbo].[SE_TA_EMP_EXCEL] ADD  DEFAULT (getdate()) FOR [REG_ID]
GO
ALTER TABLE [dbo].[SE_TA_EMP_EXCEL] ADD  DEFAULT (getdate()) FOR [UPT_ID]
GO
ALTER TABLE [dbo].[SE_TA_EXAM] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[SE_TA_EXAM] ADD  DEFAULT (getdate()) FOR [REG_ID]
GO
ALTER TABLE [dbo].[SE_TA_EXAM] ADD  DEFAULT (getdate()) FOR [UPT_ID]
GO
ALTER TABLE [dbo].[SE_TA_FACEDETECT] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[SE_TA_FACEDETECT] ADD  DEFAULT (getdate()) FOR [REG_DATE]
GO
ALTER TABLE [dbo].[SE_TA_QUIZ] ADD  DEFAULT ('Y') FOR [USE_YN]
GO
ALTER TABLE [dbo].[SE_TA_QUIZ] ADD  DEFAULT (getdate()) FOR [REG_ID]
GO
ALTER TABLE [dbo].[SE_TA_QUIZ] ADD  DEFAULT (getdate()) FOR [UPT_ID]
GO
ALTER TABLE [dbo].[E3_AUTH_MENU_BTN_MAPPING]  WITH CHECK ADD  CONSTRAINT [FK_E3_CFG_BUTTON_TO_E3_AUTH_MENU_BTN_MAPPING] FOREIGN KEY([BTN_ID])
REFERENCES [dbo].[E3_CFG_BUTTON] ([BTN_ID])
GO
ALTER TABLE [dbo].[E3_AUTH_MENU_BTN_MAPPING] CHECK CONSTRAINT [FK_E3_CFG_BUTTON_TO_E3_AUTH_MENU_BTN_MAPPING]
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'일련번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'IDX'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'FROM-IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'FROMIP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TO-IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'TOIP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'접속 허용 IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_ALLOW_IP_ADDRESS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'AUTHORITY_ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'AUTHORITY_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MENU_ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'MENU_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼아이디' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'BTN_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'사용자 권한 그룹별 메뉴 버튼 사용여부 정의' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_AUTH_MENU_BTN_MAPPING'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Mac Address' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'MACADDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'IP Address (IPv4)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'IPADDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'네트워크 Port 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'SERVICEPORT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Printer Model 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'MODELINFO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Label 형식 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'LABELTYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설치 위지 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'LOCATIONINFO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'DESCRIPTION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'BARCODE Printer  정보 관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_BARCODEPRINTER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼아이디' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼이름' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_NAME'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼형식' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼이벤트' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_EVENT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼서비스' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_SERVICE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버튼그림' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'BTN_IMAGE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'최초등록자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'최종수정자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'버튼별 권한 정의를 위한 버튼 기초 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CFG_BUTTON'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'고객사ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'CUSTOMER_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'고객사구분' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'CUSTOMER_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'고객사 이름' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'CUSTOMER_NAME'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'고객사주소1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'ADDRESS1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'고객사주소2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'ADDRESS2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'ETC'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용유무' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록시간' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정시간' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'우편번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER', @level2type=N'COLUMN',@level2name=N'POSTAL_CODE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'고객사 기준 정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_CUSTOMER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'MESSAGE ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'MESSAGE_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'언어 코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'LANGUAGE_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'메세지 타입 L: LABEL, M: 메세지' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'MESSAGE_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'메세지 본분' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'MESSAGE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자 ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록시간' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자 ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정시간' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_MESSAGE', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_USER_PASSWORD_HIS', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용자계정' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_USER_PASSWORD_HIS', @level2type=N'COLUMN',@level2name=N'USER_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'이전비밀번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_USER_PASSWORD_HIS', @level2type=N'COLUMN',@level2name=N'OLD_PASSWORD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'비밀번호 이력' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'E3_USER_PASSWORD_HIS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA구분코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입구분코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'ACCP_GB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'첨부파일코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'FILE_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'첨부파일명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'FILE_NAME'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'첨부문서관리 기본키' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN', @level2type=N'CONSTRAINT',@level2name=N'PK_SE_TA_ATTC_DOC_MAN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'첨부문서관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_DOC_MAN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'파일 ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'FILE_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'파일 그룹 ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA 구분 코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입구분코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'ACCP_GB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용자 아이디  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'EMP_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'파일종류' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'FILE_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'버전정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'REV'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'업로드파일' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'UPLOAD_FILE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'실제파일명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'REAL_FILE_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수료일자/확인일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'REAL_FILE_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'파일 승인여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'FILE_APPR_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'제출상태(미제출/제출/승인/반려)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'FILE_APPR_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'최종여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE', @level2type=N'COLUMN',@level2name=N'LST_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'첨부파일 마스터' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_ATTC_FILE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'협력사ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'CMP_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'협력사명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'CMP_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업자번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'CMP_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'협력사담당자명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'MNGR_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'연락처' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'MNGR_TEL_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'EAMIL' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'MNGR_EMAIL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'출입회사 관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_CMP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'교육자료ID(SEQ)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'EDU_MTRL_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'L-CLOUD 파일 ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'L_CLOUD_FILE_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'교육자료유형(동영상,파일)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'EDU_MTRL_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'교육자료명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'EDU_MTRL_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'순서' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'ORDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'저장경로' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'FILE_PATH'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'교육자료관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EDU'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'EMP_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'인증번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'CRTIC_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입구분(일반/차량)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'ACCESS_GB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안전교육 이수여부(교육수강확인)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'SAFE_EDU_CMPLT_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안전교육 이수일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'SAFE_EDU_CMPLT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'시험통과여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'PASS_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'시험점수' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'PASS_SCORE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'시험통과일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'PASS_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'개인정보 이용동의일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'PRIVACY_CONSENT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'SMS발송일자(교육안내)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'SMS_SND_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입불가여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'BLOCK_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사진' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'ID_PHOTO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'혈압(고)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'BP_HIGHT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'혈압(저)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'BP_LOW'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입가능여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'ACCESS_POSSIBLE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입승인일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'ACCESS_POSSIBLE_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'주소' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'ADDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'생년월일' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'BIRTH_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사진판독정보' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP', @level2type=N'COLUMN',@level2name=N'OPTIONDATAS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'출입인원관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공장코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'FACT_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA코드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'TA_CODE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'정기보수년도' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'TA_YEAR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'업로드 순번' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'UPLOAD_SEQ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공사' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'TA_SUB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'작업부서' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'REL_DEPT_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용자 ID(SEQ 전화번호)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'EMP_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'회사ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'CMP_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'이름' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'EMP_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'연락처' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'EMP_TEL_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입구분(일반/차량)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'ACCESS_GB'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'출입인원관리엑셀업로드' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EMP_EXCEL'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'시험문제ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'EXAM_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'문제 유형(A,B)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'EXAM_TYPE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'문제' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'EXAM_SENTENCE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'ANSWER_1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'ANSWER_2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'ANSWER_3'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'ANSWER_4'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'정답' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'RIGHT_ANSWER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'문제순번' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'ORDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'시험문제관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_EXAM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공장정보
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'FACT_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'장치시리얼
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'SERIALNO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TCPIPv4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'IPADDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설치위치
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'LOCATION'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안면인식기관리 기본키' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT', @level2type=N'CONSTRAINT',@level2name=N'PK_SE_TA_FACEDETECT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안면인식기 설치 위치 정보 관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_FACEDETECT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'이벤트순번' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'event_index'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'이벤트일시(YYYYMMDD HHMMSS)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'event_date'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'이벤트결과' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'event_result'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'user_id'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'출입자명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'user_name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안면인식기IP' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'device_ip'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안면인식기명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'device_name'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'안면인식기씨리얼' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG', @level2type=N'COLUMN',@level2name=N'device_sn'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'출입Log' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_LOG'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'퀴즈ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'QUIZ_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'교육자료ID(SEQ)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'EDU_MTRL_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'순번(수행시간 1, 2,3)' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'SDDN_QUIZ_ORDR'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'퀴즈문제' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'QUIZ_SENTENCE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지1' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'ANSWER_1'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지2' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'ANSWER_2'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지3' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'ANSWER_3'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'선택지4' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'ANSWER_4'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'정답' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'RIGHT_ANSWER'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사용여부' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'USE_YN'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'돌발퀴즈 수행시간' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'SDDN_QUIZ_TIME'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'REG_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'UPT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'변경일시' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ', @level2type=N'COLUMN',@level2name=N'UPT_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DESCRIPTION', @value=N'돌발퀴즈관리' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'SE_TA_QUIZ'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비종류' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'FACILITY_KND'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공정' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'PROCESS_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'FACILITY_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'FACILITY_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sign Sheet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_INFO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비종류' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'FACILITY_KND'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'1단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP1_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'2단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP2_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'3단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP3_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'4단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP4_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'5단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP5_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'6단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP6_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'7단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP7_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'8단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP8_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'9단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'STEP9_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비종류별단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILITY_STEP'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비종류' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'FACILTY_KND'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공정' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'PROCESS_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'FACILTY_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비명' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'FACILTY_NM'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sign Sheet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_FACILTY_INFO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JOB ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'JOB_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'권한ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'AUTHORITY_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'JOB 권한 매핑' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_JOB_AUTH_MAPPING'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'사업장ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'PLANT_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'TA ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'TA_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비종류' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'FACILITY_KND'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'공정' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'PROCESS_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'설비번호' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'FACILITY_NO'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'단계' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'STEP_CD'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'작업상태' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'WK_STATUS'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'담당자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'MNGR_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'완료일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'COMP_DT'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'비고' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'RMK'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'REG_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'등록일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'REG_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정자ID' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'MOD_ID'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'수정일자' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET', @level2type=N'COLUMN',@level2name=N'MOD_DATE'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'Sign Sheet' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'WK_SIGN_SHEET'
GO
