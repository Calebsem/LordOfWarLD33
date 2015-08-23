using UnityEngine;
using System.Collections;

public class CameraInteraction : MonoBehaviour {
    public Camera Camera;
    public Vector3 CenterPoint;

    private Vector3 lastMousePosition;
    public float DistanceFromPoint = 30f;
    public float Angle = 0;
    private const float borderOffset = 30f;
	// Use this for initialization
	void Start () {
        Camera = GetComponent<Camera>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            return;
        if (Input.GetMouseButton(0) && !Input.GetMouseButtonDown(0))
        {
            var zoomMultiplier = DistanceFromPoint / 30f / 5f;
            var deltaX = Input.mousePosition.x - lastMousePosition.x;
            var deltaY = Input.mousePosition.y - lastMousePosition.y;
            var transDelta = transform.TransformDirection(deltaX, 0, deltaY);
            CenterPoint += new Vector3(-transDelta.x * zoomMultiplier, 0, -transDelta.z * zoomMultiplier);
            CenterPoint = new Vector3(Mathf.Clamp(CenterPoint.x, -borderOffset, borderOffset + 7.5f), 0, Mathf.Clamp(CenterPoint.z, -borderOffset, borderOffset + 7.5f));
        }
        if(Input.mouseScrollDelta.y != 0)
        {
            DistanceFromPoint -= Input.mouseScrollDelta.y;
            DistanceFromPoint = Mathf.Clamp(DistanceFromPoint, 10, 40);
        }
        if (Input.GetMouseButton(1))
        {
            var delta = Input.mousePosition.x - lastMousePosition.x;
            Angle -= delta / 100f;
        }
        lastMousePosition = Input.mousePosition;
        transform.position = new Vector3(
            CenterPoint.x + Mathf.Cos(Angle) * DistanceFromPoint,
            DistanceFromPoint * 2,
            CenterPoint.z + Mathf.Sin(Angle) * DistanceFromPoint);
        transform.LookAt(CenterPoint);
    }
}
