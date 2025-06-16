using Godot;
using System;
using System.IO;
using EPainter.Core;

namespace EPainter.UI
{
	public partial class Main : Control
	{
		[Export] CodeEdit codeEdit;
		[Export] Rayitas rayitas;
		[Export] SpinBox sizeInput;
		[Export] TextEdit outputText;
		[Export] FileDialog saveFileDialog;
		[Export] FileDialog loadFileDialog;
		
		public override void _Ready()
		{
		}

		public override void _Process(double delta)
		{
		}

		void Run()
		{
			ErrorReporter.Reset();
			outputText.Text = "Ejecutando código...";

			string code = codeEdit.Text;

			try
			{
				Scanner scanner = new Scanner(code);
				var tokens = scanner.scanTokens();

				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Errores de análisis léxico:\n" + String.Join("\n", ErrorReporter.errors);
					return;
				}

				Parser parser = new Parser(tokens);
				var statements = parser.Parse();

				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Errores de análisis sintáctico:\n" + String.Join("\n", ErrorReporter.errors);
					return;
				}

				var interpreter = new Interpreter();
				try
				{
					interpreter.Interpret(rayitas.Canvas, statements);

					if (ErrorReporter.HasRuntimeErrors)
					{
						outputText.Text = "Errores durante la ejecución:\n" + String.Join("\n", ErrorReporter.runtimeErrors);
					}
					else
					{
						outputText.Text = "Código ejecutado correctamente.";
					}
				}
				catch (Exception ex)
				{
					if (ex is RuntimeError runtimeError)
					{
						ErrorReporter.RuntimeError(runtimeError);
						outputText.Text = $"Error durante la ejecución: {runtimeError.Message}";
						GD.PrintErr($"Error durante la ejecución: {runtimeError.Message}");
					}
					else
					{
						outputText.Text = $"Error durante la ejecución: {ex.Message}";
						GD.PrintErr($"Error durante la ejecución: {ex.Message}");
						GD.PrintErr($"Stack trace: {ex.StackTrace}");
					}
				}

				rayitas.QueueRedraw();
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error inesperado: {ex.Message}";
				GD.PrintErr($"Error inesperado: {ex.Message}");
			}
		}

		void Resize()
		{
			int newSize = (int)sizeInput.Value;
			GD.Print($"Changing canvas size to {newSize}");
			rayitas.ResizeCanvas(newSize);
			outputText.Text = $"Canvas resized to {newSize}x{newSize}";
		}

		void Save()
		{
			if (saveFileDialog != null)
			{
				GD.Print("Mostrando diálogo de guardado...");
				saveFileDialog.CurrentDir = System.IO.Directory.GetCurrentDirectory();
				saveFileDialog.CurrentFile = "codigo.pw";
				saveFileDialog.Visible = true;
			}
			else
			{
				outputText.Text = "Error: No se pudo encontrar el diálogo de guardado";
				GD.PrintErr("Error: saveFileDialog es null");
			}
		}

		void Load()
		{
			if (loadFileDialog != null)
				loadFileDialog.Visible = true;
			else
				outputText.Text = "Error: No se pudo encontrar el diálogo de carga";
		}
		
		void OnSaveFileSelected(string path)
		{
			try
			{
				string fsPath = ConvertGodotPathToFilesystemPath(path);
				
				if (!fsPath.EndsWith(".pw", StringComparison.OrdinalIgnoreCase))
				{
					fsPath += ".pw";
				}
				
				GD.Print($"Guardando archivo en: {fsPath}");
				
				System.IO.File.WriteAllText(fsPath, codeEdit.Text);
				outputText.Text = $"Archivo guardado exitosamente en: {fsPath}";
				GD.Print($"Archivo guardado: {fsPath}");
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error al guardar el archivo: {ex.Message}";
				GD.PrintErr($"Error al guardar el archivo: {ex.Message}");
			}
		}
		
		void OnLoadFileSelected(string path)
		{
			try
			{
				string fsPath = ConvertGodotPathToFilesystemPath(path);
				GD.Print($"Intentando cargar archivo desde: {fsPath}");
				
				if (!System.IO.File.Exists(fsPath))
				{
					outputText.Text = $"Error: El archivo '{fsPath}' no existe";
					GD.PrintErr($"Error: El archivo '{fsPath}' no existe");
					return;
				}
				
				string content = System.IO.File.ReadAllText(fsPath);
				codeEdit.Text = content;
				outputText.Text = $"Archivo cargado exitosamente: {fsPath}";
				GD.Print($"Archivo cargado: {fsPath}");
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error al cargar el archivo: {ex.Message}";
				GD.PrintErr($"Error al cargar el archivo: {ex.Message}");
			}
		}

		private string ConvertGodotPathToFilesystemPath(string path)
		{
			if (path.StartsWith("res://"))
			{
				string projectDir = System.IO.Directory.GetCurrentDirectory();
				string relativePath = path.Substring(6); 
				return System.IO.Path.Combine(projectDir, relativePath);
			}
			return path;
		}
	}
}

