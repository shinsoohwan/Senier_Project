using UnityEngine;
using Assets.Scripts.Util;
using UnityEngine.Events;

public class UnitPathfinder : MonoBehaviour
{
    //AI PARAMETERS
    public float moveSpeed = 5;
    public float rotationSpeed = 300;
    public rotationType updateRotation = rotationType.rotateLeftRight;

    public enum EventType
    {
        MOVE_START,
        MOVE_END,
        MOVE_FAILED,
    };

    public class Event<T> : UnityEvent<T>
    {
    }

    Event<EventType> m_event;

    //altro
    Vector3 destination;
    private Vector3 afterMovePosition; // 중간에 transform.position이 변경됬으면 이동을 중단시키기 위한 변수

    bool mustMove = true;
    float pathRefreshTime = 0;

    void Awake()
    {
        m_event = new Event<EventType>();
    }
    
    void Start()
    {
        UpdateafterMovePosition();
    }

    void UpdateafterMovePosition()
    {
        afterMovePosition = transform.position;
    }

    void FixedUpdate()
    {
        if (!Vector3Util.IsAlmostEquals(transform.position, afterMovePosition))
        {
            stop();
            return;
        }

		CorvoPathFinder pf=GetComponent<CorvoPathFinder>();
		if (destinationActive)
		{
			if (pf)
			{
				if (pf.havePath())
					checkReachedNode();
				if (Time.time > pathRefreshTime)
				{
					updatePath();
				}
				
				if (mustMove)
				{
					if (pf.havePath())
					{
						Vector3 _dir=(pf.getDestination()-transform.position).normalized;
						if (updateRotation!=rotationType.dontRotate)
						{
							Vector3 _dir2D;
							if (updateRotation==rotationType.rotateAll)//rotate all
								_dir2D=(pf.getDestination()-transform.position).normalized;
							else//don't update z axis
								_dir2D=((pf.getDestination()-Vector3.up*pf.getDestination().y)-(transform.position-Vector3.up*transform.position.y)).normalized;

							transform.rotation = Quaternion.RotateTowards(transform.rotation,Quaternion.LookRotation(_dir2D),Time.deltaTime*rotationSpeed);
						}

                        if (Vector3.Distance(transform.position, pf.getDestination() ) < Time.deltaTime * moveSpeed)
                        {
                            transform.position = pf.getDestination();
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, pf.getDestination(), Time.deltaTime * moveSpeed);
                        }

					    UpdateafterMovePosition();
					}
				}
			}
			else
            {
                Debug.LogError("No PathFinder Assigned! please assign component CorvopathFinder to this object.",gameObject);
			}
		}
	}
	
	bool destinationActive=false;
	public void goTo(Vector3 _dest)//Start moving to position following pest path
	{
	    bool isSamePos = Vector3Util.IsAlmostEquals(destination, _dest);

        UpdateafterMovePosition();

        destinationActive =true;
		destination=_dest;
		updatePath();

	    if (!isSamePos)
        {
            m_event.Invoke(
                GetComponent<CorvoPathFinder>().havePath()
                    ? 
                    EventType.MOVE_START 
                    : 
                    EventType.MOVE_FAILED
            );
        }
    }
	
	public void stop() //stop unit if moving
	{
		GetComponent<CorvoPathFinder>().forceStop();
		destinationActive=false;
        m_event.Invoke(EventType.MOVE_END);
    }
	
    public void updatePath()//reload the path to see if world has changed
    {
        if (GetComponent<CorvoPathFinder>().findPath(destination))
			pathRefreshTime = Time.time + Random.Range(9f, 12f);//update world path after X seconds (maybe world has changed?)
        else
            pathRefreshTime = Time.time + Random.Range(0.01f, 0.1f);//wait until can find new path

    }

    public void AddEventListener(UnityAction<EventType> func)
    {
        m_event.AddListener(func);
    }

    public void checkReachedNode()//Check if reached next Pathnode
    {
        Vector3 curDestination = GetComponent<CorvoPathFinder>().getDestination();
        //if (GetComponent<CorvoPathFinder>().getDestination().Equals(transform.position) )
        if (Vector3Util.IsXZAlmostEquals(transform.position, curDestination))
        {
            if (Mathf.Abs(transform.position.y - curDestination.y) < 0.1f)
            {
                GetComponent<CorvoPathFinder>().nextNode();
            }
        }
        
		//was last node?
		if (GetComponent<CorvoPathFinder>().foundPath == null)
		{
		    
            if (Vector3Util.IsXZAlmostEquals(transform.position, destination))
            {
                stop();
            }
			else
            {
                //not arrived yet
                updatePath();
            }
		}
    }
}

public enum rotationType
{
	dontRotate,
	rotateAll,
	rotateLeftRight
}