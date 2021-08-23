using WMPLib;

namespace aerocock.Models
{
    public class _const
    {
        public int fps,
            coopMaxAttempts,
            coopScoreMultiplier,
            coopWinScore,
            coopMaxTime,
            vsMaxTime,
            vsMaxScore,
            vsMaxWins,
            wordsGameTime;

        public WindowsMediaPlayer soundBackground;
        public string type, regime, speed, age;

        public double width,
            height, /*offset,*/ //real proportions 
            minVelocity,
            maxVelocity,
            multiplier,
            radius,
            gameballradius,
            fieldVerticalOffset,
            coopPointHalfWidth,
            lifeTimeMs,
            newPointsMs,
            startBlinkMs;

        public _const()
        {
            Matrix = Instructions.SetMatrix(Matrix, metrs * 100);
            fps = 30; //частота обновления экрана в игре [кадр/сек]
            width = 3; //ширина игровой области [метр]
            height = 3; //высота игровой области [метр]
            fieldVerticalOffset = 1; //значение уменьшения высоты игровой области [метр]

            //параметры игрового шара
            minVelocity = 0.4; //минимальная скорость движения, она же - при выборе скороси "легко" [метр/сек]
            maxVelocity = 1.0; //максимальная скорость движения, она же - при выборе скороси "сложно" [метр/сек]
            multiplier = 1.5; //множитель скорости после удара
            radius = 0.125; //радиус шара [метр]
            gameballradius = 0.08; //радиус игрового шара

            //задаём изначальные параметры игры (после это будет заменено на пустые поля)
            type = "words"; //тип игры (versus cooperative words)
            regime = "score"; //режим игры (на время/на очки)
            speed = "easy"; //определяет скорость движения игрового шара там, где он присутствует
            age = "adult"; //определяет высоту игровой области (взрослые/детти)

            //тип игры кооператив
            coopMaxTime = 20000; //макс время игры в режиме "на время" [секунда]
            coopMaxAttempts = 3; //макс число попыток в режиме "на очки"
            coopScoreMultiplier =
                5; //множитель числа очков при выбивании очкового шара (придумано для вывода более красивых цифр на экран)
            coopWinScore = 5; //количество очков для победы в режиме "на очки" (для режима "на время" нет максимума
            coopPointHalfWidth = 0.5; //полуширина генерации очковых шаров [метр]
            lifeTimeMs = 5 * 1000; //время жизни очкового шара [миллисекунды]
            newPointsMs = 4 * 1000; //задержка появления нового очкового шара [миллисекунды]
            startBlinkMs =
                3200; //через сколько появившийся очко-шар начнёт мерцать (говорить, что скоро исчезнет) [миллисекунды]

            //тип игры версус
            vsMaxScore = 1; //число выигрышей для победы в одном сете в режиме "на очки"
            vsMaxWins = 3; //число сетов для абсолютной победы в режиме "на очки"
            vsMaxTime = 20000; //макс время игры в режиме "на время" [секунда]

            //тип  игры слова
            wordsGameTime = 13000;
            //музыка
            soundBackground = new WindowsMediaPlayer();

            /*
            System.IO.Stream music = aerocock.Resources.BackgroundMusic;
            soundBackground = new SoundPlayer(music);// "pack://application:,,,/Objects/Audio/tada.wav");//003-14 years girl.wav");//определяем аудиодорожку на фоне меню
            soundBackground.LoadAsync();*/
        }

        public int[,] Matrix { get; set; }

        public static double metrs
        {
            get => 0.125;
            set => metrs = value;
        }
    }
}