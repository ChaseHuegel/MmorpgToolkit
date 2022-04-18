using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MmorpgToolkit
{
    public class NpcEditor : PropertyNotifier
    {
        private ViewModel ViewModel;

        public NpcEditor(ViewModel viewModel)
        {
            ViewModel = viewModel;
        }
    }
}
