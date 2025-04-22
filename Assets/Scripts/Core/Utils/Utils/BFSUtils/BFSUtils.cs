using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 通用的BFS（广度优先搜索）工具类
/// </summary>
/// <typeparam name="T">节点类型</typeparam>
public class BFSUtil<T>
{
    /// <summary>
    /// 从起始节点开始执行BFS
    /// </summary>
    /// <param name="start">起始节点</param>
    /// <param name="getNeighbors">获取邻接节点的函数</param>
    /// <param name="process">处理节点的函数</param>
    public void BFS(T start, Func<T, IEnumerable<T>> getNeighbors, Action<T> process)
    {
        if (start == null) return;
        
        Queue<T> queue = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();
        
        queue.Enqueue(start);
        visited.Add(start);
        
        while (queue.Count > 0)
        {
            T current = queue.Dequeue();
            
            // 处理当前节点
            process(current);
            
            // 获取并遍历所有邻接节点
            foreach (T neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
    }
    
    /// <summary>
    /// 带层级信息的BFS遍历
    /// </summary>
    /// <param name="start">起始节点</param>
    /// <param name="getNeighbors">获取邻接节点的函数</param>
    /// <param name="levelProcess">处理节点和其层级的函数</param>
    public void BFSWithLevel(T start, Func<T, IEnumerable<T>> getNeighbors, Action<T, int> levelProcess)
    {
        if (start == null) return;
        
        Queue<T> queue = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();
        Dictionary<T, int> levels = new Dictionary<T, int>();
        
        queue.Enqueue(start);
        visited.Add(start);
        levels[start] = 0;
        
        while (queue.Count > 0)
        {
            T current = queue.Dequeue();
            int currentLevel = levels[current];
            
            // 处理当前节点及其层级
            levelProcess(current, currentLevel);
            
            // 获取并遍历所有邻接节点
            foreach (T neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    levels[neighbor] = currentLevel + 1;
                }
            }
        }
    }
    
    /// <summary>
    /// 寻找从起点到终点的最短路径
    /// </summary>
    /// <param name="start">起点</param>
    /// <param name="end">终点</param>
    /// <param name="getNeighbors">获取邻接节点的函数</param>
    /// <param name="comparer">比较器，用于比较节点是否相等</param>
    /// <returns>最短路径，如果不存在则返回空列表</returns>
    public List<T> FindShortestPath(T start, T end, Func<T, IEnumerable<T>> getNeighbors, IEqualityComparer<T> comparer = null)
    {
        if (start == null || end == null) return new List<T>();
        
        comparer = comparer ?? EqualityComparer<T>.Default;
        
        if (comparer.Equals(start, end)) return new List<T> { start };
        
        Queue<T> queue = new Queue<T>();
        Dictionary<T, T> parentMap = new Dictionary<T, T>(comparer);
        HashSet<T> visited = new HashSet<T>(comparer);
        
        queue.Enqueue(start);
        visited.Add(start);
        
        bool found = false;
        while (queue.Count > 0 && !found)
        {
            T current = queue.Dequeue();
            
            foreach (T neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                    parentMap[neighbor] = current;
                    
                    if (comparer.Equals(neighbor, end))
                    {
                        found = true;
                        break;
                    }
                }
            }
        }
        
        // 如果找不到路径，返回空列表
        if (!found) return new List<T>();
        
        // 回溯重建路径
        List<T> path = new List<T>();
        T currentNode = end;
        
        while (true)
        {
            path.Add(currentNode);
            
            if (comparer.Equals(currentNode, start))
                break;
                
            currentNode = parentMap[currentNode];
        }
        
        // 反转路径，得到从起点到终点的路径
        path.Reverse();
        return path;
    }
    
    /// <summary>
    /// 寻找满足特定条件的节点
    /// </summary>
    /// <param name="start">起始节点</param>
    /// <param name="getNeighbors">获取邻接节点的函数</param>
    /// <param name="predicate">判断节点是否满足条件的函数</param>
    /// <returns>满足条件的节点，如果不存在则返回默认值</returns>
    public T FindNode(T start, Func<T, IEnumerable<T>> getNeighbors, Func<T, bool> predicate)
    {
        if (start == null) return default;
        
        Queue<T> queue = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();
        
        queue.Enqueue(start);
        visited.Add(start);
        
        while (queue.Count > 0)
        {
            T current = queue.Dequeue();
            
            if (predicate(current))
                return current;
                
            foreach (T neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return default;
    }
    
    /// <summary>
    /// 寻找满足特定条件的所有节点
    /// </summary>
    /// <param name="start">起始节点</param>
    /// <param name="getNeighbors">获取邻接节点的函数</param>
    /// <param name="predicate">判断节点是否满足条件的函数</param>
    /// <returns>满足条件的节点列表</returns>
    public List<T> FindAllNodes(T start, Func<T, IEnumerable<T>> getNeighbors, Func<T, bool> predicate)
    {
        List<T> result = new List<T>();
        
        if (start == null) return result;
        
        Queue<T> queue = new Queue<T>();
        HashSet<T> visited = new HashSet<T>();
        
        queue.Enqueue(start);
        visited.Add(start);
        
        while (queue.Count > 0)
        {
            T current = queue.Dequeue();
            
            if (predicate(current))
                result.Add(current);
                
            foreach (T neighbor in getNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }
        
        return result;
    }
}