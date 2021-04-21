using Sandbox;
using System;
using System.Linq;

partial class SandboxPlayer : BasePlayer
{
	TimeSince timeSinceDropped;

	[Net]
	public PlayerController VehicleController { get; set; }

	[Net]
	public Camera VehicleCamera { get; set; }

	[Net]
	public PlayerAnimator VehicleAnimator { get; set; }

	public SandboxPlayer()
	{
		Inventory = new BaseInventory( this );
	}

	public override void Respawn()
	{
		SetModel( "models/citizen/citizen.vmdl" );

		Controller = new WalkController();
		Animator = new StandardPlayerAnimator();
		Camera = new FirstPersonCamera();

		EnableAllCollisions = true;
		EnableDrawing = true;
		EnableHideInFirstPerson = true;
		EnableShadowInFirstPerson = true;

		base.Respawn();
	}

	public override void OnKilled()
	{
		base.OnKilled();

		//
		Inventory.DropActive();

		//
		// Delete any items we didn't drop
		//
		Inventory.DeleteContents();

		BecomeRagdollOnClient();

		Controller = null;
		Camera = new SpectateRagdollCamera();

		EnableAllCollisions = false;
		EnableDrawing = false;
	}

	public override PlayerController GetActiveController()
	{
		if ( DevController != null ) return DevController;
		if ( VehicleController != null ) return VehicleController;

		return base.GetActiveController();
	}

	public override Camera GetActiveCamera()
	{
		if ( DevCamera != null ) return DevCamera;
		if ( VehicleCamera != null ) return VehicleCamera;

		return base.GetActiveCamera();
	}

	public override PlayerAnimator GetActiveAnimator()
	{
		if ( VehicleAnimator != null ) return VehicleAnimator;

		return base.GetActiveAnimator();
	}


	TimeSince timeSinceJumpReleased;

	protected override void Tick()
	{
		base.Tick();

		if ( Input.ActiveChild != null )
		{
			ActiveChild = Input.ActiveChild;
		}

		if ( LifeState != LifeState.Alive )
			return;

		TickPlayerUse();

		if ( Input.Pressed( InputButton.View ) )
		{
			if ( !(Camera is FirstPersonCamera) )
			{
				Camera = new FirstPersonCamera();
			}
			else
			{
				// This is here for now so I can use it for recording clips etc
				if ( Input.Down( InputButton.Use ) )
				{
					var startPos = EyePos;
					var dir = EyeRot.Forward;

					var tr = Trace.Ray( startPos, startPos + dir * 10000.0f )
						.Ignore( this )
						.Run();

					if ( tr.Hit && tr.Entity.IsValid() && !tr.Entity.IsWorld )
					{
						Camera = new LookAtCamera
						{
							Origin = startPos,
							TargetEntity = tr.Entity
						};
					}
				}
				else
				{
					Camera = new ThirdPersonCamera();
				}
			}
		}

		if ( Input.Pressed( InputButton.Drop ) )
		{
			var dropped = Inventory.DropActive();
			if ( dropped != null )
			{
				timeSinceDropped = 0;
			}
		}

		if ( Input.Released( InputButton.Jump ) )
		{
			if ( timeSinceJumpReleased < 0.3f )
			{
				(Game.Current as Game)?.DoPlayerNoclip( this );
			}

			timeSinceJumpReleased = 0;
		}

		if ( Input.Left != 0 || Input.Forward != 0 )
		{
			timeSinceJumpReleased = 1;
		}
	}

	static EntityLimit RagdollLimit = new EntityLimit { MaxTotal = 20 };

	[ClientRpc]
	void BecomeRagdollOnClient()
	{
		var ent = new AnimEntity();
		ent.WorldPos = WorldPos;
		ent.WorldRot = WorldRot;
		ent.MoveType = MoveType.Physics;
		ent.UsePhysicsCollision = true;
		ent.EnableAllCollisions = true;
		ent.CollisionGroup = CollisionGroup.Debris;
		ent.SetModel( GetModelName() );
		ent.CopyBonesFrom( this );
		ent.SetRagdollVelocityFrom( this, 0.1f, 1, 1 );
		ent.EnableHitboxes = true;
		ent.EnableAllCollisions = true;
		ent.SurroundingBoundsMode = SurroundingBoundsType.Physics;

		foreach ( var child in Children )
		{
			if ( child is ModelEntity e )
			{
				var model = e.GetModelName();
				if ( model != null && !model.Contains( "clothes" ) )
					continue;

				var clothing = new ModelEntity();
				clothing.SetModel( model );
				clothing.SetParent( ent, true );
			}
		}

		Corpse = ent;

		RagdollLimit.Watch( ent );
	}


	public override void StartTouch( Entity other )
	{
		if ( timeSinceDropped < 1 ) return;

		base.StartTouch( other );
	}

	[ServerCmd( "inventory_current" )]
	public static void SetInventoryCurrent( string entName )
	{
		var target = ConsoleSystem.Caller;
		if ( target == null ) return;

		var inventory = target.Inventory;
		if ( inventory == null )
			return;

		for ( int i = 0; i < inventory.Count(); ++i )
		{
			var slot = inventory.GetSlot( i );
			if ( !slot.IsValid() )
				continue;

			if ( !slot.ClassInfo.IsNamed( entName ) )
				continue;

			inventory.SetActiveSlot( i, false );

			break;
		}
	}

	public override bool HasPermission( string mode )
	{
		if ( mode == "noclip" ) return true;
		if ( mode == "devcam" ) return true;
		if ( mode == "suicide" ) return true;

		return base.HasPermission( mode );
	}
}
