using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Threading;
using System.Diagnostics;

namespace FurAnjel
{
    class Mountains

    {
        public static int AreaSize = 512;
        public static int Height = 255;
        public static int DetailScaler = 4;

        public static Random random = new Random();

        public static double[,] HeightMap;
        public static Dictionary<Point, NodePoint> Nodes = new Dictionary<Point, NodePoint>();
        public static Point[] NodeArray;
        public static double[] NodeValueHeights;
        public static Point Center;
        public static double MaxDistance;
        public static Stopwatch SWA = new Stopwatch();

        public static double[,] RandomMountainHeightMap(int Size, int Height, int Movement, double Roughness)
        {
            HeightMap = new double[Size, Size];

            int CenterPosition = Size / 2;
            Center = new Point(CenterPosition, CenterPosition);

            MaxDistance = Math.Sqrt(Size * Size * 2) / 2;

            for (int i = 0; i < Movement; i++)
            {
                MakeBranch(Center, Size, Movement, Roughness, 0);
            }

            NodeArray = Nodes.Keys.ToArray();
            NodeValueHeights = new double[NodeArray.Length];
            for (int i = 0; i < NodeArray.Length; i++)
            {
                NodeValueHeights[i] = Nodes[NodeArray[i]].Height;
            }

            Console.WriteLine("prep...");

            SWA.Start();
            for (int X = 0; X < CenterPosition; X++)
            {
                int TrueX = CenterPosition - X - 1;
                for (int Y = 0; Y < CenterPosition; Y++)
                {
                    int TrueY = CenterPosition - Y - 1;
                    int Distance = (TrueX + TrueY) / 2;
                    FillPoint(HeightMap, Height, new Point(TrueX + CenterPosition, TrueY + CenterPosition));
                    FillPoint(HeightMap, Height, new Point(TrueX + CenterPosition, CenterPosition - TrueY));
                    FillPoint(HeightMap, Height, new Point(CenterPosition - TrueX, CenterPosition - TrueY));
                    FillPoint(HeightMap, Height, new Point(CenterPosition - TrueX, TrueY + CenterPosition));
                }
            }
            SWA.Stop();

            return HeightMap;

        }

        public static Stopwatch SWB = new Stopwatch();

        public static void FillPoint(double[,] HeightMap, int Height, Point point)
        {
            double multo = 1;
            double LowestScore = MaxDistance * MaxDistance;
            Point Lowest = Center;
            bool correcto = false;

            SWB.Start();

            // TODO: Swap this for outward spiraling detail-scaled pixel check maybe? 0.o might be work a try - maybe a helper wider grid of booleans to indicate what cells are worth checking.
            for (int ix = 0; ix < NodeArray.Length; ix++)
            {
                Point node = NodeArray[ix];
                double x = point.X - node.X * DetailScaler;
                double y = point.Y - node.Y * DetailScaler;
                double h = NodeValueHeights[ix];
                double CurrentScore = (x * x + y * y);// * h;
                if (CurrentScore < LowestScore)
                {
                    if (Nodes[node].Point.X == int.MaxValue)
                    {
                        continue;
                    }
                    LowestScore = CurrentScore;
                    Lowest = node;
                }
            }
            /* NodePoint LowestHelp = Nodes[Lowest];
             Point SecondLowest;
             double sl_x = LowestHelp.Next.X * DetailScaler - point.X;
             double sl_y = LowestHelp.Next.Y * DetailScaler - point.Y;
             double sl2_x = LowestHelp.Previous.X * DetailScaler - point.X;
             double sl2_y = LowestHelp.Previous.Y * DetailScaler - point.Y;
             double lsa = (sl_x * sl_x + sl_y * sl_y);
             double lsb = (sl2_x * sl2_x + sl2_y * sl2_y);
             SecondLowest = lsa < lsb ? LowestHelp.Next : LowestHelp.Previous;
             double SecondLowestScore = Math.Min(lsa, lsb);// * LowestHelp.Height;
             if (!Nodes.ContainsKey(SecondLowest) || SecondLowest == Lowest)
             {
                 SecondLowest = lsa >= lsb ? LowestHelp.Next : LowestHelp.Previous;
                 if (!Nodes.ContainsKey(SecondLowest) || SecondLowest == Lowest)
                 {
                     */
            double SecondLowestScore = MaxDistance * MaxDistance;
            Point SecondLowest = Lowest;
            for (int ix = 0; ix < NodeArray.Length; ix++)
            {
                Point node = NodeArray[ix];
                double h = NodeValueHeights[ix];
                double tx = Lowest.X - node.X;
                double ty = Lowest.Y - node.Y;
                double relVal = (tx * tx + ty * ty);
                double x = point.X - node.X * DetailScaler;
                double y = point.Y - node.Y * DetailScaler;
                double CurrentScore = (x * x + y * y);// * h;
                if (CurrentScore < SecondLowestScore && relVal < 6 && node != Lowest)
                {
                    SecondLowestScore = CurrentScore;
                    SecondLowest = node;
                }
            }
            /*  Console.WriteLine("Dorp?");
              if (SecondLowest == Lowest)
              {
                  Console.WriteLine("Dorp!!!!");
              }
            //  correcto = true;
          }
      }*/

            SWB.Stop();

            double fScore, nL;

            if (SecondLowestScore - LowestScore > 20)
            {
                double xert = Lowest.X * DetailScaler - point.X;
                double yert = Lowest.Y * DetailScaler - point.Y;
                fScore = Math.Sqrt(xert * xert + yert * yert);
                nL = Nodes[Lowest].Height;
            }
            else
            {
                double line_A = (Lowest.Y - SecondLowest.Y) * DetailScaler;
                double line_B = (SecondLowest.X - Lowest.X) * DetailScaler;
                double line_C = (SecondLowest.Y * Lowest.X * DetailScaler * DetailScaler - SecondLowest.X * Lowest.Y * DetailScaler * DetailScaler);
                fScore = (line_A * point.X + line_B * point.Y + line_C) / Math.Sqrt(line_A * line_A + line_B * line_B);
                double nA = Nodes[Lowest].Height;
                double nB = Nodes[SecondLowest].Height;
                double perpX = (SecondLowest.X - Lowest.X) * DetailScaler;
                double perpY = (SecondLowest.Y - Lowest.Y) * DetailScaler;
                double len = 1.0 / Math.Sqrt(perpX * perpX + perpY * perpY);
                perpX *= len;
                perpY *= len;

                fScore = Math.Abs(fScore);

                double pointerX = point.X + perpY * fScore;
                double pointerY = point.Y + perpX * fScore;
                double resX = (pointerX - Lowest.X);
                double resY = (Lowest.Y - pointerY);
                double nlen = Math.Sqrt(resX * resX + resY * resY);

                double lenner = nlen * len;

                //Console.WriteLine(lenner + " : " + fScore + " vs " + LowestScore + " - " + nlen + ", " + len + ", " + (pointerX - Lowest.X * DetailScaler) + ", " + (Lowest.Y * DetailScaler - pointerY));

                if (lenner < 0.05 || lenner > 0.95)
                {
                    lenner = 0.5;
                    //double xert = Lowest.X - point.X;
                    //double yert = Lowest.Y - point.Y;
                    //fScore = Math.Sqrt(xert * xert + yert * yert);// nA;
                }
                nL = nB * lenner + nA * (1.0 - lenner);
            }

            double resser = 0;
            int c = 0;

            const int samples = 3;
            for (int sx = -samples; sx <= samples; sx++)
            {
                for (int sy = -samples; sy <= samples; sy++)
                {
                    if (point.X + sx >= 0 && point.Y + sy >= 0 && point.X + sx < AreaSize && point.Y + sy < AreaSize)
                    {
                        double h = HeightMap[point.X + sx, point.Y + sy];
                        if (h > 0 && random.NextDouble() > 0.5)
                        {
                            resser += h;
                            c++;
                        }
                    }
                }
            }
            if (c > 0)
            {
                resser /= c;
            }
            /*double resx = point.X / DetailScaler - Center.X;
            double resy = point.Y / DetailScaler - Center.Y;
            resser = (resx * resx + resy * resy) / AreaSize;*/

            double Result;

            if (correcto)
            {
                Result = Math.Min(Height, Math.Max(1, resser));
            }
            else
            {
                Result = Math.Min(Height, Math.Max(1, (resser + (nL - fScore * 1.0)) * 0.5 * multo));
                //  Result = Math.Min(255, Math.Max(1, (nL - fScore * 1.0) * multo));
            }


            HeightMap[point.X, point.Y] = Result;
        }

        public static void MakeBranch(Point Origin, int Size, int Movement, double Roughness, int Depth)
        {

            Increment increment = GetIncrement(Center.X - Origin.X, Center.Y - Origin.Y);
            Increment inc1 = increment;
            double OriginDistance = Math.Max(Math.Sqrt(increment.X * increment.X + increment.Y * increment.Y), 1);
            double DistanceModifier = Size / Movement * (Depth + 1);
            increment = GetIncrement((increment.X / OriginDistance + random.NextDouble() - 0.5) * DistanceModifier, (increment.Y / OriginDistance + random.NextDouble() - 0.5) * DistanceModifier);

            Point Destination = FixBounds(new Point((int)Math.Round(Center.X + increment.X),
                (int)Math.Round(Center.Y + increment.Y)), Size);

            List<Point> Points = MakeLine(Origin, Destination, Size, Roughness);
            int PointSize = Points.Count;

            if (PointSize == 0)
            {
                return;
            }

            Destination = Points[PointSize - 1];
            double distX = Center.X - Destination.X;
            double distY = Center.Y - Destination.Y;
            double DestinationDistance = Math.Max(Math.Sqrt(distX * distX + distY * distY), 1);

            double HeightStart = Height * ((MaxDistance - OriginDistance) / MaxDistance);
            double HeightDecay = (HeightStart - Height * ((MaxDistance - DestinationDistance) / MaxDistance)) / PointSize;

            int Branching = 0;

            Point Previous = new Point(Origin.X / DetailScaler, Origin.Y / DetailScaler);
            Point prev2 = Previous;
            Point Next;

            for (int i = 0; i < PointSize; i++)
            {
                double HeightValue = HeightStart - HeightDecay * (i + 1);

                Point point = Points[i];

                Point ScaledPoint = new Point(point.X / DetailScaler, point.Y / DetailScaler);

                if (prev2.X == ScaledPoint.X && prev2.Y == ScaledPoint.Y)
                {
                    prev2 = new Point(ScaledPoint.X - Math.Sign(inc1.X), ScaledPoint.Y - Math.Sign(inc1.Y));
                    if (prev2.X == ScaledPoint.X && prev2.Y == ScaledPoint.Y)
                    {
                        prev2 = new Point(ScaledPoint.X - 1, ScaledPoint.Y - 1);
                    }
                    if (!Nodes.ContainsKey(prev2))
                    {
                        //Nodes.Add(prev2, GetNodePoint(new Point(int.MaxValue, int.MaxValue), prev2, ScaledPoint, HeightStart - HeightDecay * (i)));
                    }
                }


                Next = ScaledPoint;
                int n = 1;
                while (i + n < PointSize)
                {
                    Next = new Point(Points[i + n].X / DetailScaler, Points[i + n].Y / DetailScaler);
                    if (Next.X != ScaledPoint.X || Next.Y != ScaledPoint.Y)
                    {
                        break;
                    }
                    n++;
                }
                if (i + n >= PointSize)
                {
                    Next = new Point(Destination.X / DetailScaler, Destination.Y / DetailScaler);
                    if (Next.X == ScaledPoint.X && Next.Y == ScaledPoint.Y)
                    {
                        Next = new Point(ScaledPoint.X + Math.Sign(increment.X), ScaledPoint.Y + Math.Sign(increment.Y));
                        if (!Nodes.ContainsKey(Next))
                        {
                            //Nodes.Add(Next, GetNodePoint(ScaledPoint, Next, new Point(int.MaxValue, int.MaxValue), HeightStart - HeightDecay * (i + 2)));
                        }
                    }
                }
                if (Next.X == ScaledPoint.X && Next.Y == ScaledPoint.Y)
                {
                    Console.WriteLine("DERP NEXT");
                }
                NodePoint ScaledNodePoint = GetNodePoint(prev2, ScaledPoint, Next, HeightValue);

                if (!Nodes.ContainsKey(ScaledPoint))
                {
                    Nodes.Add(ScaledPoint, ScaledNodePoint);
                    prev2 = Previous;
                    Previous = ScaledPoint;
                }
                HeightMap[point.X, point.Y] = HeightValue;

                if (random.NextDouble() < 0.2 && Depth < Movement && Branching < 5)
                {
                    Branching++;
                    MakeBranch(Destination, Size, Movement, Roughness, Depth + 1);
                }
                //i++;

            }



        }

        public struct Increment
        {
            public double X;
            public double Y;
        }
        public static Increment GetIncrement(double X, double Y)
        {
            return new Increment() { X = X, Y = Y };
        }

        public struct NodePoint
        {
            public Point Previous;
            public Point Next;
            public Point Point;
            public double Height;
        }
        public static NodePoint GetNodePoint(Point Previous, Point Point, Point Next, double Height)
        {
            return new NodePoint() { Previous = Previous, Point = Point, Next = Next, Height = Height };
        }

        public static List<Point> MakeLine(Point Origin, Point Destination, int Size, double Roughness)
        {
            int Center = Size / 2;

            double distX = Origin.X - Destination.X;
            double distY = Origin.Y - Destination.Y;
            int Distance = (int)Math.Round(Math.Sqrt(distX * distX + distY * distY));

            int NodeCount = Math.Max(Distance, 1);

            List<Point> Line = new List<Point>(NodeCount);

            Increment increment = GetIncrement((Origin.X - Destination.X) / (double)NodeCount, (Origin.Y - Destination.Y) / (double)NodeCount);
            Increment Modified = increment;

            Point CurrentPoint = new Point(Origin.X, Origin.Y);

            Line.Add(CurrentPoint);
            for (int i = 0; i < NodeCount; i++)
            {

                if (random.NextDouble() > 0.95)
                {
                    Modified = GetIncrement((increment.X + (random.NextDouble() - 0.5) * Roughness), increment.Y + (random.NextDouble() - 0.5) * Roughness);
                }

                CurrentPoint = new Point((int)Math.Round(CurrentPoint.X + Modified.X + ((random.NextDouble() - 0.5) * Roughness)),
                (int)Math.Round(CurrentPoint.Y + Modified.Y + ((random.NextDouble() - 0.5) * Roughness)));

                if (CurrentPoint.X < 0 || CurrentPoint.X >= Size || CurrentPoint.Y < 0 || CurrentPoint.Y >= Size)
                {
                    return Line;
                }

                CurrentPoint = FixBounds(CurrentPoint, Size);



                Line.Add(CurrentPoint);
            }


            Line.Add(CurrentPoint);

            return Line;
        }




        public static Point FixBounds(Point point, int Size)
        {
            Size--;
            if (point.X < 0)
            {
                point.X = 0;
            }
            else if (point.X > Size)
            {
                point.X = Size;
            }
            if (point.Y < 0)
            {
                point.Y = 0;
            }
            else if (point.Y > Size)
            {
                point.Y = Size;
            }
            return point;
        }

    }
}