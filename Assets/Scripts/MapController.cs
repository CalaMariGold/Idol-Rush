using UnityEngine;
using UnityEngine.Tilemaps;

// This class manages the dynamic loading of tilemap chunks in a top-down world
// as the player character moves through the environment.
public class MapController : MonoBehaviour
{
    public Tilemap tilemap; // Reference to the Tilemap where tiles will be placed
    public TileBase[] tileChunks; // Array of tile chunks to place on the tilemap
    public int chunkSize = 10;
    public PlayerController playerController;

    private Vector3Int previousChunkPosition;

    void Start()
    {
        PopulateInitialChunks();
    }

    void Update()
    {
        Vector3Int currentChunkPosition = GetCurrentChunkPosition();

        // If the player has moved to a new chunk, update the adjacent chunks
        if (!currentChunkPosition.Equals(previousChunkPosition))
        {
            PopulateAdjacentChunks(currentChunkPosition);
            previousChunkPosition = currentChunkPosition;
        }
    }

    // Calculates the current chunk position based on the player's position
    Vector3Int GetCurrentChunkPosition()
    {
        Vector3 playerPosition = playerController.transform.position;
        return new Vector3Int(
            Mathf.FloorToInt(playerPosition.x / chunkSize),
            Mathf.FloorToInt(playerPosition.y / chunkSize),
            0);
    }

    // Populates the initial nine chunks around the player (center chunk and the eight surrounding ones)
    void PopulateInitialChunks()
    {
        Vector3Int initialChunkPosition = GetCurrentChunkPosition();
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                Vector3Int chunkPosition = new Vector3Int(initialChunkPosition.x + x, initialChunkPosition.y + y, 0);
                PopulateChunk(chunkPosition);
            }
        }
        previousChunkPosition = initialChunkPosition;
    }

    // Populates the chunks adjacent to the current chunk the player is in
    void PopulateAdjacentChunks(Vector3Int currentChunkPosition)
    {
        for (int x = -2; x <= 2; x++)
        {
            for (int y = -2; y <= 2; y++)
            {
                Vector3Int chunkPosition = new Vector3Int(currentChunkPosition.x + x, currentChunkPosition.y + y, 0);
                PopulateChunk(chunkPosition);
            }
        }
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
}
