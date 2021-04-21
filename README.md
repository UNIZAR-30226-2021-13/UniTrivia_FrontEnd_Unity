# UniTrivia_FrontEnd_Unity
Desarrollo Android para la aplicación UniTrivia

======= Instalación =======

Pasos para poder importar y usar el proyecto en Unity.
  1) Descargar Unity HUB e instalar la versión de Unity 2020.3.0f1. Es posible usar una versión posterior, pero es probable que aparezcan incompatibilidades o problemas.
  2) En la instalación de la versión, o posteriormente, asegurarnos que están instalados los módulos de Android Build Support.
        Unity HUB -> Installs -> 2020.3.0f1 o la versión que tengamos -> Add Modules -> Activar "Android Build Support" y sus opciones "Android SDK" y "OpenJDK"
  3) Crear un nuevo proyecto 2D con la versión instalada anteriormente.
  4) Sustituir la carpeta Assets del proyecto por la carpeta Assets subida a GitHub

======= Notas =======

- Unity simula la aplicación desde la escena abierta. Si queremos ejecutarla desde el principio, abrir "Init Scene". Dado que algunas escenas dependen de los datos generados en otras, usar otra escena directamente debe ser para probar algo específico.
- En la carpeta de Assets ya se incluye el plugin de GitHub para Unity, por si se hace una instalación manual al principio.
