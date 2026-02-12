using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSpawn : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;

    [Header("Spawn Grid Position")]
    [SerializeField] private Vector2Int spawnGridPosition;
    [SerializeField] private float spawnHeightOffset = 1f;
    private GameObject currentPlayer;

    public void Start()
    {
        //在游戏开始时生成玩家
        SpawnPlayer();
    }

    private void SpawnPlayer()
    {
        //如果有current删除
        if (currentPlayer != null)
        {
            Destroy(currentPlayer);
        }
        //获取node坐标
        Node spawnNode = GridManager.Instance.GetNode(spawnGridPosition);
        //获取当前node的世界坐标
        Vector3 worldPosition = GridManager.Instance.GetWorldPosition(spawnNode);
        //生成在格子上方,高度+
        worldPosition.y += spawnHeightOffset;
        //生成玩家在这个位置
        currentPlayer = Instantiate(playerPrefab, worldPosition, Quaternion.identity);

        spawnNode.SetOccupied(true);
    }
}
