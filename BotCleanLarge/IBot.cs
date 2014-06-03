using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BotCleanLarge
{
    public interface IBot
    {
        string[] MatrixState { get; set; }
        Position CurrentBotPosition { get; set; }
        EdgeInfo ClosestEdge { get; set; }
        string next_move(int botRow, int botColumn, int gridHeight, int gridWidth, string[] grid);
        void FindClosestEdge(Position i, List<Position> i1, char[,] matrix);
    }

    class Bot : IBot
    {
        const string Right = "RIGHT";
        const string Left = "LEFT";
        private const string Up = "UP";
        private const string Down = "DOWN";
        private const string Clean = "CLEAN";

        public string[] MatrixState { get; set; }

        public Position CurrentBotPosition { get; set; }
        public List<EdgeInfo> Edges { get; set; }
        public EdgeInfo ClosestEdge { get; set; }

        public Bot()
        {
            
        }       

        public string next_move(int botRow, int botColumn, int gridHeight, int gridWidth, string[] grid)
        {
            var matrix = ConvertToDoubleArray(gridHeight, gridWidth, grid);

            var botPosition = new Position(botRow, botColumn);

            var dirtPositions = ScanMatrix(matrix);

            FindClosestEdge(botPosition, dirtPositions, matrix);

            string movement = null;
            Position nextDirtyPoint;

            if (botPosition.Row == ClosestEdge.Position.Row && botPosition.Column == ClosestEdge.Position.Column)
            {
                movement = Clean;
            }
            /* Identify direction to go */
            else if (botPosition.Row == ClosestEdge.Position.Row)
            {
                /* Direction move Left/Right */
                if (botPosition.Column > ClosestEdge.Position.Column)
                {
                    movement = Left;
                }
                else
                {
                    movement = Right;
                }
            }
            else if (botPosition.Column == ClosestEdge.Position.Column)
            {
                // Diretion up/down
                if (botPosition.Row > ClosestEdge.Position.Row)
                {
                    movement = Up;
                }
                else
                {
                    movement = Down;
                }
            }
            else
            {
                //Isolate Matrix by getting all dirty Postions between Bot and Edge

                var minColumn = Math.Min(botPosition.Column, ClosestEdge.Position.Column);
                var maxColumn = Math.Max(botPosition.Column, ClosestEdge.Position.Column);

                var minRow = Math.Min(botPosition.Row, ClosestEdge.Position.Row);
                var maxRow = Math.Max(botPosition.Row, ClosestEdge.Position.Row);


                var isolatedDirtyPositions = dirtPositions.Where(x => x.Column >= minColumn &&
                                                                      x.Column <= maxColumn &&
                                                                      x.Row >= minRow &&
                                                                      x.Row <= maxRow).ToList();

                nextDirtyPoint = NextDirtyPoint(isolatedDirtyPositions, botPosition);

            }


            if (matrix[botPosition.Row, botPosition.Column] == 'd')
            {
                matrix[botPosition.Row, botPosition.Column] = '-';
                dirtPositions.Remove(new Position(botPosition.Row, botPosition.Column));
                CaptureState(gridHeight, matrix);
                return Clean;
            }



            //if (botPosition.Row != nextDirtyPoint.Row)
            //{
            //    movement = botPosition.Row < nextDirtyPoint.Row ? Down : Up;
            //}
            //else
            //{
            //    movement = botPosition.Column < nextDirtyPoint.Column ? Right : Left;
            //}

            matrix[botPosition.Row, botPosition.Column] = '-';

            switch (movement)
            {
                case Down:
                    CurrentBotPosition = new Position(botPosition.Row + 1, botPosition.Column);
                    break;
                case Up:
                    CurrentBotPosition = new Position(botPosition.Row - 1, botPosition.Column);
                    break;
                case Right:
                    CurrentBotPosition = new Position(botPosition.Row, botPosition.Column + 1);
                    break;
                case Left:
                    CurrentBotPosition = new Position(botPosition.Row, botPosition.Column - 1);
                    break;
            }

            if (matrix[CurrentBotPosition.Row, CurrentBotPosition.Column] != 'd')
                matrix[CurrentBotPosition.Row, CurrentBotPosition.Column] = 'b';


            CaptureState(gridHeight, matrix);


            return movement;
        }

        private static Position NextDirtyPoint(List<Position> dirtPositions, Position botPosition)
        {
            var shortestDistance = int.MaxValue;

            var nextDirtyPoint = new Position();
            foreach (var dirtPosition in dirtPositions)
            {
                int dist = Math.Abs(botPosition.Row - dirtPosition.Row) + Math.Abs(botPosition.Column - dirtPosition.Column);
                if (dist < shortestDistance)
                {
                    shortestDistance = dist;
                    nextDirtyPoint = dirtPosition;
                }
            }
            return nextDirtyPoint;
        }


        public void FindClosestEdge(Position botPosition, List<Position> dirtyPositions, char[,] matrix)
        {
            int max_X = int.MinValue;
            int max_Y = int.MinValue;
            int min_X = int.MaxValue;
            int min_Y = int.MaxValue;

            EdgeInfo closestMinX = new EdgeInfo
                {
                   Distance = int.MaxValue
                };

            EdgeInfo closestMinY = new EdgeInfo
            {
                Distance = int.MaxValue
            };

            EdgeInfo closestMaxX = new EdgeInfo
            {
                Distance = int.MaxValue
            };

            EdgeInfo closestMaxY = new EdgeInfo
            {
                Distance = int.MaxValue
            };

            List<EdgeInfo> edges = new List<EdgeInfo>()
                {
                    closestMinX,closestMinY,closestMaxX,closestMaxY
                };

            ClosestEdge = new EdgeInfo();
            ClosestEdge.Distance = int.MaxValue;

            int dirtyDistance_Y;
            int dirtyDistance_X;

            foreach (var dirtyPosition in dirtyPositions)
            {
                dirtyDistance_Y = botPosition.Row - dirtyPosition.Row;
                dirtyDistance_X = dirtyPosition.Column - botPosition.Column;

                if (dirtyDistance_X >= 0 && max_X <= dirtyDistance_X)
                {
                    max_X = dirtyDistance_X;
                    PopulateClosestEdgeInfo(dirtyPosition, closestMaxX, max_X != dirtyDistance_X, dirtyDistance_X, dirtyDistance_Y);
                }
                else if (dirtyDistance_X < 0 && min_X >= dirtyDistance_X)
                {
                    min_X = dirtyDistance_X;
                    PopulateClosestEdgeInfo(dirtyPosition, closestMinX, max_X != dirtyDistance_X, dirtyDistance_X, dirtyDistance_Y);
                }

                if (dirtyDistance_Y >= 0 && max_Y <= dirtyDistance_Y)
                {
                    max_Y = dirtyDistance_Y;
                    PopulateClosestEdgeInfo(dirtyPosition, closestMaxY, max_Y != dirtyDistance_Y, dirtyDistance_X, dirtyDistance_Y);
                    
                }
                else if (dirtyDistance_Y < 0 && min_Y >= dirtyDistance_Y)
                {
                    min_Y = dirtyDistance_Y;
                    PopulateClosestEdgeInfo( dirtyPosition, closestMinY, max_Y != dirtyDistance_Y, dirtyDistance_X, dirtyDistance_Y);
                    
                }
            }


            foreach (var edgeInfo in edges)
            {
                if (edgeInfo.Distance < ClosestEdge.Distance)
                    ClosestEdge = edgeInfo;
            }

        }

        private void PopulateClosestEdgeInfo( Position dirtyPosition, EdgeInfo currentclosestEdge,bool isOverwrite, int distanceX, int distanceY)
        {
            int totalDistance;
            totalDistance = Math.Abs(distanceY) + Math.Abs(distanceX);
            if (isOverwrite || totalDistance < currentclosestEdge.Distance)
            {
                currentclosestEdge.Distance = totalDistance;
                currentclosestEdge.Position = dirtyPosition;
            }
        }

        private void CheckEdgeInfoDistance(EdgeInfo edgeInfo, int totalDistance, Position dirtyPosition)
        {
            if (edgeInfo.Distance < totalDistance)
            {
                edgeInfo.Position = dirtyPosition;
                edgeInfo.Distance = totalDistance;
            }
        }
        
        private char[,] ConvertToDoubleArray(int gridHeight, int gridWidth, string[] grid)
        {
            var matrix = new char[gridHeight,gridWidth];

            for (var rowIndex = 0; rowIndex < grid.Length; rowIndex++)
            {
                var row = grid[rowIndex];

                for (var columnIndex = 0; columnIndex < row.Length; columnIndex++)
                {
                    var character = row[columnIndex];
                    matrix[rowIndex, columnIndex] = character;
                }
            }
            return matrix;
        }

        private static List<Position> ScanMatrix(char[,] matrix)
        {
            var dirtPositions = new List<Position>();

            for (int x = 0; x < matrix.GetLength(0); x++)
            {
                for (int y = 0; y < matrix.GetLength(1); y++)
                {
                    var character = matrix[x, y];

                    if (character == 'd')
                    {
                        dirtPositions.Add(new Position(x, y));
                    }
                }
            }
            return dirtPositions;
        }

        private void CaptureState(int gridHeight, char[,] matrix)
        {
            MatrixState = new string[gridHeight];

            for (int rowIndex = 0; rowIndex < matrix.GetLength(0); rowIndex++)
            {
                var builder = new StringBuilder();

                for (int columnIndex = 0; columnIndex < matrix.GetLength(1); columnIndex++)
                {
                    var character = matrix[rowIndex, columnIndex];

                    builder.Append(character);
                }

                MatrixState[rowIndex] = builder.ToString();
            }
        }
    }

    public class EdgeInfo
    {
        public EdgeInfo()
        {
            Distance = -1;
        }

        public Position Position { get; set; }
        public int Distance { get; set; }
 
    }
    
}