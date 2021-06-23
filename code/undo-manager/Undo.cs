using System;
using Sandbox;

public class Undo
{
	public Client Creator;
	public Entity Prop;
	public Func<string> Undoable;
	public float Time;
	public bool Avoid;

	public Undo( Client creator )
	{
		Creator = creator;
		Time = Sandbox.Time.Now;
		Avoid = false;
	}
	public Undo( Client creator, Entity prop ): this(creator)
	{
		Prop = prop;
	}
}
