using Godot;
using System;
using System.IO;
using EPainter.Core;

namespace EPainter.UI
{
	/// <summary>
	/// Clase principal que controla la interfaz de usuario de E-Painter.
	/// Maneja las interacciones con el editor de código, el lienzo y las operaciones de archivo.
	/// </summary>
	public partial class Main : Control
	{
		/// <summary>
		/// Editor de código para escribir programas E-Painter.
		/// </summary>
		[Export] CodeEdit codeEdit;
		
		/// <summary>
		/// Componente que maneja la visualización del lienzo donde se dibujan los gráficos.
		/// </summary>
		[Export] Rayitas rayitas;
		
		/// <summary>
		/// Control para ajustar el tamaño del lienzo.
		/// </summary>
		[Export] SpinBox sizeInput;
		
		/// <summary>
		/// Área de texto para mostrar mensajes de salida y errores.
		/// </summary>
		[Export] TextEdit outputText;
		
		/// <summary>
		/// Diálogo para guardar archivos de código E-Painter.
		/// </summary>
		[Export] FileDialog saveFileDialog;
		
		/// <summary>
		/// Diálogo para cargar archivos de código E-Painter.
		/// </summary>
		[Export] FileDialog loadFileDialog;
		
		/// <summary>
		/// Inicializa el nodo cuando entra en el árbol de escena.
		/// </summary>
		public override void _Ready()
		{
		}

		/// <summary>
		/// Llamado en cada frame para procesar la lógica del nodo.
		/// </summary>
		/// <param name="delta">Tiempo transcurrido desde el último frame en segundos.</param>
		public override void _Process(double delta)
		{
		}

		/// <summary>
		/// Ejecuta el código E-Painter escrito en el editor.
		/// Realiza el análisis léxico, sintáctico y la interpretación del código.
		/// Muestra los resultados o errores en el área de salida.
		/// </summary>
		void Run()
		{
			ErrorReporter.Reset();
			outputText.Text = "Executing code...";

			string code = codeEdit.Text;

			try
			{
				Scanner scanner = new Scanner(code);
				var tokens = scanner.scanTokens();

				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Lexical analysis errors:\n" + String.Join("\n", ErrorReporter.errors);
					return;
				}

				Parser parser = new Parser(tokens);
				var statements = parser.Parse();

				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Parsing errors:\n" + String.Join("\n", ErrorReporter.errors);
					return;
				}

				var interpreter = new Interpreter();
				try
				{
					interpreter.Interpret(rayitas.Canvas, statements);

					if (ErrorReporter.HasRuntimeErrors)
					{
						outputText.Text = "Runtime errors:\n" + String.Join("\n", ErrorReporter.runtimeErrors);
					}
					else
					{
						outputText.Text = "Code executed successfully.";
					}
				}
				catch (Exception ex)
				{
					if (ex is RuntimeError runtimeError)
					{
						ErrorReporter.RuntimeError(runtimeError);
						outputText.Text = $"Runtime error: {runtimeError.Message}";
						GD.PrintErr($"Runtime error: {runtimeError.Message}");
					}
					else
					{
						outputText.Text = $"Runtime error: {ex.Message}";
						GD.PrintErr($"Runtime error: {ex.Message}");
						GD.PrintErr($"Stack trace: {ex.StackTrace}");
					}
				}

				rayitas.QueueRedraw();
			}
			catch (Exception ex)
			{
				outputText.Text = $"Unexpected error: {ex.Message}";
				GD.PrintErr($"Unexpected error: {ex.Message}");
			}
		}

		/// <summary>
		/// Cambia el tamaño del lienzo según el valor establecido en el control sizeInput.
		/// Actualiza la visualización y muestra un mensaje de confirmación.
		/// </summary>
		void Resize()
		{
			int newSize = (int)sizeInput.Value;
			GD.Print($"Changing canvas size to {newSize}");
			rayitas.ResizeCanvas(newSize);
			outputText.Text = $"Canvas resized to {newSize}x{newSize}";
		}

		/// <summary>
		/// Muestra el diálogo para guardar el código actual en un archivo.
		/// </summary>
		void Save()
		{
			if (saveFileDialog != null)
			{
				GD.Print("Showing save dialog...");
				saveFileDialog.CurrentDir = Directory.GetCurrentDirectory();
				saveFileDialog.CurrentFile = "code.pw";
				saveFileDialog.Visible = true;
			}
			else
			{
				outputText.Text = "Error: Could not find save dialog";
				GD.PrintErr("Error: saveFileDialog is null");
			}
		}

		/// <summary>
		/// Muestra el diálogo para cargar un archivo de código E-Painter.
		/// </summary>
		void Load()
		{
			if (loadFileDialog != null)
				loadFileDialog.Visible = true;
			else
				outputText.Text = "Error: Could not find load dialog";
		}
		
		/// <summary>
		/// Maneja el evento cuando se selecciona un archivo en el diálogo de guardado.
		/// Guarda el contenido del editor de código en el archivo seleccionado.
		/// </summary>
		/// <param name="path">La ruta del archivo seleccionado.</param>
		void OnSaveFileSelected(string path)
		{
			try
			{
				string fsPath = ConvertGodotPathToFilesystemPath(path);
				
				if (!fsPath.EndsWith(".pw", StringComparison.OrdinalIgnoreCase))
				{
					fsPath += ".pw";
				}
				
				GD.Print($"Saving file to: {fsPath}");
				
				File.WriteAllText(fsPath, codeEdit.Text);
				outputText.Text = $"File saved successfully to: {fsPath}";
				GD.Print($"File saved: {fsPath}");
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error saving file: {ex.Message}";
				GD.PrintErr($"Error saving file: {ex.Message}");
			}
		}
		
		/// <summary>
		/// Maneja el evento cuando se selecciona un archivo en el diálogo de carga.
		/// Carga el contenido del archivo en el editor de código.
		/// </summary>
		/// <param name="path">La ruta del archivo seleccionado.</param>
		void OnLoadFileSelected(string path)
		{
			try
			{
				string fsPath = ConvertGodotPathToFilesystemPath(path);
				GD.Print($"Attempting to load file from: {fsPath}");
				
				if (!File.Exists(fsPath))
				{
					outputText.Text = $"Error: The file '{fsPath}' does not exist";
					GD.PrintErr($"Error: The file '{fsPath}' does not exist");
					return;
				}
				
				string content = File.ReadAllText(fsPath);
				codeEdit.Text = content;
				outputText.Text = $"File loaded successfully: {fsPath}";
				GD.Print($"File loaded: {fsPath}");
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error loading file: {ex.Message}";
				GD.PrintErr($"Error loading file: {ex.Message}");
			}
		}

		/// <summary>
		/// Convierte una ruta de archivo de Godot a una ruta del sistema de archivos.
		/// </summary>
		/// <param name="path">La ruta de archivo de Godot.</param>
		/// <returns>La ruta del sistema de archivos correspondiente.</returns>
		private string ConvertGodotPathToFilesystemPath(string path)
		{
			if (path.StartsWith("res://"))
			{
				string projectDir = Directory.GetCurrentDirectory();
				string relativePath = path.Substring(6); 
				return Path.Combine(projectDir, relativePath);
			}
			return path;
		}
	}
}

