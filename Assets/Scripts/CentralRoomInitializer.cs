using UnityEngine;

public class CentralRoomInitializer : MonoBehaviour
{
    private RoomTemplates templates;

    void Awake()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms")
                             .GetComponent<RoomTemplates>();

        // Registrar la sala central en la lista y posiciones ocupadas
        templates.Rooms.Add(this.gameObject);
        templates.occupiedPositions.Add(this.transform.position);
        //templates.roomsCreated++; // ya hay una sala creada
    }
}
