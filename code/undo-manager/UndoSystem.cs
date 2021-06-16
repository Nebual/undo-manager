﻿using MinimalExtended;
using Sandbox;


namespace UndoManager
{
	public partial class UndoManager : IAutoload
	{
		public bool ReloadOnHotload { get; } = false;
		public UndoManager()
		{
			Sandbox.Hooks.Entities.OnSpawned -= OnSpawned;
			Sandbox.Hooks.Entities.OnSpawned += OnSpawned;
		}

		private void OnSpawned( Entity spawned, Entity owner )
		{
			if ( owner is Player player ) {
				if ( player.GetActiveController().Client is Client clientOwner ) {
					Undoer.Add( clientOwner, spawned );
				}
			}
		}

		static readonly SoundEvent HitSound = new( "sounds/balloon_pop_cute.vsnd" ) {
			Volume = 0.5f,
			DistanceMax = 500.0f
		};

		[ServerCmd( "undo" )]
		public static async void OnUndo()
		{
			Client client = ConsoleSystem.Caller;

			if ( client == null )
				return;

			foreach ( Undo undo in Undoer.Get( client.SteamId ).ToArray() ) {
				Client creator = undo.Creator;
				Entity prop = undo.Prop;
				float time = undo.Time;

				if ( undo.Avoid ) continue;
				if ( !prop.IsValid() ) {
					Undoer.DoUndo( creator, prop, undo );

					continue;
				}

				CreateUndoParticles( To.Single( creator ), prop.Position );

				Redo redo = Redoer.Add( creator, prop, undo );

				undo.Avoid = true;

				Undoer.OnTrashbin( To.Single( creator ) );
				Undoer.HideProp( undo );

				await prop.Task.DelaySeconds( Redoer.Duration );
				if ( undo.Time + Redoer.Duration < Time.Now )
					Undoer.DoUndo( creator, prop, undo, redo );

				break;
			}
		}

		[ServerCmd( "redo" )]
		public static void OnRedo()
		{
			Client client = ConsoleSystem.Caller;

			if ( client == null )
				return;

			Entity pawn = client.Pawn;

			foreach ( Redo redo in Redoer.Get( client.SteamId ) ) {
				Client creator = redo.Creator;
				Entity prop = redo.Prop;
				Undo undo = redo.Undo;

				if ( !prop.IsValid() ) {
					Undoer.DoUndo( creator, prop, undo, redo );

					continue;
				}

				CreateUndoParticles( To.Single( creator ), prop.Position );

				Redoer.OnRedone( To.Single( creator ) );
				Redoer.ResetProp( redo );
				Redoer.Remove( creator, redo );

				undo.Avoid = false;
				undo.Time = Time.Now;

				break;
			}
		}

		[ClientRpc]
		public static void CreateUndoParticles( Vector3 pos )
		{
			using ( Prediction.Off() ) {
				Particles.Create( "particles/physgun_freeze.vpcf", pos + Vector3.Up * 2 );
				Local.Pawn?.PlaySound( HitSound.Name );
			}
		}

		public void Dispose()
		{
		}
	}
}