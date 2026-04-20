using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Net_Ease_Cloud_Form_API
{
    public class Const
    {
        //网易云API
        public const string WYY_WebUrl = "https://netease-cloud-music-api-lmttlw.vercel.app";//网易云API

        //杂
        //public const string QQ_WebUrl = "";
        //public const string KuGou_WebUrl = "";
        //public const string EEE_WebUrl = "https://api.yimian.xyz/img?type=moe&size=1920x1080";//图片流

        //各种二次元图片API
        public const string Sakura_WebUrl = "https://www.dmoe.cc/random.php?return=json";                   //樱花（json）
        public const string Sakura2_WebUrl = "https://www.dmoe.cc/random.php";                              //樱花（流）
        //public const string TouBie_WebUrl = "https://acg.toubiec.cn/random.php?ret=json";                   //晓晴博客（json）
        //public const string TouBie2_WebUrl = "https://acg.toubiec.cn/random.php";                           //晓晴博客（流）
        public const string BoTian_WebUrl = "https://api.btstu.cn/sjbz/api.php?lx=dongman&format=json";     //搏天api（json）
        public const string BoTian2_WebUrl = "https://api.btstu.cn/sjbz/api.php?lx=dongman&format=images";  //搏天api（流）
        public const string IXiaoWai_WebUrl = "https://api.ixiaowai.cn/api/api.php?return=json";        //小歪（json）gqapi/api.php(自然)  api/api.php(二次元)
        public const string IXiaoWai2_WebUrl = "https://api.ixiaowai.cn/api/api.php";                       //小歪（流）
        public const string MTY_WebUrl = "https://api.mtyqx.cn/tapi/random.php?return=json";                //墨天逸（json）tapi/random.php->api/random.php
        public const string MTY2_WebUrl = "https://api.mtyqx.cn/tapi/random.php";                           //墨天逸（流）tapi/random.php->api/random.php
        public const string AnimeXK_WebUrl = "https://api.dongmanxingkong.com/suijitupian/acg/4k/index.php?return=json";//动漫星空（json）1080p 2k 4k
        public const string AnimeXK2_WebUrl = "https://api.dongmanxingkong.com/suijitupian/acg/4k/index.php";//动漫星空（流）1080p 2k 4k
        public const string XJH_WebUrl = "https://img.xjh.me/random_img.php?return=json&type=bg&ctype=acg";  //岁月小筑（json）
        public const string XJH2_WebUrl = "https://img.xjh.me/random_img.php?return=302&type=bg&ctype=acg";  //岁月小筑（流） type:bg(1920*1080) ctype:acg、nature(自然) 全年龄段
        //public const string Paulzzh_WebUrl = "https://img.paulzzh.com/touhou/random?type=json&size=pc";    //东方Project（json）type:302 (默认)/json 支持跨域 site:konachan(次方酱)/yandere(病娇酱)/all size:pc/wap(竖屏)/all proxy:0(源)/1(反向代理)
        public const string Paulzzh2_WebUrl = "https://img.paulzzh.com/touhou/random?type=302&size=pc&proxy=1";//东方Project（流）
        public const string VVHan_WebUrl = "https://api.vvhan.com/api/acgimg?type=json";                     //韩小韩（json）
        public const string VVHan2_WebUrl = "https://api.vvhan.com/api/acgimg";                              //韩小韩（流）


        public const string BaoLuo2_WebUrl = "https://api.paugram.com/wallpaper";                 //保罗（流）
        public const string R100862_WebUrl = "https://api.r10086.com/img-api.php?type=动漫综合6"; //樱道（流）
        public const string EEEDOG2_WebUrl = "https://api.yimian.xyz/img?type=moe&size=1920x1080&R18=true";//EEE.DOG（流）R18开关：&R18=true 
        public const string Ghser2_WebUrl = "https://api.ghser.com/random/pc.php";                //三秋（流）api/pc/pe/bg(风景)


        //文字API
        public const string IXiaoWai3_WebUrl = "https://api.ixiaowai.cn/ylapi/index.php";//小歪（一言语录）：干燥的空气，尘埃的味道，我在其中…踏上旅途
        public const string IXiaoWai4_WebUrl = "https://api.ixiaowai.cn/tgrj/index.php";//小歪（舔狗日记）：昨晚你和朋友大佬一晚上游戏，你破天荒的给我看了你的战绩，虽然我看不懂但我相信你一定是最厉害的，最棒的！我给你发了好多消息夸你，告诉你我多崇拜你，你回了我一句：啥b，我翻来覆去思考这是什么意思？sh-a傻，噢你的意思是说我傻，那b就是baby的意思了吧，原来你是在叫我傻宝，这么宠溺的语气，我竟一时不相信，其实你也是喜欢我的对吧
    }
}
