using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FurAnjel
{
    class CaveGenerator
    {

        public static Random Random = new Random();
        
        public static List<Vector3> GenerateCave()
        {
            List<Vector3> Result = new List<Vector3>();
            //Result.AddRange(JaggedTunnel(new Vector3(0, 0, 0), RandomOffset(1000), 5, 25, 0.25));
            Result.AddRange(Pocket(new Vector3(0, 0, 0), new Vector3(0, 1, 0), 15, 1));








            return Result;
        }


        public static List<Vector3> Pocket(Vector3 Start, Vector3 direction, int Size, double Roughness)
        {
            List<Vector3> Pocket = new List<Vector3>();

            Vector3 Perpendicular = PerpendicularVectors(direction)[1];
            
            for (int i = 0; i < Size; i++)
            {
                Pocket.Add(Start + direction * i);
                Pocket.Add(Start + direction * i * -1);

                for (int j = 0; j < Size; j++)
                {
                    Pocket.Add(Start + direction * i + Perpendicular * (j - i + 1));
                    Pocket.Add(Start + direction * i * -1 + Perpendicular * (j - i + 1) * -1);
                }
            }

            return Pocket;
        }



        // provides a tunnel connecting 2 Points
        // roughness = how far from the original point to point trajectory the path can be
        // movement = how frequently it changes direction
        // size = radius of tunnel
        public static List<Vector3> JaggedTunnel(Vector3 Start, Vector3 End, int Size, double Roughness, double Movement)
        {
            List<Vector3> Tunnel = new List<Vector3>();

            Vector3 Heading = Start - End;
            Heading.Normalize();

            Vector3 Modified = Heading;
            
            int circumference = (int)Math.Ceiling(2 * Math.PI * Size) + 1;

            int Distance = (int)Math.Ceiling(Math.Sqrt(Math.Pow(Start.X - End.X, 2) + Math.Pow(Start.Y - End.Y, 2) + Math.Pow(Start.Z - End.Z, 2)));
            Vector3 CurrentLocation = Start;
            Tunnel.Add(CurrentLocation);

            for (int i = 0; i <= Distance; i++)
            {
                if (Random.NextDouble() < Movement)
                {
                    Modified = Heading + new Vector3((float)(Heading.X + (Random.NextDouble() - 0.5)  * Roughness), (float)(Heading.Y + (Random.NextDouble() - 0.5) * Roughness),
                        (float)(Heading.Z + (Random.NextDouble() - 0.5) * Roughness));
                    Modified.Normalize();
                }
                
                CurrentLocation = new Vector3((float)Math.Round(CurrentLocation.X + Modified.X), (float)Math.Round(CurrentLocation.Y + Modified.Y),
                    (float)Math.Round(CurrentLocation.Z + Modified.Z));
                Tunnel.Add(CurrentLocation);

                for (int r = 0; r < circumference; r++)
                {
                    for (int d = 1; d <= Size; d++)
                    {
                        Vector3 resultlocation = CurrentLocation + (RotateVector(Modified, PerpendicularVector(Modified) * d, (float)(360 / circumference * r * Math.PI / 180)));
                        resultlocation += RandomOffset(1);
                        resultlocation = new Vector3((float)Math.Round(resultlocation.X), (float)Math.Round(resultlocation.Y), (float)Math.Round(resultlocation.Z));
                        Tunnel.Add(resultlocation);
                    }
                }

                

            }
            
            return Tunnel;

        }





        public static Vector3 RandomOffset(int size)
        {
            return new Vector3((float)(Random.NextDouble() - 0.5) * size, (float)(Random.NextDouble() - 0.5) * size, (float)(Random.NextDouble() - 0.5) * size);
        }

        public static Vector3 PerpendicularVector(Vector3 pos_norm)
        {
            Vector3 UPPER = pos_norm.Z > 0.8 ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1);
            Vector3 up = Vector3.Normalize(new Vector3(UPPER - (Vector3.Dot(UPPER, pos_norm) * pos_norm)));
            return up;
        }

        public static Vector3[] PerpendicularVectors(Vector3 pos_norm)
        {

            Vector3 UPPER = pos_norm.Z > 0.8 ? new Vector3(0, 1, 0) : new Vector3(0, 0, 1);
            Vector3 up = Vector3.Normalize(new Vector3(UPPER - (Vector3.Dot(UPPER, pos_norm) * pos_norm)));
            Vector3 right = Vector3.Cross(up, pos_norm);
            return new Vector3[2] { up, right };
        }

        public static Vector3 RotateVector(Vector3 originvector, Vector3 rotatedvector, float angle)
        {
            Quaternion quat = Quaternion.FromAxisAngle(originvector, angle);
            Vector3 res = quat * rotatedvector;
            return res;
        }


    }
}
