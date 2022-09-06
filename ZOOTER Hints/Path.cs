using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZOOTR_Hints
{
    class Path
    {
        public string region { get; set; }
        public string triforce { get; set; }
        public List<String> items;

        public Path()
        {
            items = new List<String>();
        }

        public Path(string Region, string Tri)
        {
            region = Region;
            triforce = Tri;
            items = new List<String>();
        }

        public Path(string Region, string Tri, List<string> already)
        {
            region = Region;
            triforce = Tri;
            items = already;
        }

        public override bool Equals(object obj)
        {
            if (obj is Path)
            {
                Path temp = (Path)obj;
                return this.region.Equals(temp.region) && this.triforce.Equals(temp.triforce);
            }
            else
                return false;
        }
    }
}
