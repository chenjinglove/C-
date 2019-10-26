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
    public partial class StudentListForm : Form
    {
        //将构造方法私有
        private StudentListForm()
        {
            InitializeComponent();
        }

        private static StudentListForm studentListForm;
        /// <summary>
        /// 创建实例 目的 使这个窗口只能被new一次
        /// </summary>
        /// <returns></returns>
        public static StudentListForm CreateInstance()
        {
            if (studentListForm == null||studentListForm.IsDisposed)//判断是否为空和被释放
            {
                studentListForm = new StudentListForm();
            }
            return studentListForm;
        }

        private void StudentListForm_Load(object sender, EventArgs e)
        {
            InitData();
        }
        /// <summary>
        /// 掉用下之前的代码  -- 给下拉框赋初值
        /// </summary>
        private void InitData()
        {
            //初始化下拉框信息
            string sql = "select Classid,ClassName from ClassInfos";
            DataTable dt = DBHelper.GetDaTaTable(sql);//DataTable临时表
            //手动创建临时表数据行
            DataRow dr = dt.NewRow();//创建数据行
            dr["Classid"] = 0;
            dr["ClassName"] = "--请选择--";
            dt.Rows.InsertAt(dr, 0);

            this.cnoClassInfos.ValueMember = "Classid";//隐藏值
            this.cnoClassInfos.DisplayMember = "ClassName";//显示值
            this.cnoClassInfos.DataSource = dt;//将临时表的信息赋给下拉框
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sql = @"select stuNo 学号, stuName 姓名, c.ClassName 班级, sex 性别, telephone 联系方式, birthday 出生日期
                           from Student s
                           inner join ClassInfos c
                           on s.classId=c.Classid
                           where 1=1 ";//使用表的连接，便于查询
            //判断用户有没有输入
            sql += this.cnoClassInfos.SelectedValue.ToString() != "0" ? " and c.classId=@classId " : "";
            sql += this.txtName.Text.Trim() != "" ? " and stuName like @stuName " : "";//实现姓名模糊查询
            sql += this.txtStoNo.Text.Trim() != "" ? " and stuNo=@stuNo " : "";

            SqlParameter[] paras ={
                new SqlParameter("@classId",this.cnoClassInfos.SelectedValue),
                new SqlParameter("@stuName",this.txtName.Text.Trim()+"%"),
                new SqlParameter("@stuNo",this.txtStoNo.Text.Trim())
            };

            this.dgvStudents.DataSource = DBHelper.GetDaTaTable(sql, paras);
        }
    }
}
