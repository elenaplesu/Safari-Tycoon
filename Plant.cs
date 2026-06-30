using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safari
{
    public abstract class Plant
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    public class Grass : Plant
    {
        public Grass(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    public class Bush : Plant
    {
        public Bush()
        {

        }
    }

    public class Tree : Plant
    {
        public Tree()
        {
        }
    }
}
