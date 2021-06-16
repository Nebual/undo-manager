using System;
using System.Collections.Generic;
using MinimalExtended;
using Sandbox;

namespace UndoManager
{
	[Library( "undo-manager" )]
	public class AddonInfo : BaseAddonInfo
	{
		public override string Name => "UndoManager";

		public override string Description => "Undo/Redo Manager";

		public override string Author => "Argonium";

		public override double Version => 1.0;

		public override List<AddonDependency> Dependencies => new() {
			new AddonDependency() {
				Name = "SandboxPlus",
				MinVersion = 1.0
			},
		};
	}
	public partial class UndoManager : AddonClass<AddonInfo>{ }
}
