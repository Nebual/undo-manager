using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Sandbox
{
	partial class Undoer
	{
		private static Dictionary<ulong, List<Undo>> Undos = new Dictionary<ulong, List<Undo>>();

		public static Undo Add( Client creator, Entity prop )
		{
			if ( !Undos.ContainsKey( creator.SteamId ) )
				Undos.Add( creator.SteamId, new List<Undo>() );

			Undo undo = new Undo( creator, prop );

			Undos[creator.SteamId].Insert( 0, undo );

			return undo;
		}

		public static List<Undo> Get( ulong id )
		{
			if ( !Undos.ContainsKey(id) )
				Undos.Add(id, new List<Undo>());

			return Undos[id];
		}

		public static bool Remove( Client creator, Undo undo )
		{
			if ( !Undos.ContainsKey( creator.SteamId ) )
				Undos.Add( creator.SteamId, new List<Undo>() );

			return Undos[creator.SteamId].Remove( undo );
		}

		public static void DoUndo( Client creator, Entity prop, Undo undo, Redo redo = null )
		{
			Undoer.Remove( creator, undo );

			if ( redo != null )
				Redoer.Remove( creator, redo );

			if ( !prop.IsValid() ) return;

			OnUndone( To.Single( creator ) );

			prop.Delete();
		}

		public static void HideProp( Undo undo )
		{
			Entity prop = undo.Prop;

			prop.EnableDrawing = false;

			if ( prop is ModelEntity modelProp )
			{
				modelProp.EnableAllCollisions = false;
				modelProp.PhysicsEnabled = false; // todo: this isn't ideal for constrained ents (eg. wheels)
			}
		}

		[ClientRpc]
		public static void OnTrashbin()
		{
			ChatBox.AddChatEntry( "Undo", "Successfully Moved to Trashbin. Use *redo* to revert.", $"avatar:{Local.SteamId}" );
		}

		[ClientRpc]
		public static void OnUndone()
		{
			ChatBox.AddChatEntry( "Undo", "Successfully Undone.", $"avatar:{Local.SteamId}" );
		}
	}
}
