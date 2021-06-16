using Sandbox;

public class Undo
{
	public Client Creator;
	public Entity Prop;
	public float Time;
	public bool Avoid;

	public Undo( Client creator, Entity prop )
	{
		Creator = creator;
		Prop = prop;
		Time = Sandbox.Time.Now;
		Avoid = false;
	}
}
