namespace AMaze.Entities;

internal interface IEntity
{
	bool Intersect(Geometry.Seg sight, out ScanIntersection intersection);

	bool DoesCollide(Geometry.Rect rect);
}

internal interface IInteractable
{
	void Interact();
}

internal interface IDynamic
{
	void Tick();
}
