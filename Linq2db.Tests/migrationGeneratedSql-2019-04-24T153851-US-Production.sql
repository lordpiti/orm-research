/* VersionMigration migrating ================================================ */

/* Beginning Transaction */
/* CreateTable VersionInfo */
CREATE TABLE "VersionInfo" ("Version" INTEGER NOT NULL)
/* Committing Transaction */
/* VersionMigration migrated */
/* VersionUniqueMigration migrating ========================================== */

/* Beginning Transaction */
/* CreateIndex VersionInfo (Version) */
CREATE UNIQUE INDEX "UC_Version" ON "VersionInfo" ("Version" ASC)
/* AlterTable VersionInfo */
/* CreateColumn VersionInfo AppliedOn DateTime */
ALTER TABLE "VersionInfo" ADD COLUMN "AppliedOn" DATETIME
/* Committing Transaction */
/* VersionUniqueMigration migrated */
/* VersionDescriptionMigration migrating ===================================== */

/* Beginning Transaction */
/* AlterTable VersionInfo */
/* CreateColumn VersionInfo Description String */
ALTER TABLE "VersionInfo" ADD COLUMN "Description" TEXT
/* Committing Transaction */
/* VersionDescriptionMigration migrated */
/* 1201904011401: InitialDatabaseCreation migrating ========================== */

/* Beginning Transaction */
/* CreateSchema test */
/* CreateTable Product */
CREATE TABLE "Product" ("Id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, "name" TEXT NOT NULL, "categoryId" INTEGER NOT NULL, "Unit_Price" NUMERIC NOT NULL)
INSERT INTO "VersionInfo" ("Version", "AppliedOn", "Description") VALUES (1201904011401, '2019-04-24T14:38:52', 'Create the initial database')
/* Committing Transaction */
/* 1201904011401: InitialDatabaseCreation migrated */
/* 1201904011402: CreatePersonTable migrating ================================ */

/* Beginning Transaction */
/* CreateTable Category */
CREATE TABLE "Category" ("id" INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, "name" TEXT NOT NULL)
/* CreateForeignKey fk_category_product Product(categoryId) Category(id) */
INSERT INTO "Category" ("name") VALUES ('Category 1')
INSERT INTO "Category" ("name") VALUES ('Category 2')
INSERT INTO "Category" ("name") VALUES ('Category 3')
INSERT INTO "Product" ("name", "categoryId", "Unit_Price") VALUES ('Product 1', 1, 10)
INSERT INTO "Product" ("name", "categoryId", "Unit_Price") VALUES ('Product 2', 2, 20)
INSERT INTO "Product" ("name", "categoryId", "Unit_Price") VALUES ('Product 3', 3, 30)
INSERT INTO "Product" ("name", "categoryId", "Unit_Price") VALUES ('Product 4', 1, 40)
INSERT INTO "Product" ("name", "categoryId", "Unit_Price") VALUES ('Product 5', 1, 50)
/* -> 8 Insert operations completed in 00:00:00.0187066 taking an average of 00:00:00.0023383 */
INSERT INTO "VersionInfo" ("Version", "AppliedOn", "Description") VALUES (1201904011402, '2019-04-24T14:38:52', 'Create the person table')
/* Committing Transaction */
/* 1201904011402: CreatePersonTable migrated */