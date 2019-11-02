using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace 文件监控系统
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FolderBrowserDialog fbd;
        private void btnSelect_Click(object sender, EventArgs e)
        {
            fbd = new FolderBrowserDialog();
            DialogResult result =  fbd.ShowDialog();//打开模式窗体  线程停止在这个地方
            if(result==DialogResult.OK)
            {
                txtFilePath.Text= fbd.SelectedPath;
                //先把指定路径磁盘路径读取

                TreeNode rootnode = new TreeNode(); //创建根节点
                string nodeName = this.txtFilePath.Text;
                rootnode.Text = nodeName.Substring(nodeName.LastIndexOf('\\')+1);//字符串截取
                rootnode.Tag = this.txtFilePath.Text;//完整路径

                this.tvFolders.Nodes.Add(rootnode);    //添加文件夹目录

                //循环递归读取
                loadFolder(rootnode);
            }
            //this.tvFolders.ExpandAll();//展开所有的树节点
        }
        /// <summary>
        /// 递归加载文件夹
        /// </summary>
        /// <param name="childnode"></param>
        private void loadFolder(TreeNode childnode)
        {
            try
            {
                //Directory 静态类 提供物理文件操作方法
                DirectoryInfo directoryInfo = new DirectoryInfo(childnode.Tag.ToString());//非静态类 提供读取文件详细信息
                DirectoryInfo[] list = directoryInfo.GetDirectories();

                foreach (DirectoryInfo di in list)
                {
                    TreeNode tn = new TreeNode();
                    tn.Text = di.Name;
                    tn.Tag = di.FullName;//全路径
                    childnode.Nodes.Add(tn);//添加到上一级节点
                    loadFolder(tn); //循环调用
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
        /// <summary>
        /// 节点发生改变是更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvFolders_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.lvFiles.Items.Clear();  //清除之前选中项
            TreeNode selectNode = this.tvFolders.SelectedNode; //选中节点
            loadFiles(selectNode.Tag.ToString()); //获取文件夹的全路径
        }
        /// <summary>
        /// 加载物理文件信息
        /// </summary>
        /// <param name="path"></param>
        private void loadFiles(string path)
        {
            try
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(path);
                FileInfo[] fileInfos = directoryInfo.GetFiles();//获取当前文件的所有文件
                foreach (FileInfo fileInfo in fileInfos)
                {
                    ListViewItem lvi = new ListViewItem(fileInfo.Name);//主文本项
                    long size = fileInfo.Length > 1024 ? fileInfo.Length / 1024 : 1;
                    lvi.SubItems.AddRange(new string[] {
                    fileInfo.Extension,
                    fileInfo.FullName,
                    size.ToString()
                });
                    lvi.Tag = fileInfo;//附加值绑定对象
                    this.lvFiles.Items.Add(lvi);
                    //添加到父容器
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
               
            
        }

        private void tvFolders_MouseDown(object sender, MouseEventArgs e)
        {
            if(e.Button==MouseButtons.Right)
            {
                TreeNode tn = this.tvFolders.GetNodeAt(e.X, e.Y);
                if(tn!=null)
                {
                    this.tvFolders.SelectedNode = tn;
                }
            }
        }

        private ListViewItem selectItem;
        /// <summary>
        /// 列表控件显示事件(选中)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvFiles_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(this.lvFiles.SelectedItems.Count>0)
            selectItem = this.lvFiles.SelectedItems[0];//默认选中下标0 即第一个
        }
        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiOpen_Click(object sender, EventArgs e)
        {
            //文件打开分两种 一种是通过文件流操作文件内容
            //另一种是打开进程  后缀名识别
            string filepath = selectItem.SubItems[2].Text;
            Process.Start(filepath);
        }
        /// <summary>
        /// 删除事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiDelete_Click(object sender, EventArgs e)
        {
            DialogResult result= MessageBox.Show("确定要删除？","提升",MessageBoxButtons.YesNo);
            if (result== DialogResult.Yes)
            { 
                File.Delete(selectItem.SubItems[2].Text);//不可逆转删除
                selectItem = null;//清除对象
                this.lvFiles.SelectedItems[0].Remove();//移除对象
            }
        }

        private string filePath;//保存复制或剪切路径
        private bool isDelete = false;// 复制:true 剪切：false
        /// <summary>
        /// 复制事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiCopy_Click(object sender, EventArgs e)
        {
            filePath = selectItem.SubItems[2].Text;
            isDelete = true;
        }
        /// <summary>
        /// 剪切事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiCut_Click(object sender, EventArgs e)
        {
            filePath = selectItem.SubItems[2].Text;
            isDelete = false;
        }
        /// <summary>
        /// 粘贴事件    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tsmiPaster_Click(object sender, EventArgs e)
        {
            #region 物理文件操作
            //获取文件的保存路径
            string newPath = Path.Combine( this.tvFolders.SelectedNode.Tag.ToString(),selectItem.Text);
            if(isDelete)
            {
                File.Copy(filePath,newPath,true);//复制文件，且如果同名，进行覆盖 
            }
            else
            {
                File.Move(filePath,newPath);
            }
            #endregion
            #region 界面数据操作
            FileInfo fileInfo = new FileInfo(newPath);
            ListViewItem lvi = new ListViewItem(fileInfo.Name);//主文本项
            long size = fileInfo.Length > 1024 ? fileInfo.Length / 1024 : 1;
            lvi.SubItems.AddRange(new string[] {
                    fileInfo.Extension,
                    fileInfo.FullName,
                    size.ToString()
            });
            this.lvFiles.Items.Add(lvi);
            #endregion
        }

        FileSystemWatcher fsw;
        /// <summary>
        /// 开始监控事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            fsw = new FileSystemWatcher();
            //设置监控路径
            try
            {
                fsw.Path = fbd.SelectedPath;
                fsw.IncludeSubdirectories = true;//设置是否包含子文件夹
                fsw.NotifyFilter = NotifyFilters.Size | NotifyFilters.FileName;
                fsw.Filter = "*";
                fsw.Changed += Fsw_Changed;//通讯协议
                fsw.EnableRaisingEvents = true;//设置控件启用
            }
            catch (Exception)
            {
                MessageBox.Show("请先选择文件！！！");
            }

        }
        /// <summary>
        /// 触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Fsw_Changed(object sender, FileSystemEventArgs e)
        {
            FileInfo fileInfo = selectItem.Tag as FileInfo;
            MessageBox.Show("文件名：" + fileInfo);
        }

        /// <summary>
        /// 关闭监控事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStop_Click(object sender, EventArgs e)
        {
            MessageBox.Show("未发现异常，停止监控!");
        }
    }
}
