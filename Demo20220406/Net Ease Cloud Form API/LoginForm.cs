using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Net_Ease_Cloud_Model;
using Newtonsoft.Json;

namespace Net_Ease_Cloud_Form_API
{
    public partial class LoginForm : Form
    {
        public static Login_Cellphone_Model loginInfo = new Login_Cellphone_Model();
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string phone = textBox1.Text.Trim();
            string password = textBox2.Text.Trim();
            string loginpara = Common.GetWithTimeStamp(DateTime.Now);
            string res = Common.HttpGet(Const.WYY_WebUrl, string.Format("/login/cellphone?phone={0}&password={1}" + loginpara, phone, password), "");
            loginInfo = JsonConvert.DeserializeObject<Login_Cellphone_Model>(res);
            if (loginInfo.code.Equals("502"))
            {
                MessageBox.Show("密码错误");
                return;
            }
            MainForm mainForm = new MainForm();
            this.Hide();
            mainForm.Show();

        }
    }
}
