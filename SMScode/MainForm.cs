using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XG学习管理系统_2._0
{
    public partial class MainForm : Form
    {
        private UserInfos userInfos;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(UserInfos userInfos)
        {
            InitializeComponent();
            this.userInfos = userInfos;
        }

        /// <summary>
        /// 添加学生信息事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiAddStudent_Click(object sender, EventArgs e)
        {
            //创建添加学生窗口
            AddStudentForm addStudentForm = new AddStudentForm();
            addStudentForm.MdiParent = this;
            addStudentForm.Show();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text +=$"-- 欢迎{ this.userInfos.UserName} 登录";
        }
        /// <summary>
        /// 查看学生列表 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiStudentList_Click(object sender, EventArgs e)
        {
            StudentListForm studentListForm = StudentListForm.CreateInstance();
            studentListForm.MdiParent = this;
            studentListForm.Show();
        }

        private void 横向排列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void 纵向排列ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
