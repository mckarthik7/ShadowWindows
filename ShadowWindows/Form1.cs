using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
namespace ShadowWindows
{
    
    public partial class Form1 : Form
    {
        String pname="";
        List<Process> processes=new List<Process>();
        Thread t;
        public Form1()
        {
            InitializeComponent();
            Process[] processtemp = Process.GetProcesses();
            foreach (Process p in processtemp)
            {
                if (!String.IsNullOrEmpty(p.MainWindowTitle))
                {
                    processes.Add(p);
                    String[] row = { p.MainWindowTitle, "Overlay not added" };
                    var ListViewItem = new ListViewItem(row);
                    listView1.Items.Add(ListViewItem);
                }
            }
            t= new Thread(new ParameterizedThreadStart(RecheckThreadMethod));
            t.Start();
            this.Closed += new EventHandler(OnClosed);
        }
        public void OnClosed(Object sender,EventArgs e)
        {
            t.Suspend();
            Console.WriteLine("Closed");
        }
 
    void RecheckThreadMethod(Object o)
        {
            while(true)
            {
                try
                {
                    Thread.Sleep(1000);
                    reCheckRunningProcess();
                }
                catch(Exception e)
                {

                }
            }
        }

    void reCheckRunningProcess()
        {
           // Console.WriteLine("Rechecking");
            processes.Clear();
        this.Invoke((MethodInvoker)delegate
            {
                listView1.Items.Clear();
                Process[] tempProcess = Process.GetProcesses();
                foreach (Process p in tempProcess)
                {
                    if (!String.IsNullOrEmpty(p.MainWindowTitle))
                    {
                        processes.Add(p);
                        String[] row = { p.MainWindowTitle, "Overlay not added" };
                        var ListViewItem = new ListViewItem(row);
                        listView1.Items.Add(ListViewItem);
                    }
                }
   
            }
            
            );
           

        }

    private void listView1_SelectedIndexChanged(object sender, EventArgs e)
    {
        pname = processes.ElementAt(listView1.SelectedIndices[0]).ProcessName;
    }

    private void button1_Click(object sender, EventArgs e)
    {
        if(pname.Equals(""))
        {
            MessageBox.Show("Select an Item");
            return;
        }
        Native.LoadProcessInControl(pname, tabPage1);
    }
    
    }
}
