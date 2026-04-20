using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pic_API_Model
{
    public class AnimePic_Model
    {
        //搏天api返回的Json和这个是一样的，故共用
        //{
        //"code": "200",
        //"imgurl": "https://tva4.sinaimg.cn/large/0072Vf1pgy1foxlod238zj31hc0u0nj6.jpg",
        //"width": "1920",
        //"height": "1080"
        //}
        public string code;
        public string imgurl;
        public string width;
        public string height;
    }
}
