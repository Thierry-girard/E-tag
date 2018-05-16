using UnityEngine;
using System.Collections.Generic;

public class ParticleHit : MonoBehaviour {

	public GameObject collisionResult;

	public GameObject paintingPrefab;

    public GameObject selectedColor;

	private Dictionary<string,GameObject> paintingObject;

	private ParticleSystem playerParticle;

	private Color quadColor;

    //TODO
    //Réduire nb particules, réduire radius
    //Voir pb luminosité quad
    //Récupérer position main

	// Use this for initialization
	void Start () {
		playerParticle = gameObject.GetComponent<ParticleSystem> ();
		quadColor = Color.red;
		paintingObject = new Dictionary<string,GameObject> ();
	}
	
	// Update is called once per frame
	void Update () {
		quadColor = selectedColor.GetComponent<CanvasRenderer>().GetColor();

        ParticleSystem.MainModule particleModule = playerParticle.main;
        particleModule.startColor = selectedColor.GetComponent<CanvasRenderer>().GetColor();
	}

	private GameObject GetParentObject(string color) {
		if (!paintingObject.ContainsKey (color)) {

			GameObject painting = Instantiate<GameObject>(paintingPrefab);
			painting.GetComponent<MeshFilter>().mesh = new Mesh();
            Color outColor;
            ColorUtility.TryParseHtmlString(color,out outColor);
            painting.name = "Painting " + color;
            painting.GetComponent<MeshRenderer>().material.color = outColor;

            paintingObject[color] = painting;
		}
		return paintingObject [color];
	}

    public void OnParticleCollision(GameObject other)
	{
        List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

        int numCollisionEvents = playerParticle.GetCollisionEvents(other, collisionEvents);

        for (int i = 0; i < numCollisionEvents; i++)
        {
            Vector3 collisionHitLoc = collisionEvents[i].intersection;

            GameObject parentPainting = GetParentObject(ColorUtility.ToHtmlStringRGB(quadColor));
            //Create quad
            GameObject result = collisionResult;
            //Position
            result.transform.position = collisionHitLoc;
            //Rotation
            result.transform.LookAt(collisionHitLoc + -1 * collisionEvents[i].normal);
            //Set color and assign to parent
            result.transform.parent = parentPainting.transform;

            //Children
            var resultMesh = result.GetComponent<MeshFilter>();
            CombineInstance combine = new CombineInstance();
            combine.mesh = resultMesh.sharedMesh;
            combine.transform = result.transform.localToWorldMatrix * parentPainting.transform.worldToLocalMatrix;

            //Parent
            var paintingMesh = parentPainting.GetComponent<MeshFilter>();
            CombineInstance current = new CombineInstance();
            current.mesh = paintingMesh.sharedMesh;
            current.transform = parentPainting.transform.localToWorldMatrix;

            paintingMesh.sharedMesh = new Mesh();
            paintingMesh.sharedMesh.CombineMeshes(new CombineInstance[] { combine, current });

            paintingMesh.GetComponent<MeshRenderer>().material.color = quadColor;

            //Destroy(result);
        }
	}
}

