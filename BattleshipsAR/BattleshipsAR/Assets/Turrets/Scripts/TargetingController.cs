using UnityEngine;

public class TargetingController : MonoBehaviour
{
    public TurretControllerV3[] turrets;

    RaycastHit hittt;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.CompareTag("PlayArea"))
                {
                    //var position = hit.point;

                    //// account for difference in scale
                    //position.Scale(Camera.main.transform.parent.localScale);

                    //// account for difference in rotation
                    //position = Camera.main.transform.parent.rotation * position;
                    Debug.Log(hit.point);
                    hittt = hit;
                    foreach (var turret in turrets)
                        turret.TakeAim(hit.point);
                }                   
                
            }
        }        
    }

    public void Shoot()
    {
        for (int i = 0; i < turrets.Length; i++)
        {
            turrets[i].FireProjectile();
        }
    }

    private void OnGUI()
    {
        GUIStyle myStyle = new GUIStyle();
        myStyle.fontSize = 40;
        GUI.Label(new Rect(30, 70, 100, 100), "Aim hit point: " + hittt.point.ToString(), myStyle);
        //GUILayout.Label(currentState.ToString());
    }
}
