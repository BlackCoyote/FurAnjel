using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace FurAnjel
{
    /// <summary>
    /// Random helpers and utilities for the game program.
    /// </summary>
    public class Helpers
    {
        /// <summary>
        /// Creates a Box VBO.
        /// </summary>
        /// <returns>The box Vertex Array/Buffer Object.</returns>
        public static int CreateBoxVBO()
        {
            // generate a set of coordinates for the box: two triangles.
            Vector3[] Poses = new Vector3[36]
            {
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 0, 0),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),

                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 0),
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),

                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),

                new Vector3(0, 0, 0),
                new Vector3(1,0,0),
                new Vector3(1,0,1),
                new Vector3(0,0,0),
                new Vector3(1,0,1),
                new Vector3(0,0,1),

                new Vector3(0,1,0),
                new Vector3(1,1,0),
                new Vector3(1,1,1),
                new Vector3(0,1,0),
                new Vector3(1,1,1),
                new Vector3(0,1,1),
            };
            // generate a set of texture coordinates for the box.
            Vector2[] TCs = new Vector2[36]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
                
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),

                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),

                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),

                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),

                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 1),
                new Vector2(0, 1),
            };
            // Generate the indices (lazily).
            uint[] Indices = new uint[36]
            {
                0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31, 32, 33, 34, 35
            };
            // Ensure no Vertex Array is bound!
            GL.BindVertexArray(0);
            // Create a holder for the positions.
            int Pos_Buf = GL.GenBuffer();
            // Bind the buffer.
            GL.BindBuffer(BufferTarget.ArrayBuffer, Pos_Buf);
            // Fill it with data: The size in bytes of the array, the array,
            // and the fact that we'll be statically drawing it (not streaming it, reading it, etc).
            GL.BufferData(BufferTarget.ArrayBuffer, Poses.Length * Vector3.SizeInBytes, Poses, BufferUsageHint.StaticDraw);
            // Holder for texture coordinates.
            int Tex_Buf = GL.GenBuffer();
            // Similar to pos buffer.
            GL.BindBuffer(BufferTarget.ArrayBuffer, Tex_Buf);
            GL.BufferData(BufferTarget.ArrayBuffer, TCs.Length * Vector2.SizeInBytes, TCs, BufferUsageHint.StaticDraw);
            // Unbind
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            // Similar, but now elements of indices
            int Ind_Buf = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ind_Buf);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);
            // Unbind
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            // Holder for buffer group.
            int VAO = GL.GenVertexArray();
            // Bind our VAO.
            GL.BindVertexArray(VAO);
            // Bind the pos buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, Pos_Buf);
            // Attach the pos buffer to the VAO: position 0, 3 x floats, no normalization, standard array usage.
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, 0);
            // Similar for texture coordinates
            GL.BindBuffer(BufferTarget.ArrayBuffer, Tex_Buf);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 0, 0);
            // Enable the pieces
            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);
            // Bind the index array as well...
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Ind_Buf);
            // Clean up - unbind VAO for now
            GL.BindVertexArray(0);
            // Return the resultant VAO.
            return VAO;
            // TODO: Delete pos_buf, tex_buf if the Box VBO is ever deleted.
        }

        /// <summary>
        /// Compiles a VertexShader and FragmentShader to a usable shader program.
        /// </summary>
        /// <param name="VS">The input VertexShader code.</param>
        /// <param name="FS">The input FragmentShader code.</param>
        /// <returns>The internal OpenGL program ID.</returns>
        public static int CompileToProgram(string VS, string FS)
        {
            // Create a Vertex shader holder object
            int VertexObject = GL.CreateShader(ShaderType.VertexShader);
            // Add the string source to the shader object
            GL.ShaderSource(VertexObject, VS);
            // Compile the shader
            GL.CompileShader(VertexObject);
            // Check it for errors
            string VS_Info = GL.GetShaderInfoLog(VertexObject);
            GL.GetShader(VertexObject, ShaderParameter.CompileStatus, out int VS_Status);
            if (VS_Status != 1)
            {
                // Crash if there was a big error, with what information we have.
                throw new Exception("Error creating VertexShader. Error status: " + VS_Status + ", info: " + VS_Info);
            }
            // Same for Fragment shaders
            int FragmentObject = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(FragmentObject, FS);
            GL.CompileShader(FragmentObject);
            string FS_Info = GL.GetShaderInfoLog(FragmentObject);
            GL.GetShader(FragmentObject, ShaderParameter.CompileStatus, out int FS_Status);
            if (FS_Status != 1)
            {
                // More crashing
                throw new Exception("Error creating FragmentShader. Error status: " + FS_Status + ", info: " + FS_Info);
            }
            // Create a shader program object
            int Program = GL.CreateProgram();
            // Attach the shader components
            GL.AttachShader(Program, FragmentObject);
            GL.AttachShader(Program, VertexObject);
            // Link it together
            GL.LinkProgram(Program);
            // Check for problems
            string str = GL.GetProgramInfoLog(Program);
            if (str.Length != 0)
            {
                // non-fatal usually
                Console.WriteLine("Linked shader with message: '" + str + "'!");
            }
            // Remove the components (sourcing data), leaving behind the linked result program.
            GL.DeleteShader(FragmentObject);
            GL.DeleteShader(VertexObject);
            // Check for errors
            CheckError("Shader - Compile");
            // Return the resultant program.
            return Program;
        }

        /// <summary>
        /// Converts a texture (input by file name) to a valid GL texture.
        /// </summary>
        /// <param name="file_name">The file name.</param>
        /// <returns>The GL object.</returns>
        public static int LoadTexture(string file_name)
        {
            // Load a bitmap from file, and mark it as used only this block, so it Disposes at the end.
            using (Bitmap bmp = new Bitmap(file_name))
            {
                // Generate a texture.
                int tex = GL.GenTexture();
                // Bind the texture as a 2D texture.
                GL.BindTexture(TextureTarget.Texture2D, tex);
                // Lock all the bits in the bitmap for external usage as ARGB. (Which will then be reversed to BGRA)
                BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
                    ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
                // Upload the image to the texture, in 2D format, no mipmapping, RGBA texture format,
                // size of the bitmap, no border, BGRA read format, 1 standard byte per pixel, from the bmp's locked data.
                GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp.Width, bmp.Height,
                    0, OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, PixelType.UnsignedByte, bmpdata.Scan0);
                // Release the bits now that OpenGL controls the data.
                bmp.UnlockBits(bmpdata);
                // Set the parameter "min(ification) filter" to "linear",
                // to indicate that shrinking the texture will use Linear Interpolation ("lerping") to improve quality.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                // Set the parameter "mag(nification) filter" to "linear",
                // to indicate that enlarging the texture will use Linear Interpolation ("lerping") to improve quality.
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                // Set both vertical and horizontal texture reads to repeat if out-of-bounds.
                // (EG if you have '0' and '2' as texture coordinates, you will see two copies of the texture spread between those points).
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
                GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);
                // Return the resultant texture.
                return tex;
            }
        }

        /// <summary>
        /// Checks for errors within the graphics engine.
        /// </summary>
        /// <param name="time">Indicates when or where there might be an error.</param>
        public static void CheckError(string time)
        {
            // Read the GL error code.
            ErrorCode ec = GL.GetError();
            // So long as the error code isn't "none":
            while (ec != ErrorCode.NoError)
            {
                // Output some information about it!
                Console.WriteLine("Error: " + ec + ", at: " + time + "::\n " + Environment.StackTrace);
                // Reset the error code to whatever's next in line.
                ec = GL.GetError();
            }
        }
    }
}
