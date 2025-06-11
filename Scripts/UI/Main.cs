using Godot;
using System;

namespace EPainter.UI
{
	public partial class Main : Control
{
	[Export] CodeEdit codeEdit;
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

		//Scanner scanner = new Scanner(code);
	}
}

}
