using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tutorialBot.modules
{
    class ReadAThon
    {
        private static DateTime startReadAThon;
        private static DateTime endReadAThon;
        public static Sprint currentSprint = new Sprint();

        public void start(DateTime start, int duration)
        {
            startReadAThon = start;
            endReadAThon = start.AddDays(duration); 
        }

        public void startSprint(int duration)
        {
            DateTime startTime = new DateTime();
            currentSprint = new Sprint(startTime, duration);
        }
    }
}
