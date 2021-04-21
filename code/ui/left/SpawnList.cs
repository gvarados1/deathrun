using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Tests;

[Library]
public partial class SpawnList : Panel
{
	VirtualScrollPanel Canvas;

	public SpawnList()
	{
		AddClass( "spawnpage" );
		AddChild( out Canvas, "canvas" );

		Canvas.Layout.AutoColumns = true;
		Canvas.Layout.ItemSize = new Vector2( 100, 100 );
		Canvas.OnCreateCell = ( cell, data ) =>
		{
			var file = (string)data;
			var panel = cell.Add.Panel( "icon" );
			panel.Style.Set( "background-image", $"url( /models/{file}.png )" );
			panel.AddEvent( "onclick", () => ConsoleSystem.Run( "spawn", "models/" + file ) );
		};


		foreach ( var file in FileSystem.Mounted.FindFile( "models", "*.vmdl_c", true ) )
		{
			if ( file.Contains( "_lod0" ) ) continue;

			Canvas.AddItem( file );
		}
	}
}
