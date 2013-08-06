using UnityEngine;
using System.Collections;


// Each notification type should gets its own enum
public enum NotificationType {
	OnStuff,
	OnOtherStuff,
	OnSomeEvent,
	TotalNotifications
};

public delegate void OnNotificationDelegate( Notification note );

public class NotificationCenter
{
    private static NotificationCenter instance;

    private OnNotificationDelegate [] listeners = new OnNotificationDelegate[(int)NotificationType.TotalNotifications];

    // Instead of constructor we can use void Awake() to setup the instance if we sublcass MonoBehavoiur
    public NotificationCenter()
    {
        if( instance != null )
        {
            Debug.Log( "NotificationCenter instance is not null" );
            return;
        }
        instance = this;
    }
	
	
	~NotificationCenter()
	{
		instance = null;
	}


    public static NotificationCenter defaultCenter
    {
        get
        {
            if( instance == null )
                new NotificationCenter();
            return instance;
        }
    }


    public void addListener( OnNotificationDelegate newListenerDelegate, NotificationType type )
    {
        int typeInt = (int)type;
        listeners[typeInt] += newListenerDelegate;
    }


    public void removeListener( OnNotificationDelegate listenerDelegate, NotificationType type )
    {
        int typeInt = ( int )type;
        listeners[typeInt] -= listenerDelegate;
    }


    public void postNotification( Notification note )
    {
        int typeInt = ( int )note.type;

        if( listeners[typeInt] != null )
            listeners[typeInt](note);
    }
    

}




// Usage:
// NotificationCenter.defaultCenter.addListener( onNotification );
// NotificationCenter.defaultCenter.sendNotification( new Notification( NotificationTypes.OnStuff, this ) );
// NotificationCenter.defaultCenter.removeListener( onNotification, NotificationType.OnStuff );