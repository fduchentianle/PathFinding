# PathFinding

Unity 网格寻路学习 Demo，在 Inspector 中选择 A*、BFS、DFS 或 Dijkstra，对随机障碍地图进行寻路。

## 环境

- Unity 2022.3.59f1c1（项目创建版本）
- 通过 Unity Hub 打开项目，等待资源导入和依赖安装完成。

## 使用

1. 打开 `Assets/Scenes/SampleScene.unity`。
2. 在挂有 `TestPathFinding` 的对象上，通过 Inspector 的 `Algorithm` 选择算法。
3. 进入 Play，依次点击起点和终点；绿色表示找到的路径，红色表示障碍。
4. 选完终点后停止地图输入；点击 restart 重新随机生成地图并开始下一次寻路。

算法在启动时选择，切换算法请退出 Play 后修改。若自行搭建场景，请为 `TestPathFinding` 配置地图尺寸、材质和 restart 按钮，并使用带 MainCamera 标签的摄像机及 UI EventSystem。

## 算法

| 算法 | 移动规则 | 搜索方式 |
| --- | --- | --- |
| A* | 八方向，直走 1、斜走 1.4 | 按 g + h 选择节点；当前使用曼哈顿距离，因此不保证最短路径 |
| BFS | 四方向 | 队列，等代价移动时保证最少步数 |
| DFS | 四方向 | 栈，寻找可行路径，不保证最短 |
| Dijkstra | 八方向，直走 1、斜走 1.4 | 按累计代价 g 选择节点，非负代价下保证最短路径 |

八方向算法沿用当前允许斜穿墙角的规则。各算法独立随机生成地图，直接切换运行结果不代表同图性能对比。

节点保存搜索状态和预计算的可通行邻居。A* 与 Dijkstra 使用线性扫描选取最小代价节点，并通过末尾覆盖删除避免列表元素整体前移。

## 目录

- `Assets/Scripts/AStar`：A* 节点和管理器
- `Assets/Scripts/BFS`：BFS 节点和管理器
- `Assets/Scripts/DFS`：DFS 节点和管理器
- `Assets/Scripts/Dijkstra`：Dijkstra 节点和管理器
- `Assets/Scripts/TestPathFinding.cs`：统一的算法选择、点击交互和路径显示
- `Packages`、`ProjectSettings`：Unity 项目依赖和设置

保留 Unity `.meta` 文件；不提交 Library、Temp、Logs 等生成目录。项目包含的第三方资源保留其原有许可文件。
