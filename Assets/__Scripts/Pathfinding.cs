using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Debug = System.Diagnostics.Debug;

/// <summary>
/// Class for an adaptive A* pathfinding algorithm
/// </summary>
public class Pathfinding : MonoBehaviour
{
    public enum Aim
    {
        Place,
        Thing
    }
    
    /// <summary>
    /// Contains information about aim of the pathfinding search
    /// </summary>
    public class ActionAim
    {
        /// <summary>
        /// Initializes ActionAim if algorithm aim is to find a way to an object
        /// </summary>
        /// <param name="target">Object to find a way to</param>
        public ActionAim( GameObject target = default)
        {
            Aim = Aim.Thing;
            if ((Target = target) == null) return;
            var posTile = Tile.TransToTile(target.transform.position);
            X = (int)posTile.x;
            Y = (int)posTile.y;
            Init = false;
        }

        /// <summary>
        /// Initializes ActionAim if algorithm aim is to find a way to a particular place
        /// </summary>
        /// <param name="x">Coordinate x of the tile to which path is searched</param>
        /// <param name="y">Coordinate y of the tile to which path is searched</param>
        public ActionAim(int x, int y)
        {
            Aim = Aim.Place;
            X = x;
            Y = y;
            Init = false;
        }
        
        /// <summary>
        /// Shows if pathfinding trying to find path to a defined place or some object which could move
        /// </summary>
        public Aim Aim{ get; private set; }
        
        /// <summary>
        /// Keeps target gameObject if it is trying to find a way to an object, not a place
        /// </summary>
        public GameObject Target { get; private set; }
        public int X{ get; private set; }
        public int Y{ get; private set; }
        
        /// <summary>
        /// Shows if with there was a pathfinding search with the aim 
        /// </summary>
        public bool Init;
        
        /// <summary>
        /// Checks if aim of search was moved to another tile
        /// </summary>
        /// <returns>Distance from the last checked place of the aim and modern one</returns>
        public double IsMoved()
        {
            if (Target == null) return 0;
            var posTile = Tile.TransToTile(Target.transform.position);
            var isMoved = X != (int)posTile.x || Y != (int)posTile.y;
            if (!isMoved) return 0;
            double result=Math.Max(Math.Abs(X - posTile.x), Math.Abs(Y - posTile.y));
            X = (int)posTile.x;
            Y = (int)posTile.y;
            return result;
        }
    }
    
    /// <summary>
    /// Struct to keep coordinates on TileMap and weights
    /// </summary>
    /// <remarks>In this algorithm often tiles would be named cells and vise versa</remarks>
    public class Cell:IComparable
    {
        public int X { get; }
        public int Y { get; }

        /// <summary>
        /// Coordinate X of cell which was previous on the way to the cell
        /// </summary>
        public int PreX;

        /// <summary>
        /// Coordinate Y of cell which was previous on the way to the cell
        /// </summary>
        /// <remarks> No, it is not a game </remarks>
        public int PreY;

        /// <summary>
        /// Estimated distance of the shortest way, which is going
        /// through the cell, between start point and target point
        /// </summary>
        private double F => G*G + H*H;

        /// <summary>
        /// Heuristics value 
        /// </summary>
        public double H;

        /// <summary>
        /// Steps to this cell from the start 
        /// </summary>
        public double G;
        
        /// <summary>
        /// Num of last search of adaptive A* which used this value
        /// </summary>
        /// <remarks>For reason of protection from overflow this value is looped on value 60,
        /// so it can't be bigger</remarks>
        public int NumSearch;

        private readonly Pathfinding _pathfinding;
        
        /// <summary>
        /// Cell initialization
        /// </summary>
        /// <param name="x">x coordinate of the Cell</param>
        /// <param name="y">y coordinate of the Cell</param>
        /// <param name="p">Pathfinding component in which the Cell is</param>
        public Cell(int x, int y, Pathfinding p)
        {
            X = x;
            Y = y;
            PreX = -1;
            PreY = -1;
            G = 514;
            H = 514;
            _pathfinding = p;
            NumSearch = 0;
        }

        public static bool operator <=(Cell a, Cell b) => a.F <= b.F;
        public static bool operator >=(Cell a, Cell b) => a.F >= b.F;
        public static bool operator <(Cell a, Cell b) => a.F < b.F;
        public static bool operator >(Cell a, Cell b) => a.F > b.F;
        
        /// <summary>
        /// Heuristic for A* algorithm
        /// </summary>
        private void Heuristic()
        {
            H = Math.Max(Math.Abs(X - _pathfinding._target.X), Math.Abs(Y - _pathfinding._target.Y));
        }
        
        /// <summary>
        /// Heuristic for adaptive A* algorithm
        /// </summary>
        /// <remarks>It utilizes knowledge from the previous runs through adaptive A*,
        /// in order to create more optimal heuristic</remarks>
        public void AdaptiveHeuristic()
        {
            if (NumSearch != 0)
            {
                if (G + H < _pathfinding.PathCost[NumSearch - 1])
                    H = _pathfinding.PathCost[NumSearch - 1] - G;
                H -= _pathfinding.DeltaH[_pathfinding._searchNum - 1] - _pathfinding.DeltaH[NumSearch - 1];
                H = Math.Max(H, Math.Max(Math.Abs(X - _pathfinding._target.X), Math.Abs(Y - _pathfinding._target.Y)));
            }
            else
                Heuristic();

            NumSearch = _pathfinding._searchNum;
        }
        /// <summary>
        /// Checks if required Cell is the target
        /// </summary>
        /// <returns>true, if Cell is the target, otherwise false</returns>
        public bool IsTarget()
        {
            return X == _pathfinding._target.X && Y == _pathfinding._target.Y;
        }
        
        /// <summary>
        /// Implementation of CompareTo from IComparable
        /// In this case used in order to use SortedSet
        /// </summary>
        /// <param name="obj"> Object to compare with</param>
        /// <returns>1 -- if "bigger", -1 -- if "less"</returns>
        public int CompareTo(object obj)
        {
            if (obj == null) 
                return 1;
            var t = (Cell) obj;
            if (t.X == X && t.Y == Y)
                return 0;
            if (this > t || F == t.F && (X > t.X || X == t.X && Y > t.Y))
                return 1;
            return -1;
        }
    }

    /// <summary>
    /// Stack which includes the way to the aim
    /// </summary>
    public Stack<Cell> Path { get; private set; }

    // Target to achieve
    private ActionAim _target;
    // Start position                      
    private int _startX;
    private int _startY;

    private List<Cell> _oldClosed;
    
    /// <summary>
    /// Array with costs of the path on the n-th search in the n-th element
    /// </summary>
    private double[] PathCost { get; set; }
    /// <summary>
    /// Array with heuristic difference between n+1-th search aim and n-th search aim displacements
    /// </summary>
    private double[] DeltaH { get; set; }
    
    /// <summary>
    /// Shows, if search was correctly initialized 
    /// </summary>
    private bool _initialized = false;
    /// <summary>
    /// Shows, if destination was found by A* - search
    /// </summary>
    private bool _foundDest = false;
    /// <summary>
    /// Number of the A*-searches of the ways to the same aim in a row
    /// </summary>
    private int _searchNum; 
    /// <summary>
    /// Information about map tiles for A* - algorithm
    /// </summary>
    private readonly Cell[,] _cellGrid = new Cell[256, 256];
    
    /// <summary>
    /// Initializes all values which are needed for a first pass through the A* algorithm
    /// </summary>
    /// <param name="actionAim"></param>
    private void Initial(ActionAim actionAim)
    {
        _searchNum = 0;
        PathCost = new double[60];
        _oldClosed = new List<Cell>();
        _target = actionAim;
        for (var i = 0; i < 60; i++)
        {
            PathCost[i] = -1;
        }
        for (var i = 0; i < 256; i++)
        for (var j = 0; j < 256; j++)
        {
            if (Map.TileMap[i, j] > -1 && Map.TileMap[i, j] < 224)
            {
                _cellGrid[i, j] = new Cell(i, j, this);
            }
        }

        _foundDest = false;
        _initialized = true;
    }

    /// <summary>
    /// Takes the Cell from the chosen direction from <paramref name="cell"/> in one tile distance
    /// </summary>
    /// <param name="direction">Direction from which would be taken new Cell</param>
    /// <param name="cell">Cell relative to which method would take cell ta</param>
    /// <returns>Cell from the chosen direction from cell in one tile distance, if it is exist, else null</returns>
    /// <exception cref="ArgumentOutOfRangeException">If value of Map.Direction is out of Map.Direction scale</exception>
    private Cell Check(Map.Direction direction, Cell cell)
    {
        Cell t;
        switch (direction)
        {
            case Map.Direction.Up:
                t = _cellGrid[cell.X, cell.Y + 1];
                return t;
            case Map.Direction.UpRight:
                t = _cellGrid[cell.X + 1, cell.Y + 1];
                return t;
            case Map.Direction.Right:
                t = _cellGrid[cell.X + 1, cell.Y];
                return t;
            case Map.Direction.DownRight:
                t = _cellGrid[cell.X + 1, cell.Y - 1];
                return t;
            case Map.Direction.Down:
                t = _cellGrid[cell.X, cell.Y - 1];
                return t;
            case Map.Direction.DownLeft:
                t = _cellGrid[cell.X - 1, cell.Y - 1];
                return t;
            case Map.Direction.Left:
                t = _cellGrid[cell.X - 1, cell.Y];
                return t;
            case Map.Direction.UpLeft:
                t = _cellGrid[cell.X - 1, cell.Y + 1];
                return t;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
    }
    
    /// <summary>
    /// Initialization of the new adaptive A* path search
    /// </summary>
    /// <param name="actionAim">Information about aim of the pathfinding</param>
    /// <returns>true, if new search was initialised, or false,
    /// if there is no need in pathfinding(for example, aim has not changed position from the last search)</returns>
    private bool NewSearch(ActionAim actionAim)
    {
        _initialized = actionAim.Init;
        if (actionAim.Aim == Aim.Place  || !_initialized )
        {
            DeltaH = new double[60]; 
            DeltaH[_searchNum] = 0;
            Initial(actionAim);
            actionAim.Init = _initialized;
        }
        else
        {
            var t = _target.IsMoved();
                if (t == 0) return false;
            var next = _searchNum == 60 ? 0 : _searchNum;
            DeltaH[next] = DeltaH[_searchNum - 1] + t;
        }
        if(_searchNum == 60)
            for (var j = 1; j < 60; j++)
            {
                DeltaH[j] -= DeltaH[0];
            }
        if(actionAim.Aim == Aim.Thing)
            _searchNum = _searchNum % 60 + 1;
        if (_oldClosed.Count == 0) return true;
        var i = 0;
        for (; i < _oldClosed.Count && _oldClosed[i].NumSearch == _searchNum; i++)
        {
            _oldClosed[i].NumSearch = 0;
        }
        _oldClosed.RemoveRange(0,i);
        return true;
    }
    /// <summary>
    /// Adaptive A* pathfinding function
    /// </summary>
    /// <param name="startX">Coordinate x in tiles of the search start point</param>
    /// <param name="startY">Coordinate Y in tiles of the search start point</param>
    /// <param name="actionAim">Information about aim of the pathfinding</param>
    public void AStarSearchAdaptive(int startX, int startY, ActionAim actionAim)
    {
        if (!NewSearch(actionAim) || !_initialized) return;
        // Initialization of the start coordinates
        _startX = startX;
        _startY = startY;
        // Cells which are going to be checked if could be checked if they could lead to the aim
        var opened = new SortedSet<Cell>();     // Sorted Cell is used because there are a lot of insert and remove operations
        // Cells which have been already checked or which are in SortedSet<Cell> opened
        var closed = new SortedSet<Cell>();
        // Initialization of the first cell (start cell)
        var tCell = _cellGrid[_startX, _startY];
        tCell.G = 0;
        tCell.H = 0;
        tCell.NumSearch = _searchNum;
        opened.Add(tCell);
        closed.Add(tCell);
        _oldClosed.Add(tCell);
        //Path search itself
        while (opened.Count > 0)
        {
            var prevCell = opened.Min;
            // Checks if modern cell is a target (it is not a condition in the external while loop because
            // it is possible that target cell exists but not achievable by any means
            if (prevCell.IsTarget())
            {
                _foundDest = true;
                break;
            }
            opened.Remove(prevCell);
            // Check if adjacent cells exist and are achievable(corner cells are not achievable from the cell if
            // non diagonal adjacent cells are not) 
            tCell = Check(Map.Direction.Up, prevCell);
            bool up;
            if ((up = tCell != null) && !closed.Contains(tCell))
            {
                tCell.PreX = prevCell.X;
                tCell.PreY = prevCell.Y;
                tCell.AdaptiveHeuristic();
                tCell.G = prevCell.G + 1;
                tCell.NumSearch = _searchNum;
                opened.Add(tCell);
                closed.Add(tCell);
                _oldClosed.Add(tCell);
            }

            tCell = Check(Map.Direction.Down, prevCell);
            bool down;
            if ((down = tCell != null) && !closed.Contains(tCell))
            {
                tCell.PreX = prevCell.X;
                tCell.PreY = prevCell.Y;
                tCell.AdaptiveHeuristic();
                tCell.G = prevCell.G + 1;
                tCell.NumSearch = _searchNum;
                opened.Add(tCell);
                closed.Add(tCell);
                _oldClosed.Add(tCell);
            }

            tCell = Check(Map.Direction.Right, prevCell);
            bool right;
            if ((right = tCell != null) && !closed.Contains(tCell))
            {
                tCell.PreX = prevCell.X;
                tCell.PreY = prevCell.Y;
                tCell.AdaptiveHeuristic();
                tCell.G = prevCell.G + 1;
                tCell.NumSearch = _searchNum;
                opened.Add(tCell);
                closed.Add(tCell);
                _oldClosed.Add(tCell);
            }

            tCell = Check(Map.Direction.Left, prevCell);
            bool left;
            if ((left = tCell != null) && !closed.Contains(tCell))
            {
                tCell.PreX = prevCell.X;
                tCell.PreY = prevCell.Y;
                tCell.AdaptiveHeuristic();
                tCell.G = prevCell.G + 1;
                tCell.NumSearch = _searchNum;
                opened.Add(tCell);
                closed.Add(tCell);
                _oldClosed.Add(tCell);
            }

            if (up && right)
            {
                tCell = Check(Map.Direction.UpRight, prevCell);
                if (tCell != null && !closed.Contains(tCell))
                {
                    tCell.PreX = prevCell.X;
                    tCell.PreY = prevCell.Y;
                    tCell.AdaptiveHeuristic();
                    tCell.G = prevCell.G + Math.Sqrt(2);
                    tCell.NumSearch = _searchNum;
                    opened.Add(tCell);
                    closed.Add(tCell);
                    _oldClosed.Add(tCell);
                }
            }

            if (up && left)
            {
                tCell = Check(Map.Direction.UpLeft, prevCell);
                if (tCell != null && !closed.Contains(tCell))
                {
                    tCell.PreX = prevCell.X;
                    tCell.PreY = prevCell.Y;
                    tCell.AdaptiveHeuristic();
                    tCell.G = prevCell.G + Math.Sqrt(2);
                    tCell.NumSearch = _searchNum;
                    opened.Add(tCell);
                    closed.Add(tCell);
                    _oldClosed.Add(tCell);
                }
            }

            if (down && right)
            {
                tCell = Check(Map.Direction.DownRight, prevCell);
                if (tCell != null && !closed.Contains(tCell))
                {
                    tCell.PreX = prevCell.X;
                    tCell.PreY = prevCell.Y;
                    tCell.AdaptiveHeuristic();
                    tCell.G = prevCell.G + Math.Sqrt(2);
                    tCell.NumSearch = _searchNum;
                    opened.Add(tCell);
                    closed.Add(tCell);
                    _oldClosed.Add(tCell);
                }
            }

            if (down && left)
            {
                tCell = Check(Map.Direction.DownLeft, prevCell);
                if (tCell != null && !closed.Contains(tCell))
                {
                    tCell.PreX = prevCell.X;
                    tCell.PreY = prevCell.Y;
                    tCell.AdaptiveHeuristic();
                    tCell.G = prevCell.G + Math.Sqrt(2);
                    tCell.NumSearch = _searchNum;
                    opened.Add(tCell);
                    closed.Add(tCell);
                    _oldClosed.Add(tCell);
                }
            }
        }
        // Path generation
        Path = new Stack<Cell>();
        if (!_foundDest) return;
        var x = _target.X;
        var y = _target.Y;
        // Path for a movement to a place
        if (actionAim.Aim == Aim.Place)
        {
            while (_startX != x || _startY != y)
            {
                tCell = _cellGrid[x, y];
                Path.Push(tCell);
                x = tCell.PreX;
                y = tCell.PreY;
            }
        }
        else
        {
            // Path for a movement to an object (it should ends not in the same cell as an target object,
            // excluding situation when it was start at the same cell)
            PathCost[_searchNum - 1] = _cellGrid[x, y].G;
            if (_startX == x && _startY == y) return;
            x = _cellGrid[x, y].PreX;
            y = _cellGrid[_target.X, y].PreY;
            while (_startX != x || _startY != y)
            {
                tCell = _cellGrid[x, y];
                Path.Push(tCell);
                x = tCell.PreX;
                y = tCell.PreY;
            } 
        }
    }
}
