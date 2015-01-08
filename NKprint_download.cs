﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using System.Threading;
using System.Net;
using System.IO;
namespace NKprint
{
    public partial class NKprint_download : Form
    {
        List<ToJsonMy> jsonList = new List<ToJsonMy>();  //json文件列表
        List<ToJsonMy> tempList = new List<ToJsonMy>();  //json文件列表
        //下载文件的url
        private static string download_url = @"http://newfuture-uploads.stor.sinaapp.com/";
        public static string studentNum;
        //定义用来存储已操作的文件 
        //static ArrayList myDown = new ArrayList();
        //static ArrayList myPrinting = new ArrayList();
        //static ArrayList myPrinted = new ArrayList();
        //static ArrayList myPay = new ArrayList();
        //定义接收从NKprint_login窗体传值的参数
        public string downloadToken;
        public string printerName;
        public string printerId;

        //窗体类的构造函数
        public NKprint_download()
        {
            /*this.Hide();
            NKprint_login nForm = new NKprint_login();
            if (nForm.ShowDialog() != DialogResult.OK)    //判断登录窗体返回值
            {
                Environment.Exit(Environment.ExitCode);    //退出程序
                return;
            }
            nForm.Owner = this;
            nForm.Show();
            if (nForm.ShowDialog() != DialogResult.OK)    //判断登录窗体返回值
            {
                Environment.Exit(Environment.ExitCode);    //退出程序
                this.Show();
            }*/
            InitializeComponent();
            // 在现在线程和UI线程出问题的时候false一下。。。。。。
            //System.Windows.Forms.Control.CheckForIllegalCrossThreadCalls = false;
        }
        private void NKprint_download_Load(object sender, EventArgs e)
        {
            labelWelcom.Text = "你好："+printerName+"\n欢迎登陆!";
            if(!File.Exists("json.sjc"))
            {
                File.Create("json.sjc");
            }
            //登陆窗体后自动 刷新下载列表
            myRefresh();
        }
        private void labelWelcom_Click(object sender, EventArgs e)
        {
        }
        
        //手动刷新下载列表刷新下载列表
        private void buttonRefresh_Click(object sender, EventArgs e)
        {
            if (textDownload.Text == string.Empty)
            {
                MessageBox.Show("请输入查询id");
            }
            else
            {
                String Date = (DateTime.Now.ToShortDateString());
                String path = @"D:\" + Date;
                bool flag=false ;
                string filename ;
                for (int i = 0; i < jsonList.Count;i++ )
                {
                    if (jsonList[i].id == textDownload.Text)
                    {
                        flag = true;
                        filename = path+"\\"+jsonList[i].id + "_" + jsonList[i].copies + "份" + "_" + (jsonList[i].double_side == "0" ? "单面" : "双面") + "_" + jsonList[i].student_number + "_" + jsonList[i].name;
                        if (File.Exists(@filename))
                        {
                            //filename = path + filename;
                            System.Diagnostics.Process.Start( filename);
                            break;
                        }
                        else
                        {
                            filename = jsonList[i].id + "_" + jsonList[i].copies + "份" + "_" + (jsonList[i].double_side == "0" ? "单面" : "双面") + "_" + jsonList[i].student_number + "_" + jsonList[i].name;
                            fileDownload(jsonList[i].url, filename, i);
                            MessageBox.Show("正在下载输入id对应的文件！\n等待会儿再打开");
                        }
                        break;
                    }
                    
                }
                if (flag == false)
                    MessageBox.Show("未查询到" + textDownload.Text+"\n确认输入正确？");
            }
        }
        //jsonList中添加数据
        public void addJson(JArray ja)
        {
            int i = 0;//用于遍历的
            int a=jsonList.Count;
            for (i = 0; i < ja.Count; i++)//遍历ja数组
            {
                
                if(a>0)
                {
                    if(int.Parse(jsonList[jsonList.Count-1].id)<int.Parse(ja[i]["id"].ToString()))
                    {

                            ToJsonMy myJs = new ToJsonMy();
                            myJs.id = ja[i]["id"].ToString();
                            myJs.name = ja[i]["name"].ToString();
                            myJs.use_id = ja[i]["use_id"].ToString();
                            myJs.pri_id = ja[i]["pri_id"].ToString();
                            myJs.url = ja[i]["url"].ToString();
                            myJs.time = ja[i]["time"].ToString();
                            myJs.name = ja[i]["name"].ToString();
                            myJs.status = ja[i]["status"].ToString();
                            myJs.copies = ja[i]["copies"].ToString();
                            myJs.use_name = ja[i]["use_name"].ToString();
                            myJs.double_side = ja[i]["double_side"].ToString();
                            myJs.student_number = ja[i]["student_number"].ToString();
                            //如果已经付款则没有意义，不添加到jsonList 中
                            if (myJs.status != "5")
                            {
                                jsonList.Add(myJs);
                            }
                    }
                }
                else
                {
                           ToJsonMy myJs = new ToJsonMy();
                            myJs.id = ja[i]["id"].ToString();
                            myJs.name = ja[i]["name"].ToString();
                            myJs.use_id = ja[i]["use_id"].ToString();
                            myJs.pri_id = ja[i]["pri_id"].ToString();
                            myJs.url = ja[i]["url"].ToString();
                            myJs.time = ja[i]["time"].ToString();
                            myJs.name = ja[i]["name"].ToString();
                            myJs.status = ja[i]["status"].ToString();
                            myJs.copies = ja[i]["copies"].ToString();
                            myJs.use_name = ja[i]["use_name"].ToString();
                            myJs.double_side = ja[i]["double_side"].ToString();
                            myJs.student_number = ja[i]["student_number"].ToString();
                            //如果已经付款则没有意义，不添加到jsonList 中
                            if (myJs.status != "5")
                            {
                                jsonList.Add(myJs);
                            }
                
                }
                
            }
        }
        //刷新下载列表函数
        public void  myRefresh()
        {
            jsonList = remember.ReadJsonFileToList("json.sjc");
            //将API中的static赋值1；myPage是在json下载时的页码，每次重新访问的时候要置为1；
            API.myPage = 1;
            //得到json格式的文件列表
            string myJsFile = API.doGetMethodToObj(downloadToken);
#if DEBUG
            Console.WriteLine(myJsFile);
#endif
            
            JObject jo = JObject.Parse(myJsFile); 
            JArray ja = jo["files"] as JArray;
            //将JArray类型的ja转化为ToMyJohn对象数组          
            addJson(ja);
            bool myAdd=(ja.Count==20);
            while (myAdd)
            {
                myJsFile = API.doGetMethodToObj(downloadToken);
                jo = JObject.Parse(myJsFile);
                ja = jo["files"] as JArray;
                if (ja.Count != 0)
                {
                    addJson(ja);
                }
                if (ja.Count < 20)
                    myAdd = false;
                else
                    myAdd = true;
            }
            //每次json获取完成，  
            //已经将得到的文件列表保存到类型（list<yoJsonMy>）jsonList中
            //显示所得到的的文件列表，而且要显示的是状态
            //定义文件的初始状态
            string id1, userName1, fileName1, copies1, doubleSides1, statues1 = null;
            int i = 0;//用于遍历的
            for (i = 0; i < jsonList.Count; i++)
            {
                    //判断列表中的值来显示文件信息
                    id1 = jsonList[i].id;
                    userName1 = jsonList[i].student_number+jsonList[i].use_name;
                    fileName1 = jsonList[i].name;
                    copies1 = jsonList[i].copies + "份";
                    if (jsonList[i].copies == "0")//判断如果是0份，提示到现场打印
                    {
                        copies1 = "现场打印";
                    }
                    if (jsonList[i].double_side == "0")
                    {
                        doubleSides1 = "单面";
                    }
                    else
                    {
                        doubleSides1 = "双面";
                    }
                    switch (jsonList[i].status)
                    {
                        case "1":
                            statues1 = "未下载";
                            break;
                        case "2":
                            statues1 = "已下载";
                            break;
                        case "3":
                            statues1 = "正打印";
                            break;
                        case "4":
                            statues1 = "已打印";
                            break;
                        case "5":
                            statues1 = "已付款";
                            break;
                    }
                    this.myData.Rows.Add(id1, userName1, fileName1, copies1, doubleSides1, statues1);
            }             
        }
        //开启下载线程，自动下载status=1的文件
        public void threadDownload()
        {
            string downurl = null;
            string filename = null;
            //定义线程执行下载程序
            string sides = null;
            int i;
            Thread piThread1 = new Thread(delegate()
            {
                //在list中使用jsonList，遍历下载
                for (i = 0; i < jsonList.Count; i++)
                {
                    if (jsonList[i].status == "1")
                    {
                        downurl = jsonList[i].url;
                        if (jsonList[i].double_side == "0")
                        {
                            sides = "单面";
                        }
                        else
                            sides = "双面";
                        filename = jsonList[i].id + "_" + jsonList[i].copies + "份" + "_" + sides + "_" + jsonList[i].student_number + "_" + jsonList[i].name;
                        fileDownload(downurl, filename, i);
                    }
                }
            });
            piThread1.Start();
        }
        //窗体关闭事件退出当前登陆，退出应用程序
        private void NKprint_download_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
            Application.Exit();
        }
        //点击状态列，修改当前的文件状态,并post 到服务器
        private void myData_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((e.ColumnIndex + 1)==6)
            {
                //用switch
                switch (myData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString())
                {
                    case "已下载":
                        changeStatusById(e.RowIndex, "printing");
                        myData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "正打印";
                        //myPrinting.Add(myData.Rows[e.RowIndex].Cells[0].Value);
                        
                        break;
                    case "正打印":
                        changeStatusById(e.RowIndex, "printed");
                        myData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "已打印";
                        //myPrinted.Add(myData.Rows[e.RowIndex].Cells[0].Value);
                        
                        break;
                    case "已打印":
                        //要不要加判断用来确认是否post成功
                        changeStatusById(e.RowIndex, "payed");
                        myData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = "已支付";
                        //myPay.Add(myData.Rows[e.RowIndex].Cells[0].Value);
                        
                        break;
                    default:
                        break;

                }
            }            
            //MessageBox.Show("您单击的是第" + (e.RowIndex + 1) + "行第" + (e.ColumnIndex + 1) + "列！");
            //MessageBox.Show("单元格的内容是：" + myData.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
        }
        //下载从服务器得到的json数据中的用户打印文件
        public bool fileDownload(string url, string fileName, int i)
        {
            //下载文件地址等于服务器地址加上文件地址
            url = download_url + url;
            String Date = (DateTime.Now.ToShortDateString());
            String path = @"D:\" + @Date;
            //使用Directory要用到System.IO
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            WebClient webClient = new WebClient();
            String pathDoc = path + "/" + fileName;
            //添加下载完成后的事件
            webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(webClient_DownloadFileCompleted);
            try
            {
                webClient.DownloadFileAsync(new Uri(url), pathDoc, i);
            }
            catch
            {
                return false;
                //判断出错
            }
            return true;
        }
        //webClient下载完成后相应的事件，下载完成后，调用改变状态函数
        void webClient_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            int i = int.Parse(e.UserState.ToString());
            if (e.Error != null)//下载失败怎么办
            {
                myData.Rows[i].Cells[5].Value = "下载失败";
                MessageBox.Show("id="+myData.Rows[i].Cells[0].Value + "  " + e.Error.Message);   //正常捕获
                myData.Rows[i].ErrorText = "文件不存在！";
            }
            else
            {
                changeStatusByi(i, "download");
            }
        }
        public void changeStatusByi(int i, string currentStatus)
        {
            bool down, printing, printed, pay;
            down = myData.Rows[i].Cells[5].Value.Equals( "已下载");
            printing = myData.Rows[i].Cells[5].Value.Equals( "正打印");
            printed = myData.Rows[i].Cells[5].Value.Equals( "已打印");
            pay = myData.Rows[i].Cells[5].Value.Equals("已支付");
            //bool flag=!down&&!printed!printing&&!pay;
            if (!down&&!printing&&!printed&&!pay)
            {
                if (currentStatus == "download")
                    myData.Rows[i].Cells[5].Value = "已下载";
                //put到服务器状态;/api.php/File/1234?token=xxxxxxxxxxxx 
                //将下载完成的文件id添加到下载完成myDown （ArrayList）中      
                //参数： status=>文件状态'uploud','download','printing','printed','payed', 返回操作结果
                string putUrl = @"/File/" + myData.Rows[i].Cells[0].Value + "?token=" + downloadToken;
                string putPara = "status=" + currentStatus;
                string out1 = API.PostWebRequest(putUrl, putPara, new UTF8Encoding());
                Console.WriteLine(out1);
            }
        }
        //
        public void changeStatusById(int i, string currentStatus)
        {
            //put到服务器状态;/api.php/File/1234?token=xxxxxxxxxxxx 
            //将下载完成的文件id添加到下载完成myDown （ArrayList）中

            //参数： status=>文件状态'uploud','download','printing','printed','payed', 返回操作结果
            string putUrl = @"/File/" + myData.Rows[i].Cells[0].Value + "?token=" + downloadToken;
            string putPara = "status=" + currentStatus;
            string out1 = API.PostWebRequest(putUrl, putPara, new UTF8Encoding());
            Console.WriteLine(out1);
        }
        

        private void emputyData()
        {
            while (this.myData.Rows.Count != 0)
            {
                this.myData.Rows.RemoveAt(0);
            } 
        }
        //根据输入的学号定位datagridview中的文件
        private void buttonSearch_Click(object sender, EventArgs e)
        {
            bool flag = false;
            string studentNum1=string.Empty;
            if (textStudent.Text == string.Empty)
            {
                MessageBox.Show("请输入查询学号！");
            }
            else
            {
                studentNum1 = textStudent.Text;
                for (int i=0; i < myData.Rows.Count;i++ )
                {
                    if (myData.Rows[i].Cells[1].Value.ToString().Contains(studentNum1))
                    {
                        myData.Rows[i].Selected = true;
                        flag = true;
                    }
                    else
                        myData.Rows[i].Selected = false;
                }
                if (flag == false)
                    MessageBox.Show("未找到" + studentNum1);
            }
            

        }
        //打开文件下载地址
        private void 打开下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFile.open();
        }
        //退出应用程序
        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.DialogResult = DialogResult.OK;
            Application.Exit();
        }
        //显示版本信息
        private void 版本ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("云因南开打印店客户端：\n     made by NKsjc 2015.01.08。\n    欢迎交流，qq：2634329276");
        }
        ////查询打印信息
        //private void 查询打印ToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    fileDownload(@"http://bt.nankai.edu.cn/", "my.doc", 1);
        //    /*studentNum = string.Empty;
        //    NKprint_search formSearch = new NKprint_search();
        //    formSearch.Show();
        //    if(formSearch.DialogResult==DialogResult.OK)
        //    {
        //        MessageBox.Show(studentNum);
        //    }*/
        //    //string path = @"http://music.nankai.edu.cn/download.php?id=98130";// ~/文件夹/文件名
        //    //MessageBox.Show(System.IO.File.Exists(path).ToString()); 
        //    //
        //}
        //刷新datagridview的文件
        private void 刷新下载ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!File.Exists("json.sjc"))
            {
                File.Create("json.sjc");
            }
            //每次运行的时候清空datagridview中的数据
            emputyData();
            //将之前的jsonList数组清空
            jsonList.Clear();
            //刷新一次数据
            myRefresh();
            //运行下载线程
            threadDownload();
            remember.WriteJsonToTextFile(jsonList,"json.sjc");
            //把jsonList写入到txt中
        }

        private void toolTip1_Popup(object sender, PopupEventArgs e)
        {

        }
        ////本代码用于测试json写入.sjc，和重新读出
        //private void 写入jsonToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    remember.WriteJsonToTextFile(jsonList,"json.sjc");
        //    //jsonList.
        //}
        ////
        //private void 读出jsonToolStripMenuItem_Click(object sender, EventArgs e)
        //{
        //    List<ToJsonMy> newJson = new List<ToJsonMy>();
        //    newJson=remember.ReadJsonFileToList("json.sjc");
        //    MessageBox.Show(newJson[5].id+newJson[9].name);
        //}

        

    }
}
