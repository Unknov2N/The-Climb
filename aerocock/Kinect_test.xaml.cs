using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using aerocock.Models;
using Microsoft.Kinect;
using Application = System.Windows.Application;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;

//using aerocock.Game;

namespace aerocock
{
    public partial class Kinect_test : Window //Game
    {
        //побороть различные базовые классы
        private KinectSensor _sensor = KinectSensor.GetDefault();

        private Body[] bodies;
        private BodyFrameReader bodyFrameReader;
        public _const c;
        private double coeffAspectRatio, offset;
        public WriteableBitmap colorbitmap;
        private ColorFrameReader colorReader;
        private Ellipse[,] ellMatrix = new Ellipse[24, 24];
        private FrameDescription frameDescription;

        private KinectSensor kinect = KinectSensor.GetDefault();

        public int pWidth, pHeight; //, c.vsMaxWins;//VSmaxscore VSmaxwins

        public Kinect_test(ref _const c)
        {
            this.c = c;
            InitializeComponent();
        }


        private async void ColorReader_FrameArrived(object sender, ColorFrameArrivedEventArgs e)
        {
            using (var colorFrame = e.FrameReference.AcquireFrame())
            {
                if (colorFrame != null)
                {
                    colorbitmap.Lock();
                    colorFrame.CopyConvertedFrameDataToIntPtr(colorbitmap.BackBuffer,
                        (uint) (colorFrame.FrameDescription.Width * colorFrame.FrameDescription.Height * 4),
                        ColorImageFormat.Bgra);
                    colorbitmap.AddDirtyRect(new Int32Rect(0, 0, colorFrame.FrameDescription.Width,
                        colorFrame.FrameDescription.Height));

                    colorbitmap.Unlock();
                }
            }
        }

        private void SensorInit()
        {
            if (_sensor == null)
            {
                MessageBox.Show($"{nameof(_sensor)}==Кинект не подключен");
                //Close();
                return;
            }

            //colorReader = kinect.ColorFrameSource.OpenReader();
            bodyFrameReader = _sensor.BodyFrameSource.OpenReader();

            //frameDescription = kinect.ColorFrameSource.FrameDescription;
            //colorbitmap = new WriteableBitmap(frameDescription.Width, frameDescription.Height, 96, 96, PixelFormats.Bgra32, null);
            //colorReader.FrameArrived += ColorReader_FrameArrived;
            bodyFrameReader.FrameArrived += BodyFrameReader_FrameArrived;
            //this.PictureBoxForKinect.Source = colorbitmap;            
            //PictureBoxForKinect.Stretch = Stretch.Uniform;
            _sensor.Open();
        }

        private void BodyFrameReader_FrameArrived(object sender, BodyFrameArrivedEventArgs e)
        {
            var dataReceived = false;

            using (var bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null) bodies = new Body[bodyFrame.BodyCount];
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }

            if (dataReceived)
            {
                CanvaForKinect.Children.Clear();
                var PlayerColor = Brushes.Azure;
                foreach (var body in bodies)
                    if (body.IsTracked)
                    {
                        if (body.Joints[JointType.SpineMid].Position.X > 0)
                            PlayerColor = Brushes.Red;
                        foreach (var jointType in body.Joints.Keys)
                            switch (jointType)
                            {
                                case JointType.HandTipLeft:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.HandTipRight:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.FootLeft:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                case JointType.FootRight:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                                default:
                                    SetJointToCanva(body.Joints[jointType], PlayerColor);
                                    break;
                            }
                    }
            }
        }

        private void SetJointToCanva(Joint point, SolidColorBrush color)
        {
            double x =
                    -point.Position
                        .X, // * 3.7 / point.Position.Z,//*/ -Instructions.Arounding(point.Position.X, 0.125),//125),
                y = point.Position
                    .Y; // * 3.7 / point.Position.Z;//*/ Instructions.Arounding(point.Position.Y, 0.125);// 125);
            //if (c.Matrix[(int)(-x * 8 + CanvaForKinect.Height / 50), (int)(24 - (-y * 8 + Canva.Height / 50))] != 0)
            //if (c.Matrix[(int)(-x * 8 + 12), (int)(24 - (-y * 8 + 12))] != 0)
            if (true)
            {
                Ball PointBall;
                if (color == Brushes.Red)
                    PointBall = new Ball(c.radius, pHeight, pWidth, coeffAspectRatio,
                        "pack://application:,,,/Objects/Balls/Discs/red.png");
                else
                    PointBall = new Ball(c.radius, pHeight, pWidth, coeffAspectRatio,
                        "pack://application:,,,/Objects/Balls/Discs/blue.png");
                PointBall.Setpos(CanvaForKinect, x, y);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SensorInit();
            if (c.age == "child") offset = c.fieldVerticalOffset;

            if (Screen.AllScreens.Length == 2)
            {
                pHeight = (int) (double) Screen.AllScreens[1].WorkingArea.Height;
                pWidth = (int) (c.width / c.height * Screen.AllScreens[1].WorkingArea.Height);
            }
            else
            {
                pHeight = (int) SystemParameters.PrimaryScreenHeight;
                pWidth = (int) (c.width / c.height * SystemParameters.PrimaryScreenHeight);
            }

            coeffAspectRatio = pHeight / c.height;
            CanvaForKinect.Width = pWidth;
            CanvaForKinect.Height = pHeight;

            //pictureBox_Paint();

            Canva.Width = pWidth;
            Canva.Height = pHeight;
            rect.Width = pHeight * 10 / 9;
            rect.Height = rect.Width;
            Canvas.SetLeft(rect, pWidth / 10.0);
            Canvas.SetLeft(rect, pHeight / 10.0);
            //rect.StrokeThickness = 2;
            //PictureBoxForKinect.Width = pHeight * frameDescription.Width / frameDescription.Height;
            Canvas.SetLeft(PictureBoxForKinect, -(PictureBoxForKinect.Width - pHeight) / 2);
            var center = new Ellipse
            {
                Width = 10,
                Height = 10,
                Fill = Brushes.Aqua
            };
            //Ball temp = new Ball(0.075, Brushes.Green, pHeight, pWidth, coeffAspectRatio);
            //temp.setpos(0, 0);
            //temp.paint();
            //Canva.Children.Add(temp.ell);
            Canvas.SetLeft(center, pWidth / 2 - 5);
            Canvas.SetTop(center, pHeight / 2 - 5);
            //Canva.Children.Add(center);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Q)
            {
                if (_sensor != null)
                    _sensor.Close();
                foreach (Window w in Application.Current.Windows)
                    w.Close();

                if (e.Key == Key.V)
                {
                    if (_sensor != null)
                        _sensor.Close();
                    Close();
                }
            }
        }
    }
}