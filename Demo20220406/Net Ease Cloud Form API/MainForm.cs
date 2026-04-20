using Net_Ease_Cloud_Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Threading;
using Pic_API_Model;
using System.IO;
using System.Diagnostics;

namespace Net_Ease_Cloud_Form_API
{
    public partial class MainForm : Form
    {
        Login_Cellphone_Model loginInfo = new Login_Cellphone_Model();
        Dictionary<string, string> dicMusics = new Dictionary<string, string>();
        Thread thread;
        List<PicUrl_Init> picUrls = new List<PicUrl_Init>();
        private int Music_DownCount = 0, Pic_DownCount = 0;
        HttpHelper helper = null;
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            helper = new HttpHelper();
            loginInfo = LoginForm.loginInfo;
            if (loginInfo != null && loginInfo.code == "200")
            {
                pictureBox1.ImageLocation = loginInfo.profile.avatarUrl;
                label1.Text = loginInfo.profile.nickname;
                label2.Text = loginInfo.profile.signature;
            }
            picUrls = Common.PicUrlInit();
            PicBind();
            RandomMainPic();
        }
        private void 退出登陆ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string logoutpara = Common.GetWithTimeStamp(loginInfo.account.id);
            string logoutres = Common.HttpGet(Const.WYY_WebUrl, "/logout" + logoutpara, loginInfo.cookie);
            Logout_Model logout = JsonConvert.DeserializeObject<Logout_Model>(logoutres);
            #region 验证是否成功退出登录(比较繁琐)：先调用“退出登录”接口之后再调用“刷新登陆状态”接口，来确保已经退出登录
            //string refreshpara = Common.GetWithTimeStamp(loginInfo.account.id, DateTime.Now.AddSeconds(1));
            //string refreshres = Common.HttpGet(Const.WYY_WebUrl, "/login/refresh" + refreshpara, loginInfo.cookie);
            //Login_Refresh_Model login_Refresh = JsonConvert.DeserializeObject<Login_Refresh_Model>(refreshres);
            //if (logout.code.Equals("200") && login_Refresh.msg.Equals("需要登录") && login_Refresh.code.Equals("301"))
            #endregion
            if (logout.code.Equals("200"))
            {
                MessageBox.Show("退出登陆成功！");
                this.Close();
                new LoginForm().Show();
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show($"没有' '的歌曲信息");
                return;
            }
            string res = Common.HttpGet(Const.WYY_WebUrl, "/search?keywords=" + textBox1.Text.Trim(), "");
            Search_Model searchModel = JsonConvert.DeserializeObject<Search_Model>(res);
            dicMusics.Clear();
            if (searchModel.code == "200")
            {
                if (searchModel.result == null || searchModel.result.songs == null)
                {
                    MessageBox.Show($"没有'{textBox1.Text}'的歌曲信息");
                    return;
                }
            }
            searchModel.result.songs.ForEach(t =>
            {
                if (!dicMusics.Keys.Contains(t.id))//没有相同的歌曲ID的才添加
                {
                    dicMusics.Add(t.id, t.name + "-" + string.Join("&", t.artists.Select(o => o.name)));
                }
            });
            listBox1.DisplayMember = "Value";
            listBox1.ValueMember = "Key";
            listBox1.DataSource = new BindingSource(dicMusics, null);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            string songnames = string.Empty;//没有版权
            string downovers = string.Empty;//已有下载
            int nourl = 0;
            Dictionary<string, string> items = GetListBox(listBox1);
            string res = Common.HttpGet(Const.WYY_WebUrl, "/song/url?id=" + string.Join(",", items.Keys), loginInfo.cookie);//调用获取URL接口
            Song_Url_Model song_Url = JsonConvert.DeserializeObject<Song_Url_Model>(res);//返回值存入Model
            Dictionary<string, string> nameurls = new Dictionary<string, string>();//存储 名称和url路径
            song_Url.data.ForEach(t =>
            {
                if (string.IsNullOrEmpty(t.url))//如果没有版权
                {
                    if (string.IsNullOrEmpty(songnames))
                    {
                        songnames = items[t.id];
                    }
                    else
                    {
                        songnames += "\n" + items[t.id];
                    }
                    nourl++;
                }
                else
                {
                    if (!nameurls.Keys.Contains(items[t.id]))
                    {
                        nameurls.Add(items[t.id], t.url);
                    }
                    else
                    {
                        downovers += "\n" + items[t.id];
                    }
                }
            });
            if (!string.IsNullOrEmpty(songnames))
            {
                MessageBox.Show(songnames + $"\n共{items.Count}首选中的歌曲，其中{nourl}首歌曲暂无版权！");
            }
            if (!string.IsNullOrEmpty(downovers))
            {
                MessageBox.Show(downovers + "\n这些选中的歌曲已有下载！");
            }
            if (Music_DownCount > 50)
            {
                MessageBox.Show("下载队列已经超过50条首了，请进行等待！");
                return;
            }
            if (nameurls.Count > 0)
            {
                Music_DownCount += nameurls.Count;
                label3.Text = Music_DownCount + "首歌正在下载...";
                Task.Run(() =>
                {
                    StartDown_Task(nameurls);
                });
            }
        }
        private void button5_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> items = GetListBox(listBox1);
            if (items != null && items.Count > 0)
            {
                thread = new Thread(AddILikeFunc);
                thread.IsBackground = true;
                thread.Start(items);
            }
        }
        /// <summary>
        /// 获取ListBox的全部选中项
        /// </summary>
        /// <returns></returns>
        private Dictionary<string, string> GetListBox(ListBox listBox)
        {
            Dictionary<string, string> items = new Dictionary<string, string>();
            foreach (KeyValuePair<string, string> item in listBox.SelectedItems)//将ListBox所有选中项存入
            {
                items.Add(item.Key, item.Value);
            }
            return items;
        }
        private delegate void wt2(KeyValuePair<string, string> item, int timestamp, ref string likemusic);
        /// <summary>
        /// 下载歌曲
        /// </summary>
        /// <param name="_items"></param>
        private void StartDown_Task(Dictionary<string, string> items)
        {
            try
            {
                for (int i = 0; i < items.Count; i++)
                {
                    DownLoadSingle(items.ElementAt(i));
                }

            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Debug.WriteLine(exception.Message);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }

        private void DownLoadSingle(KeyValuePair<string, string> keyValuePair)
        {
            Common.HttpGet(keyValuePair);
            Music_DownCount--;
            Invoke(new Action(ChangeUI));//涉及到修改主线程（UI）的操作需要在子线程中调用Control.Invoke
        }
        private void ChangeUI()
        {
            if (Music_DownCount == 0)//当所有的下载项完成后修改UI
            {
                label3.Text = "下载已完成";
            }
            else
            {
                label3.Text = $"还剩{Music_DownCount}首歌正在下载";
            }
        }
        /// <summary>
        /// 添加到我喜欢
        /// </summary>
        /// <param name="_items"></param>
        private void AddILikeFunc(object _items)
        {
            Dictionary<string, string> items = (Dictionary<string, string>)_items;
            string likemusic = string.Empty;
            int timestamp = Common.GetTimeStamp(DateTime.Now);
            foreach (KeyValuePair<string, string> item in items)
            {
                timestamp++;
                wt2 wt2 = new wt2(AddLike);
                Invoke(wt2, item, timestamp, likemusic);
            }
            if (!string.IsNullOrEmpty(likemusic))
            {
                MessageBox.Show(likemusic + "\n添加至\"我喜欢\"成功！");
            }
            else
            {
                MessageBox.Show("需要登陆!");
            }
        }
        private void AddLike(KeyValuePair<string, string> item, int timestamp, ref string likemusic)
        {
            string res = Common.HttpGet(Const.WYY_WebUrl, string.Format("/like/simi/playlist?id={0}&timestamp={1}", item.Key, timestamp), loginInfo.cookie);
            Like_Music_Model like_Music = JsonConvert.DeserializeObject<Like_Music_Model>(res);
            if (like_Music.code == "200")
            {
                if (string.IsNullOrEmpty(likemusic))
                {
                    likemusic = item.Value;
                }
                else
                {
                    likemusic += "\n" + item.Value;
                }
            }
        }
        private void 日常签到ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string res = Common.HttpGet(Const.WYY_WebUrl, "/daily_signin", loginInfo.cookie);
            Daily_Signin_Model yunbei_Sign = JsonConvert.DeserializeObject<Daily_Signin_Model>(res);
            if (yunbei_Sign.code == "200")
            {
                MessageBox.Show("签到成功！\n经验+" + yunbei_Sign.point);
            }
            else
            {
                MessageBox.Show(yunbei_Sign.msg);
            }
        }

        private void 云贝签到ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string res = Common.HttpGet(Const.WYY_WebUrl, "/yunbei/sign", loginInfo.cookie);
            Yunbei_Sign_Model yunbei_Sign = JsonConvert.DeserializeObject<Yunbei_Sign_Model>(res);
            if (yunbei_Sign.code == "200")
            {
                MessageBox.Show("签到成功！\n云贝+" + yunbei_Sign.point);
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                PicBind();
                RandomMainPic();
            }
        }
        /// <summary>
        /// combobox绑定
        /// </summary>
        private void PicBind()
        {
            if (picUrls != null)
            {
                comboBox1.DataSource = null;
                comboBox1.DataSource = picUrls;
                comboBox1.DisplayMember = "Name";
                comboBox1.ValueMember = "Url";
            }
        }
        /// <summary>
        /// 主页随机图片
        /// </summary>
        private void RandomMainPic()
        {
            #region 从文件夹获取随机图片
            string path = Common.GetPath("Image");
            string[] imgspath = Directory.GetFiles(path, "*.jpg");
            int random_index = new Random().Next(0, imgspath.Length);
            if (imgspath.Length > 0)
            {
                Anime_Pic.ImageLocation = imgspath[random_index];
            }
            #endregion
            #region 从网络URL加载图片 网速慢可能加载慢
            //int random_index = new Random().Next(0, comboBox1.Items.Count);
            //string url = ((PicUrl_Init)comboBox1.Items[random_index]).Url.ToString();
            //pictureBox2.ImageLocation = url;
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string text = comboBox1.Text.ToString();
                string url = comboBox1.SelectedValue.ToString();
                //thread = new Thread(() => Thread_DownPic(text, url));//线程传值
                //thread.Start();
                Task.Run(() =>
                {
                    Thread_DownPic(text, url);
                });
                Pic_DownCount++;
                label4.Text = Pic_DownCount + "张图片正在下载";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void Thread_DownPic(string text, string url)
        {
            string res = string.Empty;
            string path = JsTool.GetPath("Image", text + "_" + DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".jpg");
            string cs = helper.GetWeb302URL(url);
            switch (text)
            {
                case "搏天api"://搏天api、小歪、墨天逸、樱花json格式的一样
                case "小歪":
                case "墨天逸":
                    //case "动漫星空":
                    //res = Common.HttpGet(url, "", "");
                    //AnimePic_Model animepic_Model = JsonConvert.DeserializeObject<AnimePic_Model>(res);
                    //Common.HttpGet(animepic_Model.imgurl, text, pictureBox2);//json模式
                    //Common.HttpGet(url, text, Anime_Pic);//流模式
                    //image.Save(path);
                    
                    helper.GetAndGetFile(url, null, null, path);
                    Anime_Pic.ImageLocation = path;
                    break;
                case "岁月小筑":
                    //res = Common.HttpGet(url, "", "");
                    //XJH_Model xjh_Model = JsonConvert.DeserializeObject<XJH_Model>(res);
                    //Common.HttpGet("https:" + xjh_Model.img, text, pictureBox2);//json模式
                    //Common.HttpGet(url, text, Anime_Pic);//流模式
                    helper.GetAndGetFile(url, null, null, path);
                    Anime_Pic.ImageLocation = path;
                    break;
                case "韩小韩":
                    //res = Common.HttpGet(url, "", "");
                    //VVHan_Model vvhan_Model = JsonConvert.DeserializeObject<VVHan_Model>(res);
                    //Common.HttpGet(vvhan_Model.imgurl, text, pictureBox2);//json模式
                    //Common.HttpGet(url, text, Anime_Pic);//流模式
                    helper.GetAndGetFile(url, null, null, path);
                    Anime_Pic.ImageLocation = path;
                    break;
                case "樱花":
                case "樱道"://樱道、保罗、EEE.DOG、东方Project、三秋（不稳定）只有流模式
                case "保罗":
                case "EEE.DOG":
                case "东方Project":
                case "三秋（不稳定）":
                    //Common.HttpGet(url, text, Anime_Pic);//流模式
                    helper.GetAndGetFile(url, null, null, path);
                    Anime_Pic.ImageLocation = path;
                    break;
            }
            Pic_DownCount--;
            if (Pic_DownCount == 0)
            {
                Action action = new Action(() =>
                {
                    label4.Text = "所有下载已完成";
                });
                Invoke(action);
            }
            else
            {
                Action action = new Action(() =>
                {
                    label4.Text = $"还剩{Pic_DownCount}张正在下载";
                });
                Invoke(action);
            }
        }
    }
}
