CREATE TABLE Roles (
    idRol INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Usuarios (
    idUsuario INT IDENTITY(1,1) PRIMARY KEY,
    idRol INT NOT NULL,
    nombre NVARCHAR(100) NOT NULL,
    apellido NVARCHAR(100) NOT NULL,
    correo VARCHAR(150) NOT NULL UNIQUE,
    contraseña VARCHAR(255) NOT NULL,
    estado BIT DEFAULT 1,
    fechaRegistro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Usuarios_Roles FOREIGN KEY (idRol) REFERENCES Roles(idRol)
);
GO

CREATE TABLE AuditoriaCatálogo (
    idAuditoria INT IDENTITY(1,1) PRIMARY KEY,
    idPlatillo INT NOT NULL,
    idUsuarioModif INT NULL,
    accion VARCHAR(50) NOT NULL,
    detalleCambio VARCHAR(MAX) NOT NULL,
    fechaRegistro DATETIME DEFAULT GETDATE(),
    CONSTRAINT FK_Auditoria_Usuarios FOREIGN KEY (idUsuarioModif) REFERENCES Usuarios(idUsuario) ON DELETE SET NULL
);
GO

CREATE TABLE Categorias (
    idCategoria INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Platillos (
    idPlatillo INT IDENTITY(1,1) PRIMARY KEY,
    idCategoria INT NOT NULL,
    idUsuarioUltimaModif INT NULL,
    nombre VARCHAR(100) NOT NULL,
    descripcion VARCHAR(MAX),
    precio DECIMAL(10, 2) NOT NULL,
    imagenUrl VARCHAR(255),
    tiempoPreparacion INT,
    estado VARCHAR(20) DEFAULT 'Disponible' CHECK (Estado IN ('Disponible', 'Agotado', 'Desactivado')),
    CONSTRAINT FK_Platillos_Categorias FOREIGN KEY (idCategoria) REFERENCES Categorias(idCategoria),
    CONSTRAINT FK_Platillos_Usuarios FOREIGN KEY (idUsuarioUltimaModif) REFERENCES Usuarios(idUsuario)
);
GO

CREATE TABLE Ingredientes (
    idIngrediente INT IDENTITY(1,1) PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE
);
GO

CREATE TABLE PlatilloIngredientes (
    idPlatillo INT NOT NULL,
    idIngrediente INT NOT NULL,
    PRIMARY KEY (idPlatillo, idIngrediente),
    CONSTRAINT FK_PI_Platillos FOREIGN KEY (idPlatillo) REFERENCES Platillos(idPlatillo) ON DELETE CASCADE,
    CONSTRAINT FK_PI_Ingredientes FOREIGN KEY (idIngrediente) REFERENCES Ingredientes(idIngrediente)
);
GO

INSERT INTO Roles (nombre) VALUES 
('Admin'),
('Empleado');

INSERT INTO Categorias (nombre) VALUES 
('Entradas'),
('Platos Fuertes'),
('Bebidas'),
('Postres');
GO

CREATE VIEW vw_MenuPublico AS
SELECT 
    p.idPlatillo,
    p.nombre AS platillo,
    p.descripcion,
    p.precio,
    p.imagenUrl,
    p.tiempoPreparacion,
    p.estado,
    c.nombre AS categoria
FROM Platillos p
INNER JOIN Categorias c ON p.idCategoria = c.idCategoria
WHERE p.estado IN ('Disponible', 'Agotado');
GO

CREATE VIEW vw_ReporteAuditoria AS
SELECT 
    a.idAuditoria,
    a.idPlatillo,
    ISNULL(u.nombre, 'Usuario Eliminado/Sistema') AS usuario,
    r.nombre AS rol,
    a.accion,
    a.detalleCambio,
    a.fechaRegistro
FROM AuditoriaCatálogo a
LEFT JOIN Usuarios u ON a.idUsuarioModif = u.idUsuario
LEFT JOIN Roles r ON u.idRol = r.idRol;
GO

CREATE TRIGGER trg_AuditoriaPlatillos
ON Platillos
AFTER INSERT, UPDATE, DELETE
AS
BEGIN
    SET NOCOUNT ON;

    IF EXISTS (SELECT * FROM inserted) AND NOT EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO AuditoriaCatálogo (idPlatillo, idUsuarioModif, accion, detalleCambio)
        SELECT 
            i.idPlatillo,
            i.idUsuarioUltimaModif,
            'CREACIÓN',
            CONCAT('Platillo creado: ', i.nombre, ' | Precio: $', i.precio, ' | Estado: ', i.estado)
        FROM inserted i;
    END

    IF EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO AuditoriaCatálogo (idPlatillo, idUsuarioModif, accion, detalleCambio)
        SELECT 
            i.idPlatillo,
            i.idUsuarioUltimaModif,
            'MODIFICACIÓN',
            CONCAT('Cambio en ', i.nombre, 
                   ' | Precio ant: $', d.precio, ' -> nuev: $', i.precio, 
                   ' | Estado ant: ', d.estado, ' -> nuev: ', i.estado)
        FROM inserted i
        INNER JOIN deleted d ON i.idPlatillo = d.idPlatillo;
    END

    IF NOT EXISTS (SELECT * FROM inserted) AND EXISTS (SELECT * FROM deleted)
    BEGIN
        INSERT INTO AuditoriaCatálogo (idPlatillo, idUsuarioModif, accion, detalleCambio)
        SELECT 
            d.idPlatillo,
            d.idUsuarioUltimaModif,
            'ELIMINACIÓN',
            CONCAT('Platillo eliminado del catálogo: ', d.nombre)
        FROM deleted d;
    END
END;
GO

CREATE PROCEDURE sp_GuardarPlatillo
    @IdPlatillo INT = 0,
    @IdCategoria INT,
    @IdUsuarioModif INT,
    @Nombre VARCHAR(100),
    @Descripcion VARCHAR(MAX),
    @Precio DECIMAL(10,2),
    @ImagenUrl VARCHAR(255),
    @TiempoPreparacion INT,
    @Estado VARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    IF @IdPlatillo = 0
    BEGIN
        INSERT INTO Platillos (idCategoria, idUsuarioUltimaModif, nombre, descripcion, precio, imagenUrl, tiempoPreparacion, estado)
        VALUES (@IdCategoria, @IdUsuarioModif, @Nombre, @Descripcion, @Precio, @ImagenUrl, @TiempoPreparacion, @Estado);
    END
    ELSE
    BEGIN
        UPDATE Platillos
        SET idCategoria = @IdCategoria,
            idUsuarioUltimaModif = @IdUsuarioModif,
            nombre = @Nombre,
            descripcion = @Descripcion,
            precio = @Precio,
            imagenUrl = @ImagenUrl,
            tiempoPreparacion = @TiempoPreparacion,
            estado = @Estado
        WHERE idPlatillo = @IdPlatillo;
    END
END;
GO

CREATE PROCEDURE sp_CambiarEstadoPlatillo
    @IdPlatillo INT,
    @NuevoEstado VARCHAR(20),
    @IdUsuarioModif INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE Platillos
    SET estado = @NuevoEstado,
        idUsuarioUltimaModif = @IdUsuarioModif
    WHERE idPlatillo = @IdPlatillo;
END;
GO