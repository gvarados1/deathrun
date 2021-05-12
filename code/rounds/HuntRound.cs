﻿using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenGamemode
{
    public class HuntRound : BaseRound
	{
		public override string RoundName => "RUN";
		public override int RoundDuration => 600;
		public override bool CanPlayerSuicide => true;

		public List<Player> Spectators = new();

		private string _hiddenHunter;
		private string _firstDeath;
		private bool _isGameOver;
		private int _hiddenKills;

		public override void OnPlayerKilled( Player player )
		{
			Players.Remove( player );
			Spectators.Add( player );

			player.MakeSpectator( player.EyePos );

			if ( player.Team is HiddenTeam )
			{
				if ( player.LastAttacker is Player attacker )
				{
					_hiddenHunter = attacker.Name;
				}

				_ = LoadStatsRound( "Runners have escaped!" );

				return;
			}
			else
			{
				if ( string.IsNullOrEmpty( _firstDeath ) )
				{
					_firstDeath = player.Name;
				}

				_hiddenKills++;
			}

			if ( Players.Count <= 1 )
			{
				_ = LoadStatsRound( "All runners have died! Death wins!" );
			}
		}

		public override void OnPlayerLeave( Player player )
		{
			base.OnPlayerLeave( player );

			Spectators.Remove( player );

			if ( player.Team is HiddenTeam )
			{
				_ = LoadStatsRound( "Death Disconnected" );
			}
		}

		public override void OnPlayerSpawn( Player player )
		{
			player.MakeSpectator();

			Spectators.Add( player );
			Players.Remove( player );

			base.OnPlayerSpawn( player );
		}

		protected override void OnStart()
		{
			Log.Info( "Started Hunt Round" );

			if ( Host.IsServer )
			{
				Sandbox.Player.All.ForEach( ( player ) => SupplyLoadouts( player as Player ) );
			}
		}

		protected override void OnFinish()
		{
			Log.Info( "Finished Hunt Round" );

			if ( Host.IsServer )
			{
				Spectators.Clear();
			}
		}

		protected override void OnTimeUp()
		{
			if ( _isGameOver ) return;

			Log.Info( "Hunt Time Up!" );

			_ = LoadStatsRound( "Time's up! Runners took too long!" );

			base.OnTimeUp();
		}

		private void SupplyLoadouts( Player player )
		{
			// Give everyone who is alive their starting loadouts.
			if ( player.Team != null && player.LifeState == LifeState.Alive )
			{
				player.Team.SupplyLoadout( player );
				AddPlayer( player );
			}
		}

		private async Task LoadStatsRound(string winner, int delay = 3)
		{
			_isGameOver = true;

			await Task.Delay( delay * 1000 );

			if ( Game.Instance.Round != this )
				return;

			var hidden = Game.Instance.GetTeamPlayers<HiddenTeam>().FirstOrDefault();

			Game.Instance.ChangeRound( new StatsRound
			{
				HiddenName = hidden != null ? hidden.Name : "",
				HiddenKills = _hiddenKills,
				FirstDeath = _firstDeath,
				HiddenHunter = _hiddenHunter,
				Winner = winner
			} );
		}
	}
}
