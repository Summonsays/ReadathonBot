using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutorialBot.modules
{
    internal class Sprint
    {
        private static DateTime startSprint;
        private static DateTime endSprint;

        public Sprint(){}

        public Sprint(DateTime startTime, int duration)
        {
            startSprint = startTime;
            endSprint = startTime.AddMinutes(duration); ;
        }

    }
}
