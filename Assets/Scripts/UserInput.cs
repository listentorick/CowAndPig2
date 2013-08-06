using UnityEngine;
using System.Collections;

public class UserInput : MonoBehaviour {
	
	private static UserInput instance;
	
	public UserInput()
    {
        if( instance != null )
        {
            Debug.Log( "UserInput instance is not null" );
            return;
        }
        instance = this;
    }
	
	
	public static UserInput Instance
    {
        get
        {
            if( instance == null )
                new UserInput();
            return instance;
        }
    }
		
	public Vector3 GetTarget(){
		
		#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
		return 	Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,Input.mousePosition.y,Camera.mainCamera.transform.position.x));
		#endif
		
		#if UNITY_IPHONE || UNITY_ANDROID
		
		
		//foreach (Touch touch in Input.touches) {
		if (Input.touchCount>0){
			Touch touch = Input.touches[0];
			if(touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled){
				return Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x,touch.position.y,Camera.mainCamera.transform.position.x));
			} else {
				return new Vector3(0,0,0);
			}
		} else {
			//this should be the middle of the screen?? in world space
			//Vector3 topLeftViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(0,1, Camera.mainCamera.transform.position.x));
			//Vector3 bottomRightViewPort = Camera.mainCamera.ViewportToWorldPoint(new Vector3(1,0, Camera.mainCamera.transform.position.x));
			//return new Vector3(topLeftViewPort.x, bottomRightViewPort.y + ((topLeftViewPort.y - bottomRightViewPort.y)/2), bottomRightViewPort.z);
			return new Vector3(0,0,0);
		}
		#endif
	
	}
	
	
}
