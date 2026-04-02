using System;
using tagIndex;

namespace dbMgmt
{
    public class Program
    {
        public record SongInfoDisp(string Title, string Artist, string Album, string path, TimeSpan duration, int id, bool isSelected);
        static public List<SongInfoDisp> localDbDisp = new List<SongInfoDisp>();

        static public void updateIsSelected(int id, bool isSelect)
        {
            var index = localDbDisp.FindIndex(x => x.id == id);

            if (index != -1)
            {
                localDbDisp[index] = localDbDisp[index] with { isSelected = isSelect };
            }
        }
    }
}
