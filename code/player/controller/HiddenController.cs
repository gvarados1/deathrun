using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenGamemode
{
	public class HiddenController : CustomWalkController
	{
		public override float SprintSpeed { get; set; } = 580f;
		public bool IsFrozen { get; set; }

		public override float GetWishSpeed()
		{
			var speed = base.GetWishSpeed();

			return speed;
		}

		public override void Tick()
		{
			if ( IsFrozen )
			{
				if ( Input.Pressed( InputButton.Jump ) )
				{
					BaseVelocity = Vector3.Zero;
					WishVelocity = Vector3.Zero;
					IsFrozen = false;
				}

				return;
			}

			if ( Player is Player player )
			{
				player.Stamina = MathF.Min( player.Stamina + (10f * Time.Delta), 100f );
			}

			base.Tick();
		}
	}
}
