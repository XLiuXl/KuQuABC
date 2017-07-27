using UnityEngine;
using System.Collections;

public class UIDragManager : MonoBehaviour {

    public Transform target;

    private Vector3 offset;

    private Bounds bounds;

     

    void OnPress(bool pressed)

    {

        if (target == null) return;

        if (pressed) 

        {

            bounds = NGUIMath.CalculateRelativeWidgetBounds(target.transform);

            Vector3 position = UICamera.currentCamera.WorldToScreenPoint(target.position);

            offset = new Vector3(Input.mousePosition.x - (position.x - bounds.size.x / 2), Input.mousePosition.y - (position.y - bounds.size.y / 2),0f);

        }

    }

     

    void OnDrag(Vector2 delta)

    {

        Vector3 currentPoint = new Vector3 (Input.mousePosition.x - offset.x, Input.mousePosition.y - offset.y, 0f);


        if (currentPoint.x < 0) 

        {

            currentPoint.x = 0;

        }

        if (currentPoint.x + bounds.size.x > Screen.width) 

        {

            currentPoint.x = Screen.width - bounds.size.x;

        }

        if (currentPoint.y < 0) 

        {

            currentPoint.y = 0;

        }
	
       
        if (currentPoint.y + bounds.size.y > Screen.height) 

        {

            currentPoint.y = Screen.height - bounds.size.y;

        }


        currentPoint.x += bounds.size.x / 2;

        currentPoint.y += bounds.size.y / 2;

        target.position = UICamera.currentCamera.ScreenToWorldPoint (currentPoint);

    }
}
