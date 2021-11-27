using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    public GameObject[] tiles;
    public GameObject[] walls;
    public List<Vector3> createdTilesCords;
    public List<Vector3> createdWallsCords;
    public List<GameObject> tileObject;
    public List<GameObject> wallObject;
    public int tileAmount;
    public int tileSize;
    public int wallSize;
    public float chanceUp;
    public float chanceRight;
    public float chanceDown;
    public float waitTime;
    public float extraWallHeight;
    private float _lastDirection = -1;
    private int _tileIndex = 0;
    private int _wallIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GenerateLevel());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator GenerateLevel()
    {

        for(int i=0; i<tileAmount; i++)
        {
            //0-up, 1-right, 2-down, 3-left
            float direction = Random.Range(0f, 1f);
            int tile = Random.Range(0, tiles.Length);
            CreateTile(tile);
            //Debug.Log(direction);
            CallMoveGenerator(direction);
            yield return new WaitForSeconds(waitTime);
        }
        yield return 0;
    }

    void CallMoveGenerator(float randomDirection)
    {
        if(randomDirection < chanceUp)
            MoveGenerator(0);
        else if(randomDirection < chanceRight)
            MoveGenerator(1);
        else if (randomDirection < chanceDown)
            MoveGenerator(2);
        else MoveGenerator(3);
    }

    void MoveGenerator(int direction)
    {
        _lastDirection = direction;
        switch (direction)
        {
            //X/Z Axis
            //up
            case 0:
                transform.position = new Vector3(transform.position.x, 0, transform.position.z + tileSize);
                break;
            //right
            case 1:
                transform.position = new Vector3(transform.position.x + tileSize, 0, transform.position.z);
                break;
            //down
            case 2:
                transform.position = new Vector3(transform.position.x, 0, transform.position.z - tileSize);
                break;
            //left
            case 3:
                transform.position = new Vector3(transform.position.x - tileSize, 0, transform.position.z);
                break;
        }
    }

    void CreateTile(int tileIndex)
    {
        if(!createdTilesCords.Contains(transform.position))
        {
            GameObject tempObject = (GameObject) Instantiate(tiles[tileIndex], transform.position, transform.rotation);
            tileObject.Add(tempObject);
            Vector3 scaleChange = new Vector3(tileSize/12, 0.00f, tileSize/12);
            tileObject[_tileIndex].transform.localScale+=scaleChange;
            createdTilesCords.Add(tileObject[_tileIndex].transform.position);
            _tileIndex++;
            CreateWalls();
        }
        else
        {
            tileAmount++;
        }
    }

    void CreateWalls()
    {
        //up/z+
        Vector3 position1 = new Vector3(transform.position.x, (extraWallHeight+6)/2, transform.position.z + tileSize/2);
        if(!createdWallsCords.Contains(position1))
        {
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            wallObject.Add(Instantiate(walls[0], position1, rotation) as GameObject);
            Vector3 scaleChange = new Vector3(wallSize, extraWallHeight, 0.00f);
            wallObject[_wallIndex].transform.localScale += scaleChange;
            createdWallsCords.Add(wallObject[_wallIndex].transform.position);
            _wallIndex++;
        }
        //right/x+
        Vector3 position2 = new Vector3(transform.position.x + tileSize/2, (extraWallHeight+6) / 2, transform.position.z);
        if (!createdWallsCords.Contains(position2))
        {
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            wallObject.Add(Instantiate(walls[0], position2, rotation) as GameObject);
            wallObject[_wallIndex].transform.Rotate(0f, 90f, 0f);
            Vector3 scaleChange = new Vector3(wallSize, extraWallHeight, 0.00f);
            wallObject[_wallIndex].transform.localScale += scaleChange;
            createdWallsCords.Add(wallObject[_wallIndex].transform.position);
            _wallIndex++;
        }

        //down/z-
        Vector3 position3 = new Vector3(transform.position.x, (extraWallHeight+6) / 2, transform.position.z - tileSize/2);
        if (!createdWallsCords.Contains(position3))
        {
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            wallObject.Add(Instantiate(walls[0], position3, rotation) as GameObject);
            Vector3 scaleChange = new Vector3(wallSize, extraWallHeight, 0.00f);
            wallObject[_wallIndex].transform.localScale += scaleChange;
            createdWallsCords.Add(wallObject[_wallIndex].transform.position);
            _wallIndex++;
        }
        //left/x-
        Vector3 position4 = new Vector3(transform.position.x - tileSize/2, (extraWallHeight+6) / 2, transform.position.z);
        if (!createdWallsCords.Contains(position4))
        {
            Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
            wallObject.Add(Instantiate(walls[0], position4, rotation) as GameObject);
            wallObject[_wallIndex].transform.Rotate(0f, 90f, 0f);
            Vector3 scaleChange = new Vector3(wallSize, extraWallHeight, 0.00f);
            wallObject[_wallIndex].transform.localScale += scaleChange;
            createdWallsCords.Add(wallObject[_wallIndex].transform.position);
            _wallIndex++;
        }
        //wall with door
        //TODO: usuwanie scian nie dziala, probuje sprawdzac nieistniejace sciany
        switch (_lastDirection)
        {
            //X/Z Axis
            //up
            case 0:
                Debug.Log("Try to destroy wall");
                Debug.Log(wallObject.Count);
                //Vector3 position1 = new Vector3(transform.position.x, wallHeight / 2, transform.position.z - tileSize);
                Debug.Log(wallObject[0].transform.position);
                Debug.Log(position3);
                Debug.Log(wallObject[0].transform.Equals(position3));
                for (int i = 0; i < createdWallsCords.Count; i++)
                {
                    if(createdWallsCords[i].Equals(position3))
                    {
                        Destroy(wallObject[i]);
                        wallObject.RemoveAt(i);
                        createdWallsCords.RemoveAt(i);
                        _wallIndex--;
                        Debug.Log("Destroy wall");
                    }
                }
                Quaternion rotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                wallObject.Add(Instantiate(walls[1], position3, rotation) as GameObject);
                Vector3 scaleChange = new Vector3(wallSize, extraWallHeight, 0.00f);
                wallObject[_wallIndex].transform.localScale += scaleChange;
                createdWallsCords.Add(wallObject[_wallIndex].transform.position);
                _wallIndex++;
                break;
            //right
            case 1:
                Debug.Log("Try to destroy wall");
                //Vector3 position2 = new Vector3(transform.position.x - tileSize, wallHeight / 2, transform.position.z);
                for (int i = 0; i < createdWallsCords.Count; i++)
                {
                    if (wallObject[i].transform.position.Equals(position4))
                    {
                        Destroy(wallObject[i]);
                        wallObject.RemoveAt(i);
                        createdWallsCords.RemoveAt(i);
                        _wallIndex--;
                        Debug.Log("Destroy wall");
                    }
                }
                Quaternion rotation2 = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                wallObject.Add(Instantiate(walls[1], position4, rotation2) as GameObject);
                wallObject[_wallIndex].transform.Rotate(0f, 90f, 0f);
                Vector3 scaleChange2 = new Vector3(wallSize, extraWallHeight, 0.00f);
                wallObject[_wallIndex].transform.localScale += scaleChange2;
                createdWallsCords.Add(wallObject[_wallIndex].transform.position);
                _wallIndex++;
                break;
            //down
            case 2:
                Debug.Log("Try to destroy wall");
                //Vector3 position3 = new Vector3(transform.position.x, wallHeight / 2, transform.position.z + tileSize);
                for (int i = 0; i < createdWallsCords.Count; i++)
                {
                    if (wallObject[i].transform.position.Equals(position1))
                    {
                        Destroy(wallObject[i]);
                        wallObject.RemoveAt(i);
                        createdWallsCords.RemoveAt(i);
                        _wallIndex--;
                        Debug.Log("Destroy wall");
                    }
                }
                Quaternion rotation3 = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                wallObject.Add(Instantiate(walls[1], position1, rotation3) as GameObject);
                Vector3 scaleChange3 = new Vector3(wallSize, extraWallHeight, 0.00f);
                wallObject[_wallIndex].transform.localScale += scaleChange3;
                createdWallsCords.Add(wallObject[_wallIndex].transform.position);
                _wallIndex++;
                break;
            //left
            case 3:
                Debug.Log("Try to destroy wall");
                //Vector3 position4 = new Vector3(transform.position.x + tileSize, wallHeight / 2, transform.position.z);
                for (int i = 0; i < createdWallsCords.Count; i++)
                {
                    if (wallObject[i].transform.position.Equals(position2))
                    {
                        Destroy(wallObject[i]);
                        wallObject.RemoveAt(i);
                        createdWallsCords.RemoveAt(i);
                        _wallIndex--;
                        Debug.Log("Destroy wall");
                    }
                }
                Quaternion rotation4 = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
                wallObject.Add(Instantiate(walls[1], position2, rotation4) as GameObject);
                wallObject[_wallIndex].transform.Rotate(0f, 90f, 0f);
                Vector3 scaleChange4 = new Vector3(wallSize, extraWallHeight, 0.00f);
                wallObject[_wallIndex].transform.localScale += scaleChange4;
                createdWallsCords.Add(wallObject[_wallIndex].transform.position);
                _wallIndex++;
                break;
        }
    }
}
