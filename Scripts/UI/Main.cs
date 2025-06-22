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
		#region Campos y Propiedades
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
		/// Temporizador para retrasar la validación del código mientras el usuario escribe.
		/// Evita validaciones continuas que podrían afectar el rendimiento.
		/// </summary>
		private Timer validationTimer;
		#endregion

		#region Inicialización y Eventos
		/// <summary>
		/// Inicializa el nodo cuando entra en el árbol de escena.
		/// </summary>
		public override void _Ready()
		{
			codeEdit.TextChanged += OnCodeTextChanged;
		}
		/// <summary>
		/// Se activa cuando el texto del editor de código cambia.
		/// Inicia un temporizador que validará el código después de un breve retraso.
		/// </summary>		
		private void OnCodeTextChanged()
		{
			if (validationTimer == null)
			{
				validationTimer = new Timer();
				validationTimer.OneShot = true;
				validationTimer.WaitTime = 0.5f;
				validationTimer.Timeout += ValidateCodeOnly;
				AddChild(validationTimer);
			}

			validationTimer.Stop();
			validationTimer.Start();
		}
		#endregion

		#region Validación y Ejecución de Código
		/// <summary>
		/// Valida la sintaxis del código E-Painter sin ejecutarlo.
		/// Muestra errores léxicos y sintácticos mientras el usuario escribe.
		/// </summary>
		private void ValidateCodeOnly()
		{
			string code = codeEdit.Text;

			try
			{
				ErrorReporter.Reset();

				// Análisis léxico
				Scanner scanner = new Scanner(code);
				var tokens = scanner.scanTokens();

				if (ErrorReporter.HasCompilationErrors)
				{
					ShowErrorMessage("Lexical errors:\n" + String.Join("\n", ErrorReporter.CompilationErrors));
					return;
				}

				// Análisis sintáctico
				Parser parser = new Parser(tokens);
				parser.Parse();

				if (ErrorReporter.HasCompilationErrors)
				{
					ShowErrorMessage("Parsing errors:\n" + String.Join("\n", ErrorReporter.CompilationErrors));
					return;
				}

				outputText.Text = "Code syntax is valid";
				outputText.Modulate = new Color(0.7f, 0.9f, 1.0f);
			}
			catch (Exception ex)
			{
				ShowErrorMessage($"Validation error: {ex.Message}");
			}
		}

		/// <summary>
		/// Ejecuta el código E-Painter escrito en el editor.
		/// Realiza el análisis léxico, sintáctico y la interpretación del código.
		/// Muestra los resultados o errores en el área de salida.
		/// </summary>
		void Run()
		{
			string code = codeEdit.Text;

			ErrorReporter.Reset();
			ShowInfoMessage("Executing code...");


			try
			{
				Scanner scanner = new Scanner(code);
				var tokens = scanner.scanTokens();

				if (ErrorReporter.HasCompilationErrors)
				{
					ShowErrorMessage("Lexical analysis errors:\n" + String.Join("\n", ErrorReporter.CompilationErrors));
					return;
				}

				Parser parser = new Parser(tokens);
				var statements = parser.Parse();

				if (ErrorReporter.HasCompilationErrors)
				{
					ShowErrorMessage("Parsing errors:\n" + String.Join("\n", ErrorReporter.CompilationErrors));
					return;
				}

				var interpreter = new Interpreter();
				try
				{
					interpreter.Interpret(rayitas.Canvas, statements);

					if (ErrorReporter.HasRuntimeErrors)
					{
						ShowErrorMessage("Runtime errors:\n" + String.Join("\n", ErrorReporter.RuntimeErrors));
					}
					else
					{
						ShowSuccessMessage("Code executed successfully.");
					}
				}
				catch (Exception ex)
				{
					if (ex is RuntimeError runtimeError)
					{
						ErrorReporter.RuntimeError(runtimeError);
						ShowErrorMessage($"Runtime error: {runtimeError.Message}");
						GD.PrintErr($"Runtime error: {runtimeError.Message}");
					}
					else
					{
						ShowErrorMessage($"Runtime error: {ex.Message}");
						GD.PrintErr($"Runtime error: {ex.Message}");
						GD.PrintErr($"Stack trace: {ex.StackTrace}");
					}
				}

				rayitas.QueueRedraw();
			}
			catch (Exception ex)
			{
				ShowErrorMessage($"Unexpected error: {ex.Message}");
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
			ShowInfoMessage($"Canvas resized to {newSize}x{newSize}");
		}
		#endregion

		#region Gestión de Archivos
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
				ShowErrorMessage("Error: Could not find save dialog");
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
				ShowErrorMessage("Error: Could not find load dialog");
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
				ShowSuccessMessage($"File saved successfully to: {fsPath}");
				GD.Print($"File saved: {fsPath}");
			}
			catch (Exception ex)
			{
				ShowErrorMessage($"Error saving file: {ex.Message}");
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
					ShowErrorMessage($"Error: The file '{fsPath}' does not exist");
					GD.PrintErr($"Error: The file '{fsPath}' does not exist");
					return;
				}

				string content = File.ReadAllText(fsPath);
				codeEdit.Text = content;
				ShowSuccessMessage($"File loaded successfully: {fsPath}");
				GD.Print($"File loaded: {fsPath}");
			}
			catch (Exception ex)
			{
				ShowErrorMessage($"Error loading file: {ex.Message}");
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
		#endregion

		#region Utiliidades de UI
		/// <summary>
		/// Muestra un mensaje de error en color rojo.
		/// </summary>
		/// <param name="message">El mensaje de error a mostrar.</param>
		private void ShowErrorMessage(string message)
		{
			outputText.Text = message;
			outputText.Modulate = Colors.Red;
		}

		/// <summary>
		/// Muestra un mensaje de éxito en color verde.
		/// </summary>
		/// <param name="message">El mensaje de éxito a mostrar.</param>
		private void ShowSuccessMessage(string message)
		{
			outputText.Text = message;
			outputText.Modulate = Colors.Green;
		}

		/// <summary>
		/// Muestra un mensaje informativo en color por defecto.
		/// </summary>
		/// <param name="message">El mensaje informativo a mostrar.</param>
		private void ShowInfoMessage(string message)
		{
			outputText.Text = message;
			outputText.SelfModulate = Colors.White;
		}
		#endregion
	}
}

