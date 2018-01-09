using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Input;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using System.Drawing;

namespace FurAnjel
{
    /// <summary>
    /// The primary coding area for your game.
    /// </summary>
    public class YourGame
    {
        /// <summary>
        /// The backend internal game system.
        /// </summary>
        public GameInternal Backend;

        /// <summary>
        /// Load anything we need here.
        /// </summary>
        public void Load()
        {
            // TODO: Load anything you need!

            // listen to inputs
            Backend.Window.MouseMove += Window_MouseMove;
            Backend.Window.KeyPress += Window_KeyPress;

            //GenerateMountain();
            
            Locations.AddRange(CaveGenerator.GenerateCave());

        }
        
        public void GenerateMountain()
        {
            Task.Run(() =>
            {
                double[,] HeightMap = Mountains.RandomMountainHeightMap(Mountains.AreaSize, Mountains.Height, 5, 1.5);
                for (int X = 0; X < Mountains.AreaSize; X++)
                {
                    for (int Y = 0; Y < Mountains.AreaSize; Y++)
                    {
                        float result = (float)HeightMap[X, Y];
                        Locations.Add(new Vector3(X, Y, (float)result));

                        //for (int i = 0; i < 3; i++)
                        //{
                        //    Locations.Add(new Vector3(X, Y, result - i));
                        //}
                    }
                }
                Console.WriteLine("Mountain Generated");

            });

        }

        private void Window_MouseMove(object sender, MouseMoveEventArgs e)
        {
            if (PauseRender)
            {
                return;
            }
            Point center = new Point(Backend.Window.Width / 2, Backend.Window.Height / 2);


            Yaw += ((float)center.X - e.X) / 1000;
            
            float NewPitch = Pitch + ((float)center.Y - e.Y) / 1000;
            if (NewPitch < 1.55 && NewPitch > -1.55)
            {
                Pitch = NewPitch;
            }
            CameraAngle = ForwardVector(Yaw, Pitch);
            Point mousecenter = Backend.Window.PointToScreen(center);
            Mouse.SetPosition(mousecenter.X, mousecenter.Y);
        }


        private void Window_KeyPress(object sender, KeyPressEventArgs e)
        {
            Char c = e.KeyChar;

            switch (c)
            {

                case 'z':
                    {
                        CameraOffset += CameraAngle * 2;
                        break;
                    }
                case 's':
                    {
                        CameraOffset += CameraAngle * -1 * 2;
                        break;
                    }
                case 'q':
                    {
                        CameraOffset += ForwardVector(Yaw + 1.5F, 0) * 2;
                        break;
                    }
                case 'd':
                    {
                        CameraOffset += ForwardVector(Yaw - 1.5F, 0) * 2;
                        break;
                    }
                case ' ':
                    {
                        CameraOffset += new Vector3(0, 0, 1) * 2;
                        break;
                    }
                case 'a':
                    {
                        CameraOffset += new Vector3(0, 0, 1) * -1 * 2;
                        break;
                    }




                case '5':
                    {
                        PauseRender = !PauseRender;
                        Console.WriteLine("Rendering toggled.");
                        break;
                    }


                case '0':
                    {
                        CameraZoom = 2F;
                        Pitch = 0;
                        Yaw = 0;
                        CameraAngle = new Vector3(-1, -1, -1);
                        CameraOffset = new Vector3(1, 1, 1);
                        Console.WriteLine("Camera reset.");
                        break;
                    }


            }

        }

        public static Vector3 ForwardVector(float yaw, float pitch)
        {
            double cp = Math.Cos(pitch);
            return new Vector3((float)-(cp * Math.Cos(yaw)), (float)-(cp * Math.Sin(yaw)), (float)(Math.Sin(pitch)));
        }

        public float CameraZoom = 2F;
        public Vector3 CameraOffset = new Vector3(1, 1, 1);
        public float Pitch = 0;
        public float Yaw = 0;
        public Vector3 CameraAngle = new Vector3(-1, -1, -1);

        /// <summary>
        /// Update logic here.
        /// </summary>
        /// <param name="delta"></param>
        public void Tick(double delta)
        {
            if (PauseRender)
            {
                return;
            }

        }

        public bool PauseRender = false;

        public List<Vector3> Locations = new List<Vector3>();

        public void Render()
        {
            if (PauseRender)
            {
                return;
            }
            // TODO: Render things to screen.

            // Configure the projection

            //GL.DepthFunc(DepthFunction.Less);
            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(70f * ((float)Math.PI / 180f), (float)Backend.Window.Width / (float)Backend.Window.Height, 0.1f, 5000F);
            Vector3 CameraLocation = CameraOffset * CameraZoom;
            Matrix4 view = Matrix4.LookAt(CameraLocation, CameraLocation + CameraAngle, new Vector3(0, 0, 1));
            Matrix4 matrix = view * projection;
            GL.UniformMatrix4(1, false, ref matrix);

            List<Vector3> TempLocations = new List<Vector3>(Locations);
            foreach (Vector3 location in TempLocations)
            {
                MakeBox(new Vector3(location));
            }
        }

        public void MakeBox(Vector3 Location)
        {
            GL.BindVertexArray(Backend.VBO_Box);
            GL.BindTexture(TextureTarget.Texture2D, Backend.Tex_Red_X);
            Matrix4 model = Matrix4.CreateScale(1, 1, 1) * Matrix4.CreateTranslation(Location);
            GL.UniformMatrix4(2, false, ref model);
            GL.DrawElements(PrimitiveType.Triangles, 36, DrawElementsType.UnsignedInt, 0);
        }
    }
}
