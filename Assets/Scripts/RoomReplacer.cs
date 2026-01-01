using UnityEngine;
using System.Collections.Generic;

public class RoomReplacer : MonoBehaviour
{
    private RoomTemplates templates;
    private bool executed = false;

    void Start()
    {
        templates = GameObject.FindGameObjectWithTag("Rooms").GetComponent<RoomTemplates>();
    }

    void Update()
    {
        if (templates.generationFinished && !executed)
        {
            executed = true;
            Debug.Log($"[RoomSustituer] Generación finalizada. Total salas en lista: {templates.Rooms.Count}");
            CheckRooms();
        }
    }

    void CheckRooms()
    {
        List<GameObject> newRoomsList = new List<GameObject>();

        foreach (GameObject room in templates.Rooms)
        {
            string roomName = CleanName(room.name).ToUpper();
            string newRoomName = roomName;

            RoomSpawner[] spawners = room.GetComponentsInChildren<RoomSpawner>(true);
            Debug.Log($"[RoomSustituer] Revisando sala '{roomName}' con {spawners.Length} spawn points");

            foreach (RoomSpawner spawner in spawners)
            {
                // Si ya generó algo, lo ignoramos
                if (spawner.getSpawned())
                {
                    Debug.Log($"  Spawn point {spawner.openSide} YA generado → ignorado");
                    continue;
                }

                // ⚡ Revisar spawn points que no han generado nada
                Debug.Log($"  Spawn point {spawner.openSide} NO generado → ajustando nombre de sala");

                if (spawner.openSide == 1 && newRoomName.Contains("T"))
                {
                    newRoomName = RemoveLetter(newRoomName, 'T');
                    Debug.Log($"    Quitando 'T' → {newRoomName}");
                }
                if (spawner.openSide == 2 && newRoomName.Contains("B"))
                {
                    newRoomName = RemoveLetter(newRoomName, 'B');
                    Debug.Log($"    Quitando 'B' → {newRoomName}");
                }
                if (spawner.openSide == 3 && newRoomName.Contains("R"))
                {
                    newRoomName = RemoveLetter(newRoomName, 'R');
                    Debug.Log($"    Quitando 'R' → {newRoomName}");
                }
                if (spawner.openSide == 4 && newRoomName.Contains("L"))
                {
                    newRoomName = RemoveLetter(newRoomName, 'L');
                    Debug.Log($"    Quitando 'L' → {newRoomName}");
                }
            }

            if (newRoomName != roomName)
            {
                Debug.Log($"[RoomSustituer] Reemplazando sala '{roomName}' por '{newRoomName}'");
                GameObject newRoom = ReplaceRoom(room, newRoomName);
                if (newRoom != null)
                    newRoomsList.Add(newRoom);
            }
            else
            {
                Debug.Log($"[RoomSustituer] Sala '{roomName}' no necesita cambios");
                newRoomsList.Add(room);
            }
        }

        // ⚡ Sustituir lista original por la nueva lista revisada
        templates.Rooms = newRoomsList;
        Debug.Log("[RoomSustituer] Revisión finalizada. Lista de salas actualizada.");
    }

    GameObject ReplaceRoom(GameObject oldRoom, string newName)
    {
        GameObject prefab = FindRoomPrefab(newName);
        if (prefab == null)
        {
            Debug.LogWarning($"[RoomSustituer] No se encontró prefab para '{newName}'");
            return null;
        }

        GameObject newRoom = Instantiate(prefab, oldRoom.transform.position, oldRoom.transform.rotation, templates.roomsContainer);

        Destroy(oldRoom);

        Debug.Log($"[RoomSustituer] Sala '{oldRoom.name}' reemplazada por '{newRoom.name}'");
        return newRoom;
    }

    GameObject FindRoomPrefab(string name)
    {
        if (string.IsNullOrEmpty(name)) return null;

        char first = name[0];
        GameObject[] list = null;

        if (first == 'B') list = templates.bottomRooms;
        else if (first == 'T') list = templates.topRooms;
        else if (first == 'L') list = templates.leftRooms;
        else if (first == 'R') list = templates.rightRooms;

        if (list == null) return null;

        foreach (GameObject r in list)
            if (r.name.ToUpper() == name)
                return r;

        return null;
    }

    string RemoveLetter(string source, char letter)
    {
        return source.Replace(letter.ToString(), "");
    }

    string CleanName(string name)
    {
        int index = name.IndexOf("(");
        if (index >= 0)
            return name.Substring(0, index).Trim();
        return name;
    }
}
