using System;
using System.Threading.Tasks;

namespace grinder.Configuration.Helper
{
    public delegate Task AsyncEventHandler(object sender, EventArgs e);

    public delegate Task AsyncEventHandler<in TEventArgs>(object sender, TEventArgs e);

}
