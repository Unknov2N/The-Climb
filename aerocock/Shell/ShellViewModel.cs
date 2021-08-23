using Caliburn.Micro;

namespace AeroHockey.Shell
{
    public class ShellViewModel : Conductor<IScreen>.Collection.OneActive
    {
        public ShellViewModel()
        {
            DisplayName = "AeroHockey";
        }
        
    }
}
