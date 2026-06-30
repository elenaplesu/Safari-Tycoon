using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Safari
{
    public abstract class Obstacle
    {
        private int size; 
        private int x;
        private int y;
    }

    public class Rock : Obstacle { }

    public class WaterSource : Obstacle { }


    public class River : WaterSource { }

    public class Pond : WaterSource { }

}
