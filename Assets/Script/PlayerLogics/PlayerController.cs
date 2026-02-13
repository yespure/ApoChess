using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class PlayerController : MonoBehaviour
{
    [Header("移动相关")]
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private int moveRange = 3;
    [Header("范围相关")]
    private HashSet<Node> reachableNodes;
    private Node currentNode;
    public enum PlayerState
    {
        Idle,           
        Moving,                  
    }
    [Header("玩家状态机")]
    private PlayerState state = PlayerState.Idle;
    private void Start()
    {
        currentNode = GetCurrentNode();
        CalculateRange();
    }

    void Update()
    {
        if (state != PlayerState.Idle) return;
        if (Input.GetMouseButtonDown(0))
        {
            Move();
        }
    }

    void Move()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            //射线点击位置设定为 target
            Vector3 point = hit.point;

            Vector2Int gridPos = new Vector2Int(
                Mathf.RoundToInt(point.x / 2f),
                Mathf.RoundToInt(point.z / 2f)
            );
            //获得起点输入path finding
            Node start = GetCurrentNode();
            //获得目的地输入path finding
            Node target = GridManager.Instance.GetNode(gridPos);
            //不能走不行
            if (target == null || !target.CanWalk())
            {
                Debug.Log("没法走");
                return;
            }
            //超出范围不行
            if (!reachableNodes.Contains(target))
            {
                Debug.Log("超出移动范围");
                return;
            }

            List<Node> path = PathFinding.FindPath(start, target);

            if (path != null)
                //开始移动
                StartCoroutine(FollowPath(path));
        }
    }

    Node GetCurrentNode()
    {
        //获得当前玩家位置
        Vector2Int pos = new Vector2Int(
            Mathf.RoundToInt(transform.position.x / 2f),
            Mathf.RoundToInt(transform.position.z / 2f)
        );

        return GridManager.Instance.GetNode(pos);
    }
    //根据path finding算法输出的路径移动
    IEnumerator FollowPath(List<Node> path)
    {
        state = PlayerState.Moving;

        foreach (Node node in path)
        {
            //获得target客观坐标
            Vector3 targetPos = GridManager.Instance.GetWorldPosition(node);
            //根据path移动
            while (Vector3.Distance(transform.position, targetPos) > 0.05f)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPos,
                    moveSpeed * Time.deltaTime);

                yield return null;
            }
        }

        state = PlayerState.Idle;
        //重新计算range刷新
        currentNode = GetCurrentNode();
        CalculateRange();
    }
    //移动范围限制算法调用
    void CalculateRange()
    {
        reachableNodes = GridRange.GetReachableNodes(currentNode, moveRange);
    }
    //范围画线可视化 可删
    private void OnDrawGizmos()
    {
        if (reachableNodes == null) return;
        if (GridManager.Instance == null) return;

        Gizmos.color = new Color(0f, 0.6f, 1f, 0.35f); // 半透明蓝

        foreach (Node node in reachableNodes)
        {
            Vector3 pos = GridManager.Instance.GetWorldPosition(node);
            pos.y += 0.05f; // 防止和地面Z fighting

            Gizmos.DrawCube(pos, new Vector3(1.8f, 0.02f, 1.8f));
        }
    }
}
