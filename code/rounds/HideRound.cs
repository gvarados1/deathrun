using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenGamemode
{
	public partial class HideRound : BaseRound
	{
		[ServerVar( "hdn_host_always_hidden", Help = "Make the host always the hidden." )]
		public static bool HostAlwaysHidden { get; set; } = false;

		public override string RoundName => "PREPARE";
		public override int RoundDuration => 5;

		private bool _roundStarted;

		public override void OnPlayerSpawn( Player player )
		{
			if ( Players.Contains( player ) ) return;

			AddPlayer( player );

			if ( _roundStarted )
			{
				player.Team = Game.Instance.IrisTeam;
				player.Team.OnStart( player );

			}

			base.OnPlayerSpawn( player );
		}

		protected override void OnStart()
		{
			Log.Info( "Started Hide Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach((player) => player.Respawn());

				if ( Players.Count == 0 ) return;

				// Select a random Hidden player.
				var hidden = Players[Rand.Int( Players.Count - 1 )];

				if ( HostAlwaysHidden )
				{
					hidden = Players[0];
				}

				Assert.NotNull( hidden );

				hidden.Team = Game.Instance.HiddenTeam;
				hidden.Team.OnStart( hidden );

				// Make everyone else I.R.I.S.
				Players.ForEach( ( player ) =>
				{
					if ( player != hidden )
					{
						player.Team = Game.Instance.IrisTeam;
						player.Team.OnStart( player );
					}
				} );

				//Reset Deathrun Traps via map relay
				
				

				_roundStarted = true;
			}
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Hide Round" );
		}

		protected override void OnTimeUp()
		{
			Log.Info( "Hide Time Up!" );

			Game.Instance.ChangeRound( new HuntRound() );

			base.OnTimeUp();
		}
	}
}
