using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
namespace ShadowWindows
{
    
    public partial class Form1 : Form
    {
        bool askedOnce = false;
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter,int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool UpdateWindow(IntPtr hWnd);

        public static int GWL_STYLE = -16;
        public static int WS_CHILD = 0x40000000; 

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
                    Console.WriteLine("E:{0}", e);
                }
            }
        }
    void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        Console.WriteLine("Selected");
        askedOnce = !askedOnce;
        if (listView1.SelectedIndices[0] >= 0&& askedOnce)
        {
            Process p = processes.ElementAt(listView1.SelectedIndices[0]);
            DialogResult dialogResult = MessageBox.Show("Do you want to start this process in shadow windows?", "Alert", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                try
                {
                    p.WaitForInputIdle();
                }
                catch(Exception err)
                {

                    Console.WriteLine("This process probably has no GUI : {0}", err.Message);
                }
               
                IntPtr guestHandle = this.panel1.Handle;
                SetWindowLong(guestHandle, GWL_STYLE, GetWindowLong(guestHandle, GWL_STYLE) | WS_CHILD);
                IntPtr x = SetParent(p.MainWindowHandle, panel1.Handle);
                if (x == null)
                {

                    Console.WriteLine("Last error: " + Marshal.GetLastWin32Error());
                }
                uint uflags = 0x0001 | 0x0002 | 0x0004 | 0x0020 ;              
               bool value= SetWindowPos(panel1.Handle, this.Handle, 0, 0, 600, 600,uflags);
                if(!value)
                {
                    Console.WriteLine("Error {0}",Marshal.GetLastWin32Error());
                }
                UpdateWindow(panel1.Handle);
            }
            else if (dialogResult == DialogResult.No)
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

   
    }
}
