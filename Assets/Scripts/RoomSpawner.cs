using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public int openSide; // 1=bottom, 2=top, 3=right, 4=left
    private RoomTemplates templates;
    private bool spawned = false;

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
        Invoke("Spawn", 0.1f);
    }

    void Spawn()
    {
        if (spawned || templates.roomsCreated >= templates.maxRooms) return;

        // Evitar generar sala encima de otra
        if (templates.occupiedPositions.Contains(transform.position)) return;

        GameObject prefab = null;
        switch (openSide)
        {
            case 1: prefab = templates.bottomRooms[Random.Range(0, templates.bottomRooms.Length)]; break;
            case 2: prefab = templates.topRooms[Random.Range(0, templates.topRooms.Length)]; break;
            case 3: prefab = templates.leftRooms[Random.Range(0, templates.leftRooms.Length)]; break;
            case 4: prefab = templates.rightRooms[Random.Range(0, templates.rightRooms.Length)]; break;
        }
        if(templates.roomsCreated == 0)
        {
            prefab = templates.startRoom;
        }
        if (prefab != null)
        {
            GameObject newRoom = Instantiate(prefab, transform.position, prefab.transform.rotation, templates.roomsContainer);


            spawned = true;
            templates.roomsCreated++;
            templates.occupiedPositions.Add(transform.position);

            // Marcar spawn points opuestos (padre-hijo)
            RoomSpawner[] newSpawners = newRoom.GetComponentsInChildren<RoomSpawner>();
            foreach (RoomSpawner sp in newSpawners)
                if (IsOppositeSide(sp.openSide, this.openSide))
                    sp.setSpawned(true);
        }

        if (templates.roomsCreated >= templates.maxRooms)
        {
            templates.generationFinished = true;

            // ⚡ Aquí añadimos todas las salas de la escena a la lista original
            templates.Rooms.Clear();
            GameObject[] allRooms = GameObject.FindGameObjectsWithTag("Room");
            foreach (GameObject r in allRooms)
                templates.Rooms.Add(r);

            Debug.Log($"[RoomSpawner] Todas las salas añadidas a la lista: {templates.Rooms.Count}");
        }
    }

    bool IsOppositeSide(int side1, int side2)
    {
        return (side1 == 1 && side2 == 2) || (side1 == 2 && side2 == 1) ||
               (side1 == 3 && side2 == 4) || (side1 == 4 && side2 == 3);
    }

    public bool getSpawned() => spawned;
    public void setSpawned(bool value) => spawned = value;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SpawnPoint"))
        {
            // No destruimos spawn points aquí, solo usamos la lista ocupada
        }
    }
}
