using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography.X509Certificates;
using System.Windows.Forms.VisualStyles;
using ZedGraph;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace CG_OpenCV
{
    class ImageClass
    {

        public static int verificarPixel(int pixel)
        {
            if (pixel > 255)
            {
                pixel = 255;
            }
            if (pixel < 0)
            {
                pixel = 0;
            }
            return pixel;
        }

        /// <summary>
        /// Image Negative using EmguCV library
        /// Slower method
        /// </summary>
        /// <param name="img">Image</param>
        public static void Negative(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                int nChan = m.nChannels; // number of channels - 3
                int step = m.widthStep;

                int y, x;
                int width = img.Width;
                int height = img.Height;

                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        (dataPtr + nChan * x + step * y)[0] = (byte)(255 - (dataPtr + nChan * x + step * y)[0]);
                        (dataPtr + nChan * x + step * y)[1] = (byte)(255 - (dataPtr + nChan * x + step * y)[1]);
                        (dataPtr + nChan * x + step * y)[2] = (byte)(255 - (dataPtr + nChan * x + step * y)[2]);
                    }

                }
            }
        }

        /// <summary>
        /// Convert to gray
        /// Direct access to memory - faster method
        /// </summary>
        /// <param name="img">image</param>
        public static void ConvertToGray(Image<Bgr, byte> img)
        {
            unsafe
            {
                // direct access to the image memory(sequencial)
                // direcion top left -> bottom right

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                byte blue, green, red, gray;

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels; // number of channels - 3
                int padding = m.widthStep - m.nChannels * m.width; // alinhament bytes (padding)
                int x, y;

                if (nChan == 3) // image in RGB
                {
                    for (y = 0; y < height; y++)
                    {
                        for (x = 0; x < width; x++)
                        {
                            //retrive 3 colour components
                            blue = dataPtr[0];
                            green = dataPtr[1];
                            red = dataPtr[2];

                            // convert to gray
                            gray = (byte)Math.Round(((int)blue + green + red) / 3.0);

                            // store in the image
                            dataPtr[0] = gray;
                            dataPtr[1] = gray;
                            dataPtr[2] = gray;

                            // advance the pointer to the next pixel
                            dataPtr += nChan;
                        }

                        //at the end of the line advance the pointer by the aligment bytes (padding)
                        dataPtr += padding;
                    }
                }
            }
        }

        public static void BrightContrast(Image<Bgr, byte> img, int bright, double contrast)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                int nChan = m.nChannels; // number of channels - 3
                int step = m.widthStep;

                int y, x;
                int width = img.Width;
                int height = img.Height;


                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {
                        int valor = (int)Math.Round((dataPtr + nChan * x + step * y)[0] * contrast + bright);
                        if (valor > 255)
                        {
                            valor = 255;
                        }
                        else if (valor < 0)
                        {
                            valor = 0;
                        }

                        (dataPtr + nChan * x + step * y)[0] = (byte)valor;

                        valor = (int)Math.Round((dataPtr + nChan * x + step * y)[1] * contrast + bright);
                        if (valor > 255)
                        {
                            valor = 255;
                        }
                        else if (valor < 0)
                        {
                            valor = 0;
                        }

                        (dataPtr + nChan * x + step * y)[1] = (byte)valor;

                        valor = (int)Math.Round((dataPtr + nChan * x + step * y)[2] * contrast + bright);
                        if (valor > 255)
                        {
                            valor = 255;
                        }
                        else if (valor < 0)
                        {
                            valor = 0;
                        }

                        (dataPtr + nChan * x + step * y)[2] = (byte)valor;

                    }

                }
            }

        }

        public static void RedChannel(Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer(); // Pointer to the image
                int nChan = m.nChannels; // number of channels - 3
                int step = m.widthStep;

                int y, x;
                int width = img.Width;
                int height = img.Height;


                for (y = 0; y < height; y++)
                {
                    for (x = 0; x < width; x++)
                    {

                        (dataPtr + nChan * x + step * y)[0] = (dataPtr + nChan * x + step * y)[2];
                        (dataPtr + nChan * x + step * y)[1] = (dataPtr + nChan * x + step * y)[2];

                    }

                }
            }
        }

        public static void Translation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int dx, int dy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                MIplImage mc = imgCopy.MIplImage;
                byte* dataC = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;


                int nx, ny;
                for (int i = 0; i < height; i++)
                {

                    for (int j = 0; j < width; j++)
                    {
                        nx = j - dx;
                        ny = i - dy;

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {

                            (dataPtr + i * m.widthStep + j * nChan)[0] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[1] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[2] = 0;
                        }
                        else
                        {

                            (dataPtr + i * m.widthStep + j * nChan)[0] = (dataC + ny * m.widthStep + nx * nChan)[0];
                            (dataPtr + i * m.widthStep + j * nChan)[1] = (dataC + ny * m.widthStep + nx * nChan)[1];
                            (dataPtr + i * m.widthStep + j * nChan)[2] = (dataC + ny * m.widthStep + nx * nChan)[2];
                        }

                    }
                }
            }
        }


        public static void Rotation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float angle)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();
                MIplImage mc = imgCopy.MIplImage;
                byte* dataC = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                for (int i = 0; i < height; i++)
                {

                    for (int j = 0; j < width; j++)
                    {

                        int newX = (int)Math.Round((j - (width / 2.0)) * Math.Cos(angle) - ((height / 2.0) - i) * Math.Sin(angle) + (width / 2.0));
                        int newY = (int)Math.Round((height / 2.0) - (j - (width / 2.0)) * Math.Sin(angle) - ((height / 2.0) - i) * Math.Cos(angle));

                        if (newX < 0 || newX >= width || newY < 0 || newY >= height)
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[1] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = (dataC + newY * m.widthStep + newX * nChan)[0];
                            (dataPtr + i * m.widthStep + j * nChan)[1] = (dataC + newY * m.widthStep + newX * nChan)[1];
                            (dataPtr + i * m.widthStep + j * nChan)[2] = (dataC + newY * m.widthStep + newX * nChan)[2];
                        }

                    }
                }
            }
        }

        public static void Scale(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataC = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                int nx, ny;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        nx = (int)(j / scaleFactor);
                        ny = (int)(i / scaleFactor);

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[1] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = (dataC + ny * m.widthStep + nx * nChan)[0];
                            (dataPtr + i * m.widthStep + j * nChan)[1] = (dataC + ny * m.widthStep + nx * nChan)[1];
                            (dataPtr + i * m.widthStep + j * nChan)[2] = (dataC + ny * m.widthStep + nx * nChan)[2];
                        }

                    }
                }
            }
        }

        public static void Scale_point_xy(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float scaleFactor, int centerX, int centerY)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataC = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                int nx, ny;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        nx = (int)Math.Round(((j - (width / 2)) / scaleFactor) + centerX);
                        ny = (int)Math.Round(((i - (height / 2)) / scaleFactor) + centerY);

                        if (nx < 0 || nx >= width || ny < 0 || ny >= height)
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[1] = 0;
                            (dataPtr + i * m.widthStep + j * nChan)[2] = 0;
                        }
                        else
                        {
                            (dataPtr + i * m.widthStep + j * nChan)[0] = (dataC + ny * m.widthStep + nx * nChan)[0];
                            (dataPtr + i * m.widthStep + j * nChan)[1] = (dataC + ny * m.widthStep + nx * nChan)[1];
                            (dataPtr + i * m.widthStep + j * nChan)[2] = (dataC + ny * m.widthStep + nx * nChan)[2];
                        }
                    }
                }
            }
        }
        public static void Mean(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                //canto superior esquerdo
                (dataPtr + 0 * width + 0 * nChan)[0] = (byte)Math.Round((((dataPtrCopy + 0 * width + 0 * nChan)[0]) * 4 + ((dataPtrCopy + 1 * width + 0 * nChan)[0]) * 2 + ((dataPtrCopy + 0 * width + 1 * nChan)[0]) * 2 + (dataPtrCopy + 1 * width + 1 * nChan)[0]) / 9.0);
                (dataPtr + 0 * width + 0 * nChan)[1] = (byte)Math.Round((((dataPtrCopy + 0 * width + 0 * nChan)[1]) * 4 + ((dataPtrCopy + 1 * width + 0 * nChan)[1]) * 2 + ((dataPtrCopy + 0 * width + 1 * nChan)[1]) * 2 + (dataPtrCopy + 1 * width + 1 * nChan)[1]) / 9.0);
                (dataPtr + 0 * width + 0 * nChan)[2] = (byte)Math.Round((((dataPtrCopy + 0 * width + 0 * nChan)[2]) * 4 + ((dataPtrCopy + 1 * width + 0 * nChan)[2]) * 2 + ((dataPtrCopy + 0 * width + 1 * nChan)[2]) * 2 + (dataPtrCopy + 1 * width + 1 * nChan)[2]) / 9.0);

                //canto superior direito
                (dataPtr + 0 * width + (width - 1) * nChan)[0] = (byte)Math.Round((((dataPtrCopy + 0 * width + (width - 1) * nChan)[0]) * 4 + ((dataPtrCopy + 1 * width + (width - 1) * nChan)[0]) * 2 + ((dataPtrCopy + 0 * width + (width - 2) * nChan)[0]) * 2 + (dataPtrCopy + 1 * width + (width - 2) * nChan)[0]) / 9.0);
                (dataPtr + 0 * width + (width - 1) * nChan)[1] = (byte)Math.Round((((dataPtrCopy + 0 * width + (width - 1) * nChan)[1]) * 4 + ((dataPtrCopy + 1 * width + (width - 1) * nChan)[1]) * 2 + ((dataPtrCopy + 0 * width + (width - 2) * nChan)[1]) * 2 + (dataPtrCopy + 1 * width + (width - 2) * nChan)[1]) / 9.0);
                (dataPtr + 0 * width + (width - 1) * nChan)[2] = (byte)Math.Round((((dataPtrCopy + 0 * width + (width - 1) * nChan)[2]) * 4 + ((dataPtrCopy + 1 * width + (width - 1) * nChan)[2]) * 2 + ((dataPtrCopy + 0 * width + (width - 2) * nChan)[2]) * 2 + (dataPtrCopy + 1 * width + (width - 2) * nChan)[2]) / 9.0);

                //canto inferior esquerdo
                (dataPtr + (height - 1) * width + 0 * nChan)[0] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + 0 * nChan)[0]) * 4 + ((dataPtrCopy + (height - 2) * width + 0 * nChan)[0]) * 2 + ((dataPtrCopy + (height - 1) * width + 1 * nChan)[0]) * 2 + (dataPtrCopy + (height - 2) * width + 1 * nChan)[0]) / 9.0);
                (dataPtr + (height - 1) * width + 0 * nChan)[1] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + 0 * nChan)[1]) * 4 + ((dataPtrCopy + (height - 2) * width + 0 * nChan)[1]) * 2 + ((dataPtrCopy + (height - 1) * width + 1 * nChan)[1]) * 2 + (dataPtrCopy + (height - 2) * width + 1 * nChan)[1]) / 9.0);
                (dataPtr + (height - 1) * width + 0 * nChan)[2] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + 0 * nChan)[2]) * 4 + ((dataPtrCopy + (height - 2) * width + 0 * nChan)[2]) * 2 + ((dataPtrCopy + (height - 1) * width + 1 * nChan)[2]) * 2 + (dataPtrCopy + (height - 2) * width + 1 * nChan)[2]) / 9.0);

                //canto inferior direito
                (dataPtr + (height - 1) * width + (width - 1) * nChan)[0] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + (width - 1) * nChan)[0]) * 4 + ((dataPtrCopy + (height - 2) * width + (width - 1) * nChan)[0]) * 2 + ((dataPtrCopy + (height - 1) * width + (width - 2) * nChan)[0]) * 2 + (dataPtrCopy + (height - 2) * width + (width - 2) * nChan)[0]) / 9.0);
                (dataPtr + (height - 1) * width + (width - 1) * nChan)[1] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + (width - 1) * nChan)[1]) * 4 + ((dataPtrCopy + (height - 2) * width + (width - 1) * nChan)[1]) * 2 + ((dataPtrCopy + (height - 1) * width + (width - 2) * nChan)[1]) * 2 + (dataPtrCopy + (height - 2) * width + (width - 2) * nChan)[1]) / 9.0);
                (dataPtr + (height - 1) * width + (width - 1) * nChan)[2] = (byte)Math.Round((((dataPtrCopy + (height - 1) * width + (width - 1) * nChan)[2]) * 4 + ((dataPtrCopy + (height - 2) * width + (width - 1) * nChan)[2]) * 2 + ((dataPtrCopy + (height - 1) * width + (width - 2) * nChan)[2]) * 2 + (dataPtrCopy + (height - 2) * width + (width - 2) * nChan)[2]) / 9.0);

                //primeira linha vertical e depois a segunda  linha vertical
                for (int i = 1; i < height - 1; i++)
                {
                    (dataPtr + i * m.widthStep + 0 * nChan)[0] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + 0 * nChan)[0]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + 0 * nChan)[0]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + 0 * nChan)[0]) * 2 + (dataPtrCopy + i * m.widthStep + 1 * nChan)[0] + (dataPtrCopy + (i - 1) * m.widthStep + 1 * nChan)[0] + (dataPtrCopy + (i + 1) * m.widthStep + 1 * nChan)[0]) / 9.0);
                    (dataPtr + i * m.widthStep + 0 * nChan)[1] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + 0 * nChan)[1]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + 0 * nChan)[1]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + 0 * nChan)[1]) * 2 + (dataPtrCopy + i * m.widthStep + 1 * nChan)[1] + (dataPtrCopy + (i - 1) * m.widthStep + 1 * nChan)[1] + (dataPtrCopy + (i + 1) * m.widthStep + 1 * nChan)[1]) / 9.0);
                    (dataPtr + i * m.widthStep + 0 * nChan)[2] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + 0 * nChan)[2]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + 0 * nChan)[2]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + 0 * nChan)[2]) * 2 + (dataPtrCopy + i * m.widthStep + 1 * nChan)[2] + (dataPtrCopy + (i - 1) * m.widthStep + 1 * nChan)[2] + (dataPtrCopy + (i + 1) * m.widthStep + 1 * nChan)[2]) / 9.0);

                    (dataPtr + i * m.widthStep + (width - 1) * nChan)[0] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + (width - 1) * nChan)[0]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + (width - 1) * nChan)[0]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + (width - 1) * nChan)[0]) * 2 + (dataPtrCopy + i * m.widthStep + (width - 2) * nChan)[0] + (dataPtrCopy + (i - 1) * m.widthStep + (width - 2) * nChan)[0] + (dataPtrCopy + (i + 1) * m.widthStep + (width - 2) * nChan)[0]) / 9.0);
                    (dataPtr + i * m.widthStep + (width - 1) * nChan)[1] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + (width - 1) * nChan)[1]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + (width - 1) * nChan)[1]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + (width - 1) * nChan)[1]) * 2 + (dataPtrCopy + i * m.widthStep + (width - 2) * nChan)[1] + (dataPtrCopy + (i - 1) * m.widthStep + (width - 2) * nChan)[1] + (dataPtrCopy + (i + 1) * m.widthStep + (width - 2) * nChan)[1]) / 9.0);
                    (dataPtr + i * m.widthStep + (width - 1) * nChan)[2] = (byte)Math.Round((((dataPtrCopy + i * m.widthStep + (width - 1) * nChan)[2]) * 2 + ((dataPtrCopy + (i - 1) * m.widthStep + (width - 1) * nChan)[2]) * 2 + ((dataPtrCopy + (i + 1) * m.widthStep + (width - 1) * nChan)[2]) * 2 + (dataPtrCopy + i * m.widthStep + (width - 2) * nChan)[2] + (dataPtrCopy + (i - 1) * m.widthStep + (width - 2) * nChan)[2] + (dataPtrCopy + (i + 1) * m.widthStep + (width - 2) * nChan)[2]) / 9.0);
                }

                //primeira linha horizontal e depois a segunda linha horizontal
                for (int j = 1; j < width - 1; j++)
                {
                    (dataPtr + 0 * m.widthStep + j * nChan)[0] = (byte)Math.Round((((dataPtrCopy + 0 * m.widthStep + j * nChan)[0]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j - 1) * nChan)[0]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j + 1) * nChan)[0]) * 2 + (dataPtrCopy + 1 * m.widthStep + j * nChan)[0] + (dataPtrCopy + 1 * m.widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + 1 * m.widthStep + (j + 1) * nChan)[0]) / 9.0);
                    (dataPtr + 0 * m.widthStep + j * nChan)[1] = (byte)Math.Round((((dataPtrCopy + 0 * m.widthStep + j * nChan)[1]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j - 1) * nChan)[1]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j + 1) * nChan)[1]) * 2 + (dataPtrCopy + 1 * m.widthStep + j * nChan)[1] + (dataPtrCopy + 1 * m.widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + 1 * m.widthStep + (j + 1) * nChan)[1]) / 9.0);
                    (dataPtr + 0 * m.widthStep + j * nChan)[2] = (byte)Math.Round((((dataPtrCopy + 0 * m.widthStep + j * nChan)[2]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j - 1) * nChan)[2]) * 2 + ((dataPtrCopy + 0 * m.widthStep + (j + 1) * nChan)[2]) * 2 + (dataPtrCopy + 1 * m.widthStep + j * nChan)[2] + (dataPtrCopy + 1 * m.widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + 1 * m.widthStep + (j + 1) * nChan)[2]) / 9.0);

                    (dataPtr + (height - 1) * m.widthStep + j * nChan)[0] = (byte)Math.Round((((dataPtrCopy + (height - 1) * m.widthStep + j * nChan)[0]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j - 1) * nChan)[0]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j + 1) * nChan)[0]) * 2 + (dataPtrCopy + (height - 2) * m.widthStep + j * nChan)[0] + (dataPtrCopy + (height - 2) * m.widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (height - 2) * m.widthStep + (j + 1) * nChan)[0]) / 9.0);
                    (dataPtr + (height - 1) * m.widthStep + j * nChan)[1] = (byte)Math.Round((((dataPtrCopy + (height - 1) * m.widthStep + j * nChan)[1]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j - 1) * nChan)[1]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j + 1) * nChan)[1]) * 2 + (dataPtrCopy + (height - 2) * m.widthStep + j * nChan)[1] + (dataPtrCopy + (height - 2) * m.widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (height - 2) * m.widthStep + (j + 1) * nChan)[1]) / 9.0);
                    (dataPtr + (height - 1) * m.widthStep + j * nChan)[2] = (byte)Math.Round((((dataPtrCopy + (height - 1) * m.widthStep + j * nChan)[2]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j - 1) * nChan)[2]) * 2 + ((dataPtrCopy + (height - 1) * m.widthStep + (j + 1) * nChan)[2]) * 2 + (dataPtrCopy + (height - 2) * m.widthStep + j * nChan)[2] + (dataPtrCopy + (height - 2) * m.widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (height - 2) * m.widthStep + (j + 1) * nChan)[2]) / 9.0);
                }

                //meio
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        (dataPtr + i * m.widthStep + j * nChan)[0] = (byte)Math.Round(((dataPtrCopy + i * m.widthStep + j * nChan)[0] + (dataPtrCopy + i * m.widthStep + (j + 1) * nChan)[0] + (dataPtrCopy + i * m.widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (i - 1) * m.widthStep + j * nChan)[0] + (dataPtrCopy + (i + 1) * m.widthStep + j * nChan)[0] + (dataPtrCopy + (i + 1) * m.widthStep + (j + 1) * nChan)[0] + (dataPtrCopy + (i + 1) * m.widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (i - 1) * m.widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (i - 1) * m.widthStep + (j + 1) * nChan)[0]) / 9.0);
                        (dataPtr + i * m.widthStep + j * nChan)[1] = (byte)Math.Round(((dataPtrCopy + i * m.widthStep + j * nChan)[1] + (dataPtrCopy + i * m.widthStep + (j + 1) * nChan)[1] + (dataPtrCopy + i * m.widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (i - 1) * m.widthStep + j * nChan)[1] + (dataPtrCopy + (i + 1) * m.widthStep + j * nChan)[1] + (dataPtrCopy + (i + 1) * m.widthStep + (j + 1) * nChan)[1] + (dataPtrCopy + (i + 1) * m.widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (i - 1) * m.widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (i - 1) * m.widthStep + (j + 1) * nChan)[1]) / 9.0);
                        (dataPtr + i * m.widthStep + j * nChan)[2] = (byte)Math.Round(((dataPtrCopy + i * m.widthStep + j * nChan)[2] + (dataPtrCopy + i * m.widthStep + (j + 1) * nChan)[2] + (dataPtrCopy + i * m.widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (i - 1) * m.widthStep + j * nChan)[2] + (dataPtrCopy + (i + 1) * m.widthStep + j * nChan)[2] + (dataPtrCopy + (i + 1) * m.widthStep + (j + 1) * nChan)[2] + (dataPtrCopy + (i + 1) * m.widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (i - 1) * m.widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (i - 1) * m.widthStep + (j + 1) * nChan)[2]) / 9.0);


                    }
                }
            }
        }

        public static void NonUniform(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, float[,] matrix, float matrixWeight)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int blueInt, greenInt, redInt;
                byte blue, green, red;



                //canto superior esquerdo
                blueInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + 1 * nChan)[0] * matrix[0, 2])) / matrixWeight);
                greenInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[1]) * matrix[0, 0] + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + 1 * nChan)[1] * matrix[0, 2])) / matrixWeight);
                redInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[2]) * matrix[0, 0] + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + 0 * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + 1 * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + 1 * nChan)[2] * matrix[0, 2])) / matrixWeight);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * width + 0 * nChan)[0] = blue;
                (dataPtr + 0 * width + 0 * nChan)[1] = green;
                (dataPtr + 0 * width + 0 * nChan)[2] = red;


                //canto superior direito 
                blueInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[0] * matrix[2, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[0] * matrix[2, 0])) / matrixWeight);
                greenInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[1] * matrix[2, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[1] * matrix[2, 0])) / matrixWeight);
                redInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[2] * matrix[2, 2]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[2] * matrix[2, 0])) / matrixWeight);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * widthStep + (width - 1) * nChan)[0] = blue;
                (dataPtr + 0 * widthStep + (width - 1) * nChan)[1] = green;
                (dataPtr + 0 * widthStep + (width - 1) * nChan)[2] = red;

                //canto  inferior esqerdo
                blueInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[0] * matrix[2, 2]) + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[0] * matrix[0, 2]) / matrixWeight);
                greenInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[1] * matrix[2, 2]) + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[1] * matrix[0, 2]) / matrixWeight);
                redInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[2] * matrix[2, 2]) + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[2] * matrix[0, 2]) / matrixWeight);


                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + (height - 1) * widthStep + 0 * nChan)[0] = blue;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[1] = green;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[2] = red;

                //canto inferior direito
                blueInt = (byte)Math.Round((((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] * matrix[2, 0]) + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0] * matrix[0, 0]) / matrixWeight);
                greenInt = (byte)Math.Round((((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] * matrix[2, 0]) + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1] * matrix[0, 0]) / matrixWeight);
                redInt = (byte)Math.Round((((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] * matrix[2, 0]) + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2] * matrix[0, 0]) / matrixWeight);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);


                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = blue;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = green;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = red;

                //primeira linha vertical e depois a segunda  linha vertical
                for (int i = 1; i < height - 1; i++)
                {
                    blueInt = (int)Math.Round((((dataPtrCopy + i * widthStep + 0 * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + 0 * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + i * widthStep + 1 * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[0] * matrix[2, 2])) / matrixWeight);
                    greenInt = (int)Math.Round((((dataPtrCopy + i * widthStep + 0 * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + 0 * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + i * widthStep + 1 * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[1] * matrix[2, 2])) / matrixWeight);
                    redInt = (int)Math.Round((((dataPtrCopy + i * widthStep + 0 * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + 0 * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + i * widthStep + 1 * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[2] * matrix[2, 2])) / matrixWeight);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + 0 * nChan)[0] = blue;
                    (dataPtr + i * widthStep + 0 * nChan)[1] = green;
                    (dataPtr + i * widthStep + 0 * nChan)[2] = red;


                    blueInt = (int)Math.Round((((dataPtrCopy + i * widthStep + (width - 1) * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (width - 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] * matrix[2, 2]) + ((dataPtrCopy + i * widthStep + (width - 2) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] * matrix[2, 0])) / matrixWeight);
                    greenInt = (int)Math.Round((((dataPtrCopy + i * widthStep + (width - 1) * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (width - 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] * matrix[2, 2]) + ((dataPtrCopy + i * widthStep + (width - 2) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] * matrix[2, 0])) / matrixWeight);
                    redInt = (int)Math.Round((((dataPtrCopy + i * widthStep + (width - 1) * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (width - 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] * matrix[2, 2]) + ((dataPtrCopy + i * widthStep + (width - 2) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] * matrix[2, 0])) / matrixWeight);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + (width - 1) * nChan)[0] = blue;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[1] = green;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[2] = red;

                }

                //primeira linha horizontal e depois a segunda linha horizontal
                for (int j = 1; j < width - 1; j++)
                {
                    blueInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + j * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + j * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[0] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + j * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[0]) * matrix[2, 2]) / matrixWeight);
                    greenInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + j * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + j * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[1] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + j * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[1]) * matrix[2, 2]) / matrixWeight);
                    redInt = (int)Math.Round((((dataPtrCopy + 0 * widthStep + j * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + 0 * widthStep + j * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[2] * matrix[0, 2]) + ((dataPtrCopy + 1 * widthStep + j * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[2]) * matrix[2, 2]) / matrixWeight);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + 0 * widthStep + j * nChan)[0] = blue;
                    (dataPtr + 0 * widthStep + j * nChan)[1] = green;
                    (dataPtr + 0 * widthStep + j * nChan)[2] = red;


                    blueInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + j * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + j * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + j * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] * matrix[0, 2])) / matrixWeight);
                    greenInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + j * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + j * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + j * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] * matrix[0, 2])) / matrixWeight);
                    redInt = (int)Math.Round((((dataPtrCopy + (height - 1) * widthStep + j * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + (height - 1) * widthStep + j * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] * matrix[2, 2]) + ((dataPtrCopy + (height - 2) * widthStep + j * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] * matrix[0, 2])) / matrixWeight);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + (height - 1) * widthStep + j * nChan)[0] = blue;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[1] = green;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[2] = red;
                }

                //meio
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {
                        blueInt = (int)Math.Round((((dataPtrCopy + i * widthStep + j * nChan)[0] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (j + 1) * nChan)[0] * matrix[1, 2]) + ((dataPtrCopy + i * widthStep + (j - 1) * nChan)[0] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + j * nChan)[0] * matrix[0, 1]) + ((dataPtrCopy + (i + 1) * widthStep + j * nChan)[0] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] * matrix[2, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] * matrix[2, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] * matrix[0, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] * matrix[0, 2])) / matrixWeight);
                        greenInt = (int)Math.Round((((dataPtrCopy + i * widthStep + j * nChan)[1] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (j + 1) * nChan)[1] * matrix[1, 2]) + ((dataPtrCopy + i * widthStep + (j - 1) * nChan)[1] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + j * nChan)[1] * matrix[0, 1]) + ((dataPtrCopy + (i + 1) * widthStep + j * nChan)[1] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] * matrix[2, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] * matrix[2, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] * matrix[0, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] * matrix[0, 2])) / matrixWeight);
                        redInt = (int)Math.Round((((dataPtrCopy + i * widthStep + j * nChan)[2] * matrix[1, 1]) + ((dataPtrCopy + i * widthStep + (j + 1) * nChan)[2] * matrix[1, 2]) + ((dataPtrCopy + i * widthStep + (j - 1) * nChan)[2] * matrix[1, 0]) + ((dataPtrCopy + (i - 1) * widthStep + j * nChan)[2] * matrix[0, 1]) + ((dataPtrCopy + (i + 1) * widthStep + j * nChan)[2] * matrix[2, 1]) + ((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] * matrix[2, 2]) + ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] * matrix[2, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] * matrix[0, 0]) + ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] * matrix[0, 2])) / matrixWeight);

                        blue = (byte)verificarPixel(blueInt);
                        green = (byte)verificarPixel(greenInt);
                        red = (byte)verificarPixel(redInt);

                        (dataPtr + i * widthStep + j * nChan)[0] = blue;
                        (dataPtr + i * widthStep + j * nChan)[1] = green;
                        (dataPtr + i * widthStep + j * nChan)[2] = red;
                    }
                }
            }
        }

        public static void Sobel(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int blueInt, greenInt, redInt;
                byte blue, green, red;
                int sxB, sxG, sxR, syB, syG, syR;

                //canto superior esquerdo[0,0]

                sxB = ((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + 0 * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + 1 * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[0]);
                sxG = ((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + 0 * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + 1 * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[1]);
                sxR = ((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + 0 * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + 1 * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[2]);
                syB = ((dataPtrCopy + 1 * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + 0 * widthStep + 1 * nChan)[0]);
                syG = ((dataPtrCopy + 1 * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + 0 * widthStep + 1 * nChan)[1]);
                syR = ((dataPtrCopy + 1 * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + 1 * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + 0 * widthStep + 1 * nChan)[2]);

                blueInt = Math.Abs(sxB) + Math.Abs(syB);
                greenInt = Math.Abs(sxG) + Math.Abs(syG);
                redInt = Math.Abs(sxR) + Math.Abs(syR);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * widthStep + 0 * nChan)[0] = blue;
                (dataPtr + 0 * widthStep + 0 * nChan)[1] = green;
                (dataPtr + 0 * widthStep + 0 * nChan)[2] = red;

                //canto superior direito[0,w-1]

                sxB = ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[0]);
                sxG = ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[1]);
                sxR = ((dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[2]);
                syB = ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[0]);
                syG = ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[1]);
                syR = ((dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + (width - 2) * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + 0 * widthStep + (width - 2) * nChan)[2]);

                blueInt = Math.Abs(sxB) + Math.Abs(syB);
                greenInt = Math.Abs(sxG) + Math.Abs(syG);
                redInt = Math.Abs(sxR) + Math.Abs(syR);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * widthStep + (width - 1) * nChan)[0] = blue;
                (dataPtr + 0 * widthStep + (width - 1) * nChan)[1] = green;
                (dataPtr + 0 * widthStep + (width - 1) * nChan)[2] = red;

                //canto inferior esquerdo[h-1,0]

                sxB = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[0]) - ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[0]);
                sxG = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[1]) - ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[1]);
                sxR = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[2]) - ((dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[2]);
                syB = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[0]) - ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[0]);
                syG = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[1]) - ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[1]);
                syR = ((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[2]) - ((dataPtrCopy + (height - 2) * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + 1 * nChan)[2]);

                blueInt = Math.Abs(sxB) + Math.Abs(syB);
                greenInt = Math.Abs(sxG) + Math.Abs(syG);
                redInt = Math.Abs(sxR) + Math.Abs(syR);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + (height - 1) * widthStep + 0 * nChan)[0] = blue;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[1] = green;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[2] = red;

                //canto inferior direito[h-1,w-1]

                sxB = ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0]) - ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0]);
                sxG = ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1]) - ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1]);
                sxR = ((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2]) - ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2]);
                syB = ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0]) - ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0]);
                syG = ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1]) - ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1]);
                syR = ((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2]) - ((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2]);

                blueInt = Math.Abs(sxB) + Math.Abs(syB);
                greenInt = Math.Abs(sxG) + Math.Abs(syG);
                redInt = Math.Abs(sxR) + Math.Abs(syR);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = blue;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = green;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = red;

                //cima e baixo

                for (int j = 1; j < width - 1; j++)
                {

                    sxB = ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[0] * 3 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[0]);
                    sxG = ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[1] * 3 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[1]);
                    sxR = ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[2] * 3 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[2]);
                    syB = ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + 1 * widthStep + j * nChan)[0] * 2 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[0]) - ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + 0 * widthStep + j * nChan)[0] * 2 + (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[0]);
                    syG = ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + 1 * widthStep + j * nChan)[1] * 2 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[1]) - ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + 0 * widthStep + j * nChan)[1] * 2 + (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[1]);
                    syR = ((dataPtrCopy + 1 * widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + 1 * widthStep + j * nChan)[2] * 2 + (dataPtrCopy + 1 * widthStep + (j + 1) * nChan)[2]) - ((dataPtrCopy + 0 * widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + 0 * widthStep + j * nChan)[2] * 2 + (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[2]);

                    blueInt = Math.Abs(sxB) + Math.Abs(syB);
                    greenInt = Math.Abs(sxG) + Math.Abs(syG);
                    redInt = Math.Abs(sxR) + Math.Abs(syR);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + 0 * widthStep + j * nChan)[0] = blue;
                    (dataPtr + 0 * widthStep + j * nChan)[1] = green;
                    (dataPtr + 0 * widthStep + j * nChan)[2] = red;


                    sxB = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) - ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]);
                    sxG = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) - ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]);
                    sxR = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) - ((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] * 3 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]);
                    syB = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (height - 1) * widthStep + j * nChan)[0] * 2 + (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) - ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (height - 2) * widthStep + j * nChan)[0] * 2 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]);
                    syG = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (height - 1) * widthStep + j * nChan)[1] * 2 + (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) - ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (height - 2) * widthStep + j * nChan)[1] * 2 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]);
                    syR = ((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (height - 1) * widthStep + j * nChan)[2] * 2 + (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2]) - ((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (height - 2) * widthStep + j * nChan)[2] * 2 + (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]);

                    blueInt = Math.Abs(sxB) + Math.Abs(syB);
                    greenInt = Math.Abs(sxG) + Math.Abs(syG);
                    redInt = Math.Abs(sxR) + Math.Abs(syR);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + (height - 1) * widthStep + j * nChan)[0] = blue;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[1] = green;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[2] = red;

                }

                //primeira linha horizontal e depois a segunda linha horizontal
                for (int i = 1; i < height - 1; i++)
                {

                    sxB = ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[0] + (dataPtrCopy + i * widthStep + 0 * nChan)[0] * 2 + (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[0] + (dataPtrCopy + i * widthStep + 1 * nChan)[0] * 2 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[0]);
                    sxG = ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[1] + (dataPtrCopy + i * widthStep + 0 * nChan)[1] * 2 + (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[1] + (dataPtrCopy + i * widthStep + 1 * nChan)[1] * 2 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[1]);
                    sxR = ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[2] + (dataPtrCopy + i * widthStep + 0 * nChan)[2] * 2 + (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[2] + (dataPtrCopy + i * widthStep + 1 * nChan)[2] * 2 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[2]);
                    syB = ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[0] * 3 + (dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[0]);
                    syG = ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[1] * 3 + (dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[1]);
                    syR = ((dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + (i + 1) * widthStep + 1 * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + 0 * nChan)[2] * 3 + (dataPtrCopy + (i - 1) * widthStep + 1 * nChan)[2]);

                    blueInt = Math.Abs(sxB) + Math.Abs(syB);
                    greenInt = Math.Abs(sxG) + Math.Abs(syG);
                    redInt = Math.Abs(sxR) + Math.Abs(syR);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + 0 * nChan)[0] = blue;
                    (dataPtr + i * widthStep + 0 * nChan)[1] = green;
                    (dataPtr + i * widthStep + 0 * nChan)[2] = red;


                    sxB = ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] + (dataPtrCopy + i * widthStep + (width - 1) * nChan)[0] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] + (dataPtrCopy + i * widthStep + (width - 2) * nChan)[0] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]);
                    sxG = ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] + (dataPtrCopy + i * widthStep + (width - 1) * nChan)[1] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] + (dataPtrCopy + i * widthStep + (width - 2) * nChan)[1] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]);
                    sxR = ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] + (dataPtrCopy + i * widthStep + (width - 1) * nChan)[2] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] + (dataPtrCopy + i * widthStep + (width - 2) * nChan)[2] * 2 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]);
                    syB = ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] * 3 + (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]);
                    syG = ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] * 3 + (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]);
                    syR = ((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] * 3 + (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]);

                    blueInt = Math.Abs(sxB) + Math.Abs(syB);
                    greenInt = Math.Abs(sxG) + Math.Abs(syG);
                    redInt = Math.Abs(sxR) + Math.Abs(syR);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + (width - 1) * nChan)[0] = blue;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[1] = green;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[2] = red;

                }

                //centro da imagem 
                for (int i = 1; i < height - 1; i++)

                {
                    for (int j = 1; j < width - 1; j++)
                    {

                        sxB = ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] + 2 * (dataPtrCopy + i * widthStep + (j - 1) * nChan)[0] + (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] + 2 * (dataPtrCopy + i * widthStep + (j + 1) * nChan)[0] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]);
                        sxG = ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] + 2 * (dataPtrCopy + i * widthStep + (j - 1) * nChan)[1] + (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] + 2 * (dataPtrCopy + i * widthStep + (j + 1) * nChan)[1] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]);
                        sxR = ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] + 2 * (dataPtrCopy + i * widthStep + (j - 1) * nChan)[2] + (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] + 2 * (dataPtrCopy + i * widthStep + (j + 1) * nChan)[2] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]);
                        syB = ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] + 2 * (dataPtrCopy + (i + 1) * widthStep + j * nChan)[0] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) - ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] + 2 * (dataPtrCopy + (i - 1) * widthStep + j * nChan)[0] + (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]);
                        syG = ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] + 2 * (dataPtrCopy + (i + 1) * widthStep + j * nChan)[1] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) - ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] + 2 * (dataPtrCopy + (i - 1) * widthStep + j * nChan)[1] + (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]);
                        syR = ((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] + 2 * (dataPtrCopy + (i + 1) * widthStep + j * nChan)[2] + (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) - ((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] + 2 * (dataPtrCopy + (i - 1) * widthStep + j * nChan)[2] + (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        blueInt = (int)Math.Abs(sxB) + (int)Math.Abs(syB);
                        greenInt = (int)Math.Abs(sxG) + (int)Math.Abs(syG);
                        redInt = (int)Math.Abs(sxR) + (int)Math.Abs(syR);

                        blue = (byte)verificarPixel(blueInt);
                        green = (byte)verificarPixel(greenInt);
                        red = (byte)verificarPixel(redInt);

                        (dataPtr + i * widthStep + j * nChan)[0] = blue;
                        (dataPtr + i * widthStep + j * nChan)[1] = green;
                        (dataPtr + i * widthStep + j * nChan)[2] = red;

                    }
                }
            }
        }

        public static void Diferentiation(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;

                int blueInt, greenInt, redInt;
                byte blue, green, red;
                int widthStep = m.widthStep;

                //canto superior esquerdo
                blueInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] - (dataPtrCopy + 0 * widthStep + 1 * nChan)[0]) + Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[0] - (dataPtrCopy + 1 * widthStep + 0 * nChan)[0]);
                greenInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] - (dataPtrCopy + 0 * widthStep + 1 * nChan)[1]) + Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[1] - (dataPtrCopy + 1 * widthStep + 0 * nChan)[1]);
                redInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] - (dataPtrCopy + 0 * widthStep + 1 * nChan)[2]) + Math.Abs((dataPtrCopy + 0 * widthStep + 0 * nChan)[2] - (dataPtrCopy + 1 * widthStep + 0 * nChan)[2]);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * width + 0 * nChan)[0] = blue;
                (dataPtr + 0 * width + 0 * nChan)[1] = green;
                (dataPtr + 0 * width + 0 * nChan)[2] = red;

                //canto superior direito
                blueInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[0]);
                greenInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[1]);
                redInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + 1 * widthStep + (width - 1) * nChan)[2]);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + 0 * width + (width - 1) * nChan)[0] = blue;
                (dataPtr + 0 * width + (width - 1) * nChan)[1] = green;
                (dataPtr + 0 * width + (width - 1) * nChan)[2] = red;

                //canto inferior esquerdo
                blueInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[0]);
                greenInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[1]);
                redInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + 0 * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + 1 * nChan)[2]);

                blue = (byte)verificarPixel(blueInt);
                green = (byte)verificarPixel(greenInt);
                red = (byte)verificarPixel(redInt);

                (dataPtr + (height - 1) * widthStep + 0 * nChan)[0] = blue;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[1] = green;
                (dataPtr + (height - 1) * widthStep + 0 * nChan)[2] = red;

                //canto inferior direito

                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = 0;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = 0;
                (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = 0;

                //primeira linha vertical e depois a segunda  linha vertical
                for (int i = 1; i < height - 1; i++)
                {

                    blueInt = (int)Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[0] - (dataPtrCopy + i * widthStep + 1 * nChan)[0]) + Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[0]);
                    greenInt = (int)Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[1] - (dataPtrCopy + i * widthStep + 1 * nChan)[1]) + Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[1]);
                    redInt = (int)Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[2] - (dataPtrCopy + i * widthStep + 1 * nChan)[2]) + Math.Abs((dataPtrCopy + i * widthStep + 0 * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + 0 * nChan)[2]);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + 0 * nChan)[0] = blue;
                    (dataPtr + i * widthStep + 0 * nChan)[1] = green;
                    (dataPtr + i * widthStep + 0 * nChan)[2] = red;


                    blueInt = (int)Math.Abs((dataPtrCopy + i * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]);
                    greenInt = (int)Math.Abs((dataPtrCopy + i * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]);
                    redInt = (int)Math.Abs((dataPtrCopy + i * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2]);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + i * widthStep + (width - 1) * nChan)[0] = blue;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[1] = green;
                    (dataPtr + i * widthStep + (width - 1) * nChan)[2] = red;

                }

                //primeira linha horizontal e depois a segunda linha horizontal
                for (int j = 1; j < width - 1; j++)
                {

                    blueInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[0] - (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[0]) + Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[0] - (dataPtrCopy + 1 * widthStep + j * nChan)[0]);
                    greenInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[1] - (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[1]) + Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[1] - (dataPtrCopy + 1 * widthStep + j * nChan)[1]);
                    redInt = (int)Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[2] - (dataPtrCopy + 0 * widthStep + (j + 1) * nChan)[2]) + Math.Abs((dataPtrCopy + 0 * widthStep + j * nChan)[2] - (dataPtrCopy + 1 * widthStep + j * nChan)[2]);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + 0 * widthStep + j * nChan)[0] = blue;
                    (dataPtr + 0 * widthStep + j * nChan)[1] = green;
                    (dataPtr + 0 * widthStep + j * nChan)[2] = red;


                    blueInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + j * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]);
                    greenInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + j * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]);
                    redInt = (int)Math.Abs((dataPtrCopy + (height - 1) * widthStep + j * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2]);

                    blue = (byte)verificarPixel(blueInt);
                    green = (byte)verificarPixel(greenInt);
                    red = (byte)verificarPixel(redInt);

                    (dataPtr + (height - 1) * widthStep + j * nChan)[0] = blue;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[1] = green;
                    (dataPtr + (height - 1) * widthStep + j * nChan)[2] = red;

                }

                //meio
                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {

                        blueInt = (int)Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[0] - (dataPtrCopy + i * widthStep + (j + 1) * nChan)[0]) + Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + j * nChan)[0]);
                        greenInt = (int)Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[1] - (dataPtrCopy + i * widthStep + (j + 1) * nChan)[1]) + Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + j * nChan)[1]);
                        redInt = (int)Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[2] - (dataPtrCopy + i * widthStep + (j + 1) * nChan)[2]) + Math.Abs((dataPtrCopy + i * widthStep + j * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + j * nChan)[2]);

                        blue = (byte)verificarPixel(blueInt);
                        green = (byte)verificarPixel(greenInt);
                        red = (byte)verificarPixel(redInt);

                        (dataPtr + i * widthStep + j * nChan)[0] = blue;
                        (dataPtr + i * widthStep + j * nChan)[1] = green;
                        (dataPtr + i * widthStep + j * nChan)[2] = red;


                    }
                }
            }
        }

        public static void Median(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                double[] distancias = new double[9];
                int menorValor;

                //ordem de todos os valores
                //0|1|2
                //3|4|5
                //6|7|8

                //canto superior esquerdo

                distancias[0] =
                    (Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[2]);

                distancias[1] = distancias[0];

                distancias[2] =
                    (Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[2]);

                distancias[3] = distancias[0];

                distancias[4] = distancias[0];

                distancias[5] = distancias[2];

                distancias[6] =
                    (Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (1) * nChan)[2]);

                distancias[7] = distancias[6];

                distancias[8] =
                    (Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (0) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (1) * nChan)[2])) * 2 +

                     (Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (0) * nChan)[2])) * 2;

                menorValor = Array.IndexOf(distancias, distancias.Min());

                if (menorValor == 0 || menorValor == 1 || menorValor == 3 || menorValor == 4)
                {

                    (dataPtr + (0) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (0) * nChan)[0];
                    (dataPtr + (0) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (0) * nChan)[1];
                    (dataPtr + (0) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (0) * nChan)[2];

                }
                else if (menorValor == 2 || menorValor == 5)
                {

                    (dataPtr + (0) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (1) * nChan)[0];
                    (dataPtr + (0) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (1) * nChan)[1];
                    (dataPtr + (0) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (1) * nChan)[2];

                }
                else if (menorValor == 6 || menorValor == 7)
                {

                    (dataPtr + (0) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (0) * nChan)[0];
                    (dataPtr + (0) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (0) * nChan)[1];
                    (dataPtr + (0) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (0) * nChan)[2];

                }
                else if (menorValor == 8)
                {

                    (dataPtr + (0) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (1) * nChan)[0];
                    (dataPtr + (0) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (1) * nChan)[1];
                    (dataPtr + (0) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (1) * nChan)[2];

                }

                //canto superior direito

                distancias[0] =
                    (Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2])) * 4 +

                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2])) * 2;

                distancias[1] =
                    (Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2])) * 2;

                distancias[2] = distancias[1];

                distancias[3] = distancias[0];

                distancias[4] = distancias[1];

                distancias[5] = distancias[1];

                distancias[6] =
                    (Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2])) * 2;

                distancias[7] =
                    (Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2])) * 4 +

                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2]);

                distancias[8] = distancias[7];

                menorValor = Array.IndexOf(distancias, distancias.Min());

                if (menorValor == 0 || menorValor == 3)
                {

                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[0];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[1];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (width - 2) * nChan)[2];

                }
                else if (menorValor == 1 || menorValor == 2 || menorValor == 4 || menorValor == 5)
                {

                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[0];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[1];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (width - 1) * nChan)[2];

                }
                else if (menorValor == 6)
                {

                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[0];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[1];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (width - 2) * nChan)[2];

                }
                else if (menorValor == 7 || menorValor == 8)
                {

                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[0];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[1];
                    (dataPtr + (0) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (width - 1) * nChan)[2];

                }

                // canto inferior esquerdo

                distancias[0] =
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2])) * 2;

                distancias[1] = distancias[0];

                distancias[2] =
                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2])) * 4 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2])) * 2;

                distancias[3] =
                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2])) * 2;

                distancias[4] = distancias[3];

                distancias[5] =
                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2])) * 2 +

                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2])) * 4;

                distancias[6] = distancias[3];

                distancias[7] = distancias[3];

                distancias[8] = distancias[5];

                menorValor = Array.IndexOf(distancias, distancias.Min());

                if (menorValor == 0 || menorValor == 1)
                {

                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (0) * nChan)[2];

                }
                else if (menorValor == 2)
                {

                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (1) * nChan)[2];

                }
                else if (menorValor == 3 || menorValor == 4 || menorValor == 6 || menorValor == 7)
                {

                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2];

                }
                else if (menorValor == 5 || menorValor == 8)
                {

                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (0) * nChan)[2];

                }

                //canto inferior direito

                distancias[0] =
                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2])) * 4;

                distancias[1] =
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2])) * 4;

                distancias[2] = distancias[1];

                distancias[3] =
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2])) * 4;

                distancias[4] =
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2]) +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2])) * 2 +

                    (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1]) +
                    Math.Abs((dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2])) * 2;

                distancias[5] = distancias[4];

                distancias[6] = distancias[3];

                distancias[7] = distancias[4];

                distancias[8] = distancias[4];

                menorValor = Array.IndexOf(distancias, distancias.Min());

                if (menorValor == 0)
                {

                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (width - 2) * nChan)[2];

                }
                else if (menorValor == 1 || menorValor == 2)
                {

                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (width - 1) * nChan)[2];

                }
                else if (menorValor == 3 || menorValor == 6)
                {

                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (width - 2) * nChan)[2];

                }
                else if (menorValor == 4 || menorValor == 5 || menorValor == 7 || menorValor == 8)
                {

                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[0];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[1];
                    (dataPtr + (height - 1) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (width - 1) * nChan)[2];

                }

                //linha superior

                for (int j = 1; j < width - 1; j++)
                {

                    distancias[0] =
                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2]);

                    distancias[1] =
                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[2]);

                    distancias[2] =
                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2]);

                    distancias[3] = distancias[0];

                    distancias[4] = distancias[1];

                    distancias[5] = distancias[2];

                    distancias[6] =
                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2]);

                    distancias[7] =
                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2]);

                    distancias[8] =
                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (1) * widthStep + (j) * nChan)[2]);

                    menorValor = Array.IndexOf(distancias, distancias.Min());

                    if (menorValor == 0 || menorValor == 3)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (j - 1) * nChan)[2];

                    }
                    else if (menorValor == 1 || menorValor == 4)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (j) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (j) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (j) * nChan)[2];

                    }
                    else if (menorValor == 2 || menorValor == 5)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (0) * widthStep + (j + 1) * nChan)[2];

                    }
                    else if (menorValor == 6)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (j - 1) * nChan)[2];

                    }
                    else if (menorValor == 7)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (j) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (j) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (j) * nChan)[2];

                    }
                    else if (menorValor == 8)
                    {

                        (dataPtr + (0) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[0];
                        (dataPtr + (0) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[1];
                        (dataPtr + (0) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (1) * widthStep + (j + 1) * nChan)[2];

                    }
                }

                //linha inferior

                for (int j = 1; j < width - 1; j++)
                {

                    distancias[0] =
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2])) * 2;

                    distancias[1] =
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2])) * 2;

                    distancias[2] =
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2])) * 2;

                    distancias[3] =
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2])) * 2;

                    distancias[4] =
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2])) * 2;

                    distancias[5] =
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2])) * 2;

                    distancias[6] = distancias[3];

                    distancias[7] = distancias[4];

                    distancias[8] = distancias[5];

                    menorValor = Array.IndexOf(distancias, distancias.Min());

                    if (menorValor == 0)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (j - 1) * nChan)[2];

                    }
                    else if (menorValor == 1)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (j) * nChan)[2];

                    }
                    else if (menorValor == 2)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 2) * widthStep + (j + 1) * nChan)[2];

                    }
                    else if (menorValor == 3 || menorValor == 6)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (j - 1) * nChan)[2];

                    }
                    else if (menorValor == 4 || menorValor == 7)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (j) * nChan)[2];

                    }
                    else if (menorValor == 5 || menorValor == 8)
                    {

                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[0];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[1];
                        (dataPtr + (height - 1) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (height - 1) * widthStep + (j + 1) * nChan)[2];

                    }
                }

                //coluna esquerda

                for (int i = 1; i < height - 1; i++)
                {

                    distancias[0] =
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2]);

                    distancias[1] = distancias[0];

                    distancias[2] =
                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2]);

                    distancias[3] =
                        (Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2]);

                    distancias[4] = distancias[3];

                    distancias[5] =
                        (Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2]);

                    distancias[6] =
                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2]);

                    distancias[7] = distancias[6];

                    distancias[8] =
                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (0) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (1) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2])) * 2;

                    menorValor = Array.IndexOf(distancias, distancias.Min());

                    if (menorValor == 0 || menorValor == 1)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (0) * nChan)[2];

                    }
                    else if (menorValor == 2)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (1) * nChan)[2];

                    }
                    else if (menorValor == 3 || menorValor == 4)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (0) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (0) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (0) * nChan)[2];

                    }
                    else if (menorValor == 5)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (1) * nChan)[2];

                    }
                    else if (menorValor == 6 || menorValor == 7)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (0) * nChan)[2];

                    }
                    else if (menorValor == 8)
                    {

                        (dataPtr + (i) * widthStep + (0) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (0) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (0) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (1) * nChan)[2];

                    }
                }

                //coluna direita

                for (int i = 1; i < height - 1; i++)
                {

                    distancias[0] =
                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2])) * 2;

                    distancias[1] =
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2])) * 2;

                    distancias[2] = distancias[1];

                    distancias[3] =
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2])) * 2;

                    distancias[4] =
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2]) +

                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2])) * 2;

                    distancias[5] = distancias[4];

                    distancias[6] =
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2])) * 2;

                    distancias[7] =
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2]) +

                        (Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2])) * 2 +

                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1]) +
                        Math.Abs((dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2]);

                    distancias[8] = distancias[7];

                    menorValor = Array.IndexOf(distancias, distancias.Min());

                    if (menorValor == 0)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (width - 2) * nChan)[2];

                    }
                    else if (menorValor == 1 || menorValor == 2)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (width - 1) * nChan)[2];

                    }
                    else if (menorValor == 3)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (width - 2) * nChan)[2];

                    }
                    else if (menorValor == 4 || menorValor == 5)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (width - 1) * nChan)[2];

                    }
                    else if (menorValor == 6)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (width - 2) * nChan)[2];

                    }
                    else if (menorValor == 7 || menorValor == 8)
                    {

                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[0];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[1];
                        (dataPtr + (i) * widthStep + (width - 1) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (width - 1) * nChan)[2];

                    }

                }

                //pixeis centrais

                for (int i = 1; i < height - 1; i++)
                {
                    for (int j = 1; j < width - 1; j++)
                    {

                        distancias[0] =
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[1] =
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[2] =
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[3] =
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[4] =
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                             Math.Abs((dataPtrCopy + (i) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[5] =
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[6] =
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[7] =
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2]);

                        distancias[8] =
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2]) +

                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1]) +
                            Math.Abs((dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2] - (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2]);

                        menorValor = Array.IndexOf(distancias, distancias.Min());

                        switch (menorValor)
                        {
                            case 0:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (j - 1) * nChan)[2];
                                break;

                            case 1:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (j) * nChan)[2];
                                break;

                            case 2:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i + 1) * widthStep + (j + 1) * nChan)[2];
                                break;

                            case 3:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j - 1) * nChan)[2];
                                break;

                            case 4:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];
                                break;

                            case 5:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j + 1) * nChan)[2];
                                break;

                            case 6:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (j - 1) * nChan)[2];
                                break;

                            case 7:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (j) * nChan)[2];
                                break;

                            case 8:
                                (dataPtr + (i) * widthStep + (j) * nChan)[0] = (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[0];
                                (dataPtr + (i) * widthStep + (j) * nChan)[1] = (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[1];
                                (dataPtr + (i) * widthStep + (j) * nChan)[2] = (dataPtrCopy + (i - 1) * widthStep + (j + 1) * nChan)[2];
                                break;

                            default:
                                break;
                        }
                    }
                }
            }
        }

        public static int[] Histogram_Gray(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;
                int[] histograma = new int[256];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        int pixel = (int)Math.Round(((double)(dataPtr + (i) * widthStep + (j) * nChan)[0] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[1] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[2]) / 3);
                        histograma[pixel] += 1;
                    }
                }
                return histograma;
            }
        }

        public static int[,] Histogram_RGB(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;
                int[,] histograma = new int[3, 256];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        int blue = (int)(dataPtr + (i) * widthStep + (j) * nChan)[0];
                        int green = (int)(dataPtr + (i) * widthStep + (j) * nChan)[1];
                        int red = (int)(dataPtr + (i) * widthStep + (j) * nChan)[2];

                        histograma[0, blue] += 1;
                        histograma[1, green] += 1;
                        histograma[2, red] += 1;
                    }
                }
                return histograma;
            }
        }
        public static int[,] Histogram_All(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;
                int[,] histograma = new int[4, 256];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        int blue = (int)(dataPtr + (i) * widthStep + (j) * nChan)[0];
                        int green = (int)(dataPtr + (i) * widthStep + (j) * nChan)[1];
                        int red = (int)(dataPtr + (i) * widthStep + (j) * nChan)[2];
                        int gray = (int)Math.Round(((double)(dataPtr + (i) * widthStep + (j) * nChan)[0] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[1] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[2]) / 3);

                        histograma[0, blue] += 1;
                        histograma[1, green] += 1;
                        histograma[2, red] += 1;
                        histograma[3, gray] += 1;
                    }
                }
                return histograma;
            }
        }
        public static void ConvertToBW(Emgu.CV.Image<Bgr, byte> img, int threshold)
        {
            unsafe
            {

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        int pixel = (int)Math.Round(((double)(dataPtr + (i) * widthStep + (j) * nChan)[0] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[1] + (double)(dataPtr + (i) * widthStep + (j) * nChan)[2]) / 3);


                        if (pixel <= threshold)
                        {
                            pixel = 0;
                        }
                        else
                        {
                            pixel = 255;
                        }

                        (dataPtr + i * widthStep + j * nChan)[0] = (byte)pixel;
                        (dataPtr + i * widthStep + j * nChan)[1] = (byte)pixel;
                        (dataPtr + i * widthStep + j * nChan)[2] = (byte)pixel;

                    }
                }



            }
        }

        public static void ConvertToBW_Otsu(Emgu.CV.Image<Bgr, byte> img)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                int padding = m.widthStep - m.nChannels * m.width;
                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                double[] histograma = new double[256];
                int x, y, z;
                double total = width * height;
                double varianciaMaxima = -999;
                int treshold = 0;
                int pixel;

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pixel = (int)Math.Round(((double)(dataPtr + i * widthStep + j * nChan)[0] + (double)(dataPtr + i * widthStep + j * nChan)[1] + (double)(dataPtr + i * widthStep + j * nChan)[2]) / 3.0);
                        histograma[pixel] += 1;
                        (dataPtr + i * widthStep + j * nChan)[0] = (byte)pixel;
                        (dataPtr + i * widthStep + j * nChan)[1] = (byte)pixel;
                        (dataPtr + i * widthStep + j * nChan)[2] = (byte)pixel;

                    }
                }

                for (x = 0; x < 256; x++)
                {
                    double u1 = 0, u2 = 0, q1 = 0, q2 = 0, variancia = 0;

                    for (y = 0; y <= x; y++)
                    {
                        q1 += (double)(histograma[y]) / total;
                        u1 += y * ((double)(histograma[y]) / total);
                    }
                    if (q1 == 0)
                    {
                        u1 = 0;
                    }
                    else
                    {
                        u1 /= q1;

                    }
                    for (z = x + 1; z < 256; z++)
                    {
                        q2 += (double)(histograma[z]) / total;
                        u2 += z * ((double)histograma[z] / total);
                    }
                    if (q2 == 0)
                    {
                        u2 = 0;
                    }
                    else
                    {
                        u2 /= q2;

                    }
                    variancia = q1 * q2 * Math.Pow(u1 - u2, 2);
                    if (varianciaMaxima < variancia)
                    {
                        varianciaMaxima = variancia;
                        treshold = x;
                    }
                }
                for (y = 0; y < height; y++)
                {
                    for (z = 0; z < width; z++)
                    {
                        if ((int)(dataPtr + y * widthStep + z * nChan)[0] <= treshold)
                        {
                            (dataPtr + y * widthStep + z * nChan)[0] = 0;
                        }
                        else
                        {
                            (dataPtr + y * widthStep + z * nChan)[0] = 255;
                        }

                        (dataPtr + y * widthStep + z * nChan)[1] = (dataPtr + y * widthStep + z * nChan)[0];
                        (dataPtr + y * widthStep + z * nChan)[2] = (dataPtr + y * widthStep + z * nChan)[0];
                    }
                }
            }
        }

        public static int[,] compLigadas(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy) // usado o Algoritmo Interativo
        {
            //Na matriz:
            //1º valor = pixel canto superior esquerdo
            //2º valor = pixel canto superior direito
            //3º valor = pixel canto inferior esquerdo
            //4º valor = pixel canto inferior direito

            //1º valor - qual imagem; 2º valor - qual pixel; 3º valor - qual coordenada;
            //3º valor está em altura - largura

            unsafe
            {

                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int etiqueta = 0;
                int[,] pixeisEtiqueta = new int[height, width];
                bool trocou = true;
                List<int> numerosEtiImg = new List<int>();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        if ((int)(dataPtr + i * widthStep + j * nChan)[0] != 0 && (int)(dataPtr + i * widthStep + j * nChan)[1] != 0 && (int)(dataPtr + i * widthStep + j * nChan)[2] != 0)
                        {
                            etiqueta++;
                            pixeisEtiqueta[i, j] = etiqueta;
                        }
                        else
                        {
                            pixeisEtiqueta[i, j] = 0;
                        }
                    }
                }

                while (true)
                {

                    trocou = false;

                    // cima-baixo e esquerda-direita
                    for (int i = 0; i < height - 1; i++)
                    {
                        for (int j = 0; j < width - 1; j++)
                        {

                            if (pixeisEtiqueta[i, j] != 0)
                            {

                                if (pixeisEtiqueta[i + 1, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j - 1];
                                }

                                if (pixeisEtiqueta[i + 1, j] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j];
                                }

                                if (pixeisEtiqueta[i + 1, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j + 1];
                                }

                                if (pixeisEtiqueta[i, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i, j - 1];
                                }

                                if (pixeisEtiqueta[i, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i, j + 1];
                                }

                                if (pixeisEtiqueta[i - 1, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j - 1];
                                }

                                if (pixeisEtiqueta[i - 1, j] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j];
                                }

                                if (pixeisEtiqueta[i - 1, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j + 1];
                                }
                            }
                        }
                    }

                    if (trocou == false)
                    {
                        break;
                    }

                    trocou = false;

                    //baixo-cima e direita-esquerda
                    for (int i = height - 1; i > 0; i--)
                    {
                        for (int j = width - 1; j > 0; j--)
                        {

                            if (pixeisEtiqueta[i, j] != 0)
                            {

                                if (pixeisEtiqueta[i + 1, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j - 1];
                                }

                                if (pixeisEtiqueta[i + 1, j] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j];
                                }

                                if (pixeisEtiqueta[i + 1, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i + 1, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i + 1, j + 1];
                                }

                                if (pixeisEtiqueta[i, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i, j - 1];
                                }

                                if (pixeisEtiqueta[i, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i, j + 1];
                                }

                                if (pixeisEtiqueta[i - 1, j - 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j - 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j - 1];
                                }

                                if (pixeisEtiqueta[i - 1, j] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j];
                                }

                                if (pixeisEtiqueta[i - 1, j + 1] < pixeisEtiqueta[i, j] && pixeisEtiqueta[i - 1, j + 1] != 0)
                                {
                                    trocou = true;
                                    pixeisEtiqueta[i, j] = pixeisEtiqueta[i - 1, j + 1];
                                }
                            }

                        }
                    }

                    if (trocou == false)
                    {
                        break;
                    }
                }

                

                return pixeisEtiqueta;
            }
        }

        public static int[][,] pixeisCantosSRodar(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int[,] pixeisEtiqueta)
        {

            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                //usei isto para poder retirar os repetidos, de modo a saber o numero de imagens a colar

                List<int> listaComRepetidos = new List<int>();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        listaComRepetidos.Add(pixeisEtiqueta[i, j]);
                    }
                }

                List<int> listaSemRepetidos = listaComRepetidos.Distinct().ToList();
                listaSemRepetidos.Sort();

                int[] valoresProcurar = listaSemRepetidos.ToArray();

                int[][,] coordenadasCantos = new int[valoresProcurar.Length - 1][,];

                int pos = 0;
                bool verifica = false;

                for (int valores = 0; valores < valoresProcurar.Length; valores++)
                {
                    if (valoresProcurar[valores] != 0)
                    {

                        verifica = false;

                        int[,] valoresCantos = new int[4, 2];

                        //procura width - height para nao haver erros nos pixeis

                        for (int j = 0; j < width; j++)
                        {
                            for (int i = 0; i < height; i++)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {

                                    valoresCantos[0, 0] = j;
                                    valoresCantos[0, 1] = i;
                                    verifica = true;

                                }
                            }
                        }

                        verifica = false;
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = width - 1; j > 0; j--)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {

                                    valoresCantos[1, 0] = j;
                                    valoresCantos[1, 1] = i;
                                    verifica = true;

                                }
                            }
                        }

                        verifica = false;

                        for (int i = height - 1; i > 0; i--)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {

                                    valoresCantos[2, 0] = j;
                                    valoresCantos[2, 1] = i;
                                    verifica = true;

                                }
                            }
                        }

                        verifica = false;

                        for (int j = width - 1; j > 0; j--)
                        {
                            for (int i = height - 1; i > 0; i--)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {

                                    valoresCantos[3, 0] = j;
                                    valoresCantos[3, 1] = i;
                                    verifica = true;

                                }
                            }
                        }

                        verifica = false;

                        coordenadasCantos[pos] = valoresCantos;
                        pos++;
                    }
                }

                return coordenadasCantos;
            }

        }
        public static int[][,] pixeisCantosCRodar(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, int[,] pixeisEtiqueta)
        {

            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                //usei isto para poder retirar os repetidos, de modo a saber o numero de imagens a colar

                List<int> listaComRepetidos = new List<int>();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        listaComRepetidos.Add(pixeisEtiqueta[i, j]);
                    }
                }

                List<int> listaSemRepetidos = listaComRepetidos.Distinct().ToList();
                listaSemRepetidos.Sort();

                int[] valoresProcurar = listaSemRepetidos.ToArray();

                int[][,] coordenadasCantos = new int[valoresProcurar.Length - 1][,];

                bool verifica = false;
                int pos = 0;
                int count = 0;

                for (int valores = 0; valores < valoresProcurar.Length; valores++)
                {
                    if (valoresProcurar[valores] != 0)
                    {


                        int[,] valoresCantos = new int[4, 2];

                        //procura width - height para nao haver erros nos pixeis

                        int valorX = 0, valorY = 0;

                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores])
                                {
                                    if (i > valorX)
                                    {
                                        valorX = i;
                                    }

                                    if (j > valorY)
                                    {
                                        valorY = j;
                                    }

                                    valoresCantos[3, 0] = valorY;
                                    valoresCantos[3, 1] = valorX;
                                }
                            }
                        }
                       
                        count = 0;

                        verifica = false;
                        
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = width - 1; j > 0; j--)
                            {
                                
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {
                                    if(count == 0)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        valoresCantos[1, 0] = j;
                                        valoresCantos[1, 1] = i;
                                        verifica = true; 
                                    }
                                    

                                }
                            }
                        }

                        count = 0;
                        verifica = false;

                        for (int i = height - 1; i > 0; i--)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores] && verifica == false)
                                {
                                    if (count == 0)
                                    {
                                        count++;
                                    }
                                    else
                                    {
                                        valoresCantos[2, 0] = j;
                                        valoresCantos[2, 1] = i;
                                        verifica = true;
                                    }

                                }
                            }
                        }

                        valorX = height - 1;
                        valorY = width - 1;

                        for (int i = height - 1; i > 0; i--)
                        {
                            for (int j = width - 1; j > 0; j--)
                            {
                                if (pixeisEtiqueta[i, j] == valoresProcurar[valores])
                                {

                                    if (i < valorX)
                                    {
                                        valorX = i;
                                    }

                                    if (j < valorY)
                                    {
                                        valorY = j;
                                    }
                                    valoresCantos[0, 0] = valorY;
                                    valoresCantos[0, 1] = valorX;

                                }
                            }
                        }

                        coordenadasCantos[pos] = valoresCantos;
                        pos++;
                    }
                }

                return coordenadasCantos;
            }
            
        }

        public static double[] graus(int[][,] pixeisCantosImagens)
        {

            double[] grausImagens = new double[pixeisCantosImagens.Length];

            for (int numberImg = 0; numberImg < pixeisCantosImagens.Length; numberImg++)
            {

                int dx = Math.Abs(pixeisCantosImagens[numberImg][3, 0] - pixeisCantosImagens[numberImg][2, 0]); // csdX - cidX
                int dy = Math.Abs(pixeisCantosImagens[numberImg][3, 1] - pixeisCantosImagens[numberImg][2, 1]); // csdY - cidY

                double angle;

                if (dx == 0 || dy == 0)
                {
                    angle = 0;
                }
                else
                {
                    angle = (Math.Atan2(dy, dx)); //Math.Round(Math.Atan2(dy , dx) * (180.0 / Math.PI))(Math.Atan2(dy, dx)) para graus
                }

                grausImagens[numberImg] = angle;
            }

            return grausImagens;
        }

        public static Image<Bgr, byte> rodarImagem(int[][,] pixeisCantosImagens, int peça, double angle, Image<Bgr, byte> img)
        {
            unsafe
            {

                int y = (int)(pixeisCantosImagens[peça][1, 1] * 0.95);
                int x = (int)(pixeisCantosImagens[peça][0, 0] * 0.95);

                int widthRotacao = (pixeisCantosImagens[peça][3, 0]) - x;
                int heigtRotacao = (pixeisCantosImagens[peça][2, 1]) - y;

                int w = (int)Math.Abs((widthRotacao) * 1.05);
                int h = (int)Math.Abs((heigtRotacao) * 1.05);


                Rectangle retangulo = new Rectangle(x, y, (int)w, (int)h);

                Image<Bgr, byte> imgRot = img.Copy(retangulo);

                Rotation(imgRot, imgRot.Clone(), (float)(angle)); // (float) (angle * (Math.PI / 180))) para graus

                pixeisCantosImagens = pixeisCantosCRodar(imgRot, imgRot.Clone(), compLigadas(imgRot, imgRot.Clone()));


                MIplImage m = imgRot.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgRot.Clone().MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();

                int width = imgRot.Width;
                int height = imgRot.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                // meter a linha de cima da imagem rodada toda reta

                for (int j = 0; j < width; j++)
                {

                    byte pixelR = (dataPtrCopy + (pixeisCantosImagens[0][0,1] + 2) * widthStep + (j) * nChan)[0];
                    byte pixelG = (dataPtrCopy + (pixeisCantosImagens[0][0,1] + 2) * widthStep + (j) * nChan)[1];
                    byte pixelB = (dataPtrCopy + (pixeisCantosImagens[0][0,1] + 2) * widthStep + (j) * nChan)[2];

                    if((dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[0] == 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[1] == 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[2] == 0)
                    {
                        (dataPtr + (pixeisCantosImagens[0][0, 1] + 1) * widthStep + (j) * nChan)[0] = pixelR;
                        (dataPtr + (pixeisCantosImagens[0][0, 1] + 1) * widthStep + (j) * nChan)[1] = pixelG;
                        (dataPtr + (pixeisCantosImagens[0][0, 1] + 1) * widthStep + (j) * nChan)[2] = pixelB;
                    }

                    if ((dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[0] != 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[1] != 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[2] != 0)
                    {
                        (dataPtr + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[0] = 0;
                        (dataPtr + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[1] = 0;
                        (dataPtr + (pixeisCantosImagens[0][0, 1]) * widthStep + (j) * nChan)[2] = 0;
                    }

                }

                // meter a linha de baixo da imagem rodada toda reta

                for (int j = 0; j < imgRot.Width; j++)
                {

                    byte pixelR = (dataPtrCopy + (pixeisCantosImagens[0][2, 1] - 2) * widthStep + (j) * nChan)[0];
                    byte pixelG = (dataPtrCopy + (pixeisCantosImagens[0][2, 1] - 2) * widthStep + (j) * nChan)[1];
                    byte pixelB = (dataPtrCopy + (pixeisCantosImagens[0][2, 1] - 2) * widthStep + (j) * nChan)[2];

                    if ((dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[0] == 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[1] == 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[2] == 0)
                    {
                        (dataPtr + (pixeisCantosImagens[0][2, 1] - 1) * widthStep + (j) * nChan)[0] = pixelR;
                        (dataPtr + (pixeisCantosImagens[0][2, 1] - 1) * widthStep + (j) * nChan)[1] = pixelG;
                        (dataPtr + (pixeisCantosImagens[0][2, 1] - 1) * widthStep + (j) * nChan)[2] = pixelB;
                    }

                    if ((dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[0] != 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[1] != 0 &&
                        (dataPtrCopy + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[2] != 0)
                    {
                        (dataPtr + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[0] = 0;
                        (dataPtr + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[1] = 0;
                        (dataPtr + (pixeisCantosImagens[0][2, 1]) * widthStep + (j) * nChan)[2] = 0;
                    }

                }



                mc = imgRot.Clone().MIplImage;
                dataPtrCopy = (byte*)mc.imageData.ToPointer();

                int margemX = 0;

                for (int j = pixeisCantosImagens[0][0, 1]; j < pixeisCantosImagens[0][2, 1]; j++)
                {
                    for (int i = pixeisCantosImagens[0][0, 0]; i < pixeisCantosImagens[0][1, 0]; i++)
                    {

                        if((dataPtrCopy + (j) * widthStep + (i) * nChan)[0] != 0 &&
                        (dataPtrCopy + (j) * widthStep + (i) * nChan)[1] != 0 &&
                        (dataPtrCopy + (j) * widthStep + (i) * nChan)[2] != 0)
                        {
                            margemX = i;
                            break;
                        }
                    }

                    for (int i = pixeisCantosImagens[0][0, 0]; i < margemX; i++)
                    {

                        (dataPtr + (j) * widthStep + (i) * nChan)[0] = (byte)(dataPtrCopy + (j) * widthStep + (margemX) * nChan)[0];
                        (dataPtr + (j) * widthStep + (i) * nChan)[1] = (byte)(dataPtrCopy + (j) * widthStep + (margemX) * nChan)[1];
                        (dataPtr + (j) * widthStep + (i) * nChan)[2] = (byte)(dataPtrCopy + (j) * widthStep + (margemX) * nChan)[2];

                    }
                }
                return imgRot;
            }
        }

        public static void larguraMaior(int[][,] pixeisCantosImagens, int img1, int img2, int[] tamanhosImgAlt, Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mCopy = img.Clone().MIplImage;
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                int width = img.Clone().Width;
                int height = img.Clone().Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int bx2 = pixeisCantosImagens[img2][2, 0];

                int margemCima1Baixo2 = 0, margemBaixo1Cima2 = 0;

                for (int cm = pixeisCantosImagens[img1][0, 0]; cm < pixeisCantosImagens[img1][1, 0]; cm++)
                {

                    if ((dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[0] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[0] ||
                    (dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[1] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[1] ||
                    (dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[2] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[2])
                    {
                        margemCima1Baixo2++;
                    }

                    bx2++;
                }

                int cm2 = pixeisCantosImagens[img2][0, 0];

                for (int bx = pixeisCantosImagens[img1][2, 0]; bx < pixeisCantosImagens[img1][3, 0]; bx++)
                {
                    if ((dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[0] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[0] ||
                    (dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[1] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[1] ||
                    (dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[2] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[2])
                    {
                        margemBaixo1Cima2++;
                    }

                    cm2++;
                }

                int valorPixelFinalCima1 = pixeisCantosImagens[img1][0, 1] - tamanhosImgAlt[img2];
                int valorPixelFinalCima2 = pixeisCantosImagens[img2][0, 1] - tamanhosImgAlt[img1];

                int valorPixelFinalBaixo1 = tamanhosImgAlt[img2] + pixeisCantosImagens[img2][2, 1];
                int valorPixelFinalBaixo2 = tamanhosImgAlt[img1] + pixeisCantosImagens[img1][2, 1];

                int[,] etiqueta = compLigadas(img, img.Clone());

                List<int> listaComRepetidos = new List<int>();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        listaComRepetidos.Add(etiqueta[i, j]);
                    }
                }

                List<int> listaSemRepetidos = listaComRepetidos.Distinct().ToList();
                listaSemRepetidos.Sort();
                listaSemRepetidos.Remove(0);

                int[] valoresProcurar = listaSemRepetidos.ToArray();

                bool cima1 = false, cima2 = false;
                bool baixo1 = false, baixo2 = false;

                for (int i = 0; i < pixeisCantosImagens.Length; i++)
                {
                    if (i != img1)
                    {
                        if (valorPixelFinalCima2 > 0 && (etiqueta[valorPixelFinalCima2, pixeisCantosImagens[img2][0, 0]] != valoresProcurar[i]))
                        {
                            cima2 = true;
                        }

                        if (valorPixelFinalBaixo2 < height && (etiqueta[valorPixelFinalBaixo2, pixeisCantosImagens[img2][0, 0]] != valoresProcurar[i]))
                        {
                            baixo2 = true;
                        }

                    }
                    if (i != img2)
                    {
                        if (valorPixelFinalCima1 > 0 && (etiqueta[valorPixelFinalCima1, pixeisCantosImagens[img1][0, 0]] != valoresProcurar[i]))
                        {
                            cima1 = true;
                        }

                        if (valorPixelFinalBaixo1 < height && (etiqueta[valorPixelFinalBaixo1, pixeisCantosImagens[img1][0, 0]] != valoresProcurar[i]))
                        {
                            baixo1 = true;
                        }
                    }
                }



                if (margemCima1Baixo2 < margemBaixo1Cima2 && (baixo1 || cima2))
                {
                    if (baixo1)
                    {
                        for (int i = pixeisCantosImagens[img2][0, 1]; i < pixeisCantosImagens[img2][2, 1] + 3; i++)
                        {
                            for (int j = pixeisCantosImagens[img2][0, 0]; j < pixeisCantosImagens[img2][1, 0] + 3; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordJ = pixeisCantosImagens[img1][2, 1] + 1;

                        for (int i = pixeisCantosImagens[img2][0, 1]; i <= pixeisCantosImagens[img2][2, 1]; i++)
                        {
                            int coordI = pixeisCantosImagens[img1][2, 0];

                            for (int j = pixeisCantosImagens[img2][0, 0]; j <= pixeisCantosImagens[img2][1, 0]; j++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordI++;
                            }

                            coordJ++;
                        }
                    }
                    else if (cima2)
                    {
                        
                        for (int i = pixeisCantosImagens[img1][0, 1]; i < pixeisCantosImagens[img1][2, 1] + 3; i++)
                        {
                            for (int j = pixeisCantosImagens[img1][0, 0]; j < pixeisCantosImagens[img1][1, 0] + 3; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordJ = pixeisCantosImagens[img2][0, 1] - 1;

                        for (int i = pixeisCantosImagens[img1][2, 1]; i >= pixeisCantosImagens[img1][0, 1]; i--)
                        {
                            int coordI = pixeisCantosImagens[img2][0, 0];

                            for (int j = pixeisCantosImagens[img1][2, 0]; j <= pixeisCantosImagens[img1][3, 0] + 1; j++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordI++;
                            }

                            coordJ--;
                        }
                        
                    }



                }
                else if (margemCima1Baixo2 > margemBaixo1Cima2 && (baixo2 || cima1)) // não tenho a certeza
                {
                    if (baixo2)
                    {
                        for (int i = pixeisCantosImagens[img1][0, 1]; i < pixeisCantosImagens[img1][2, 1] + 3; i++)
                        {
                            for (int j = pixeisCantosImagens[img1][0, 0]; j < pixeisCantosImagens[img1][1, 0] + 3; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordJ = pixeisCantosImagens[img2][2, 1] + 1;

                        for (int i = pixeisCantosImagens[img1][0, 1]; i <= pixeisCantosImagens[img1][2, 1]; i++)
                        {
                            int coordI = pixeisCantosImagens[img2][2, 0];

                            for (int j = pixeisCantosImagens[img1][0, 0]; j <= pixeisCantosImagens[img1][1, 0]; j++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordI++;
                            }

                            coordJ++;
                        }
                    }
                    else if (cima1)
                    {
                        for (int i = pixeisCantosImagens[img2][0, 1]; i < pixeisCantosImagens[img2][2, 1] + 3; i++)
                        {
                            for (int j = pixeisCantosImagens[img2][0, 0]; j < pixeisCantosImagens[img2][1, 0] + 3; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }
                      
                        int coordJ = pixeisCantosImagens[img1][0, 1] - 1;

                        for (int i = pixeisCantosImagens[img2][2, 1]; i >= pixeisCantosImagens[img2][0, 1]; i--)
                        {
                            int coordI = pixeisCantosImagens[img1][0, 0];

                            for (int j = pixeisCantosImagens[img2][2, 0]; j <= pixeisCantosImagens[img2][3, 0]; j++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordI++;
                            }

                            coordJ--;
                        }
                    }
                    

                }
            }
        }

        public static void alturaMaior(int[][,] pixeisCantosImagens, int img1, int img2, int[] tamanhosImgLarg, Image<Bgr, byte> img, Image<Bgr, byte> imgCopy)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mCopy = img.Clone().MIplImage;
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                int width = img.Clone().Width;
                int height = img.Clone().Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int margemEsq1Drt2 = 0, margemDrt1Esq2 = 0;

                int drt2 = pixeisCantosImagens[img2][1, 1];

                for (int esq = pixeisCantosImagens[img1][0, 1]; esq < pixeisCantosImagens[img1][2, 1]; esq++)
                {
                    if ((dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[0] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[0] ||
                    (dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[1] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[1] ||
                    (dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[2] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[2])
                    {
                        margemEsq1Drt2++;
                    }
                    drt2++;
                }

                int esq2 = pixeisCantosImagens[img2][0, 1];

                for (int drt = pixeisCantosImagens[img1][1, 1]; drt < pixeisCantosImagens[img1][3, 1]; drt++)
                {
                    if ((dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[0] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[0] ||
                    (dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[1] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[1] ||
                    (dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[2] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[2])
                    {
                        margemDrt1Esq2++;
                    }
                    esq2++;
                }

                

                int valorPixelFinalDrt1 = tamanhosImgLarg[img2] + pixeisCantosImagens[img1][1, 0];
                int valorPixelFinalDrt2 = tamanhosImgLarg[img1] + pixeisCantosImagens[img2][1, 0];

                int valorPixelFinalEsq1 = pixeisCantosImagens[img2][0, 0] - tamanhosImgLarg[img2];
                int valorPixelFinalEsq2 = pixeisCantosImagens[img1][0, 0] - tamanhosImgLarg[img1];

                

                int[,] etiqueta = compLigadas(img, img.Clone());

                List<int> listaComRepetidos = new List<int>();

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        listaComRepetidos.Add(etiqueta[i, j]);
                    }
                }

                List<int> listaSemRepetidos = listaComRepetidos.Distinct().ToList();
                listaSemRepetidos.Sort();
                listaSemRepetidos.Remove(0);

                int[] valoresProcurar = listaSemRepetidos.ToArray();

                bool direita1 = false, direita2 = false;
                bool esquerda1 = false, esquerda2 = false;

                for (int i = 0; i < pixeisCantosImagens.Length; i++)
                {
                    if(i != img1)
                    {
                        if (valorPixelFinalDrt2 < width && (etiqueta[pixeisCantosImagens[img2][0, 1] , valorPixelFinalDrt2] != valoresProcurar[i]))
                        {
                            direita2 = true;
                        }

                        if (valorPixelFinalEsq2 > 0 && (etiqueta[pixeisCantosImagens[img2][0, 1], valorPixelFinalEsq2] != valoresProcurar[i]))
                        {
                            esquerda2 = true;
                        }

                    }
                    if(i != img2)
                    {
                        if (valorPixelFinalDrt1 < width && (etiqueta[pixeisCantosImagens[img1][0, 1], valorPixelFinalDrt1] != valoresProcurar[i]))
                        {
                            direita1 = true;
                        }

                        if (valorPixelFinalEsq1 > 0 && (etiqueta[pixeisCantosImagens[img1][0, 1], valorPixelFinalEsq1] != valoresProcurar[i]))
                        {
                            esquerda1 = true;
                        }
                    }
                }



                if (margemDrt1Esq2 > margemEsq1Drt2 && (direita1 || esquerda2))
                {

                    if (direita1) //ver isto bem
                    {
                        for (int i = pixeisCantosImagens[img2][0, 1] - 1; i < pixeisCantosImagens[img2][2, 1] + 1; i++)
                        {
                            for (int j = pixeisCantosImagens[img2][0, 0] - 1; j < pixeisCantosImagens[img2][1, 0] + 1; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordI = pixeisCantosImagens[img1][1, 0] + 1;

                        for (int j = pixeisCantosImagens[img2][0, 0]; j <= pixeisCantosImagens[img2][1, 0]; j++)
                        {
                            int coordJ = pixeisCantosImagens[img1][1, 1];

                            for (int i = pixeisCantosImagens[img2][0, 1]; i <= pixeisCantosImagens[img2][2, 1]; i++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordJ++;
                            }

                            coordI++;
                        }
                    }
                    else if (esquerda2)
                    {
                        for (int i = pixeisCantosImagens[img1][0, 1] - 1; i < pixeisCantosImagens[img1][2, 1] + 1; i++)
                        {
                            for (int j = pixeisCantosImagens[img1][0, 0] - 1; j < pixeisCantosImagens[img1][1, 0] + 1; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordI = pixeisCantosImagens[img2][2, 0] - 1;

                        for (int j = pixeisCantosImagens[img1][1, 0]; j >= pixeisCantosImagens[img1][0, 0]; j--)
                        {
                            int coordJ = pixeisCantosImagens[img2][2, 1];

                            for (int i = pixeisCantosImagens[img1][2, 1]; i >= pixeisCantosImagens[img1][0, 1]; i--)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordJ--;
                            }
                            coordI--;
                        }
                    }

                }
                else if (margemDrt1Esq2 < margemEsq1Drt2 && (direita2 || esquerda1))
                {
                    if (direita2)
                    {
                        for (int i = pixeisCantosImagens[img1][0, 1] - 1; i < pixeisCantosImagens[img1][2, 1] + 2; i++)
                        {
                            for (int j = pixeisCantosImagens[img1][0, 0] - 1; j < pixeisCantosImagens[img1][1, 0] + 2; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordI = pixeisCantosImagens[img2][1, 0] + 1;

                        for (int j = pixeisCantosImagens[img1][0, 0]; j <= pixeisCantosImagens[img1][1, 0]; j++)
                        {
                            int coordJ = pixeisCantosImagens[img2][1, 1];

                            for (int i = pixeisCantosImagens[img1][0, 1]; i <= pixeisCantosImagens[img1][2, 1]; i++)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordJ++;
                            }

                            coordI++;
                        }

                        
                        
                    }
                    else if (esquerda1)
                    {
                        for (int i = pixeisCantosImagens[img2][0, 1]; i < pixeisCantosImagens[img2][2, 1] + 1; i++)
                        {
                            for (int j = pixeisCantosImagens[img2][0, 0]; j < pixeisCantosImagens[img2][1, 0] + 1; j++)
                            {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;
                            }
                        }

                        int coordI = pixeisCantosImagens[img1][2, 0] - 1;

                        for (int j = pixeisCantosImagens[img2][1, 0]; j >= pixeisCantosImagens[img2][0, 0]; j--)
                        {
                            int coordJ = pixeisCantosImagens[img1][2, 1];

                            for (int i = pixeisCantosImagens[img2][2, 1]; i >= pixeisCantosImagens[img2][0, 1]; i--)
                            {

                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[0] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[0];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[1] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[1];
                                (dataPtr + (coordJ) * widthStep + (coordI) * nChan)[2] = (dataPtrCopy + (i) * widthStep + (j) * nChan)[2];

                                coordJ--;
                            }

                            coordI--;
                        }
                    } 
                }


            }
        }

        public static int[] margemAltura(Image<Bgr, byte> img, int[][,] pixeisCantosImagens, int img1, int img2)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mCopy = img.Clone().MIplImage;
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                int width = img.Clone().Width;
                int height = img.Clone().Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int margemEsq1Drt2 = 0, margemDrt1Esq2 = 0;

                int drt2 = pixeisCantosImagens[img2][1, 1];

                for (int esq = pixeisCantosImagens[img1][0, 1]; esq < pixeisCantosImagens[img1][2, 1]; esq++)
                {
                    if ((dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[0] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[0] ||
                    (dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[1] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[1] ||
                    (dataPtr + esq * widthStep + pixeisCantosImagens[img1][0, 0] * nChan)[2] == (dataPtr + drt2 * widthStep + pixeisCantosImagens[img2][1, 0] * nChan)[2])
                    {
                        margemEsq1Drt2++;
                    }
                    drt2++;
                }

                int esq2 = pixeisCantosImagens[img2][0, 1];

                for (int drt = pixeisCantosImagens[img1][1, 1]; drt < pixeisCantosImagens[img1][3, 1]; drt++)
                {
                    if ((dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[0] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[0] ||
                    (dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[1] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[1] ||
                    (dataPtr + drt * widthStep + pixeisCantosImagens[img1][1, 0] * nChan)[2] == (dataPtr + esq2 * widthStep + pixeisCantosImagens[img2][0, 0] * nChan)[2])
                    {
                        margemDrt1Esq2++;
                    }
                    esq2++;
                }

                int[] val = new int[4];
                val[0] = margemEsq1Drt2;
                val[1] = margemDrt1Esq2;
                val[2] = img1;
                val[3] = img2;

                return val;
            }
        }

        public static int[] margemLargura(Image<Bgr, byte> img, int[][,] pixeisCantosImagens, int img1, int img2)
        {
            unsafe
            {
                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mCopy = img.Clone().MIplImage;
                byte* dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                int width = img.Clone().Width;
                int height = img.Clone().Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                int bx2 = pixeisCantosImagens[img2][2, 0];

                int margemCima1Baixo2 = 0, margemBaixo1Cima2 = 0;

                for (int cm = pixeisCantosImagens[img1][0, 0]; cm < pixeisCantosImagens[img1][1, 0]; cm++)
                {

                    if ((dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[0] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[0] ||
                    (dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[1] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[1] ||
                    (dataPtr + pixeisCantosImagens[img1][0, 1] * widthStep + cm * nChan)[2] == (dataPtr + pixeisCantosImagens[img2][2, 1] * widthStep + bx2 * nChan)[2])
                    {
                        margemCima1Baixo2++;
                    }

                    bx2++;
                }

                int cm2 = pixeisCantosImagens[img2][0, 0];

                for (int bx = pixeisCantosImagens[img1][2, 0]; bx < pixeisCantosImagens[img1][3, 0]; bx++)
                {
                    if ((dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[0] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[0] ||
                    (dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[1] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[1] ||
                    (dataPtr + pixeisCantosImagens[img1][2, 1] * widthStep + bx * nChan)[2] == (dataPtr + pixeisCantosImagens[img2][0, 1] * widthStep + cm2 * nChan)[2])
                    {
                        margemBaixo1Cima2++;
                    }

                    cm2++;
                }

                int[] val = new int[4];
                val[0] = margemCima1Baixo2;
                val[1] = margemBaixo1Cima2;
                val[2] = img1;
                val[3] = img2;

                return val;
            }
        }


        public static int[] multiplasImagens(Image<Bgr, byte> img, int[][,] pixeisCantosImagens, int valorComparatorio, int[] tamanhosImg, bool altura)
        {
            unsafe
            {
                List<int> lista = new List<int>();

                for (int i = 0; i < tamanhosImg.Length; i++)
                {
                    if (tamanhosImg[i] == valorComparatorio)
                    {
                        lista.Add(i);
                    }
                }
                int[] listaArray = lista.ToArray();


                int[][] valores = new int[listaArray.Length][];

                for (int imagem1 = 0; imagem1 < listaArray.Length; imagem1++)
                {
                    for (int imagem2 = 0; imagem2 < listaArray.Length; imagem2++)
                    {
                        if (imagem1 != imagem2)
                        {
                            if (altura)
                            {
                                valores[imagem1] = margemAltura(img, pixeisCantosImagens, listaArray[imagem1], listaArray[imagem2]);
                            }
                            else
                            {
                                valores[imagem1] = margemLargura(img, pixeisCantosImagens, listaArray[imagem1], listaArray[imagem2]);
                            }
                        }
                    }

                }

                int valor0Maior = valores[0][0];
                int valor1Maior = valores[0][1];

                for (int i = 0; i < valores.Length; i++)
                {
                    int[] imagens = valores[i];

                    if (imagens[0] > valor0Maior)
                    {
                        valor0Maior = imagens[0];
                    }

                    if (imagens[1] > valor1Maior)
                    {
                        valor1Maior = imagens[1];
                    }

                }

                int val = valor0Maior;
                int pos = 0;

                if (valor0Maior < valor1Maior)
                {
                    val = valor1Maior;
                    pos = 1;
                }

                int[] imagensFinais = new int[2];

                for (int i = 0; i < valores.Length; i++)
                {
                    if (valores[i][pos] == val)
                    {
                        imagensFinais[0] = valores[1][2];
                        imagensFinais[1] = valores[1][3];
                        break;
                    }
                }

                return imagensFinais;
            }
        }
        public static void puzzle(Image<Bgr, byte> img, Image<Bgr, byte> imgCopy, out List<int[]> Pieces_positions, out List<int> Pieces_angle, int level)
        {
            unsafe
            {


                MIplImage m = img.MIplImage;
                byte* dataPtr = (byte*)m.imageData.ToPointer();

                MIplImage mc = imgCopy.MIplImage;
                byte* dataPtrCopy = (byte*)mc.imageData.ToPointer();


                int width = img.Width;
                int height = img.Height;
                int nChan = m.nChannels;
                int widthStep = m.widthStep;

                byte corFundoR = (dataPtr + (0) * widthStep + (0) * nChan)[0];
                byte corFundoG = (dataPtr + (0) * widthStep + (0) * nChan)[1];
                byte corFundoB = (dataPtr + (0) * widthStep + (0) * nChan)[2];

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {

                        if ((dataPtrCopy + i * widthStep + j * nChan)[0] == corFundoR &&
                        (dataPtrCopy + i * widthStep + j * nChan)[1] == corFundoG &&
                        (dataPtrCopy + i * widthStep + j * nChan)[2] == corFundoB)
                        {

                            (dataPtr + i * widthStep + j * nChan)[0] = (byte)0;
                            (dataPtr + i * widthStep + j * nChan)[1] = (byte)0;
                            (dataPtr + i * widthStep + j * nChan)[2] = (byte)0;

                        }
                    }
                }

                int[][,] pixeisCantosImagens = pixeisCantosSRodar(img, imgCopy, compLigadas(img, imgCopy));

                double[] grausImagens = graus(pixeisCantosImagens);

               for (int peça = 0; peça < grausImagens.Length; peça++)
                {
                    double angle = grausImagens[peça];

                    if (angle != 0)
                    {

                        Image<Bgr, byte> imgRot = rodarImagem(pixeisCantosImagens, peça, angle, img);

                        MIplImage mRot = imgRot.MIplImage;
                        byte* dataPtrRot = (byte*)mRot.imageData.ToPointer();

                        int nChanRot = mRot.nChannels;
                        int widthStepRot = mRot.widthStep;

                        int y = (int)(pixeisCantosImagens[peça][1, 1] * 0.95);
                        int x = (int)(pixeisCantosImagens[peça][0, 0] * 0.95);

                        int widthRotacao = (pixeisCantosImagens[peça][3, 0]) - x;
                        int heigtRotacao = (pixeisCantosImagens[peça][2, 1]) - y;

                        int w = (int)Math.Abs((widthRotacao) * 1.05);
                        int h = (int)Math.Abs((heigtRotacao) * 1.05);

                        int rotY = 0;

                        for (int i = y; i < h + y; i++)
                        {
                             int rotX = 0;

                             for (int j = x; j < w + x; j++)
                             {

                                (dataPtr + i * widthStep + j * nChan)[0] = (byte)(dataPtrRot + rotY * widthStepRot + rotX * nChanRot)[0];
                                (dataPtr + i * widthStep + j * nChan)[1] = (byte)(dataPtrRot + rotY * widthStepRot + rotX * nChanRot)[1];
                                (dataPtr + i * widthStep + j * nChan)[2] = (byte)(dataPtrRot + rotY * widthStepRot + rotX * nChanRot)[2];


                                rotX++;
                             }
                             rotY++;
                        }
                    }
                }

                bool verificarRoda = false;

                for (int peça = 0; peça < grausImagens.Length; peça++)
                {
                    double angle = grausImagens[peça];

                    if (angle != 0)
                    {
                        verificarRoda = true;
                    }
                }
                
                if (!verificarRoda)
                {
                    pixeisCantosImagens = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));
                }
                else
                {
                    pixeisCantosImagens = pixeisCantosCRodar(img, img.Clone(), compLigadas(img, img.Clone()));
                }

                // para o argumento da função, os graus
                Pieces_angle = new List<int>();

                for (int a = 0; a < grausImagens.Length; a++)
                {
                    int graus = (int)Math.Round(grausImagens[a] * (180.0 / Math.PI));
                    Pieces_angle.Add(graus);
                }

                // para o argumento da função, o centro
                Pieces_positions = new List<int[]>();
                
                for(int i = 0;  i < pixeisCantosImagens.Length; i++)
                {
                    int[] dummy_vector = new int[2];

                    dummy_vector[0] = (pixeisCantosImagens[i][0, 0] + pixeisCantosImagens[i][1, 0]) / 2 ;
                    dummy_vector[1] = (pixeisCantosImagens[i][0, 1] + pixeisCantosImagens[i][2, 1]) / 2;

                    Pieces_positions.Add(dummy_vector);
                }

                

                int[] tamanhosImgLarg = new int[pixeisCantosImagens.Length];
                int[] tamanhosImgAlt = new int[pixeisCantosImagens.Length];


                for (int tam = 0; tam < pixeisCantosImagens.Length; tam++)
                {
                    int largura = (int)Math.Sqrt(Math.Pow(pixeisCantosImagens[tam][1, 0] - pixeisCantosImagens[tam][0, 0], 2) + Math.Pow(pixeisCantosImagens[tam][1, 1] - pixeisCantosImagens[tam][0, 1], 2));

                    int altura = (int)Math.Sqrt(Math.Pow(pixeisCantosImagens[tam][2, 1] - pixeisCantosImagens[tam][0, 1], 2) + Math.Pow(pixeisCantosImagens[tam][2, 0] - pixeisCantosImagens[tam][0, 0], 2));

                    tamanhosImgLarg[tam] = largura;
                    tamanhosImgAlt[tam] = altura;

                }


                if (grausImagens.Length == 1)
                {

                    Rectangle rect = new Rectangle(pixeisCantosImagens[0][0, 0], pixeisCantosImagens[0][0, 1], tamanhosImgLarg[0], tamanhosImgAlt[0]);

                    Image<Bgr, byte> tmp_i = img.Copy(rect);

                    tmp_i.Resize(img.Width, img.Height, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).CopyTo(img);

                }
                else if (grausImagens.Length == 2)
                {

                    MIplImage mCopy = img.Clone().MIplImage;
                    dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                    int widthCopy = img.Clone().Width;
                    int heightCopy = img.Clone().Height;
                    int nChanCopy = mCopy.nChannels;
                    int widthStepCopy = mCopy.widthStep;

                    if (tamanhosImgLarg.Length != 1 || tamanhosImgAlt.Length != 1)
                    {

                        int larg = Math.Abs(tamanhosImgLarg[0] - tamanhosImgLarg[1]);
                        int alt = Math.Abs(tamanhosImgAlt[0] - tamanhosImgAlt[1]);

                        if (larg < alt)
                        {
                                                        
                            larguraMaior(pixeisCantosImagens, 0, 1, tamanhosImgAlt, img, img.Clone());

                            pixeisCantosImagens = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));

                            Translation(img, img.Clone(), -pixeisCantosImagens[0][0, 0] + 1, -pixeisCantosImagens[0][0, 1] + 1);

                            // sempre que mexer em pixeisCantosImagens2, retirar 1 para saber o valor exato que se pretende
                            int[][,] pixeisCantosImagens2 = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));

                            Rectangle rect;

                            if (verificarRoda)
                            {
                                Translation(img, img.Clone(), -2, -2);

                                int escolherLarg = 1;

                                if (grausImagens[0] < grausImagens[1])
                                {
                                    escolherLarg = 0;
                                }

                                rect = new Rectangle(0, 0, tamanhosImgLarg[escolherLarg], tamanhosImgAlt[0] + tamanhosImgAlt[1]);

                            }
                            else
                            {
                                Translation(img, img.Clone(), -1, -1);
                                rect = new Rectangle(0, 0, pixeisCantosImagens2[0][1, 0] - 1, pixeisCantosImagens2[0][3, 1] - 1);

                            }


                            Image<Bgr, byte> tmp_i = img.Copy(rect);

                            tmp_i.Resize(img.Width, img.Height, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).CopyTo(img);
                            
                        }
                        else if (larg > alt)
                        {

                            Rectangle rect;

                            if (verificarRoda)
                            {

                                alturaMaior(pixeisCantosImagens, 0, 1, tamanhosImgLarg, img, img.Clone());

                                int[][,] pixeisCantosImagens2 = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));



                                Translation(img, img.Clone(), -pixeisCantosImagens2[0][0, 0] + 1, -pixeisCantosImagens2[0][0, 1] + 1);

                                // sempre que mexer em pixeisCantosImagens2, retirar 1 para saber o valor exato que se pretende
                                pixeisCantosImagens2 = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));

                                Translation(img, img.Clone(), -1, -1); //vai ser preciso, mas é para não me esquecer


                                int escolherAlt = 1;

                                if (grausImagens[0] > grausImagens[1])
                                {
                                    escolherAlt = 0;
                                }

                                rect = new Rectangle(0, 0, tamanhosImgLarg[0] + tamanhosImgLarg[1], tamanhosImgAlt[escolherAlt]);

                            }
                            else
                            {
                                alturaMaior(pixeisCantosImagens, 0, 1, tamanhosImgLarg, img, img.Clone());

                                int[][,] pixeisCantosImagens2 = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));



                                Translation(img, img.Clone(), -pixeisCantosImagens2[0][0, 0] + 1, -pixeisCantosImagens2[0][0, 1] + 1);

                                // sempre que mexer em pixeisCantosImagens2, retirar 1 para saber o valor exato que se pretende
                                pixeisCantosImagens2 = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));

                                Translation(img, img.Clone(), -1, -1); //vai ser preciso, mas é para não me esquecer




                                rect = new Rectangle(0, 0, pixeisCantosImagens2[0][3, 0] - 1, pixeisCantosImagens2[0][3, 1] - 1);
                            }

                            Image<Bgr, byte> tmp_i = img.Copy(rect);


                            tmp_i.Resize(img.Width, img.Height, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).CopyTo(img);



                        }

                        if (verificarRoda)
                        {
                            Median(img, img.Clone());
                        }
                    }
                }
                else
                {


                    for (int nrImg = 0; nrImg < grausImagens.Length; nrImg++)
                    {
                        MIplImage mCopy = img.Clone().MIplImage;
                        dataPtrCopy = (byte*)mCopy.imageData.ToPointer();

                        int widthCopy = img.Clone().Width;
                        int heightCopy = img.Clone().Height;
                        int nChanCopy = mCopy.nChannels;
                        int widthStepCopy = mCopy.widthStep;

                        bool verLarg = false, verAlt = false;

                        int valorLarg = height, valorAlt = width;

                        for (int larg = 0; larg < tamanhosImgLarg.Length; larg++)
                        {
                            for (int larg2 = 0; larg2 < tamanhosImgLarg.Length; larg2++)
                            {
                                if (larg != larg2 && tamanhosImgLarg[larg] == tamanhosImgLarg[larg2] && valorLarg > tamanhosImgLarg[larg])
                                {
                                    valorLarg = tamanhosImgLarg[larg];
                                    verLarg = true;

                                }
                            }
                        }

                        for (int alt = 0; alt < tamanhosImgAlt.Length; alt++)
                        {
                            for (int alt2 = 0; alt2 < tamanhosImgAlt.Length; alt2++)
                            {
                                if (alt != alt2 && tamanhosImgAlt[alt] == tamanhosImgAlt[alt2] && valorAlt > tamanhosImgAlt[alt])
                                {
                                    valorAlt = tamanhosImgAlt[alt];
                                    verAlt = true;

                                }
                            }
                        }

                        if (verLarg && verAlt)
                        {
                            if (valorAlt < valorLarg)
                            {
                                verLarg = false;
                            }
                            else
                            {
                                verAlt = false;
                            }
                        }

                        if (verAlt)
                        {

                            int[] imgs = multiplasImagens(img, pixeisCantosImagens, valorAlt, tamanhosImgAlt, true);
                                                      
                            alturaMaior(pixeisCantosImagens, imgs[0], imgs[1], tamanhosImgLarg, img, img.Clone());


                        }
                        else if (verLarg)
                        {
                            int[] imgs = multiplasImagens(img, pixeisCantosImagens, valorLarg, tamanhosImgLarg, false);
                                                        
                            larguraMaior(pixeisCantosImagens, imgs[0], imgs[1], tamanhosImgAlt, img, img.Clone());
                        }

                        pixeisCantosImagens = pixeisCantosSRodar(img, img.Clone(), compLigadas(img, img.Clone()));

                        tamanhosImgLarg = new int[pixeisCantosImagens.Length];
                        tamanhosImgAlt = new int[pixeisCantosImagens.Length];


                        for (int tam = 0; tam < pixeisCantosImagens.Length; tam++)
                        {
                            int largura = (int)Math.Sqrt(Math.Pow(pixeisCantosImagens[tam][1, 0] - pixeisCantosImagens[tam][0, 0], 2) + Math.Pow(pixeisCantosImagens[tam][1, 1] - pixeisCantosImagens[tam][0, 1], 2));

                            int altura = (int)Math.Sqrt(Math.Pow(pixeisCantosImagens[tam][2, 1] - pixeisCantosImagens[tam][0, 1], 2) + Math.Pow(pixeisCantosImagens[tam][2, 0] - pixeisCantosImagens[tam][0, 0], 2));

                            tamanhosImgLarg[tam] = largura;
                            tamanhosImgAlt[tam] = altura;

                        }
                    }

                    Rectangle rect = new Rectangle(pixeisCantosImagens[0][0,0], pixeisCantosImagens[0][0, 1], tamanhosImgLarg[0], tamanhosImgAlt[0]);

                    Image<Bgr, byte> tmp_i = img.Copy(rect);

                    tmp_i.Resize(img.Width, img.Height, Emgu.CV.CvEnum.INTER.CV_INTER_CUBIC).CopyTo(img);
                }




            }
            
        }


    }
}
