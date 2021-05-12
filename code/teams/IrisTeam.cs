using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sandbox;

namespace HiddenGamemode
{
    class IrisTeam : BaseTeam
	{
		public override string HudClassName => "team_iris";
		public override string Name => "Runners";

		public override void OnStart( Player player )
		{

			player.SetModel( "models/citizen/citizen.vmdl" );

			if ( Host.IsServer )
			{
				player.RemoveClothing();
				//player.AttachClothing( "models/citizen_clothes/trousers/trousers.lab.vmdl" );
				//player.AttachClothing( "models/citizen_clothes/jacket/labcoat.vmdl" );
				//player.AttachClothing( "models/citizen_clothes/shoes/shoes.workboots.vmdl" );
				//player.AttachClothing( "models/citizen_clothes/hat/hat_securityhelmet.vmdl" );

				// Pants
				var model = Rand.FromArray(new[]
				{
					"models/citizen_clothes/trousers/trousers.jeans.vmdl",
					"models/citizen_clothes/trousers/trousers.lab.vmdl",
					"models/citizen_clothes/trousers/trousers.police.vmdl",
					"models/citizen_clothes/trousers/trousers.smart.vmdl",
					"models/citizen_clothes/trousers/trousers.smarttan.vmdl",
					"models/citizen/clothes/trousers_tracksuit.vmdl",
					"models/citizen_clothes/trousers/trousers_tracksuitblue.vmdl",
					"models/citizen_clothes/trousers/trousers_tracksuit.vmdl",
					"models/citizen_clothes/shoes/shorts.cargo.vmdl",
				});
				if (model != "") { player.AttachClothing(model); }

				// Jacket
				model = Rand.FromArray(new[]
				{
					"models/citizen_clothes/jacket/labcoat.vmdl",
					"models/citizen_clothes/jacket/jacket.red.vmdl",
					"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
					"models/citizen_clothes/jacket/jacket_heavy.vmdl",

					"models/citizen_clothes/jacket/labcoat.vmdl",
					"models/citizen_clothes/jacket/jacket.red.vmdl",
					"models/citizen_clothes/jacket/jacket.tuxedo.vmdl",
					"models/citizen_clothes/jacket/jacket_heavy.vmdl",

					"models/citizen_clothes/vest/vest.highvis.vmdl",
					"",
				});
				if (model != "") { player.AttachClothing(model); }

				// Shoes
				model = Rand.FromArray(new[]
				{
					"models/citizen_clothes/shoes/trainers.vmdl",
					"models/citizen_clothes/shoes/shoes.workboots.vmdl",
				});
				if (model != "") { player.AttachClothing(model); }

				// Hat
				model = Rand.FromArray(new[]
				{
					"models/citizen_clothes/hair/hair_femalebun.black.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.blonde.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.brown.vmdl",
					"models/citizen_clothes/hair/hair_femalebun.red.vmdl",

					"models/citizen_clothes/hair/hair_malestyle01.blonde.vmdl",
					"models/citizen_clothes/hair/hair_malestyle02.vmdl",
					"models/citizen_clothes/hair/hair_malestyle03.vmdl",

					"models/citizen_clothes/hat/hat_hardhat.vmdl",
					"models/citizen_clothes/hat/hat_woolly.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmet.vmdl",
					"models/citizen_clothes/hat/hat_beret.red.vmdl",
					"models/citizen_clothes/hat/hat.tophat.vmdl",
					"models/citizen_clothes/hat/hat_beret.black.vmdl",
					"models/citizen_clothes/hat/hat_cap.vmdl",
					"models/citizen_clothes/hat/hat_leathercap.vmdl",
					"models/citizen_clothes/hat/hat_leathercapnobadge.vmdl",
					"models/citizen_clothes/hat/hat_securityhelmetnostrap.vmdl",
					"models/citizen_clothes/hat/hat_service.vmdl",
					"models/citizen_clothes/hat/hat_uniform.police.vmdl",
					"models/citizen_clothes/hat/hat_woollybobble.vmdl",
					"",
				});
				if (model != "") { player.AttachClothing(model); }

				// Rare
				model = Rand.FromArray(new[]
				{
					"models/citizen_clothes/beards/beard_trucker_brown.vmdl",
					"models/citizen_clothes/accessory/unibrow.vmdl",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
					"",
				});
				if (model != "") { player.AttachClothing(model); }
			}

			player.EnableAllCollisions = true;
			player.EnableDrawing = true;
			player.EnableHideInFirstPerson = true;
			player.EnableShadowInFirstPerson = true;

			player.Controller = new IrisController();
			player.Camera = new FirstPersonCamera();
		}

		public override void OnJoin( Player player  )
		{
			Log.Info( $"{player.Name} joined the Runner team." );

			base.OnJoin( player );
		}

		public override void OnPlayerKilled( Player player )
		{
			player.GlowActive = false;
		}

		public override void OnLeave( Player player  )
		{
			Log.Info( $"{player.Name} left the Runner team." );

			base.OnLeave( player );
		}
	}
}
