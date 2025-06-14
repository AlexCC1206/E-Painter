using Godot;
using System;
using EPainter.Core;

namespace EPainter.UI
{
	public partial class Main : Control
	{
		[Export] CodeEdit codeEdit;
		[Export] Rayitas rayitas;
		// Called when the node enters the scene tree for the first time.
		public override void _Ready()
		{

		}

		// Called every frame. 'delta' is the elapsed time since the previous frame.
		public override void _Process(double delta)
		{
		}

		void Run()
		{
			string code = codeEdit.Text;

			// Scanner
			Scanner scanner = new Scanner(code);
			var tokens = scanner.scanTokens();

			// Parser
			Parser parser = new Parser(tokens);
			var statements = parser.Parse();

			// Intérprete
			var interpreter = new Interpreter();
			try
			{
				interpreter.Interpret(rayitas.Canvas, statements);
			}
			catch (Exception ex)
			{
				GD.Print($"Error durante la ejecución: {ex.Message}");
			}

			// Actualizar la vista
			rayitas.QueueRedraw();
		}
	}

}
