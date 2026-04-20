using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net_Ease_Cloud_Model
{
    public class Recommend_Songs_Model
    {
        public string code;
        public RecommendData data;
    }
    public class RecommendData
    {
        public List<RecommendDailySongs> dailySongs;
        public List<string> orderSongs;
        public List<RecommendReasons> recommendReasons;
    }
    public class RecommendDailySongs
    {
        public string name;// サマータイムラブ,
        public string id;// 32803125,
        public string pst;// 0,
        public string t;// 0,
        public List<ReommendAr> ar;// [],
        public List<string> alia;// [],
        public string pop;// 100,
        public string st;// 0,
        public string rt;// null,
        public string fee;// 8,
        public string v;// 14,
        public string crbt;// null,
        public string cf;// ,
        public ReommendAl al;// {},
        public string dt;// 255000,
        public ReommendH h;// {},
        public ReommendM m;// {},
        public ReommendL l;// {},
        public string a;// null,
        public string cd;// 1,
        public string no;// 1,
        public string rtUrl;// null,
        public string ftype;// 0,
        public List<string> rtUrls;// [],
        public string djId;// 0,
        public string copyright;// 0,
        public string s_id;// 0,
        public string mark;// 9007199255011328,
        public string originCoverType;// 1,
        public string originSongSimpleData;// null,
        public string single;// 0,
        public string noCopyrightRcmd;// null,
        public string rtype;// 0,
        public string rurl;// null,
        public string mst;// 9,
        public string cp;// 7003,
        public string mv;// 0,
        public string publishTime;// 1435075200000,
        public string reason;// 根据你可能喜欢的单曲 ヒロイン育成計画public string ,
        public ReommendPrivilege privilege;// {},
        public string alg;// rt_itembased
    }
    public class RecommendReasons
    {
        public string songId;
        public string reason;
    }
    public class ReommendAr
    {
        public string id;// 1015381,
        public string name;//Shiggy Jr.,
        public List<string> tns;//[],
        public List<string> alias;//[]
    }
    public class ReommendAl
    {
        public string id;// 3171183
        public string name;// サマータイムラブ
        public string picUrl;//http://p3.music.126.net/9SEnfKIvSPpXYdu_mM_azw==/7960464185727595.jpg
        public List<string> tns;// []
        public string pic;// 7960464185727595
    }
    public class ReommendH
    {
        public string br;
        public string fid;
        public string size;
        public string vd;
    }
    public class ReommendM
    {
        public string br;
        public string fid;
        public string size;
        public string vd;
    }
    public class ReommendL
    {
        public string br;
        public string fid;
        public string size;
        public string vd;
    }
    public class ReommendPrivilege
    {
        public string id;// 32803125,
        public string fee;// 8,
        public string payed;// 0,
        public string st;// 0,
        public string pl;// 128000,
        public string dl;// 0,
        public string sp;// 7,
        public string cp;// 1,
        public string subp;// 1,
        public string cs;// false,
        public string maxbr;// 320000,
        public string fl;// 128000,
        public string toast;// false,
        public string flag;// 260,
        public string preSell;// false,
        public string playMaxbr;// 320000,
        public string downloadMaxbr;// 320000,
        public string rscl;// null,
        public RecommendFreeTrialPrivilege freeTrialPrivilege;// {},
        public List<RecommendChargeInfoList> chargeInfoList;// []
    }
    public class RecommendFreeTrialPrivilege
    {
        public string resConsumable;
        public string userConsumable;
    }
    public class RecommendChargeInfoList
    {
        public string rate;
        public string chargeUrl;
        public string chargeMessage;
        public string chargeType;
    }
}
