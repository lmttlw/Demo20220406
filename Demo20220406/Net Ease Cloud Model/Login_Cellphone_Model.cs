using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net_Ease_Cloud_Model
{
    /// <summary>
    /// 手机号登陆
    /// </summary>
    public class Login_Cellphone_Model
    {

        public string loginType;// 1
        public string code;// 200
        public LoginAccount account;
        public string token;
        public LoginProfile profile;
        public List<LoginBindings> bindings;
        public string cookie;
    }
    public class LoginAccount
    {
        public string id;//6360696916
        public string userName; //1_15573313867
        public string type; //1
        public string status;// 0
        public string whitelistAuthority;// 0
        public string createTime;// 1649224037571
        public string salt;//[B@6d720603
        public string tokenVersion;// 1
        public string ban;// 0
        public string baoyueVersion;// 0
        public string donateVersion;// 0
        public string vipType;// 0
        public string viptypeVersion;// 0
        public string anonimousUser;// false
        public string uninitialized;//false
    }
    public class LoginProfile
    {
        public string userId;// 6360696916
        public string avatarImgIdStr;//109951165647004069
        public string backgroundImgIdStr;//109951162868128395
        public string userType;// 0
        public string accountStatus;// 0
        public string vipType;// 0
        public string gender;// 0
        public string avatarImgId;// 109951165647004060
        public string nickname;// lmttlw
        public string birthday;// -2209017600000
        public string backgroundImgId;// 109951162868128400
        public string city;//440300
        public string avatarUrl;//https://p3.music.126.net/SUeqMM8HOIpHv9Nhl9qt9w==/109951165647004069.jpg
        public string defaultAvatar;// false
        public string province;// 440000
        public string expertTags;//null
        public LoginExperts experts;// { }
        public string mutual;// false
        public string remarkName;// null
        public string authStatus;// 0
        public string djStatus;//0
        public string followed;//false
        public string backgroundUrl;// https://p2.music.126.net/2zSNIqTcpHL2jIvU6hG0EA==/109951162868128395.jpg
        public string detailDescription;
        public string description;
        public string signature;
        public string authority;//0
        public string avatarImgId_str;//109951165647004069
        public string followeds;// 0
        public string follows;// 1
        public string eventCount;// 0
        public string avatarDetail;// null
        public string playlistCount;// 2
        public string playlistBeSubscribedCount;//0
    }
    public class LoginExperts
    {

    }
    public class LoginBindings
    {
        public string url;
        public string userId; //6360696916;
        public string expired; //false;
        public string bindingTime; //1649224037588;
        public string tokenJsonStr;
        public string expiresIn; //2147483647;
        public string refreshTime; //1649224037;
        public string id;// 13579794193;
        public string type;
    }
}

