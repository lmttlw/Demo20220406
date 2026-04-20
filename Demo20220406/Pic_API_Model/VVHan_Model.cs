using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pic_API_Model
{
    public class VVHan_Model
    {
        //{
        //"success": true,
        //"imgurl": "https://static.4ce.cn/star3/origin/33eef92023890564275aa9d4c1459d28.jpg?rw=5120&rh=3200&_fileSize=566&_orientation=1",
        //"info": {
        //"width": 1920,
        //"height": 1080,
        //"type": "img"
        //}
        //}
        public string success;
        public string imgurl;
        public VVHan_info info;
        public class VVHan_info
        {
            public string width;
            public string height;
            public string type;
        }
    }

}
