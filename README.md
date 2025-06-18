# E-Painter

![E-Painter Logo](icon.png)

## Descripción General
E-Painter es un lenguaje de programación y entorno de desarrollo para la creación de gráficos y dibujos a través de código. El proyecto utiliza C# y el motor Godot para proporcionar una interfaz visual donde se pueden ejecutar programas escritos en el lenguaje E-Painter.

## Estructura del Proyecto

### Core (Núcleo)
El núcleo del proyecto está organizado en varios módulos:

#### AST (Árbol de Sintaxis Abstracta)
- `Expression.cs`: Define la estructura para todas las expresiones del lenguaje.
- `Statement.cs`: Define la estructura para todas las sentencias del lenguaje.

#### Environment (Entorno)
- `Canvas.cs`: Implementa el lienzo donde se realizan los dibujos.
- `EPainterState.cs`: Mantiene el estado actual del intérprete (posición, color, tamaño).

#### Error (Manejo de Errores)
- `ErrorReporter.cs`: Sistema centralizado para reportar errores.
- `ParseError.cs`: Excepciones para errores de análisis sintáctico.
- `RuntimeError.cs`: Excepciones para errores en tiempo de ejecución.
- `ScannerException.cs`: Excepciones para errores de análisis léxico.
- `GotoException.cs`: Excepciones para gestionar saltos de control de flujo.

#### Interpreter (Intérprete)
- `Interpreter.cs`: Ejecuta el código E-Painter procesado.
- `ExprVisitor.cs`: Implementa el patrón Visitor para evaluar expresiones.
- `StmtVisitor.cs`: Implementa el patrón Visitor para ejecutar sentencias.

#### Lexer (Analizador Léxico)
- `Scanner.cs`: Transforma el código fuente en tokens.
- `Token.cs`: Representa un token individual.
- `TokenType.cs`: Enumera los tipos de tokens disponibles.

#### Parser (Analizador Sintáctico)
- `Parser.cs`: Transforma los tokens en un árbol de sintaxis abstracta.

### UI (Interfaz de Usuario)
- `Main.cs`: Controla la interfaz principal del programa.
- `Rayitas.cs`: Implementa la representación visual del lienzo.

## Flujo de Ejecución

1. **Análisis Léxico**: El código fuente se procesa mediante el `Scanner` para producir una secuencia de tokens.
2. **Análisis Sintáctico**: La secuencia de tokens se procesa mediante el `Parser` para crear un árbol de sintaxis abstracta.
3. **Interpretación**: El árbol de sintaxis abstracta se recorre utilizando los visitantes `ExprVisitor` y `StmtVisitor` para ejecutar el programa.
4. **Visualización**: Los resultados de la ejecución se muestran en el `Canvas` a través de la interfaz de usuario.

## Características del Lenguaje E-Painter

### Comandos Principales
- `Spawn(x, y)`: Establece la posición inicial del cursor.
- `Color(color)`: Cambia el color del pincel.
- `Size(k)`: Establece el tamaño del pincel.
- `DrawLine(dirX, dirY, distancia)`: Dibuja una línea.
- `DrawCircle(dirX, dirY, radio)`: Dibuja un círculo.
- `DrawRectangle(dirX, dirY, distancia, ancho, alto)`: Dibuja un rectángulo.
- `Fill()`: Rellena una forma cerrada.

### Control de Flujo
- `Etiquetas`: Se pueden definir puntos de destino para saltos.
- `GoTo[etiqueta](condición)`: Salta a una etiqueta si se cumple la condición.

### Variables y Expresiones
- Variables con asignación mediante flecha (`<-`).
- Operadores aritméticos: suma, resta, multiplicación, división, módulo, potencia.
- Operadores de comparación: igualdad, desigualdad, mayor que, menor que, etc.
- Operadores lógicos: AND, OR.

### Funciones Integradas
- `GetActualX()`: Obtiene la coordenada X actual.
- `GetActualY()`: Obtiene la coordenada Y actual.
- `GetCanvasSize()`: Obtiene el tamaño del lienzo.
- `GetColorCount()`: Obtiene el número de colores utilizados.
- `IsBrushColor(color)`: Verifica el color actual del pincel.
- `IsBrushSize(tamaño)`: Verifica el tamaño actual del pincel.
- `IsCanvasColor(x, y, color)`: Verifica el color de un píxel específico.

## Requisitos del Sistema
- .NET 8.0
- Godot Engine 4.x o superior

## Ejecución del Proyecto
El proyecto se ejecuta a través del motor Godot, abriendo el archivo `project.godot` y utilizando la opción de ejecución del editor.

## Autor

[Alexander Gutiérrez Ricardo] - [gutyalex217@gmail.com]

---

