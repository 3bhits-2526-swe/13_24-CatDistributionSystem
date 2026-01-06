using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    private Dictionary<BuildingBehaviour, List<BuildingBehaviour>> connections =
        new Dictionary<BuildingBehaviour, List<BuildingBehaviour>>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public bool ConnectBuildings(BuildingBehaviour from, BuildingBehaviour to)
    {
        if (from == null || to == null) return false;

        if (!connections.ContainsKey(from))
            connections[from] = new List<BuildingBehaviour>();

        if (!connections[from].Contains(to))
        {
            connections[from].Add(to);
            return true;
        }

        return false;
    }

    public void DisconnectBuildings(BuildingBehaviour from, BuildingBehaviour to)
    {
        if (connections.ContainsKey(from))
            connections[from].Remove(to);
    }

    public List<BuildingBehaviour> GetConnections(BuildingBehaviour building)
    {
        if (connections.ContainsKey(building))
            return connections[building];
        return new List<BuildingBehaviour>();
    }
}