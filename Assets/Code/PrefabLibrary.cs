//GENERATED CLASS FROM PrefabLibraryGenerator.cs

using UnityEngine;

[DefaultExecutionOrder(-1000)]//make it run first!
public class PrefabLibrary : MonoBehaviour
{
	public BouncyThing Ball;
	public Horse Horse;
	public Texture2D[] Paintings;
	public GameObject[] Shapes;

	public static PrefabLibrary instance;

	public void Awake()
	{
		instance = this;
	}

}