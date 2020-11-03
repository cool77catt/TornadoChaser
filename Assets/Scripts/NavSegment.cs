using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavSegment : MonoBehaviour
{
    private List<Debris> _debrisList = new List<Debris>();

    private Vector3 _size;

    public void Start()
    {
        //_size = GetComponents<>
    }

    public void PopulateDebris(Debris[] debrisPrefabChoices, int maxElements, NavSegment prevSegment)
    {
        if (debrisPrefabChoices.Length == 0)
        {
            return;
        }

        for (int i = 0; i < maxElements; i++)
        {
            // Generate a new debris object
            Debris d = Instantiate<Debris>(
                debrisPrefabChoices[Random.Range(0, debrisPrefabChoices.Length)],
                this.transform
            );
            
            // Perform a random orientation
            Vector3 rotation = new Vector3(
                0.0f,
                Random.Range(-180.0f, 180.0f),
                Random.Range(0, 4) * 90
            );
            d.transform.rotation = Quaternion.Euler(rotation);

            // Place the object
            Vector3 debrisSize = d.GetComponent<Collider>().bounds.size;
            d.transform.localPosition = new Vector3(
                0,
                debrisSize.y / 2.0f,
                0
            );

            // Save the object to the list
            _debrisList.Add(d);
        }
    }

    public void DestroySegment()
    {
        // Destroy any debris that was created
        foreach (Debris d in _debrisList)
        {
            Destroy(d.gameObject);
        }

        // Destroy the game object
        Destroy(gameObject);
    }
}
