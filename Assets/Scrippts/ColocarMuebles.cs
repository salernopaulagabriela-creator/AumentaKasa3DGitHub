using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public enum PlacementType { Floor, Wall }

[System.Serializable]
public class FurnitureItem
{
    public string name;
    public GameObject prefab;
    public PlacementType placement;
}

public class ColocarMuebles : MonoBehaviour
{
    public FurnitureItem[] items;
    private int selectedIndex = -1; // -1 = nada elegido todavía
    private ARRaycastManager raycastManager;
    private ARPlaneManager planeManager;
    private static List<ARRaycastHit> hits = new List<ARRaycastHit>();

    void Awake()
    {
        raycastManager = GetComponent<ARRaycastManager>();
        planeManager = GetComponent<ARPlaneManager>();
    }

     void Start()
    {
    selectedIndex = Seleccion.indice;
    }

    // Lo llaman los botones de la UI
    public void SelectFurniture(int index)
    {
        selectedIndex = index;
    }

   

    void Update()
    {
        if (selectedIndex < 0 || selectedIndex >= items.Length) return;

        Vector2 touchPos;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            // Ignorar toques sobre los botones
            if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)) return;
            touchPos = Input.GetTouch(0).position;
        }
        else if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current.IsPointerOverGameObject()) return;
            touchPos = Input.mousePosition;
        }
        else return;

        if (!raycastManager.Raycast(touchPos, hits, TrackableType.PlaneWithinPolygon))
            return;

        ARRaycastHit hit = hits[0];
        ARPlane plane = planeManager.GetPlane(hit.trackableId);
        if (plane == null) return;

        FurnitureItem item = items[selectedIndex];
        bool isWall = plane.alignment == PlaneAlignment.Vertical;

        if (item.placement == PlacementType.Wall && !isWall) return;
        if (item.placement == PlacementType.Floor && isWall) return;

        Quaternion rotation = isWall
            ? Quaternion.LookRotation(-plane.normal, Vector3.up)
            : hit.pose.rotation;

        Instantiate(item.prefab, hit.pose.position, rotation);
    }
}