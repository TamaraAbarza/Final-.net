-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Servidor: 127.0.0.1
-- Tiempo de generación: 03-07-2025 a las 21:06:36
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
  `id` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `idEntidad` int(11) NOT NULL,
  `entidad` varchar(100) DEFAULT NULL,
  `fechaAccion` datetime NOT NULL,
  `accion` tinyint(1) NOT NULL,
  `descripcion` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `auditoria`
--

INSERT INTO `auditoria` (`id`, `idUsuario`, `idEntidad`, `entidad`, `fechaAccion`, `accion`, `descripcion`) VALUES
(18, 1, 15, 'Contrato', '2025-07-03 00:00:00', 1, 'Creación de contrato'),
(19, 1, 4, 'Pago', '2025-07-03 00:00:00', 1, 'Nuevo pago');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `contrato`
--

CREATE TABLE `contrato` (
  `id` int(11) NOT NULL,
  `idInquilino` int(11) NOT NULL,
  `idPropietario` int(11) NOT NULL,
  `idInmueble` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL,
  `fechaInicio` date NOT NULL,
  `fechaFin` date NOT NULL,
  `vigente` tinyint(1) NOT NULL,
  `montoMensual` double NOT NULL,
  `fechaTerminacion` date DEFAULT NULL,
  `multaTerminacion` double DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `contrato`
--

INSERT INTO `contrato` (`id`, `idInquilino`, `idPropietario`, `idInmueble`, `idUsuario`, `fechaInicio`, `fechaFin`, `vigente`, `montoMensual`, `fechaTerminacion`, `multaTerminacion`) VALUES
(15, 2, 7, 13, 1, '2025-07-01', '2025-10-28', 1, 500000, NULL, NULL);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inmueble`
--

CREATE TABLE `inmueble` (
  `id` int(11) NOT NULL,
  `direccion` varchar(255) NOT NULL,
  `ambientes` int(11) NOT NULL,
  `superficie` int(11) NOT NULL,
  `latitud` double NOT NULL,
  `longitud` double NOT NULL,
  `idPropietario` int(11) NOT NULL,
  `idTipo` int(11) NOT NULL,
  `uso` varchar(100) NOT NULL,
  `precio` double NOT NULL,
  `disponible` tinyint(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inmueble`
--

INSERT INTO `inmueble` (`id`, `direccion`, `ambientes`, `superficie`, `latitud`, `longitud`, `idPropietario`, `idTipo`, `uso`, `precio`, `disponible`) VALUES
(4, 'Av. Illia 250, San Luis', 3, 95, -33.2989, -66.3365, 2, 2, 'Residencial', 380000, 0),
(5, 'Pedernera 789, Villa Mercedes', 2, 70, -33.67, -65.46, 3, 2, 'Comercial', 420000, 1),
(6, 'Av. del Sol 1000, Merlo', 4, 160, -32.348, -64.99, 4, 1, 'Residencial', 350000, 1),
(8, 'Rivadavia 800, San Luis', 3, 85, -33.3005, -66.335, 2, 2, 'Residencial', 375000, 1),
(9, 'Fray Mamerto Esquiú 120, Villa Mercedes', 5, 200, -33.68, -65.45, 3, 1, 'Residencial', 400000, 1),
(10, '25 de Mayo 450, Merlo', 1, 55, -32.345, -64.995, 4, 4, 'Comercial', 210000, 1),
(11, 'Av. Sarmiento 150, San Luis', 1, 70, -33.2975, -66.336, 5, 5, 'Comercial', 280000, 1),
(13, 'calle 1234', 2, 500, -1221, 1212, 7, 1, 'Residencial', 500000, 0);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `inquilino`
--

CREATE TABLE `inquilino` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `telefono` varchar(30) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `dni` varchar(20) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `inquilino`
--

INSERT INTO `inquilino` (`id`, `nombre`, `apellido`, `telefono`, `email`, `dni`) VALUES
(2, 'Lucía', 'Sánchez', '1122334455', 'luciasanchez@example.com', '12345678'),
(3, 'Sofía', 'Pérez', '3344556677', 'sofiaperez@example.com', '34567890'),
(4, 'Alejandro', 'Gómez', '4455667788', 'alejandrogomez@example.com', '45678901'),
(5, 'Pablo', 'Rodríguez', '02664341144', 'pablo@correo.com', '12344321');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `pago`
--

CREATE TABLE `pago` (
  `id` int(11) NOT NULL,
  `numeroPago` int(11) NOT NULL,
  `fecha` date NOT NULL,
  `detalle` text DEFAULT NULL,
  `importe` double NOT NULL,
  `estado` tinyint(1) NOT NULL,
  `idContrato` int(11) NOT NULL,
  `idUsuario` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `pago`
--

INSERT INTO `pago` (`id`, `numeroPago`, `fecha`, `detalle`, `importe`, `estado`, `idContrato`, `idUsuario`) VALUES
(4, 1, '2025-07-03', 'enero', 500000, 1, 15, 1);

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `propietario`
--

CREATE TABLE `propietario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) NOT NULL,
  `apellido` varchar(100) NOT NULL,
  `dni` varchar(20) NOT NULL,
  `telefono` varchar(30) NOT NULL,
  `email` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `propietario`
--

INSERT INTO `propietario` (`id`, `nombre`, `apellido`, `dni`, `telefono`, `email`) VALUES
(2, 'Juan', 'López', '123456789', '5551234567', 'juan@example.com'),
(3, 'María', 'García', '87654321', '2664345579', 'maria@example.com'),
(4, 'Daniel', 'Rodríguez', '34567890', '3344556677', 'danielrodriguez@example.com'),
(5, 'Chester', 'Abarza', '65789650', '2665443787', 'chester@mail.com'),
(7, 'Propietario', 'Nuevo', '1234566', '1233445566', 'emailejemplo@gmail.com');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `tipo`
--

CREATE TABLE `tipo` (
  `id` int(11) NOT NULL,
  `tipoInmueble` varchar(100) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `tipo`
--

INSERT INTO `tipo` (`id`, `tipoInmueble`) VALUES
(1, 'Casa'),
(2, 'Departamento'),
(4, 'Oficina'),
(5, 'Local Comercial');

-- --------------------------------------------------------

--
-- Estructura de tabla para la tabla `usuario`
--

CREATE TABLE `usuario` (
  `id` int(11) NOT NULL,
  `nombre` varchar(100) DEFAULT NULL,
  `apellido` varchar(100) DEFAULT NULL,
  `email` varchar(100) DEFAULT NULL,
  `password` varchar(255) DEFAULT NULL,
  `rol` int(11) DEFAULT NULL,
  `avatarUrl` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Volcado de datos para la tabla `usuario`
--

INSERT INTO `usuario` (`id`, `nombre`, `apellido`, `email`, `password`, `rol`, `avatarUrl`) VALUES
(1, 'Tamara', 'Admin', 'administrador@gmail.com', 'DF4003F282A36710426874095B583ED7', 1, '/uploads/05193db9-0779-4892-8e84-0e9df73ca200_FGcsfOhXEAI23FO.jpg'),
(5, 'Frank', 'Grimes', 'empleado@gmail.com', 'DF4003F282A36710426874095B583ED7', 2, '/uploads/b7705e01-7746-4a7f-b8db-39fbd32ea80b_img1.jpg');

--
-- Índices para tablas volcadas
--

--
-- Indices de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idInquilino` (`idInquilino`),
  ADD KEY `idPropietario` (`idPropietario`),
  ADD KEY `idInmueble` (`idInmueble`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idPropietario` (`idPropietario`),
  ADD KEY `idTipo` (`idTipo`);

--
-- Indices de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `pago`
--
ALTER TABLE `pago`
  ADD PRIMARY KEY (`id`),
  ADD KEY `idContrato` (`idContrato`),
  ADD KEY `idUsuario` (`idUsuario`);

--
-- Indices de la tabla `propietario`
--
ALTER TABLE `propietario`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `tipo`
--
ALTER TABLE `tipo`
  ADD PRIMARY KEY (`id`);

--
-- Indices de la tabla `usuario`
--
ALTER TABLE `usuario`
  ADD PRIMARY KEY (`id`);

--
-- AUTO_INCREMENT de las tablas volcadas
--

--
-- AUTO_INCREMENT de la tabla `auditoria`
--
ALTER TABLE `auditoria`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=20;

--
-- AUTO_INCREMENT de la tabla `contrato`
--
ALTER TABLE `contrato`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=16;

--
-- AUTO_INCREMENT de la tabla `inmueble`
--
ALTER TABLE `inmueble`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=14;

--
-- AUTO_INCREMENT de la tabla `inquilino`
--
ALTER TABLE `inquilino`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- AUTO_INCREMENT de la tabla `pago`
--
ALTER TABLE `pago`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT de la tabla `propietario`
--
ALTER TABLE `propietario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `tipo`
--
ALTER TABLE `tipo`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=8;

--
-- AUTO_INCREMENT de la tabla `usuario`
--
ALTER TABLE `usuario`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=6;

--
-- Restricciones para tablas volcadas
--

--
-- Filtros para la tabla `auditoria`
--
ALTER TABLE `auditoria`
  ADD CONSTRAINT `auditoria_ibfk_1` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`id`);

--
-- Filtros para la tabla `contrato`
--
ALTER TABLE `contrato`
  ADD CONSTRAINT `contrato_ibfk_1` FOREIGN KEY (`idInquilino`) REFERENCES `inquilino` (`id`),
  ADD CONSTRAINT `contrato_ibfk_2` FOREIGN KEY (`idPropietario`) REFERENCES `propietario` (`id`),
  ADD CONSTRAINT `contrato_ibfk_3` FOREIGN KEY (`idInmueble`) REFERENCES `inmueble` (`id`),
  ADD CONSTRAINT `contrato_ibfk_4` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`id`);

--
-- Filtros para la tabla `inmueble`
--
ALTER TABLE `inmueble`
  ADD CONSTRAINT `inmueble_ibfk_1` FOREIGN KEY (`idPropietario`) REFERENCES `propietario` (`id`),
  ADD CONSTRAINT `inmueble_ibfk_2` FOREIGN KEY (`idTipo`) REFERENCES `tipo` (`id`);

--
-- Filtros para la tabla `pago`
--
ALTER TABLE `pago`
  ADD CONSTRAINT `pago_ibfk_1` FOREIGN KEY (`idContrato`) REFERENCES `contrato` (`id`),
  ADD CONSTRAINT `pago_ibfk_2` FOREIGN KEY (`idUsuario`) REFERENCES `usuario` (`id`);
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
