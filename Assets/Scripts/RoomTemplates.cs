using UnityEngine;
using System.Collections.Generic;

public class RoomTemplates : MonoBehaviour
{
    public GameObject[] bottomRooms;
    public GameObject[] topRooms;
    public GameObject[] rightRooms;
    public GameObject[] leftRooms;

    public List<GameObject> Rooms = new List<GameObject>();
    public HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();

    public int roomsCreated = 0;
    public int maxRooms = 20;
    public bool generationFinished = false;

    public Transform roomsContainer;

    public GameObject startRoom;
}
