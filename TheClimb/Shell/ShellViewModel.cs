using Caliburn.Micro;

namespace TheClimb.Shell
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel()
        {
            DisplayName = "AeroHockey";
        }

        #region Modes

        public bool IsVersusMode { get; set; }

        public void OnIsVersusModeChanged()
        {
            IsCooperativeMode = false;
            IsWordsMode = false;
        }

        public bool IsCooperativeMode { get; set; }

        public void OnIsCooperativeModeChanged()
        {
            IsVersusMode = false;
            IsWordsMode = false;
        }

        public bool IsWordsMode { get; set; }

        public void OnIsWordsModeChanged()
        {
            IsVersusMode = false;
            IsCooperativeMode = false;
        }

        #endregion
    }
}