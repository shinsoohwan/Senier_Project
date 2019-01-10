using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.Util;
using System.Text;
using Jint.Parser;

public class CorvoPathFinder:MonoBehaviour
{
	public float nodeWidth=2f;
    [Range(0.02f, 9000)]
    public float unitRay = 1f;
    [Range(0,90)]
	public float climbAngle=25f;
    public float unitStepHeight = 0.5f;
    [Range(3f, 1000)]
    public int gridSizeX=40;
    [Range(3f, 1000)]
    public int gridSizeY = 40;
    [Range(1f, 50)]
    public int gridLevels=7;
    public float levelHeight=4f;
	public LayerMask walkMask=~0;//LayerMask.GetMask("Default","onlyAmbient","Water"); 
	public LayerMask prohibiteMask;//LayerMask.GetMask("Default","onlyAmbient","Water"); 
	//public LayerMask ignoreMask;//LayerMask.GetMask("Default","onlyAmbient","Water"); 
    [Range(1f, 100)]
    public int expansionCoefficient = 50;
	public diagonalPath diagonals = diagonalPath.everywhere;
    public Transform pathFindingPos;

    //PERFORMANCE:
	public ProcessingSpeed processingSpeed=ProcessingSpeed.low;//if you have many AI or other stupid stuff make it low for better performnce

    GridNode[,,] grid=new GridNode[0,0,0];

#if UNITY_EDITOR
    public Transform testPathDestination;
#endif



	public Vector3 getDestination()
	{
		return foundPath.getPosition();
	}
	public Vector3[] getPath()
	{
		if (havePath ()) 
		{
			List<Vector3> _list = new List<Vector3> ();
			GridNode _node = foundPath;
			while (_node != null) 
			{
				_list.Add (_node.getPosition ());
				_node = _node.nextPathNode;
			}
			return _list.ToArray();
		}
		return new Vector3[0];
	}

    public bool havePath()
    {
        return foundPath != null;
	}

	public void nextNode()
	{
		foundPath = foundPath.nextPathNode;
		if (foundPath == null)
			clearPath();
	}

	public bool findPath(Vector3 _destination,Vector3 _startPos)
	{
		if (!calculating)
		{
			calculating = true;
			StartCoroutine(calculatePath(_destination, _startPos));
			return true;
		}
		return false;

	}
	public bool findPath(Vector3 _destination,Transform _updatePos=null)
	{
		if (!calculating)
		{
			if (!_updatePos)
				_updatePos = transform;

			calculating = true;
			StartCoroutine(calculatePath(_destination, transform.position));
			return true;
		}
		return false;
	}


	public void clearGrid()
	{
		haveGrid = false;
		generating = false;
		grid = new GridNode[0, 0,0];
	}
	public void clearPath()
	{
		foundPath = null;
		//_havePath = false;
		_nearestATALL = _nearestToDest = null;
		_discovered.Clear();
	}
	public void forceStop()
	{
		calculating =generating= false;
		clearGrid();
		clearPath();
	}
	
    //INTERNAL PROCESSING/YOU DONT NEED TO EDIT HERE
    int sX =0, sY=0,lvs=0;
    public void generateGrid(bool _force =false)
    {
        if (!generating || _force)
        {
            clearGrid();
            sX = gridSizeX + 1;
            sY = gridSizeY + 1;
            lvs = gridLevels;
            if (sX < 3 || sY < 3)
            {
                Debug.LogError("Grid can't be less the 3x3! ", gameObject);
                return;
            }
            grid = new GridNode[sX, sY, lvs];

            StartCoroutine(generatingGrid());
        }
    }

    bool haveGrid=false;
    bool generating = false;
    IEnumerator generatingGrid()
    {
        haveGrid = false;
        generating = true;

        RaycastHit _hit;
        //crea griglia di punti

        int passibase = 0, maxPassi = setMaxPassi();
        //MIA VERSIONE
        double _startTime = Time.time;

        bool _diagonal = false;
        for (int lev = 0; lev < gridLevels; lev++)
        {
            Vector3 _up = Vector3.down * gridLevels / 2 * levelHeight + Vector3.up * (lev + 1) * levelHeight;
            for (int i = 0; i < sX; i++)//RIGHE
            {
                for (int j = 0; j < sY; j++)//COLONNE
                {
                    passibase++;
                    grid[i, j,lev] = null;
                    Vector3 _startPos = Vector3Util.GridVector(Vector3Util.GridVector(transform.position) + Vector3.right * (i - (sX / 2)) * nodeWidth + Vector3.forward * (j - (sY / 2)) * nodeWidth);

                    if (Physics.Raycast(_startPos + _up, -Vector3.up, out _hit, levelHeight * 1.3f, walkMask))
                    {
                        if (prohibiteMask == (prohibiteMask | (1 << _hit.collider.gameObject.layer)))
                            continue;
						if (_hit.collider.transform.root==transform.root)
							continue;
                        
                        switch (diagonals)
                        {
                            case diagonalPath.everywhere:
                                _diagonal = true;
                                break;
                            case diagonalPath.nowhere:
                                _diagonal = false;
                                break;
                        }

                        TestVector(_hit.point);

                        if (lev == 0)
                            grid[i, j, lev] = new GridNode(_hit.point);
                        else
                        {
                            if (grid[i, j, lev - 1] != null)
                            {
                                if (_hit.point.y - grid[i, j, lev - 1].getPosition().y > nodeWidth / 3)
                                    grid[i, j, lev] = new GridNode(_hit.point);
                                else
                                    continue;
                            }
                            else
                                grid[i, j, lev] = new GridNode(_hit.point);
                        }

                        if (j > 0) // 왼쪽이 아니야.
                        {
                            if (_diagonal)
                            {
                                if (i > 0) // 높이 올라가지 않는다.
                                {
                                    //ALTO SINISTRA
                                    conectGridPoints(grid[i, j, lev], grid[i - 1, j - 1, lev]);
                                    if (lev > 0)
                                    {
                                        conectGridPoints(grid[i, j, lev], grid[i - 1, j - 1, lev - 1]);
                                        if (i < sX - 1 && j < sY - 1)
                                            conectGridPoints(grid[i, j, lev], grid[i + 1, j + 1, lev - 1]);
                                    }
                                }
                            }

                            // 높음
                            conectGridPoints(grid[i, j, lev], grid[i, j - 1, lev]);
                            if (lev > 0)
                            {
                                conectGridPoints(grid[i, j, lev], grid[i, j - 1, lev - 1]);
                                if (j < sY - 1)
                                    conectGridPoints(grid[i, j, lev], grid[i, j + 1, lev - 1]);
                            }
                        }
                        if (i > 0)// 그것을 극복하지 못한다.
                        {
                            if (_diagonal)
                            {
                                if (j < sY - 1)//그것은 오른쪽으로 오지 않는다.
                                {
                                    //왼쪽 하단
                                    conectGridPoints(grid[i, j, lev], grid[i - 1, j + 1, lev]);
                                    if (lev > 0)
                                    {
                                        conectGridPoints(grid[i, j, lev], grid[i - 1, j + 1, lev - 1]);
                                        if (i < sX - 1 && j > 0)
                                            conectGridPoints(grid[i, j, lev], grid[i + 1, j - 1, lev - 1]);
                                    }
                                }
                            }
                            //SINISTRA
                            conectGridPoints(grid[i, j, lev], grid[i - 1, j, lev]);
                            if (lev > 0)
                            {
                                conectGridPoints(grid[i, j, lev], grid[i - 1, j, lev - 1]);
                                if (i < sX - 1)
                                    conectGridPoints(grid[i, j, lev], grid[i + 1, j, lev - 1]);
                            }
                        }
                        //STOPPA PER AZIONI
                        passibase += 4;
                        //CHECK E' ABBASTANZA VICINO
                    } 

                    if (passibase > maxPassi )
                    {
                        passibase = 0;
						if (Application.isPlaying)
							yield return null;

						if (!generating)//forced stop
                        {
                            yield break;
                        }
                    }
                }
            }
        }


        if (!Application.isPlaying)
            print("Grid generated. Time needed: " + (Time.time - _startTime));
        generating = false;
        haveGrid = true;
    }

    static public void TestVector(GridNode node)
    {
        TestVector(node.getPosition());
    }

    static public void TestVector( Vector3 vec)
    {
        float x_div = (vec.x + 1) / 2;
        x_div = x_div - (int)x_div;
        float y_div = (vec.y + 1) / 2;
        y_div = y_div - (int)y_div;

        if (0.05 < x_div && x_div < 0.95)
        {
            if (0.05 < y_div && y_div < 0.95)
            {
                Debug.Log(vec);
            }
        }

    }

    public void conectGridPoints(GridNode a, GridNode b)
    {
        if (a == null || b == null)
            return;
;
        TestVector(a);
        TestVector(b);

        Vector3 _dir = (b.getPosition() - a.getPosition()).normalized;
        Vector3 _dirNM = new Vector3(_dir.x, 0, _dir.z);
        
        if (Vector3.Angle(_dir, _dirNM) <= climbAngle)// oppure se la griglia ?vicina ad un muro
        {
            //direzione sgombra
            Collider[] _obstacles= Physics.OverlapCapsule(a.getPosition() + Vector3.up * unitRay + Vector3.up * unitStepHeight, b.getPosition() + Vector3.up * unitRay + Vector3.up * unitStepHeight, unitRay, walkMask);

            if (_obstacles.Length > 0) return;//PER VERSIONE ACQUISTABILE
            a.addNearNode(b);//si aggiungono a vicenda
            b.addNearNode(a);//mettere controllo che quello pi?in basso per caduta ?sempre raggiungibile fino ad una certa altezza
        }
    }


    [HideInInspector]
    public GridNode foundPath = null;

    Queue<GridNode> _discovered = new Queue<GridNode>();//nodi gi?controllati
    GridNode _nearestATALL = null;
    GridNode _nearestToDest = null;
    bool calculating = false;
    //bool _havePath = false;
    IEnumerator calculatePath(Vector3 _destination, Vector3 _pos, Transform _updatePos = null)
    {
        generateGrid();
        while (!haveGrid)
            yield return new WaitForSeconds(Random.Range(0.01f, 0.07f));

        float DeltaDistX = Mathf.Abs(sX / 2 * nodeWidth);
        float DeltaDistY = Mathf.Abs(sY / 2 * nodeWidth);

        if (_destination.x > transform.position.x + DeltaDistX)
            _destination = new Vector3(transform.position.x + DeltaDistX, _destination.y, _destination.z);
        else if (_destination.x < transform.position.x - DeltaDistX)
            _destination = new Vector3(transform.position.x - DeltaDistX, _destination.y, _destination.z);

        if (_destination.z > transform.position.z + DeltaDistY)
            _destination = new Vector3(_destination.x, _destination.y, transform.position.z + DeltaDistY);
        else if (_destination.z < transform.position.z - DeltaDistY)
            _destination = new Vector3(_destination.x, _destination.y, transform.position.z - DeltaDistY);


        _discovered.Clear();
        if (_updatePos==null)
        {
            if (pathFindingPos)
                _pos = pathFindingPos.position;
            else
                _pos = transform.position;
        }

        GridNode[] _nearestNode = nearestNode(_pos);

        GridNode _firstNode = new GridNode(_pos);
        _firstNode.nearNodes = _nearestNode;
        _firstNode.isChecked = true;

        if (0 < _nearestNode.Length)
        {
            _nearestToDest = _firstNode;

            _discovered.Enqueue(_firstNode);
            float _nearestToDestDist = Mathf.Infinity;


            ///GridNode currentNode = _firstNode;

            while (0 < _discovered.Count)
            {
                GridNode currentNode = _discovered.Dequeue();

                float toDestDist = Vector2.Distance(new Vector2(currentNode.getPosition().x, currentNode.getPosition().z),
                    new Vector2(_destination.x, _destination.z));

                if (toDestDist < _nearestToDestDist)
                {
                    _nearestToDestDist = toDestDist;
                    _nearestToDest = currentNode;
                }

                foreach (GridNode _adiacente in currentNode.nearNodes)
                {
                    if (_adiacente.isChecked)
                        continue;
                    
                    _adiacente.isChecked = true;
                    
                    _adiacente.previousPathNode = currentNode;
                    _discovered.Enqueue(_adiacente);
                   
                    //currentNode
                }
            }

            // 찾은 길 후처리
            if (_nearestToDestDist < 1.9f)
            {
                _nearestATALL = _nearestToDest;

                // 꼬리처럼 매달려 있는 previousPathNode들에게 nextPathNode 값 정의
                for (GridNode node = _nearestATALL; node != null; node = node.previousPathNode)
                {
                    if (node.previousPathNode == null || node.previousPathNode == _firstNode) // 꼬리 없음
                    {
                        node.previousPathNode = null;
                        foundPath = node;
                        break;
                    }

                    node.previousPathNode.nextPathNode = node;
                }
            }
        }

        _nearestATALL = _nearestToDest = null;
        calculating = false;
        clearGrid();
    }
    public bool isCalculating()
    {
        return calculating;
    }

    float iMultiplicator(int i)
    {
        //return 1;
		return (1 + (Mathf.Abs(i - (float)sX / 2) / (101-expansionCoefficient)));
    }
    float jMultiplicator(int j)
    {
        //return 1;
		return (1 + (Mathf.Abs(j-(float)(sY / 2)) / (101-expansionCoefficient)));
    }

    void OnDrawGizmosSelected()
    {
        Color _gizcolor = Color.green;

        if (foundPath==null && grid.Length<=9 || generating)
        {
            _gizcolor = Color.green;
            Gizmos.color = _gizcolor;
            Gizmos.DrawWireCube(transform.position, new Vector3(gridSizeX * nodeWidth, levelHeight * gridLevels, gridSizeY * nodeWidth));


            for (int lev = 0; lev <= gridLevels; lev++)
            {
                Vector3 _ypos = Vector3.down * gridLevels / 2 * levelHeight + Vector3.up * (lev) * levelHeight;

                if (lev == (int)((gridLevels + 1) / 2))
				{
					_gizcolor = Color.cyan;
					_gizcolor.a = 0.25f;
					Gizmos.color = _gizcolor;
                    for (int i = -sX / 2; i <= sX / 2; i++)//disegna righe
                        Gizmos.DrawLine(transform.position + Vector3.right * i * nodeWidth - Vector3.forward * gridSizeY / 2 * nodeWidth , transform.position + Vector3.right * i * nodeWidth + Vector3.forward * gridSizeY / 2 * nodeWidth );
                    for (int i = -sY / 2; i <= sY / 2; i++)//disegna righe
                        Gizmos.DrawLine(transform.position + Vector3.forward * i * nodeWidth - Vector3.right * gridSizeX / 2 * nodeWidth , transform.position + Vector3.forward * i * nodeWidth + Vector3.right * gridSizeX / 2 * nodeWidth );
                }
				_gizcolor = Color.yellow;
				Gizmos.color = _gizcolor;
				//Gizmos.DrawLine(transform.position - Vector3.right * gridSizeX / 2 * nodeWidth + _ypos,transform.position + Vector3.right * gridSizeX / 2 * nodeWidth + _ypos);
				Gizmos.DrawLine(transform.position - Vector3.forward * gridSizeY / 2 * nodeWidth + _ypos,transform.position + Vector3.forward * gridSizeY / 2 * nodeWidth + _ypos);
            }
        }

        
        if (foundPath == null || generating)
        {
            _gizcolor = Color.yellow;
            Gizmos.color = _gizcolor;
            if (grid.Length > 9 && _nearestATALL == null)
            {
                foreach (GridNode _node in grid)
                {
                    if (_node == null)
                        continue;
                    foreach (GridNode _subnode in _node.nearNodes)
                    {
                        Gizmos.DrawLine(_node.getLookablePosition(), _subnode.getLookablePosition());
                    }
                }
            }
            if (!Application.isPlaying)
            {
                if (_discovered.Count > 0)
                {
                    _gizcolor = Color.green;
                    Gizmos.color = _gizcolor;
                    foreach (GridNode _node in _discovered)
                    {
                        Gizmos.DrawSphere(_node.getPosition(), nodeWidth / 4);
                    }
                }
                _gizcolor = Color.white;
                Gizmos.color = _gizcolor;
                foreach (GridNode _node in grid)
                {
                    if (_node!=null)
                    {
                        if (_node.previousPathNode != null && _node.isChecked)
                            Gizmos.DrawLine(_node.getLookablePosition(), _node.previousPathNode.getLookablePosition());
                    }
                    //Gizmos.DrawSphere(_node.getPosition(), nodeWidth / 4);
                }
            }
        }
        if (_nearestATALL!=null)
        {
            _gizcolor = Color.blue;
            Gizmos.color = _gizcolor;
            Gizmos.DrawSphere(_nearestATALL.getPosition(), nodeWidth / 1.5f);
        }

        /*//START POSITION ARRAY
        _gizcolor = Color.red;
        Gizmos.color = _gizcolor;
        Gizmos.DrawSphere(transform.position + Vector3.right * -sX/2 * nodeWidth - Vector3.forward * gridSizeY / 2 * nodeWidth, nodeWidth/6);
        _gizcolor = Color.cyan;
        Gizmos.color = _gizcolor;
        Gizmos.DrawSphere(transform.position + Vector3.right * sX / 2 * nodeWidth + Vector3.forward * gridSizeY / 2 * nodeWidth, nodeWidth / 6);*/

        if (foundPath != null)
        {
            _gizcolor = Color.cyan;
            Gizmos.color = _gizcolor;
            Gizmos.DrawSphere(foundPath.getPosition(), nodeWidth / 2);

            GridNode _next = foundPath.nextPathNode;
            while (_next != null)
            {
                Gizmos.DrawSphere(_next.getPosition(), nodeWidth / 4);
                if (_next.nextPathNode != null)
                    Gizmos.DrawLine(_next.getLookablePosition(), _next.nextPathNode.getLookablePosition());
                //Gizmos.DrawSphere(_previous.getPosition(), nodeWidth / 3);
                _next = _next.nextPathNode;
            }
        }

    }

    public int setMaxPassi()
    {
        switch (processingSpeed)
        {
            case ProcessingSpeed.veryHigh:
                return 20000;
            case ProcessingSpeed.high:
                return 5000;
            case ProcessingSpeed.medium:
                return 1000;
            case ProcessingSpeed.low:
                return 700;
            case ProcessingSpeed.veryLow:
                return 200;
        }
        return 2000;
    }


    public GridNode[] nearestNode(Vector3 _pos)
    {
        GridNode _nearest = null;
        List<GridNode> _similarDistNodes = new List<GridNode>();
        float _nearestDist = Mathf.Infinity;
        foreach(GridNode _node in grid)
        {
            /*
            if (_node == grid[sX / 2, sY / 2,gridLevels/2])
                continue;*/
            if (_node==null)
            {
                continue;
            }
            if (_node.nearNodes.Length <= 0)
            {
                continue;
            }
        
            float _dist = Vector2.Distance(
                new Vector2(_pos.x, _pos.z), 
                new Vector2(_node.getPosition().x, _node.getPosition().z));
            if (0.0001 < _dist - _nearestDist)
            {
                continue;
            }

            if (_nearestDist - _dist < 0.0001)
            {
                _similarDistNodes.Push(_node);
                continue;
            }
            
            _nearestDist = _dist;
            _nearest = _node;
            _similarDistNodes.Clear();

        }

        if (_nearest != null)
           _similarDistNodes.Push(_nearest);

        return _similarDistNodes.ToArray();
    }

}


public class GridNode
{
    //Vector3Int positionInt;
	Vector3 position;
	public GridNode[] nearNodes=new GridNode[0];
    public GridNode previousPathNode = null;
    public GridNode nextPathNode = null;
    public bool isChecked=false;


    public GridNode(Vector3 _pos)
    {
        position = new Vector3((float)(_pos.x + 0.0), _pos.y, (float)(_pos.z + 0.0));
    }

    public GridNode(Vector3Int _pos)
	{
        //positionInt = _pos;
        position = _pos;
	}

    public Vector3 getPosition()
	{
		return position;
    }
    public Vector3 getLookablePosition()
    {
        return position+Vector3.up*0.3f;
    }

    public void addNearNode(GridNode _node)
    {
        CorvoPathFinder.TestVector(_node);
        GridNode[] _temp = new GridNode[nearNodes.Length + 1];
        for (int _x = 0; _x < nearNodes.Length; _x++)
            _temp[_x] = nearNodes[_x];
        _temp[_temp.Length - 1] = _node;//nodo aggiunto
        nearNodes = _temp;
	}
}

/*
public class PathNode
{
    Vector3 position;
    public PathNode next = null;
    public PathNode previous = null;

    public PathNode(Vector3 _pos,PathNode _previous)
    {
        position = _pos;
        previous = _previous;
    }

    public Vector3 getPosition()
    {
        return position;
    }
}*/

public enum diagonalPath
{
    everywhere,
    nowhere
}
public enum ProcessingSpeed
{
	veryHigh,
	high,
	medium,
	low,
	veryLow
}