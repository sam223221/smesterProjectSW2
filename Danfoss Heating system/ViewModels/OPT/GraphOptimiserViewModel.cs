using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Danfoss_Heating_system.ViewModels.OPT;

public partial class GraphOptimiserViewModel : ViewModelBase
{
    private MainWindowViewModel viewChange;

    public GraphOptimiserViewModel(MainWindowViewModel mv)
    {
        viewChange = mv;

        viewChange._window.Width = 1980;
        viewChange._window.Height = 1080;
    }


}

