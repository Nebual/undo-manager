﻿using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Sandbox
{
	partial class Redoer
	{
		private static Dictionary<ulong, List<Redo>> Redos = new Dictionary<ulong, List<Redo>>();
		public static float Duration = 5f;

		public static Redo Add( Client creator, Entity prop, Undo undo )
		{
			if ( !Redos.ContainsKey( creator.SteamId ) )
				Redos.Add( creator.SteamId, new List<Redo>() );

			Redo redo = new Redo( creator, prop, undo );

			Redos[creator.SteamId].Insert( 0, redo );

			return redo;
		}

		public static List<Redo> Get( ulong id )
		{
			if ( !Redos.ContainsKey( id ) )
				Redos.Add( id, new List<Redo>() );

			return Redos[id];
		}

		public static bool Remove( Client creator, Redo redo )
		{
			if ( !Redos.ContainsKey( creator.SteamId ) )
				Redos.Add( creator.SteamId, new List<Redo>() );

			return Redos[creator.SteamId].Remove( redo );
		}

		public static void ResetProp( Redo redo )
		{
			Entity prop = redo.Prop;

			prop.Position = redo.Position;
			prop.Rotation = redo.Rotation;
			prop.Velocity = redo.Velocity;
			prop.EnableDrawing = true;

			if ( prop is ModelEntity modelProp )
			{
				modelProp.EnableAllCollisions = true;
				modelProp.PhysicsEnabled = true;
			}
		}

		[ClientRpc]
		public static void OnRedone()
		{
			ChatBox.AddChatEntry( "Redo", "Successfully Redone.", $"avatar:{Local.SteamId}" );
		}
	}
}
