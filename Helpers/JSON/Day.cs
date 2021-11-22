using System.Collections.Generic;

namespace TelegramOnlyBot.Helpers.JSON
{
    public class Day
    {
        public int Number { get; set; }

        public List<Lesson> Lessons { get; set; }
    }
}
