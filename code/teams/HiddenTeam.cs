using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenGamemode
{
    class HiddenTeam : BaseTeam
	{
		public override bool HideNameplate => true;
		public override string HudClassName => "team_hidden";
		public override string Name => "Death";
		public Player CurrentPlayer { get; set; }

		public override void OnStart( Player player )
		{

			if ( Host.IsServer )
			{
				player.RemoveClothing();
				player.AttachClothing("models/citizen_clothes/dress/dress.kneelength.vmdl");
			}

			player.SetModel( "models/citizen/citizen.vmdl" );

			player.EnableAllCollisions = true;
			player.EnableDrawing = true;
			player.EnableHideInFirstPerson = true;
			player.EnableShadowInFirstPerson = true;

			player.Controller = new HiddenController();
			player.Camera = new FirstPersonCamera();
		}

		public override void OnTick( Player player )
		{

			if ( player.Input.Pressed( InputButton.Use) )
			{
				if ( player.Controller is not HiddenController controller )
					return;

				if ( controller.IsFrozen )
					return;

				var trace = Trace.Ray( player.EyePos, player.EyePos + player.EyeRot.Forward * 40f )
					.HitLayer( CollisionLayer.WORLD_GEOMETRY )
					.Ignore( player )
					.Ignore( player.ActiveChild )
					.Radius( 2 )
					.Run();

				if ( trace.Hit )
					controller.IsFrozen = true;
			}
		}

		public override void OnJoin( Player player  )
		{
			Log.Info( $"{player.Name} joined the Runner team." );

			CurrentPlayer = player;

			base.OnJoin( player );
		}

		public override void OnLeave( Player player )
		{
			Log.Info( $"{player.Name} left the Runner team." );

			CurrentPlayer = null;

			base.OnLeave( player );
		}
	}
}
