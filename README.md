# PuntoVenta

Sistema de gestión de punto de venta desarrollado con .NET MAUI. Este proyecto permite gestionar empleados, productos, proveedores, y realizar ventas.

## Características

- Gestión de inventario (CRUD de productos y proveedores).
- Gestión de empleados (CRUD de usuarios).
- Funcionalidad de ventas con carrito, cálculo de subtotal y total.

## Requisitos previos

Antes de clonar y ejecutar el proyecto, asegúrate de tener instalados:

- [Visual Studio 2022](https://visualstudio.microsoft.com/) (con la carga de trabajo **.NET MAUI** y **ASP.NET y desarrollo web**).
- [MySQL Server](https://dev.mysql.com/downloads/) configurado con la base de datos necesaria.
- [Git](https://git-scm.com/).

## Configuración de la base de datos

1. Crea una base de datos en MySQL llamada `puntoventa`.
2. Ejecuta el siguiente script para crear las tablas necesarias:

```sql
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

-- Volcando estructura para tabla puntoventa.clientes
CREATE TABLE IF NOT EXISTS `clientes` (
  `ClienteID` int(11) NOT NULL AUTO_INCREMENT,
  `Nombre` varchar(100) DEFAULT NULL,
  `Apellidos` varchar(100) DEFAULT NULL,
  `Celular` varchar(20) DEFAULT NULL,
  PRIMARY KEY (`ClienteID`)
) ENGINE=InnoDB AUTO_INCREMENT=16 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

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
```

Por ultimo en el exporador de soluciones, ve a modles, abre conexion.cs y cambia los parámetros de la cadena de conexion, a donde alojaste la base de datos.

### **Pasos adicionales**
1. Sustituye `https://github.com/tuusuario/PuntoVenta.git` con la URL real de tu repositorio.
2. Agrega capturas de pantalla de tu aplicación para ilustrar su funcionamiento.
3. Verifica que la cadena de conexión esté correctamente configurada para tus pruebas locales.

