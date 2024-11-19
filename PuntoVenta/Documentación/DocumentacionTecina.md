# Documentación Técnica de la Aplicación PuntoVenta

## 1. Diseño de la Aplicación

La aplicación PuntoVenta está construida utilizando **.NET MAUI** 
para permitir la ejecución en múltiples plataformas (Android, iOS, Windows). 
Utiliza el patrón de arquitectura **MVVM (Modelo-Vista-ViewModel)**, lo que facilita 
la separación de las responsabilidades entre la lógica de negocio, la interfaz 
de usuario y la interacción con los datos.

### Componentes Principales:
- **Modelo (Model)**: Representa los datos de la aplicación, como productos, clientes, ventas, etc.
- **Vista (View)**: Define la interfaz de usuario en las diferentes plataformas.
- **ViewModel**: Contiene la lógica de negocio, maneja la interacción entre la vista y el modelo, y se encarga de la manipulación de datos.

### Tecnologías Utilizadas:
- **.NET MAUI**: Para la interfaz multiplataforma.
- **MySQL: Como base de datos relacional.
- **SHA-256**: Para la encriptación de contraseñas de los empleados.

## 2. Estructura de la Base de Datos

La base de datos de la aplicación es relacional y consta de varias tablas que gestionan las ventas, clientes,
inventarios administracion, detalles de las ventas, proveedores y empleados. A continuación se describen las tablas principales.

### Tablas Principales:
- **`administracion`**: Almacena usuarios y conytraseñas de los administradores.
- **`empleados`**: Almacena usuarios y contraseñas de los empleados.
- **`proveedores`**: Almacena informacion de los proveedores.
- **`ventas`**: Almacena información sobre las ventas realizadas.
- **`clientes`**: Contiene los datos de los clientes.
- **`inventario`**: Registra los productos disponibles para la venta.
- **`detalleVentas`**: Guarda los detalles de las ventas, asociando productos con las ventas.

### Relación entre las Tablas:
Las tablas están relacionadas a través de claves foráneas, lo que garantiza la integridad referencial entre ellas. A continuación se muestra el **Diagrama ER**.

## 3. Diagrama ER (Entidad-Relación)

![Diagrama ER](diagrama_ER.png)

Este diagrama muestra la relación entre las tablas en la base de datos:
- **`ventas`** tiene una relación de uno a muchos con **`detalleVentas`**.
- **`clientes`** tiene una relación de uno a muchos con **`ventas`**.
- **`inventario`** tiene una relación de uno a muchos con **`detalleVentas`**.

## 4. Diagrama de Clases

![Diagrama de Clases](diagrama_clases.png)

### Clases Principales:
- **ProductoVenta**: Gestiona la creación de ventas, cálculos de totales y vinculación con clientes y productos.
- **Producto**: Administra las operaciones relacionadas con el inventario de productos.
- **Empleado**: Administra las operaciones de empleados, como agregar, modificar y eliminar empleados.

## 5. Flujo de Datos

El flujo de datos sigue estos pasos:
1. Un **cliente** selecciona productos desde el inventario.
2. Los productos se agregan al **carrito** de compras.
3. La **venta** se registra en la base de datos con el **cliente** y los **productos**.
4. Los **detalles de la venta** se almacenan en la tabla `detalleVentas` para mantener la integridad de los productos vendidos.