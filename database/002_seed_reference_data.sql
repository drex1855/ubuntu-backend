-- =====================================================================
-- Datos de referencia opcionales (catalogos vacios de ejemplo).
-- No incluye la cuenta Admin inicial a proposito: esa la crea la propia
-- aplicacion en el primer arranque (ver Infrastructure/Persistence/DataSeeder.cs
-- y la seccion "Seed" de appsettings.json), porque necesita pasar la
-- contraseña por el mismo hasher (PBKDF2) que usa el login -- un hash
-- puesto a mano aqui casi seguro no calzaria con el algoritmo real.
-- =====================================================================
USE webcam_studio;

INSERT INTO Sites (Id, Name, Description, IsActive, CreatedAt) VALUES
  (UUID(), 'Chaturbate', 'Plataforma de streaming', 1, UTC_TIMESTAMP(6)),
  (UUID(), 'Stripchat', 'Plataforma de streaming', 1, UTC_TIMESTAMP(6));

INSERT INTO Rooms (Id, Name, Description, IsActive, CreatedAt) VALUES
  (UUID(), 'Habitacion 1', 'Estudio principal', 1, UTC_TIMESTAMP(6)),
  (UUID(), 'Habitacion 2', 'Estudio secundario', 1, UTC_TIMESTAMP(6));

-- Elementos de checklist de ejemplo para "Habitacion 1" (ajusta el Id real
-- de la habitacion que se genero arriba antes de correr este INSERT, o
-- mejor: crealos desde la API con POST /api/checklists/rooms/{roomId}/items).
-- SET @room1 := (SELECT Id FROM Rooms WHERE Name = 'Habitacion 1' LIMIT 1);
-- INSERT INTO ChecklistTemplateItems (Id, RoomId, Name, Description, IsActive, DisplayOrder, CreatedAt) VALUES
--   (UUID(), @room1, 'Camara', 'Camara principal en buen estado', 1, 1, UTC_TIMESTAMP(6)),
--   (UUID(), @room1, 'Iluminacion', 'Luces de estudio funcionando', 1, 2, UTC_TIMESTAMP(6)),
--   (UUID(), @room1, 'Ropa de cama', 'Sabanas limpias y en buen estado', 1, 3, UTC_TIMESTAMP(6));
