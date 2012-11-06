using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Editor.Elements
{
    public interface IActivable
    {
        bool Active
        {
            get;
            set;
        }
    }
}