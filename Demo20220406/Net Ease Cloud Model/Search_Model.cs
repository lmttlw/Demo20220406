using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net_Ease_Cloud_Model
{
    /// <summary>
    /// 精简查询
    /// </summary>
    public class Search_Model
    {
        public SearchResult result;
        public string code;
    }
    public class SearchResult
    {
        public List<SearchSongs> songs;
        public string hasMore;
        public string songCount;
    }
    public class SearchSongs
    {
        public string id;
        public string name;
        public List<SearchArtists> artists;
        public SearchAlbum album;
        public string duration;
        public string copyrightId;
        public string status;
        public List<string> alias;
        public string rtype;
        public string ftype;
        public string mvid;
        public string fee;
        public string rUrl;
        public string mark;
    }
    public class SearchArtists
    {
        public string id;// 12085569,
        public string name;//李蚊香 ,
        public string picUrl;// null,
        public List<string> alias;// [],
        public string albumSize;// 0,
        public string picId;// 0,
        public string img1v1Url;//https://p1.music.126.net/6y-UleORITEDbvrOLV0Q8A==/5639395138885805.jpg
        public string img1v1;// 0,
        public string trans;// null
    }
    public class SearchAlbum
    {
        public string id;
        public string name;
        public SearchArtist artist;
        public string publishTime;
        public string size;
        public string copyrightId;
        public string status;
        public string picId;
        public string mark;
    }
    public class SearchArtist
    {
        public string id;// 0,
        public string name;//
        public string picUrl;// null,
        public List<string> alias;// [],
        public string albumSize;// 0,
        public string picId;// 0,
        public string img1v1Url;//https://p1.music.126.net/6y-UleORITEDbvrOLV0Q8A==/5639395138885805.jpg
        public string img1v1;// 0,
        public string trans;// null
    }
    public class SearchAlias
    {

    }
}
