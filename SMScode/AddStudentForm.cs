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
    public partial class AddStudentForm : Form
    {
        public AddStudentForm()
        {
            InitializeComponent();
        }
        /// <summary>
        /// 窗体加载事件 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddStudentForm_Load(object sender, EventArgs e)
        {
            InitData();
        }
        /// <summary>
        /// 初始化信息方法
        /// </summary>
        private void InitData()
        {
            //初始化下拉框信息
            string sql = "select Classid,ClassName from ClassInfos";
            DataTable dt = DBHelper.GetDaTaTable(sql);//DataTable临时表
            //手动创建临时表数据行
            DataRow dr= dt.NewRow();//创建数据行
            dr["Classid"] = 0;
            dr["ClassName"] = "--请选择--";
            dt.Rows.InsertAt(dr, 0);

            this.cnoClassInfos.ValueMember = "Classid";//隐藏值
            this.cnoClassInfos.DisplayMember = "ClassName";//显示值
            this.cnoClassInfos.DataSource = dt;//将临时表的信息赋给下拉框
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            Addstudent();
        }
        /// <summary>
        /// 添加学生信息
        /// </summary>
        private void Addstudent()
        {
            //判断学生有没有输入姓名
            if(txtStuName.Text=="")
            {
                MessageBox.Show("请输入学生姓名！！");
                return;
            }

            //获取界面学生信息
            string classId= this.cnoClassInfos.SelectedValue.ToString();
            string stuName = this.txtStuName.Text;
            string sex = this.rbtnMan.Checked ? "男" : "女";//三元判断式
            string telephone = this.txtTelphone.Text;
            DateTime birthday = this.dtpBirthday.Value;

            string sql = $@"insert into Student (stuName, classId, sex, telephone, birthday)
                           values (@stuName, @classId, @sex, @telephone, @birthday)";

            SqlParameter[] paras =
            {
                new SqlParameter("@stuName",stuName),
                new SqlParameter("@classId",classId),
                new SqlParameter("@sex",sex),
                new SqlParameter("@telephone",telephone),
                new SqlParameter("@birthday",birthday)
            };
            int result = DBHelper.ExecuteNonQuery(sql, paras);
            if(result>0)
            {
                MessageBox.Show("添加信息成功");
                this.Close();
            }
            else
            {
                MessageBox.Show("添加失败");
            }
            //insert into Student(stuName, classId, sex, telephone, birthday)
            //value(@stuName, @classId, @sex, @telephone, @birthday)
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
