-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 10-10-2025 a las 20:52:03
-- Versión del servidor: 10.4.32-MariaDB
-- Versión de PHP: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Base de datos: `inmobiliaria_db`
--

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `auditoria`
--

CREATE TABLE `auditoria` (
  `Id` int(11) NOT NULL,
  `Tabla` varchar(50) NOT NULL,
  `Operacion` varchar(20) NOT NULL,
  `RegistroId` int(11) NOT NULL,
  `Fecha` datetime NOT NULL DEFAULT current_timestamp(),
  `DatosAnteriores` text DEFAULT NULL,
  `DatosNuevos` text DEFAULT NULL,
  `Usuario` varchar(100) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `auditoria`
--

INSERT INTO `auditoria` (`Id`, `Tabla`, `Operacion`, `RegistroId`, `Fecha`, `DatosAnteriores`, `DatosNuevos`, `Usuario`) VALUES
(1, 'Contrato', 'Alta', 2, '2025-10-09 14:41:43', NULL, '{\"Id\":2,\"FechaInicio\":\"2025-10-01T00:00:00\",\"FechaFin\":\"2025-10-31T00:00:00\",\"Monto\":200000,\"InquilinoId\":2,\"InmuebleId\":5,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(2, 'Pago', 'Alta', 4, '2025-10-09 15:33:07', NULL, '{\"Id\":4,\"Numero\":1,\"Fecha\":\"2025-10-09T00:00:00\",\"Importe\":200000,\"Detalle\":\"Pago del mes de Octubre\",\"Anulado\":false,\"ContratoId\":1}', 'admin@inmobiliaria.com'),
(3, 'Pago', 'Anulación', 4, '2025-10-09 15:34:29', '{\"Id\":4,\"Numero\":1,\"Fecha\":\"2025-10-09T00:00:00\",\"Importe\":200000.00,\"Detalle\":\"Pago del mes de Octubre\",\"Anulado\":false}', '{\"Id\":4,\"Numero\":1,\"Fecha\":\"2025-10-09T00:00:00\",\"Importe\":200000.00,\"Detalle\":\"Pago del mes de Octubre\",\"Anulado\":true}', 'admin@inmobiliaria.com'),
(4, 'Contrato', 'Terminación', 2, '2025-10-09 18:15:38', '{\"Id\":2,\"FechaInicio\":\"2025-10-01T00:00:00\",\"FechaFin\":\"2025-10-31T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":2,\"InmuebleId\":5,\"Vigente\":true,\"Multa\":null,\"FechaFinAnticipada\":null}', '{\"Id\":2,\"FechaInicio\":\"2025-10-01T00:00:00\",\"FechaFin\":\"2025-10-31T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":2,\"InmuebleId\":5,\"Vigente\":false,\"Multa\":3000000,\"FechaFinAnticipada\":\"2025-10-09T00:00:00\"}', 'admin@inmobiliaria.com'),
(5, 'Contrato', 'Alta', 3, '2025-10-09 18:19:01', NULL, '{\"Id\":3,\"FechaInicio\":\"2025-10-07T00:00:00\",\"FechaFin\":\"2025-10-08T00:00:00\",\"Monto\":200000,\"InquilinoId\":1,\"InmuebleId\":3,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(6, 'Contrato', 'Terminación', 3, '2025-10-09 18:19:27', '{\"Id\":3,\"FechaInicio\":\"2025-10-07T00:00:00\",\"FechaFin\":\"2025-10-08T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":1,\"InmuebleId\":3,\"Vigente\":true,\"Multa\":null,\"FechaFinAnticipada\":null}', '{\"Id\":3,\"FechaInicio\":\"2025-10-07T00:00:00\",\"FechaFin\":\"2025-10-08T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":1,\"InmuebleId\":3,\"Vigente\":false,\"Multa\":0,\"FechaFinAnticipada\":\"2025-10-08T00:00:00\"}', 'admin@inmobiliaria.com'),
(7, 'Pago', 'Alta', 5, '2025-10-09 18:21:08', NULL, '{\"Id\":5,\"Numero\":1,\"Fecha\":\"2025-10-09T00:00:00\",\"Importe\":200000,\"Detalle\":\"Pago Unico\",\"Anulado\":false,\"ContratoId\":3}', 'admin@inmobiliaria.com'),
(8, 'Contrato', 'Alta', 4, '2025-10-09 18:45:40', NULL, '{\"Id\":4,\"FechaInicio\":\"2025-10-09T00:00:00\",\"FechaFin\":\"2025-10-31T00:00:00\",\"Monto\":200000,\"InquilinoId\":2,\"InmuebleId\":3,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(9, 'Contrato', 'Alta', 5, '2025-10-09 19:02:14', NULL, '{\"Id\":5,\"FechaInicio\":\"2025-10-09T00:00:00\",\"FechaFin\":\"2025-10-31T00:00:00\",\"Monto\":200000,\"InquilinoId\":2,\"InmuebleId\":3,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(10, 'Contrato', 'Alta', 6, '2025-10-09 19:05:36', NULL, '{\"Id\":6,\"FechaInicio\":\"2025-11-01T00:00:00\",\"FechaFin\":\"2026-11-01T00:00:00\",\"Monto\":200000,\"InquilinoId\":2,\"InmuebleId\":5,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(11, 'Contrato', 'Alta', 7, '2025-10-09 19:10:58', NULL, '{\"Id\":7,\"FechaInicio\":\"2025-10-09T00:00:00\",\"FechaFin\":\"2025-10-10T00:00:00\",\"Monto\":200000,\"InquilinoId\":1,\"InmuebleId\":6,\"Vigente\":true}', 'admin@inmobiliaria.com'),
(12, 'Contrato', 'Terminación', 7, '2025-10-09 19:11:29', '{\"Id\":7,\"FechaInicio\":\"2025-10-09T00:00:00\",\"FechaFin\":\"2025-10-10T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":1,\"InmuebleId\":6,\"Vigente\":true,\"Multa\":null,\"FechaFinAnticipada\":null}', '{\"Id\":7,\"FechaInicio\":\"2025-10-09T00:00:00\",\"FechaFin\":\"2025-10-10T00:00:00\",\"Monto\":200000.00,\"InquilinoId\":1,\"InmuebleId\":6,\"Vigente\":false,\"Multa\":3000000,\"FechaFinAnticipada\":\"2025-10-09T00:00:00\"}', 'admin@inmobiliaria.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `Id` int(11) NOT NULL,
  `FechaInicio` date NOT NULL,
  `FechaFin` date NOT NULL,
  `Monto` decimal(10,2) NOT NULL,
  `FechaFinAnticipada` date DEFAULT NULL,
  `Multa` decimal(10,2) DEFAULT NULL,
  `Vigente` tinyint(1) NOT NULL DEFAULT 1,
  `InquilinoId` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`Id`, `FechaInicio`, `FechaFin`, `Monto`, `FechaFinAnticipada`, `Multa`, `Vigente`, `InquilinoId`, `InmuebleId`) VALUES
(1, '2025-09-23', '2025-10-23', 200000.00, NULL, NULL, 1, 1, 1),
(2, '2025-10-01', '2025-10-31', 200000.00, NULL, NULL, 0, 2, 5),
(3, '2025-10-07', '2025-10-08', 200000.00, NULL, NULL, 0, 1, 3),
(4, '2025-10-09', '2025-10-31', 200000.00, NULL, NULL, 1, 2, 3),
(6, '2025-11-01', '2026-11-01', 200000.00, NULL, NULL, 1, 2, 5),
(7, '2025-10-09', '2025-10-10', 200000.00, NULL, NULL, 0, 1, 6);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `imagen`
--

CREATE TABLE `imagen` (
  `Id` int(11) NOT NULL,
  `InmuebleId` int(11) NOT NULL,
  `Url` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `imagen`
--

INSERT INTO `imagen` (`Id`, `InmuebleId`, `Url`) VALUES
(2, 1, '/uploads/inmuebles/1/c91e4cea-9d2f-440c-b8c6-bad01002c29b.jpg'),
(3, 1, '/uploads/inmuebles/1/ef2465eb-a68f-4430-8aed-1c4156af9b56.jpg'),
(6, 3, '/uploads/inmuebles/3/32e78c31-4384-4f48-9743-7ec1e3d72ccc.jpg');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `Id` int(11) NOT NULL,
  `Direccion` varchar(255) NOT NULL,
  `Uso` varchar(50) NOT NULL,
  `Tipo` int(11) NOT NULL,
  `Ambientes` int(11) NOT NULL,
  `Latitud` decimal(10,8) DEFAULT NULL,
  `Longitud` decimal(11,8) DEFAULT NULL,
  `Precio` decimal(10,2) NOT NULL,
  `Disponible` tinyint(1) NOT NULL DEFAULT 1,
  `PropietarioId` int(11) NOT NULL,
  `Habilitado` tinyint(1) NOT NULL DEFAULT 1,
  `Portada` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`Id`, `Direccion`, `Uso`, `Tipo`, `Ambientes`, `Latitud`, `Longitud`, `Precio`, `Disponible`, `PropietarioId`, `Habilitado`, `Portada`) VALUES
(1, 'Barrio 544 Viviendas, Manzana P,  Casa 32', 'Residencial', 1, 7, -33.00000000, -66.00000000, 1300000.00, 0, 4, 1, '/Uploads/Inmuebles/portada_1.jpg'),
(3, 'Barrio 544 Viviendas, Manzana P,  Casa 31', 'Residencial', 1, 6, -34.00000000, -66.00000000, 2000000.00, 0, 1, 1, '/Uploads/Inmuebles/portada_3.jpg'),
(5, 'Rivadavia 1100, San Luis', 'Residencial', 2, 4, -48.00000000, -125.00000000, 2000000.00, 0, 4, 1, NULL),
(6, 'Cordoba 1138, Jesus Maria', 'Residencial', 2, 3, -99.99999999, -133.00000000, 10000000.00, 1, 7, 1, NULL),
(7, 'Cordoba 1135, Jesus Maria', 'Comercial', 3, 1, -99.99999999, -131.00000000, 50000000.00, 1, 7, 1, NULL),
(8, 'Cordoba 1100, Jesus Maria', 'Comercial', 3, 1, -99.99999999, -131.00000000, 50000000.00, 1, 5, 1, NULL),
(9, 'Rivadavia 120, San Luis', 'Residencial', 2, 4, -99.99999999, -156.00000000, 5000000.00, 1, 5, 1, NULL),
(10, 'Barrio 544 Viviendas, Manzana P,  Casa 29', 'Residencial', 1, 6, -99.99999999, -133.00000000, 50000000.00, 1, 4, 1, NULL),
(11, 'Manuel Astrada 10, Merlo', 'Residencial', 2, 3, 99.99999999, -100.00000000, 10000000.00, 1, 5, 1, NULL),
(12, 'Barrio 544 Viviendas, Manzana P,  Casa 10', 'Residencial', 1, 11, 99.99999999, 200.00000000, 50000000.00, 1, 4, 1, NULL),
(13, 'Cordoba 1, Jesus Maria', 'Comercial', 1, 4, -99.99999999, -156.00000000, 5000000.00, 1, 7, 1, NULL),
(14, 'Maipu 110, San Luis', 'Residencial', 1, 5, -99.99999999, 200.00000000, 5000000.00, 1, 1, 1, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`Id`, `Nombre`, `Apellido`, `DNI`, `Telefono`, `Email`) VALUES
(1, 'Santiago', 'Becerra', '46072720', '26640164897', 'santi_bece_04@gmail.com'),
(2, 'Luca', 'Alonso', '34343434', '2664334455', 'luca@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `Id` int(11) NOT NULL,
  `Contrato_Id` int(11) NOT NULL,
  `FechaPago` date NOT NULL,
  `Importe` decimal(10,2) NOT NULL,
  `NumeroPago` int(11) NOT NULL,
  `Detalle` varchar(100) NOT NULL DEFAULT '',
  `Anulado` tinyint(1) NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`Id`, `Contrato_Id`, `FechaPago`, `Importe`, `NumeroPago`, `Detalle`, `Anulado`) VALUES
(4, 1, '2025-10-09', 200000.00, 1, 'Pago del mes de Octubre', 1),
(5, 3, '2025-10-09', 200000.00, 1, 'Pago Unico', 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `DNI` varchar(20) NOT NULL,
  `Telefono` varchar(20) DEFAULT NULL,
  `Email` varchar(100) NOT NULL,
  `Clave` varchar(255) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`Id`, `Nombre`, `Apellido`, `DNI`, `Telefono`, `Email`, `Clave`) VALUES
(1, 'Juan', 'Perez', '12345678', '123-456-7890', 'juan.perez@example.com', 'password123'),
(4, 'Martin', 'Becerra', '47266622', '2664304069', 'martinbecerrasl7@gmail.com', '47266622'),
(5, 'Carolina', 'Becerra', '38731849', '2664334456', 'carobecerra@gmail.com', 'zxcvbnm12'),
(7, 'Mariano', 'Luzza', '30120120', '2664101010', 'mariano@gmail.com', '12345678');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `Id` int(11) NOT NULL,
  `Nombre` varchar(100) NOT NULL,
  `Apellido` varchar(100) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Clave` varchar(255) NOT NULL,
  `Rol` int(11) NOT NULL,
  `Avatar` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`Id`, `Nombre`, `Apellido`, `Email`, `Clave`, `Rol`, `Avatar`) VALUES
(1, 'Admin', 'Principal', 'admin@inmobiliaria.com', '/zEGpjs2H5OIO+fg0i1ExnZtou/q2sDrMyZCkjelCFU=', 1, '/Uploads/avatar_1.jpeg'),
(2, 'Martin', 'Becerra', 'martinbecerrasl7@gmail.com', '/zEGpjs2H5OIO+fg0i1ExnZtou/q2sDrMyZCkjelCFU=', 1, '/Uploads/avatar_2.jpg'),
(4, 'Carolina', 'Luzza', 'caroluzza@gmail.com', 'BUQ/D7x0miEOtqY8hDcD1uFHeW+GvdaNwlWuSCoHi6s=', 2, NULL);

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  ADD PRIMARY KEY (`Id`);

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InquilinoId` (`InquilinoId`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `InmuebleId` (`InmuebleId`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `PropietarioId` (`PropietarioId`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`DNI`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`Id`),
  ADD KEY `fk_pago_contrato` (`Contrato_Id`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `DNI` (`DNI`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`Id`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=13;

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `imagen`
--
ALTER TABLE `imagen`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=11;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=15;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `Id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`InquilinoId`) REFERENCES `inquilino` (`Id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`Id`);

--
-- Filtros para la tabla `imagen`
--
ALTER TABLE `imagen`
  ADD CONSTRAINT `imagen_ibfk_1` FOREIGN KEY (`InmuebleId`) REFERENCES `inmueble` (`Id`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`PropietarioId`) REFERENCES `propietario` (`Id`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `fk_pago_contrato` FOREIGN KEY (`Contrato_Id`) REFERENCES `contrato` (`Id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
