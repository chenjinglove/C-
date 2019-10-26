using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace XG学习管理系统_2._0
{
    public partial class loginForm : Form
    {
        public loginForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void loginForm_Load(object sender, EventArgs e)
        {

        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            Login();
            
        }
        
        private void Login()
        {
            UserInfos userInfos = null;//存放登录人信息
            string userName = this.txtUserName.Text.Trim();//获取用户名信息，Trim去空格
            string password = this.txtPsd.Text.Trim();
            string sql = $@"select UserId, UserName, Password, sex
                          from Userinfos
                          where UserName=@userName and Password=@password";
            SqlParameter[] paras =
            {
                new SqlParameter ("@userName",userName),
                new SqlParameter ("@password",password)
            };//初始化器
            //释放资源 带异常处理机制
            SqlDataReader reader = DBHelper.ExecuteReader(sql, paras);
            
            if (reader.Read())//前进到下一条数据
            {
                userInfos = new UserInfos();
                userInfos.Userid = (int)reader["UserId"];
                userInfos.UserName = reader["UserName"].ToString();
                userInfos.Password = reader["Password"].ToString();
                userInfos.sex = (bool)reader["sex"];
            }
            reader.Close();
          
            if(userInfos!=null)
            {
                //创建主窗口对象
                MainForm mainForm = new MainForm(userInfos);
                mainForm.Show();
                this.Hide();//隐藏登录窗体
            }
            else
            {
                MessageBox.Show("登录失败！");
            }

        }
    }
}
