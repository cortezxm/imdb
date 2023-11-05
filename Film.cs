using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace imdb
{
    internal class Film
    {
        public string type;
        public string url;
        public string name;
        public string year;
        public string direction;
        public string starrings;
        public string description;
        public string score;
        public string runtime;
        public Film()
        {

        }

        public Film(string type, string url, string name, string year, string starrings)
        {
            this.type = type;
            this.url = url;
            this.name = name;
            this.year = year;
            this.starrings = starrings;

        }
    }
}
