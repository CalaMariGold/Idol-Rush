using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

// This class manages the dynamic loading of tilemap chunks in a top-down world
// as the player character moves through the environment.
public class MapController : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase[] tileChunks;
    public int chunkSize = 10;
    public PlayerController playerController;
    public Camera mainCamera;

    private Vector3Int previousChunkPosition;
    private HashSet<Vector3Int> activeChunks;
    private float updateThresholdDistance = 10f; // Update chunks when the player moves this distance


    void Start()
    {
        activeChunks = new HashSet<Vector3Int>();
        UpdateChunks(true);
    }
    void Update()
    {
        UpdateChunks(false);
    }


    Rect GetCameraBounds()
    {
        float verticalExtend = mainCamera.orthographicSize;
        float horizontalExtend = verticalExtend * Screen.width / Screen.height; // Aspect ratio adjustment
        Vector2 cameraPosition = (Vector2)mainCamera.transform.position;
        return new Rect(cameraPosition.x - horizontalExtend, cameraPosition.y - verticalExtend, horizontalExtend * 2, verticalExtend * 2);
    }

    // Calculate the current chunk position based on player position
    Vector3Int GetCurrentChunkPosition()
    {
        Vector3 playerPosition = playerController.transform.position;
        return new Vector3Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize),
            0);
    }

    // Update the active chunks based on player position
    void UpdateChunks(bool forceUpdate)
    {
        // Calculate the distance the player has moved since the last update
        float distanceMoved = Vector3.Distance(playerController.transform.position, previousChunkPosition);

        if (forceUpdate || distanceMoved > updateThresholdDistance)
        {
            Rect cameraBounds = GetCameraBounds();
            cameraBounds.xMin -= chunkSize / 2;
            cameraBounds.xMax += chunkSize / 2;
            cameraBounds.yMin -= chunkSize / 2;
            cameraBounds.yMax += chunkSize / 2;

            UpdateChunkStatus(cameraBounds);
            previousChunkPosition = GetCurrentChunkPosition();
        }
    }



    void UpdateChunkStatus(Rect cameraBounds)
    {
        int viewDistanceInChunks = 5;
        Vector2 cameraVelocity = mainCamera.GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero;
        int predictiveLoadingFactor = Mathf.CeilToInt(cameraVelocity.magnitude / chunkSize);

        // Store the current chunk position at the start
        Vector3Int currentChunkPosition = GetCurrentChunkPosition();

        HashSet<Vector3Int> updatedActiveChunks = new HashSet<Vector3Int>();

        for (int x = -viewDistanceInChunks - predictiveLoadingFactor; x <= viewDistanceInChunks + predictiveLoadingFactor; x++)
        {
            for (int y = -viewDistanceInChunks - predictiveLoadingFactor; y <= viewDistanceInChunks + predictiveLoadingFactor; y++)
            {
                Vector3Int chunkPosition = currentChunkPosition + new Vector3Int(x, y, 0);
                if (IsChunkInCameraBounds(chunkPosition, cameraBounds))
                {
                    if (!activeChunks.Contains(chunkPosition))
                    {
                        PopulateChunk(chunkPosition);
                    }
                    updatedActiveChunks.Add(chunkPosition);
                }
                else if (activeChunks.Contains(chunkPosition))
                {
                    DeactivateChunk(chunkPosition);
                }
            }
        }

        activeChunks = updatedActiveChunks;
    }


    

    bool IsChunkInCameraBounds(Vector3Int chunkPosition, Rect cameraBounds)
    {
        // Calculate the bounds of the chunk considering its full size.
        Vector2 chunkCenter = new Vector2(chunkPosition.x * chunkSize + chunkSize / 2, chunkPosition.y * chunkSize + chunkSize / 2);
        Rect chunkRect = new Rect(chunkCenter.x - chunkSize / 2, chunkCenter.y - chunkSize / 2, chunkSize, chunkSize);

        // Inflate the camera bounds slightly to ensure chunks at the edge are included.
        Rect inflatedCameraBounds = cameraBounds;
        inflatedCameraBounds.xMin -= chunkSize;
        inflatedCameraBounds.xMax += chunkSize;
        inflatedCameraBounds.yMin -= chunkSize;
        inflatedCameraBounds.yMax += chunkSize;

        return inflatedCameraBounds.Overlaps(chunkRect);
    }

    // Populates a single chunk with tiles at the specified chunkPosition
    void PopulateChunk(Vector3Int chunkPosition)
    {
        // Iterate over each tile position within the chunk and set a tile if one isn't already set
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3Int tilePosition = new Vector3Int(chunkPosition.x * chunkSize + x, chunkPosition.y * chunkSize + y, 0);
                // Check if the tile at the position is empty before placing a new one
                if (!tilemap.HasTile(tilePosition))
                {
                    // Select a random tile chunk from the array and place it
                    TileBase tileChunk = tileChunks[Random.Range(0, tileChunks.Length)];
                    tilemap.SetTile(tilePosition, tileChunk);
                }
            }
        }
    }


    void DeactivateChunk(Vector3Int chunkPosition)
    {
        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                Vector3Int tilePosition = chunkPosition * chunkSize + new Vector3Int(x, y, 0);
                tilemap.SetTile(tilePosition, null);
            }
        }
    }

}
