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
		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// Inicializar el área de texto de salida
		if (outputText != null)
		{
			outputText.Text = "E-Painter listo para usar. Escriba código y presione 'Run' para ejecutar.";
		}
		else
		{
			GD.PrintErr("Error: outputText es null");
		}
		
		// Conectamos los botones a sus respectivas funciones
		var runButton = GetNode<Button>("Panel/TopBar/Run");
		if (runButton != null) 
			runButton.Pressed += Run;
		else
			GD.PrintErr("Error: No se encontró el botón Run");
			
		var resizeButton = GetNode<Button>("Panel/TopBar/Resize");
		if (resizeButton != null)
			resizeButton.Pressed += Resize;
		else
			GD.PrintErr("Error: No se encontró el botón Resize");
			
		var saveButton = GetNode<Button>("Panel/TopBar/Save");
		if (saveButton != null)
			saveButton.Pressed += Save;
		else
			GD.PrintErr("Error: No se encontró el botón Save");
			
		var loadButton = GetNode<Button>("Panel/TopBar/Load");
		if (loadButton != null)
			loadButton.Pressed += Load;
		else
			GD.PrintErr("Error: No se encontró el botón Load");
		
		// Configurar diálogos de archivos
		if (saveFileDialog != null)
		{
			// Desconectar antes de conectar para evitar conexiones duplicadas
			saveFileDialog.FileSelected -= OnSaveFileSelected;
			saveFileDialog.FileSelected += OnSaveFileSelected;
			GD.Print("Diálogo de guardado conectado correctamente");
		}
		else
		{
			GD.PrintErr("Error: saveFileDialog es null");
		}
		
		if (loadFileDialog != null)
		{
			// Desconectar antes de conectar para evitar conexiones duplicadas
			loadFileDialog.FileSelected -= OnLoadFileSelected;
			loadFileDialog.FileSelected += OnLoadFileSelected;
			GD.Print("Diálogo de carga conectado correctamente");
		}
		else
		{
			GD.PrintErr("Error: loadFileDialog es null");
		}
		
		// Verificar que el editor de código exista
		if (codeEdit == null)
		{
			GD.PrintErr("Error: codeEdit es null");
		}
	}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		void Run()
		{
			// Limpiar mensajes de error anteriores
			ErrorReporter.Reset();
			outputText.Text = "Ejecutando código...";
			
			string code = codeEdit.Text;
			
			try
			{
				// Scanner
				Scanner scanner = new Scanner(code);
				var tokens = scanner.scanTokens();
				
				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Errores de análisis léxico:\n" + String.Join("\n", ErrorReporter.Errors);
					return;
				}

				// Parser
				Parser parser = new Parser(tokens);
				var statements = parser.Parse();
				
				if (ErrorReporter.HasErrors)
				{
					outputText.Text = "Errores de análisis sintáctico:\n" + String.Join("\n", ErrorReporter.Errors);
					return;
				}

				// Intérprete
				var interpreter = new Interpreter();
				try
				{
					interpreter.Interpret(rayitas.Canvas, statements);
					
					if (ErrorReporter.HasRuntimeErrors)
					{
						outputText.Text = "Errores durante la ejecución:\n" + String.Join("\n", ErrorReporter.RuntimeErrors);
					}
					else
					{
						outputText.Text = "Código ejecutado correctamente.";
					}
				}
				catch (Exception ex)
				{
					outputText.Text = $"Error durante la ejecución: {ex.Message}";
					GD.PrintErr($"Error durante la ejecución: {ex.Message}");
				}

				// Actualizar la vista
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
			rayitas.ResizeCanvas(newSize);
		}
		
		void Save()
		{
			if (saveFileDialog != null)
				saveFileDialog.Visible = true;
			else
				outputText.Text = "Error: No se pudo encontrar el diálogo de guardado";
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
				System.IO.File.WriteAllText(path, codeEdit.Text);
				outputText.Text = $"Archivo guardado exitosamente en: {path}";
				GD.Print($"Archivo guardado: {path}");
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
				if (!System.IO.File.Exists(path))
				{
					outputText.Text = $"Error: El archivo '{path}' no existe";
					GD.PrintErr($"Error: El archivo '{path}' no existe");
					return;
				}
				
				string content = System.IO.File.ReadAllText(path);
				codeEdit.Text = content;
				outputText.Text = $"Archivo cargado exitosamente: {path}";
				GD.Print($"Archivo cargado: {path}");
			}
			catch (Exception ex)
			{
				outputText.Text = $"Error al cargar el archivo: {ex.Message}";
				GD.PrintErr($"Error al cargar el archivo: {ex.Message}");
			}
		}
	}
}

