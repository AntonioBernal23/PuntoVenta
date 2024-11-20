-- --------------------------------------------------------
-- Host:                         192.168.100.9
-- Versión del servidor:         11.5.2-MariaDB - mariadb.org binary distribution
-- SO del servidor:              Win64
-- HeidiSQL Versión:             12.6.0.6765
-- --------------------------------------------------------

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET NAMES utf8 */;
/*!50503 SET NAMES utf8mb4 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- Volcando estructura de base de datos para puntoventa
CREATE DATABASE IF NOT EXISTS `puntoventa` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_general_ci */;
USE `puntoventa`;

-- Volcando estructura para tabla puntoventa.administradores
CREATE TABLE IF NOT EXISTS `administradores` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  `apellidos` varchar(30) DEFAULT NULL,
  `usuario` varchar(20) DEFAULT NULL,
  `contraseña` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Volcando datos para la tabla puntoventa.administradores: ~0 rows (aproximadamente)
INSERT INTO `administradores` (`id`, `nombre`, `apellidos`, `usuario`, `contraseña`) VALUES
	(1, 'Antonio', 'Bernal Martínez', 'tony', '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4');

-- Volcando estructura para tabla puntoventa.clientes
CREATE TABLE IF NOT EXISTS `clientes` (
  `ClienteID` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) DEFAULT NULL,
  `Apellidos` varchar(100) DEFAULT NULL,
  `Celular` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ClienteID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Volcando datos para la tabla puntoventa.clientes: ~3 rows (aproximadamente)
INSERT INTO `clientes` (`ClienteID`, `Nombre`, `Apellidos`, `Celular`) VALUES
	(13, 'Pedro', 'Pica Piedras', '2222222222'),
	(14, 'Flor', 'Perez', 'Chufulus'),
	(15, 'smb', 'jbsd', '7262');

-- Volcando estructura para tabla puntoventa.detalleventas
CREATE TABLE IF NOT EXISTS `detalleventas` (
  `DetalleVentaID` int(11) NOT NULL AUTO_INCREMENT,
  `VentaID` int(11) NOT NULL,
  `ClienteID` int(11) NOT NULL,
  `ProductoID` int(11) NOT NULL,
  `Cantidad` int(11) DEFAULT NULL,
  `Subtotal` decimal(11,2) DEFAULT NULL,
  PRIMARY KEY (`DetalleVentaID`),
  KEY `VentaID` (`VentaID`),
  KEY `ClienteID` (`ClienteID`),
  KEY `ProductoID` (`ProductoID`),
  CONSTRAINT `detalleVentas_ibfk_1` FOREIGN KEY (`VentaID`) REFERENCES `ventas` (`VentaID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `detalleVentas_ibfk_2` FOREIGN KEY (`ClienteID`) REFERENCES `clientes` (`ClienteID`) ON DELETE CASCADE ON UPDATE CASCADE,
  CONSTRAINT `detalleVentas_ibfk_3` FOREIGN KEY (`ProductoID`) REFERENCES `inventario` (`idProducto`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=21 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Volcando datos para la tabla puntoventa.detalleventas: ~3 rows (aproximadamente)
INSERT INTO `detalleventas` (`DetalleVentaID`, `VentaID`, `ClienteID`, `ProductoID`, `Cantidad`, `Subtotal`) VALUES
	(18, 13, 13, 6, 2, 100.00),
	(19, 14, 14, 4, 20, 800.00),
	(20, 15, 15, 4, 2, 80.00);

-- Volcando estructura para tabla puntoventa.empleados
CREATE TABLE IF NOT EXISTS `empleados` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(20) NOT NULL,
  `apellidos` varchar(30) DEFAULT NULL,
  `usuario` varchar(20) DEFAULT NULL,
  `contraseña` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Volcando datos para la tabla puntoventa.empleados: ~0 rows (aproximadamente)

-- Volcando estructura para tabla puntoventa.inventario
CREATE TABLE IF NOT EXISTS `inventario` (
  `idProducto` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(30) NOT NULL,
  `precio` decimal(11,2) NOT NULL,
  `cantidad` int(11) NOT NULL,
  `descripcion` varchar(50) NOT NULL,
  `proveedor` varchar(30) NOT NULL,
  PRIMARY KEY (`idProducto`),
  KEY `proveedor` (`proveedor`),
  CONSTRAINT `inventario_ibfk_1` FOREIGN KEY (`proveedor`) REFERENCES `proveedores` (`nombre`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=12 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla puntoventa.inventario: ~6 rows (aproximadamente)
INSERT INTO `inventario` (`idProducto`, `nombre`, `precio`, `cantidad`, `descripcion`, `proveedor`) VALUES
	(1, 'Omeprazol', 50.00, 18, 'Caja con 14 tabletas', 'Medicinas Garza'),
	(2, 'Paracetamol', 20.00, 20, 'Caja con 20 tabletas', 'Laboratorios López'),
	(3, 'Ibuprofeno', 38.00, 20, 'Caja con 10 tabletas', 'Medicinas Garza'),
	(4, 'Losartan', 40.00, 0, 'Caja con 30 tabletas', 'Farmacéutica Orozco'),
	(5, 'Aspirina', 48.00, 17, 'Caja con 40 tabletas', 'Medicamentos Nacionales'),
	(6, 'Ampicilina', 50.00, 18, 'Caja con 40 tabletas', 'Laboratorios ABC'),
	(11, 'Loratadina', 35.00, 20, 'Caja con 10 tabletas', 'Laboratorios Sánchez');

-- Volcando estructura para tabla puntoventa.proveedores
CREATE TABLE IF NOT EXISTS `proveedores` (
  `idProveedor` int(11) NOT NULL AUTO_INCREMENT,
  `nombre` varchar(100) NOT NULL,
  `direccion` varchar(100) NOT NULL,
  `codigoPostal` varchar(10) NOT NULL,
  `numeroTelefonico` varchar(15) NOT NULL,
  PRIMARY KEY (`idProveedor`),
  UNIQUE KEY `nombre_UNIQUE` (`nombre`)
) ENGINE=InnoDB AUTO_INCREMENT=31 DEFAULT CHARSET=latin1 COLLATE=latin1_swedish_ci;

-- Volcando datos para la tabla puntoventa.proveedores: ~10 rows (aproximadamente)
INSERT INTO `proveedores` (`idProveedor`, `nombre`, `direccion`, `codigoPostal`, `numeroTelefonico`) VALUES
	(21, 'Laboratorios ABC', 'Calle Falsa 123, CDMX', '01234', '5551234567'),
	(22, 'Farmacéutica Del Sur', 'Avenida Principal 456, Guadalajara', '56789', '3339876543'),
	(23, 'Medicamentos Nacionales', 'Boulevard de la Salud 789, Monterrey', '67890', '8181234567'),
	(24, 'Laboratorios Sánchez', 'Calle de la Medicina 101, Puebla', '23456', '2227654321'),
	(25, 'Industria Farmacéutica Mexicana', 'Avenida Siempre Viva 202, Mérida', '34567', '9998765432'),
	(26, 'Laboratorios Juárez', 'Calle de los Remedios 303, Tijuana', '89012', '6641239876'),
	(27, 'Farmacéutica Orozco', 'Avenida del Pacífico 404, León', '45678', '4776543210'),
	(28, 'Medicinas Garza', 'Calle Central 505, Querétaro', '78901', '4423210987'),
	(29, 'Laboratorios Pérez', 'Calle de la Esperanza 606, Morelia', '90123', '4436549870'),
	(30, 'Laboratorios López', 'Avenida de la Ciencia 707, Cancún', '34512', '9981236547');

-- Volcando estructura para tabla puntoventa.ventas
CREATE TABLE IF NOT EXISTS `ventas` (
  `VentaID` int(11) NOT NULL AUTO_INCREMENT,
  `ClienteID` int(11) DEFAULT NULL,
  `Fecha` datetime DEFAULT current_timestamp(),
  `Total` decimal(10,2) DEFAULT NULL,
  `Empleado` varchar(100) DEFAULT NULL,
  PRIMARY KEY (`VentaID`),
  KEY `ventas_ibfk_1` (`ClienteID`),
  CONSTRAINT `ventas_ibfk_1` FOREIGN KEY (`ClienteID`) REFERENCES `clientes` (`ClienteID`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- Volcando datos para la tabla puntoventa.ventas: ~3 rows (aproximadamente)
INSERT INTO `ventas` (`VentaID`, `ClienteID`, `Fecha`, `Total`, `Empleado`) VALUES
	(13, 13, '2024-11-18 02:09:10', 100.00, 'Antonio'),
	(14, 14, '2024-11-18 12:59:41', 800.00, 'Antonio'),
	(15, 15, '2024-11-18 13:02:06', 80.00, 'Antonio');

/*!40103 SET TIME_ZONE=IFNULL(@OLD_TIME_ZONE, 'system') */;
/*!40101 SET SQL_MODE=IFNULL(@OLD_SQL_MODE, '') */;
/*!40014 SET FOREIGN_KEY_CHECKS=IFNULL(@OLD_FOREIGN_KEY_CHECKS, 1) */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40111 SET SQL_NOTES=IFNULL(@OLD_SQL_NOTES, 1) */;
