


CREATE DATABASE IF NOT EXISTS webcam_studio
  CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE webcam_studio;

-- ---------------------------------------------------------------------
-- Modulo: Cuentas de modelos (G)
-- ---------------------------------------------------------------------
CREATE TABLE ModelAccounts (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  FullName            VARCHAR(150)  NOT NULL,
  Email               VARCHAR(200)  NOT NULL,
  PhoneNumber         VARCHAR(30)   NULL,
  PasswordHash        VARCHAR(400)  NOT NULL,
  Role                VARCHAR(20)   NOT NULL,
  Status              VARCHAR(20)   NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  UNIQUE KEY UX_ModelAccounts_Email (Email),
  UNIQUE KEY UX_ModelAccounts_PhoneNumber (PhoneNumber)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Modulo: Boton de WhatsApp (C)
-- ---------------------------------------------------------------------
CREATE TABLE Conversations (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  ModelAccountId      CHAR(36)      NOT NULL,
  Channel             VARCHAR(20)   NOT NULL,
  StartedAt           DATETIME(6)   NOT NULL,
  LastMessageAt       DATETIME(6)   NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  KEY IX_Conversations_ModelAccountId_LastMessageAt (ModelAccountId, LastMessageAt),
  CONSTRAINT FK_Conversations_ModelAccounts
    FOREIGN KEY (ModelAccountId) REFERENCES ModelAccounts (Id)
) ENGINE=InnoDB;

CREATE TABLE Messages (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  ConversationId      CHAR(36)      NOT NULL,
  Direction           VARCHAR(20)   NOT NULL,
  Content             VARCHAR(4000) NOT NULL,
  SentAt              DATETIME(6)   NOT NULL,
  WasProcessed        TINYINT(1)    NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  KEY IX_Messages_ConversationId_SentAt (ConversationId, SentAt),
  CONSTRAINT FK_Messages_Conversations
    FOREIGN KEY (ConversationId) REFERENCES Conversations (Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Modulo: Inventario de tienda (D)
-- ---------------------------------------------------------------------
CREATE TABLE Products (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  Name                VARCHAR(150)  NOT NULL,
  Sku                 VARCHAR(60)   NULL,
  Category            VARCHAR(100)  NULL,
  Unit                VARCHAR(30)   NOT NULL,
  MinimumStock        INT           NOT NULL,
  CurrentStock        INT           NOT NULL,
  IsActive            TINYINT(1)    NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  UNIQUE KEY UX_Products_Sku (Sku)
) ENGINE=InnoDB;

CREATE TABLE StockMovements (
  Id                    CHAR(36)      NOT NULL PRIMARY KEY,
  ProductId             CHAR(36)      NOT NULL,
  Type                  VARCHAR(20)   NOT NULL,
  Quantity              INT           NOT NULL,
  Reason                VARCHAR(300)  NULL,
  PerformedByAccountId  CHAR(36)      NOT NULL,
  PerformedAt           DATETIME(6)   NOT NULL,
  ResultingStock        INT           NOT NULL,
  CreatedAt             DATETIME(6)   NOT NULL,
  CreatedByAccountId    CHAR(36)      NULL,
  UpdatedAt             DATETIME(6)   NULL,
  UpdatedByAccountId    CHAR(36)      NULL,
  KEY IX_StockMovements_ProductId_PerformedAt (ProductId, PerformedAt),
  CONSTRAINT FK_StockMovements_Products
    FOREIGN KEY (ProductId) REFERENCES Products (Id),
  CONSTRAINT FK_StockMovements_ModelAccounts
    FOREIGN KEY (PerformedByAccountId) REFERENCES ModelAccounts (Id)
) ENGINE=InnoDB;

CREATE TABLE StockAlerts (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  ProductId           CHAR(36)      NOT NULL,
  Status               VARCHAR(20)  NOT NULL,
  Message              VARCHAR(500) NOT NULL,
  TriggeredAt          DATETIME(6)  NOT NULL,
  ResolvedAt           DATETIME(6)  NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  CONSTRAINT FK_StockAlerts_Products
    FOREIGN KEY (ProductId) REFERENCES Products (Id) ON DELETE CASCADE
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Modulo: Reporte de tokens (E)
-- ---------------------------------------------------------------------
CREATE TABLE Sites (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  Name                VARCHAR(150)  NOT NULL,
  Description         VARCHAR(300)  NULL,
  IsActive            TINYINT(1)    NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  UNIQUE KEY UX_Sites_Name (Name)
) ENGINE=InnoDB;

CREATE TABLE TokenReports (
  Id                    CHAR(36)       NOT NULL PRIMARY KEY,
  ModelAccountId        CHAR(36)       NOT NULL,
  SiteId                CHAR(36)       NOT NULL,
  Period                DATE           NOT NULL,
  TokensAmount          DECIMAL(18,2)  NOT NULL,
  MonetaryValue         DECIMAL(18,2)  NOT NULL,
  RegisteredByAccountId CHAR(36)       NOT NULL,
  RegisteredAt          DATETIME(6)    NOT NULL,
  CreatedAt             DATETIME(6)    NOT NULL,
  CreatedByAccountId    CHAR(36)       NULL,
  UpdatedAt             DATETIME(6)    NULL,
  UpdatedByAccountId    CHAR(36)       NULL,
  KEY IX_TokenReports_Model_Site_Period (ModelAccountId, SiteId, Period),
  CONSTRAINT FK_TokenReports_ModelAccounts
    FOREIGN KEY (ModelAccountId) REFERENCES ModelAccounts (Id),
  CONSTRAINT FK_TokenReports_Sites
    FOREIGN KEY (SiteId) REFERENCES Sites (Id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Modulo: Checklist de habitaciones (F)
-- ---------------------------------------------------------------------
CREATE TABLE Rooms (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  Name                VARCHAR(150)  NOT NULL,
  Description         VARCHAR(300)  NULL,
  IsActive            TINYINT(1)    NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  UNIQUE KEY UX_Rooms_Name (Name)
) ENGINE=InnoDB;

CREATE TABLE ChecklistTemplateItems (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  RoomId              CHAR(36)      NOT NULL,
  Name                VARCHAR(150)  NOT NULL,
  Description         VARCHAR(300)  NULL,
  IsActive            TINYINT(1)    NOT NULL,
  DisplayOrder        INT           NOT NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  CONSTRAINT FK_ChecklistTemplateItems_Rooms
    FOREIGN KEY (RoomId) REFERENCES Rooms (Id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE ChecklistRuns (
  Id                     CHAR(36)      NOT NULL PRIMARY KEY,
  RoomId                 CHAR(36)      NOT NULL,
  PerformedByAccountId   CHAR(36)      NOT NULL,
  PerformedAt            DATETIME(6)   NOT NULL,
  AvailableMaterialsNotes VARCHAR(1000) NULL,
  CreatedAt              DATETIME(6)   NOT NULL,
  CreatedByAccountId     CHAR(36)      NULL,
  UpdatedAt              DATETIME(6)   NULL,
  UpdatedByAccountId     CHAR(36)      NULL,
  KEY IX_ChecklistRuns_RoomId_PerformedAt (RoomId, PerformedAt),
  CONSTRAINT FK_ChecklistRuns_Rooms
    FOREIGN KEY (RoomId) REFERENCES Rooms (Id),
  CONSTRAINT FK_ChecklistRuns_ModelAccounts
    FOREIGN KEY (PerformedByAccountId) REFERENCES ModelAccounts (Id)
) ENGINE=InnoDB;

CREATE TABLE ChecklistItemResults (
  Id                  CHAR(36)      NOT NULL PRIMARY KEY,
  ChecklistRunId      CHAR(36)      NOT NULL,
  TemplateItemId      CHAR(36)      NOT NULL,
  Status              VARCHAR(20)   NOT NULL,
  Observation         VARCHAR(1000) NULL,
  CreatedAt           DATETIME(6)   NOT NULL,
  CreatedByAccountId  CHAR(36)      NULL,
  UpdatedAt           DATETIME(6)   NULL,
  UpdatedByAccountId  CHAR(36)      NULL,
  CONSTRAINT FK_ChecklistItemResults_ChecklistRuns
    FOREIGN KEY (ChecklistRunId) REFERENCES ChecklistRuns (Id) ON DELETE CASCADE,
  CONSTRAINT FK_ChecklistItemResults_ChecklistTemplateItems
    FOREIGN KEY (TemplateItemId) REFERENCES ChecklistTemplateItems (Id)
) ENGINE=InnoDB;

CREATE TABLE MaintenanceRequests (
  Id                    CHAR(36)      NOT NULL PRIMARY KEY,
  ChecklistRunId        CHAR(36)      NOT NULL,
  ChecklistItemResultId CHAR(36)      NULL,
  RoomId                CHAR(36)      NOT NULL,
  Description           VARCHAR(1000) NOT NULL,
  Status                VARCHAR(20)   NOT NULL,
  ResolvedAt            DATETIME(6)   NULL,
  CreatedAt             DATETIME(6)   NOT NULL,
  CreatedByAccountId    CHAR(36)      NULL,
  UpdatedAt             DATETIME(6)   NULL,
  UpdatedByAccountId    CHAR(36)      NULL,
  CONSTRAINT FK_MaintenanceRequests_ChecklistRuns
    FOREIGN KEY (ChecklistRunId) REFERENCES ChecklistRuns (Id) ON DELETE CASCADE,
  CONSTRAINT FK_MaintenanceRequests_ChecklistItemResults
    FOREIGN KEY (ChecklistItemResultId) REFERENCES ChecklistItemResults (Id) ON DELETE SET NULL,
  CONSTRAINT FK_MaintenanceRequests_Rooms
    FOREIGN KEY (RoomId) REFERENCES Rooms (Id)
) ENGINE=InnoDB;

-- ---------------------------------------------------------------------
-- Transversal: Auditoria (Z)
-- ---------------------------------------------------------------------
CREATE TABLE AuditLogs (
  Id            CHAR(36)      NOT NULL PRIMARY KEY,
  AccountId     CHAR(36)      NULL,
  Module        VARCHAR(60)   NOT NULL,
  Action        VARCHAR(100)  NOT NULL,
  EntityName    VARCHAR(100)  NULL,
  EntityId      CHAR(36)      NULL,
  DetailsJson   TEXT          NULL,
  CreatedAt     DATETIME(6)   NOT NULL,
  KEY IX_AuditLogs_Module_CreatedAt (Module, CreatedAt),
  CONSTRAINT FK_AuditLogs_ModelAccounts
    FOREIGN KEY (AccountId) REFERENCES ModelAccounts (Id) ON DELETE SET NULL
) ENGINE=InnoDB;


CREATE TABLE IF NOT EXISTS __EFMigrationsHistory (
  MigrationId    VARCHAR(150) NOT NULL PRIMARY KEY,
  ProductVersion VARCHAR(32)  NOT NULL
) ENGINE=InnoDB;
