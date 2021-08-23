using System;
using System.IO;
using System.Text;

namespace aerocock.Models
{
    public class Instructions
    {
        public enum CountType
        {
            START,
            STOP
        }

        public static void ChangeMatrixInFile(int[,] matrix)
        {
            Directory.CreateDirectory(@"C:\TheCliff\Features\");
            var path = @"C:\TheCliff\Features\Matrix.txt";
            using (var writer = new StreamWriter(path, false, Encoding.Default))
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    for (var i = 0; i < matrix.GetLength(1); i++) writer.Write(matrix[i, j] + " ");
                    writer.WriteLine();
                }
            }
        }

        public static int[,] CreateMatrix(double santim)
        {
            var path = @"C:\TheCliff\Features\";

            Directory.CreateDirectory(path);
            using (var writer = new StreamWriter(path + "Matrix.txt", false, Encoding.Default))
            {
                for (var j = 0; j < 300 / santim; j++)
                {
                    for (var i = 0; i < 300 / santim; i++) writer.Write("0 ");
                    writer.WriteLine();
                }
            }

            var matrix = new int[(int) (300 / santim), (int) (300 / santim)];
            Log.Info(path + "Создана пустая матрица");
            return matrix;
        }

        public static int[,] SetMatrix(int[,] matrix, double santim)
        {
            try
            {
                if (!File.Exists(@"C:\TheCliff\Features\Matrix.txt"))
                    throw new Exception("Отсутствует матрица в каталоге");
                matrix = new int[(int) (300 / santim) + 10, (int) (300 / santim) + 10];
                using (var reader = new StreamReader(@"C:\TheCliff\Features\Matrix.txt", Encoding.Default))
                {
                    string line;
                    for (var i = 0; (line = reader.ReadLine()) != null; i++)
                    {
                        var subline = line.Split(' ');
                        for (var j = 0; j < subline.Length - 1; j++) matrix[j, i] = int.Parse(subline[j]);
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
            if (x % santim < santim)
            {
                around = x - x % santim; // 100;
                return around;
            }

            around = x - x % santim + santim; // 100;
            return around;
        }

        public static string GameBox_Paint(ref _const c, ref double offset)
        {
            var boxsource = "pack://application:,,,/Objects/Boxes/" + c.type + "/box_neon_";
            if (c.width == 3.0)
                ///box.HorizontalAlignment = HorizontalAlignment.Left;
                boxsource += "3";
            else
                boxsource += "4";

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

        public class Log
        {
            public static void Info(string message)
            {
                var BasePath = @"C:\TheCliff\Log\"; //Environment.SpecialFolder.ApplicationData.ToString() + @"\Log\";
                var AllPath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ".txt";
                Directory.CreateDirectory(BasePath);
                {
                    using (var writer = new StreamWriter(AllPath, true, Encoding.Default))
                    {
                        writer.WriteLine(DateTime.Now + " " + message);
                    }
                }
            }

            public static void GameCounter(string message, CountType type)
            {
                var BasePath =
                    @"C:\TheCliff\GameCounter\"; //Environment.SpecialFolder.ApplicationData + @"\GameCounter\";
                var AllPath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ' ' + type + ".txt";
                Directory.CreateDirectory(BasePath);

                int? counter = null;
                try
                {
                    if (type == CountType.START)
                        using (var reader = new StreamReader(AllPath))
                        {
                            var TextForCount = reader.ReadToEnd();
                            counter = TextForCount.Split('\n').Length;
                        }
                }
                catch (Exception exc)
                {
                    counter = 1;
                    Error("[Instructions.Log.GameCounter] " + exc.Message);
                }

                using (var writer = new StreamWriter(AllPath, true, Encoding.Default))
                {
                    if (counter != null)
                        writer.WriteLine("[" + counter + "] " + DateTime.Now + " " + message);
                    else
                        writer.WriteLine(DateTime.Now + " " + message);
                }
            }

            public static void Error(string message)
            {
                var BasePath = Environment.SpecialFolder.ApplicationData + @"\Error\";
                var Allpath = BasePath + DateTime.Now.ToString().Split(' ')[0] + ".txt";
                Directory.CreateDirectory(BasePath);
                using (var writer = new StreamWriter(Allpath, true, Encoding.Default))
                {
                    writer.WriteLine(DateTime.Now.ToString() + ' ' + message);
                }
            }
        }
    }
    /*
    public class Events
    {

    }*/
}