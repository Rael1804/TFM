using UnityEngine;
using System.Collections.Generic;

public class ChooseRoom : MonoBehaviour
{
    public RoomTemplates templates;

    [Header("Materials")]
    public Material kitchenMat;
    public Material livingRoomMat;
    public Material bathroomMat;
    public Material libraryMat;
    public Material storageMat;
    public Material laundryMat;
    public Material combatRoomMat;
    public Material gymMat;
    public Material mirrorRoomMat;
    public Material armoryMat;
    public Material pharmacyMat;
    public Material greenHouseMat;

    private bool executed = false;
    private const int ROOMS_TO_IGNORE = 12;

    void Update()
    {
        if (templates.generationFinished && !executed)
        {
            executed = true;
            AssignRooms();
        }
    }

    void AssignRooms()
    {
        // 🔹 Solo rooms reales (ignoramos las 12 primeras)
        List<GameObject> rooms = templates.Rooms.GetRange(
            ROOMS_TO_IGNORE,
            templates.Rooms.Count - ROOMS_TO_IGNORE
        );

        // 🔹 Dead rooms = nombre de una sola letra
        List<GameObject> deadRooms = new List<GameObject>();
        List<GameObject> normalRooms = new List<GameObject>();

        foreach (GameObject room in rooms)
        {
            if (CleanName(room.name).Length == 1)
                deadRooms.Add(room);
            else
                normalRooms.Add(room);
        }

        // 🔹 Tipos obligatorios
        AssignMandatory(RoomType.Kitchen, deadRooms);
        AssignMandatory(RoomType.Bathroom, deadRooms);
        AssignMandatory(RoomType.LivingRoom, rooms);

        // 🔹 Tipos restantes
        List<RoomType> remainingTypes = new List<RoomType>()
        {
            RoomType.Library,
            RoomType.Storage,
            RoomType.Laundry,
            RoomType.CombatRoom,
            RoomType.Gym,
            RoomType.MirrorRoom,
            RoomType.Armory,
            RoomType.Pharmacy,
            RoomType.GreenHouse
        };

        Shuffle(remainingTypes);

        // 🔹 Asignar el resto con variedad
        foreach (GameObject room in rooms)
        {
            Room roomComp = room.GetComponent<Room>();
            if (roomComp != null && roomComp.roomType != 0)
                continue;

            RoomType chosen;

            if (remainingTypes.Count > 0)
            {
                chosen = remainingTypes[0];
                remainingTypes.RemoveAt(0);
            }
            else
            {
                chosen = RoomType.LivingRoom;
            }

            SetRoom(room, chosen);
        }

        Debug.Log("✔ Rooms asignadas correctamente");
    }

    void AssignMandatory(RoomType type, List<GameObject> possibleRooms)
    {
        if (possibleRooms.Count == 0)
        {
            Debug.LogError($"❌ No hay salas válidas para {type}");
            return;
        }

        GameObject room = possibleRooms[Random.Range(0, possibleRooms.Count)];
        possibleRooms.Remove(room);

        SetRoom(room, type);
    }

    void SetRoom(GameObject room, RoomType type)
    {
        Room r = room.GetComponent<Room>();

        if (r.roomType != RoomType.None)
        {
            Debug.LogWarning($"⚠ {room.name} ya era {r.roomType}, intento de asignar {type} ignorado");
            return;
        }

        r.roomType = type;
        ApplyMaterial(room, type);

        Debug.Log($"✔ {room.name} -> {type}");
    }


    void ApplyMaterial(GameObject room, RoomType type)
    {
        Material mat = GetMaterial(type);
        if (mat == null) return;

        Transform wall = room.transform.Find("Wall");
        if (wall == null)
        {
            Debug.LogWarning($"⚠ No Wall en {room.name}");
            return;
        }

        foreach (MeshRenderer r in wall.GetComponentsInChildren<MeshRenderer>())
        {
            r.material = mat;
        }
    }

    Material GetMaterial(RoomType type)
    {
        switch (type)
        {
            case RoomType.Kitchen: return kitchenMat;
            case RoomType.LivingRoom: return livingRoomMat;
            case RoomType.Bathroom: return bathroomMat;
            case RoomType.Library: return libraryMat;
            case RoomType.Storage: return storageMat;
            case RoomType.Laundry: return laundryMat;
            case RoomType.CombatRoom: return combatRoomMat;
            case RoomType.Gym: return gymMat;
            case RoomType.MirrorRoom: return mirrorRoomMat;
            case RoomType.Armory: return armoryMat;
            case RoomType.Pharmacy: return pharmacyMat;
            case RoomType.GreenHouse: return greenHouseMat;
        }
        return null;
    }

    string CleanName(string name)
    {
        int i = name.IndexOf("(");
        return i >= 0 ? name.Substring(0, i).Trim() : name;
    }

    void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            (list[i], list[rnd]) = (list[rnd], list[i]);
        }
    }
}
