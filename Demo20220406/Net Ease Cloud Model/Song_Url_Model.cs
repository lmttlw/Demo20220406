using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net_Ease_Cloud_Model
{
    /// <summary>
    /// 歌曲的URL信息
    /// </summary>
    public class Song_Url_Model
    {
        public List<SongData> data;
        public string code;
    }
    public class SongData
    {
        public string id;// 472194327
        public string url;// http://m7.music.126.net/20220408152720/b3d07f56f103caa1c9101e06c96babfd/ymusic/099e/55dd/d265/c2664802fc14106bb145fc5d23bd6425.mp3
        public string br;// 320000
        public string size;// 11971440
        public string md5;// c2664802fc14106bb145fc5d23bd6425
        public string code;// 200
        public string expi;// 1200
        public string type;// mp3
        public string gain;// 0
        public string fee;// 0
        public string uf;// null
        public string payed;// 0
        public string flag;// 0
        public string canExtend;// false
        public SongFreeTrialInfo freeTrialInfo;// null
        public string level;// exhigh
        public string encodeType;//mp3
        public SongFreeTrialPrivilege freeTrialPrivilege;// {}
        public SongFreeTimeTrialPrivilege freeTimeTrialPrivilege;// {}
        public string urlSource;// 0
    }
    public class SongFreeTrialInfo
    {
        public string start;
        public string end;
    }
    public class SongFreeTrialPrivilege
    {
        public string resConsumable;
        public string userConsumable;
        public string listenType;
    }
    public class SongFreeTimeTrialPrivilege
    {
        public string resConsumable;
        public string userConsumable;
        public string type;
        public string remainTime;
    }
}
