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

			Scanner scanner = new Scanner(code);
			var tokens = scanner.scanTokens();

			Parser parser = new Parser(tokens);
			var statements = parser.Parse();

			//Interpreter interpreter = new Interpreter(rayitas.Canvas, statements);

			rayitas.QueueRedraw();

		}
	}

}
