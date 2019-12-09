using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aerocock.Models
{

    public class Instructions
    {
        public enum CountType { START, STOP }

        public class Log
        {
            public static void Info(string message)
            {
                string BasePath = @"C:\TheCliff\Log\";//Environment.SpecialFolder.ApplicationData.ToString() + @"\Log\";
                string AllPath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ".txt";
                Directory.CreateDirectory(BasePath);
                {
                    using (StreamWriter writer = new StreamWriter(AllPath, true, Encoding.Default))
                        writer.WriteLine(DateTime.Now + " " + message);
                }
            }

            public static void GameCounter(string message, CountType type)
            {
                string BasePath = @"C:\TheCliff\GameCounter\";//Environment.SpecialFolder.ApplicationData + @"\GameCounter\";
                string AllPath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ' ' + type + ".txt";
                Directory.CreateDirectory(BasePath);

                int? counter = null;
                try
                {
                    if (type == CountType.START)
                    {
                        using (StreamReader reader = new StreamReader(AllPath))
                        {
                            string TextForCount = reader.ReadToEnd();
                            counter = TextForCount.Split('\n').Length;
                        }
                    }
                }
                catch (Exception exc)
                {
                    counter = 1;
                    Error("[Instructions.Log.GameCounter] " + exc.Message);
                }

                using (StreamWriter writer = new StreamWriter(AllPath, true, Encoding.Default))
                {
                    if (counter != null)
                        writer.WriteLine("[" + counter + "] " + DateTime.Now.ToString() + " " + message);
                    else
                        writer.WriteLine(DateTime.Now.ToString() + " " + message);
                }
            }

            public static void Error(string message)
            {
                string BasePath = Environment.SpecialFolder.ApplicationData + @"\Error\";
                string Allpath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ".txt";
                Directory.CreateDirectory(BasePath);
                using (StreamWriter writer = new StreamWriter(Allpath, true, Encoding.Default))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ' ' + message);
                }
            }
        }

        public static void ChangeMatrixInFile(int[,] matrix)
        {
            Directory.CreateDirectory(@"C:\TheCliff\Features\");
            string path = @"C:\TheCliff\Features\Matrix.txt";
            using (StreamWriter writer = new StreamWriter(path, false, Encoding.Default))
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    for (int i = 0; i < matrix.GetLength(1); i++)
                    {
                        writer.Write(matrix[i, j] + " ");
                    }
                    writer.WriteLine();
                }
            }
        }

        public static int[,] CreateMatrix(double santim)
        {
            string path = @"C:\TheCliff\Features\";

            Directory.CreateDirectory(path);
            using (StreamWriter writer = new StreamWriter(path + "Matrix.txt", false, Encoding.Default))
            {
                for (int j = 0; j < (300 / santim); j++)
                {
                    for (int i = 0; i < (300 / santim); i++)
                    {
                        writer.Write("0 ");
                    }
                    writer.WriteLine();
                }

            }
            int[,] matrix = new int[(int)(300 / santim), (int)(300 / santim)];
            Log.Info(path + "Создана пустая матрица");
            return matrix;
        }

        public static int[,] SetMatrix(int[,] matrix, double santim)
        {
            try
            {
                if (!File.Exists(@"C:\TheCliff\Features\Matrix.txt"))
                    throw new Exception("Отсутствует матрица в каталоге");
                matrix = new int[(int)(300 / santim)+10, (int)(300 / santim)+10];
                using (StreamReader reader = new StreamReader(@"C:\TheCliff\Features\Matrix.txt", Encoding.Default))
                {
                    string line;
                    for (int i = 0; (line = reader.ReadLine()) != null; i++)
                    {
                        var subline = line.Split(' ');
                        for (int j = 0; j < subline.Length - 1; j++)
                        {
                            matrix[j, i] = int.Parse(subline[j].ToString());
                        }
                    }
                    return matrix;
                    //c.Matrix = matrix;
                }
            }
            catch (Exception exc)
            {
                Log.Error("[Instructions.SetMatrix] " + exc);
                return CreateMatrix(santim);
            }

        }

        public static double Arounding(double x, double santim)
        {
            double around;
            //x *= 100;
            if (x % santim < (double)santim)
            {
                around = (x - x % santim); // 100;
                return around;
            }
            else
            {
                around = (x - x % santim + santim); // 100;
                return around;
            }
        }

        public static string GameBox_Paint(ref _const c, ref double offset)
        {
            string boxsource = "pack://application:,,,/Objects/Boxes/" + c.type + "/box_neon_";
            if (c.width == 3.0)
            {
                ///box.HorizontalAlignment = HorizontalAlignment.Left;
                boxsource += "3";
            }
            else
            {
                boxsource += "4";
            }

            if (c.age == "child")
            {
                offset = c.fieldVerticalOffset;
                boxsource += "-2";
            }
            else
            {
                boxsource += "-3";
            }
            boxsource += ".png";
            return boxsource;
            //C:\Users\Cheers!\Desktop\temp\ClimBall\aerocock\Objects\Boxes\versus\box_neon_3-3.png
        }

    }
    /*
    public class Events
    {

    }*/
}
