using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraControls : MonoBehaviour
{
	const string INPUT_MOUSE_SCROLLWHEEL = "Mouse ScrollWheel";
	
	const float MIN_CAM_DISTANCE = 1f;
	const float MAX_CAM_DISTANCE = 10f;
        
    
	[Range(.3f,2f)]
	public float zoomSpeed = .8f;
    
	float distance = 0f;

	void Start()
	{
		distance = Vector3.Distance(transform.position, Vector3.zero);
	}

    void LateUpdate()
    {
       
        if ( Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL) != 0f )
		{
			float delta = Input.GetAxis(INPUT_MOUSE_SCROLLWHEEL);
		
            Vector3 cameraPos = transform.position;

            cameraPos.y += delta * distance;
            if (!(cameraPos.y > 15 || cameraPos.y < 5))
            {
                transform.position = cameraPos;
            }

        }
      
    }

}
