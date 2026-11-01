/* =============================================================================
   Accesibilidad por Area / Subarea (20261101)

   La accesibilidad llegaba hasta CBU -> Region -> Sitio (dbo.AccessibilityNetUsers,
   una fila por usuario y sitio). Se agrega una restriccion por Area/Subarea que
   aplica al USUARIO en todos sus sitios (no por sitio), para que el filtro viaje
   como dos arreglos mas (AreaList / SubareaList) junto a los de CBU/Region/Sitio.

   Semantica de dbo.AccessibilityNetUsersArea:
     - Sin filas para el usuario  -> sin restriccion: ve todas las areas.
     - (Area, NULL)               -> toda el Area, con todas sus subareas.
     - (Area, Subarea)            -> solo esa Subarea.

   Es una tabla aparte (no columnas en AccessibilityNetUsers) para no multiplicar
   las filas por sitio: varios SPs hacen JOIN directo contra esa tabla.

   Re-ejecutable. Orden: tabla -> tipo -> funcion -> SPs.
   ========================================================================== */

IF OBJECT_ID('dbo.AccessibilityNetUsersArea', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.AccessibilityNetUsersArea (
        UserId  nvarchar(128)    NOT NULL,
        Area    uniqueidentifier NOT NULL,
        Subarea uniqueidentifier NULL
    );

    CREATE UNIQUE CLUSTERED INDEX UX_AccessibilityNetUsersArea
        ON dbo.AccessibilityNetUsersArea (UserId, Area, Subarea);
END
GO

-- Igual que FK_AccessibilityNetUsers_AspNetUsers: al borrar el usuario se va su restriccion.
IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_AccessibilityNetUsersArea_AspNetUsers')
    ALTER TABLE dbo.AccessibilityNetUsersArea
        ADD CONSTRAINT FK_AccessibilityNetUsersArea_AspNetUsers
        FOREIGN KEY (UserId) REFERENCES dbo.AspNetUsers (Id) ON DELETE CASCADE;
GO

IF TYPE_ID('dbo.AccessibilityAreaType') IS NULL
    CREATE TYPE dbo.AccessibilityAreaType AS TABLE (
        UserId  nvarchar(128)    NOT NULL,
        Area    uniqueidentifier NOT NULL,
        Subarea uniqueidentifier NULL
    );
GO

/* Areas/Subareas activas que el usuario puede ver, ya expandidas.
   IsRestricted = 0 cuando el usuario no tiene filas: devuelve todo el catalogo
   activo. WholeArea = 1 cuando ve el Area completa (sin restriccion, o con una
   fila (Area, NULL)): el header no necesita filtrar por Subarea en esa Area.
   La usan el login (para el header) y, despues, los SPs que validen del lado
   del servidor. */
CREATE OR ALTER FUNCTION dbo.fn_UserAllowedAreas (@UserId nvarchar(128))
RETURNS TABLE
AS
RETURN
SELECT a.Identifier       AS AreaId,
       a.DirectionName    AS AreaName,
       s.Identifier       AS SubareaId,
       s.SubDirectionName AS SubareaName,
       CAST(r.HasRows AS bit) AS IsRestricted,
       CAST(CASE WHEN r.HasRows = 0
                   OR EXISTS (SELECT 1 FROM dbo.AccessibilityNetUsersArea w
                              WHERE w.UserId = @UserId AND w.Area = a.Identifier AND w.Subarea IS NULL)
                 THEN 1 ELSE 0 END AS bit) AS WholeArea
FROM   Catalogs.Area a
LEFT JOIN Catalogs.Subarea s ON s.Area = a.Identifier AND s.Status = 1
CROSS APPLY (SELECT CASE WHEN EXISTS (SELECT 1 FROM dbo.AccessibilityNetUsersArea x
                                      WHERE x.UserId = @UserId)
                         THEN 1 ELSE 0 END AS HasRows) r
WHERE  a.Status = 1
  AND  (r.HasRows = 0
        OR EXISTS (SELECT 1
                   FROM   dbo.AccessibilityNetUsersArea u
                   WHERE  u.UserId = @UserId
                     AND  u.Area   = a.Identifier
                     AND  (u.Subarea IS NULL OR u.Subarea = s.Identifier)));
GO

/* ¿El usuario puede atender un registro de esta Area/Subarea? Regla UNICA para
   los SPs que resuelven aprobadores, candidatos y destinatarios: se agrega junto
   al JOIN con dbo.AccessibilityNetUsers, como un filtro mas de la accesibilidad.
     - Usuario sin filas en AccessibilityNetUsersArea -> sin restriccion: si.
     - Registro sin Area (solicitud sin unidad, carga masiva) -> si: no hay con
       que comparar, y negarlo dejaria la solicitud sin quien la atienda.
     - (Area, NULL) = toda el Area; (Area, Subarea) = solo esa Subarea.
   Uso: AND EXISTS (SELECT 1 FROM dbo.fn_UserAreaAllows(anu.UserId, ci.Area, ci.Subarea)) */
CREATE OR ALTER FUNCTION dbo.fn_UserAreaAllows (@UserId nvarchar(128), @AreaId uniqueidentifier, @SubareaId uniqueidentifier)
RETURNS TABLE
AS
RETURN
SELECT Allowed = CAST(1 AS bit)
WHERE  @AreaId IS NULL
   OR  NOT EXISTS (SELECT 1 FROM dbo.AccessibilityNetUsersArea x WHERE x.UserId = @UserId)
   OR  EXISTS (SELECT 1
               FROM   dbo.AccessibilityNetUsersArea x
               WHERE  x.UserId = @UserId
                 AND  x.Area   = @AreaId
                 AND  (x.Subarea IS NULL OR x.Subarea = @SubareaId));
GO

/* Catalogo activo Area -> Subarea, para las opciones de la pantalla de usuarios. */
CREATE OR ALTER PROCEDURE dbo.Usp_Security_AccessibilityAreaCatalog_GET
AS
BEGIN
    SET NOCOUNT ON;

    SELECT a.Identifier       AS AreaId,
           a.DirectionName    AS AreaName,
           s.Identifier       AS SubareaId,
           s.SubDirectionName AS SubareaName
    FROM   Catalogs.Area a
    LEFT JOIN Catalogs.Subarea s ON s.Area = a.Identifier AND s.Status = 1
    WHERE  a.Status = 1
    ORDER BY a.DirectionName, s.SubDirectionName;
END
GO

/* @Expanded = 0: lo que se le asigno al usuario (pantalla de edicion);
                  SubareaId NULL significa toda el Area.
   @Expanded = 1: lo que el usuario puede ver, expandido (login / header). */
CREATE OR ALTER PROCEDURE dbo.Usp_Security_AccessibilityArea_GETL
    @UserId   nvarchar(128),
    @Expanded bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    IF @Expanded = 1
    BEGIN
        SELECT AreaId, AreaName, SubareaId, SubareaName, IsRestricted, WholeArea
        FROM   dbo.fn_UserAllowedAreas(@UserId)
        ORDER BY AreaName, SubareaName;
        RETURN;
    END

    SELECT u.Area             AS AreaId,
           a.DirectionName    AS AreaName,
           u.Subarea          AS SubareaId,
           s.SubDirectionName AS SubareaName,
           CAST(1 AS bit)     AS IsRestricted
    FROM   dbo.AccessibilityNetUsersArea u
    JOIN   Catalogs.Area a ON a.Identifier = u.Area
    LEFT JOIN Catalogs.Subarea s ON s.Identifier = u.Subarea
    WHERE  u.UserId = @UserId
    ORDER BY a.DirectionName, s.SubDirectionName;
END
GO

/* Alta de usuario: sitios + restriccion por Area. @Areas vacio = sin restriccion.
   Un llamador que no manda @Areas sigue funcionando (un TVP omitido llega vacio). */
CREATE OR ALTER PROCEDURE [dbo].[Usp_Security_AccessibilityNetUsers_INS]
    @Accesibility AS [dbo].[AccessibilityRegionCBUNetUsers] READONLY,
    @Areas        AS [dbo].[AccessibilityAreaType] READONLY
AS
BEGIN
    SET NOCOUNT ON;

    BEGIN TRY
        BEGIN TRANSACTION;

        INSERT INTO [dbo].[AccessibilityNetUsers] ([UserId],[SiteByRegion],[Site],[Region],[CBU])
        SELECT [UserId],[SiteByRegion],[Site],[Region],[CBU] FROM @Accesibility;

        INSERT INTO dbo.AccessibilityNetUsersArea (UserId, Area, Subarea)
        SELECT DISTINCT UserId, Area, Subarea FROM @Areas;

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

/* Edicion de accesibilidad. @ReplaceAreas distingue "no me mandaron areas"
   (llamador viejo: no se tocan) de "me mandaron areas vacias" (quitar la
   restriccion). Se reemplazan en la misma transaccion que los sitios. */
CREATE OR ALTER PROCEDURE [dbo].[Usp_Security_AccessibilityNetUsers_Upd]
    @Accesibility AS [dbo].[AccessibilityRegionCBUNetUsers] READONLY,
    @Areas        AS [dbo].[AccessibilityAreaType] READONLY,
    @ReplaceAreas bit = 0
AS
BEGIN
    SET NOCOUNT ON;

    -- Guard: si el TVP viene vacio, NO borrar nada (evita perder la accesibilidad
    -- del usuario por un guardado incompleto/vacio del front).
    IF NOT EXISTS (SELECT 1 FROM @Accesibility)
        RETURN;

    BEGIN TRY
        BEGIN TRANSACTION;
        -- Reemplaza SOLO la accesibilidad de los usuarios del TVP. IN (no '=') por
        -- si llega mas de un UserId; DELETE+INSERT atomicos.
        DELETE FROM dbo.AccessibilityNetUsers
        WHERE UserId IN (SELECT DISTINCT UserId FROM @Accesibility);

        INSERT INTO dbo.AccessibilityNetUsers ([UserId],[SiteByRegion],[Site],[Region],[CBU])
        SELECT [UserId],[SiteByRegion],[Site],[Region],[CBU] FROM @Accesibility;

        IF @ReplaceAreas = 1
        BEGIN
            DELETE FROM dbo.AccessibilityNetUsersArea
            WHERE UserId IN (SELECT DISTINCT UserId FROM @Accesibility);

            INSERT INTO dbo.AccessibilityNetUsersArea (UserId, Area, Subarea)
            SELECT DISTINCT ar.UserId, ar.Area, ar.Subarea
            FROM   @Areas ar
            WHERE  ar.UserId IN (SELECT UserId FROM @Accesibility);
        END

        COMMIT TRANSACTION;
    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO
