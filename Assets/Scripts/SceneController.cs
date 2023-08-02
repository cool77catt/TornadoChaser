using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    [Header("Nav-Segments")]
    public int nActiveNavSegments = 10;
    public int nOldNavSegments = 2;
    public NavSegment[] navSegmentOptions;

    [Header("Debris")]
    public Debris[] debrisPrefabChoices;

    [Header("Tornado")]
    public float initialDistance = 150.0f;

    private List<NavSegment> _activeNavSegments = new List<NavSegment>();
    private List<NavSegment> _oldNavSegments = new List<NavSegment>();
    private Player _player;
    private Tornado _tornado;

    // Start is called before the first frame update
    void Start()
    {
        // Get a reference to the player and tornado
        _player = GameObject.FindObjectOfType<Player>();
        _tornado = GameObject.FindObjectOfType<Tornado>();

        NavSegment[] rogueSegments = FindObjectsOfType<NavSegment>();
        foreach (NavSegment seg in rogueSegments)
        {
            Destroy(seg.gameObject);
        }

        while (_activeNavSegments.Count < nActiveNavSegments)
        {
            // Genereate a new segment
            createNewNavSegment(true);
        }

        // Position the player on the first segment
        Vector3 playerPosition = (_activeNavSegments[1].transform.position
            + _activeNavSegments[0].transform.position) / 2.0f;
        Vector3 playerSize = _player.GetComponent<Collider>().bounds.size;
        playerPosition.y = playerSize.y / 1.5f;
        _player.transform.position = playerPosition;

        // Position the tornado
        _tornado.transform.position = Vector3.forward * initialDistance;
    }

    private void createNewNavSegment(bool populateDebris)
    {
        // Select a random segment option
        int randIndex = Random.Range(0, navSegmentOptions.Length);
        NavSegment seg = Instantiate<NavSegment>(navSegmentOptions[randIndex]);

        // Set the position of the segment
        if (_activeNavSegments.Count == 0)
        {
            seg.transform.position = Vector3.zero;
        }
        else
        {
            // Get the position of the previous segments box collider trigger
            NavSegment prevSeg = _activeNavSegments[_activeNavSegments.Count - 1];
            BoxCollider[] colliders = prevSeg.GetComponents<BoxCollider>();
            if (colliders.Length != 1)
            {
                Debug.Log("Error, previous segment does not conform to a single box collider at the root object");
            }
            Vector3 colliderPosition = prevSeg.transform.position + colliders[0].center;

            // Position the new segment to be butt-up to the previous one
            Vector3 newPosition = prevSeg.transform.position;
            newPosition.z = colliderPosition.z;
            seg.transform.position = newPosition;
        }

        // Populate debris
        //if (populateDebris && _activeNavSegments.Count > 0)
        //{
        //    seg.PopulateDebris(
        //        debrisPrefabChoices,
        //        1,
        //        _activeNavSegments[_activeNavSegments.Count-1]
        //    );
        //}

        // Add the segment to the list
        _activeNavSegments.Add(seg);
    }

    public void NavSegmentExited(NavSegment segOfInterest)
    {
        // Remove all segments prior to and leading up to the segment of interest
        // Done this way in case we somehow missed a segment, so that we don't
        // get out of sync.
        int segIndex = _activeNavSegments.IndexOf(segOfInterest);
        for (int i = 0; i <= segIndex; i++)
        {
            // Add to the old segments then remove form the active segments list
            _oldNavSegments.Add(_activeNavSegments[0]);
            _activeNavSegments.RemoveAt(0);
        }

        // Destroy old segments
        while (_oldNavSegments.Count > nOldNavSegments)
        {
            _oldNavSegments[0].DestroySegment();
            _oldNavSegments.RemoveAt(0);
        }

        // Generate a new segment, populate debris, and add to the list
        createNewNavSegment(true);
    }
}
