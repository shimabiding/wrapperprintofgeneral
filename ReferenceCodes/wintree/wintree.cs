
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

using Automation = System.Windows.Automation;
// csc /r:C:\Windows\Microsoft.NET\assembly\GAC_MSIL\UIAutomationClient\v4.0_4.0.0.0__31bf3856ad364e35\UIAutomationClient.dll ^
//     /r:C:\Windows\Microsoft.NET\assembly\GAC_MSIL\UIAutomationTypes\v4.0_4.0.0.0__31bf3856ad364e35\UIAutomationTypes.dll ^
//     /r:C:\Windows\Microsoft.NET\assembly\GAC_MSIL\WindowsBase\v4.0_4.0.0.0__31bf3856ad364e35\WindowsBase.dll ^
//     %*

public static class MyWin32Util
{
    const int MaxTextLength = 500; // for GetClassName, GetWindowText

    public static string MyGetClassName(IntPtr hWnd, out int retCode)
    {
        StringBuilder csb = new StringBuilder(MaxTextLength);
        retCode = NativeMethods.GetClassName(hWnd, csb, csb.Capacity);
        if ( retCode > 0 ) {
            return csb.ToString();
        }
        else {
            return string.Empty;
        }
    }

    public static string MyGetWindowText(IntPtr hWnd, out int retCode)
    {
        //ウィンドウのタイトルを取得する
        StringBuilder tsb = new StringBuilder(MaxTextLength);
        retCode = NativeMethods.GetWindowText(hWnd, tsb, tsb.Capacity);

        if ( retCode > 0 ) {
            return tsb.ToString();
        }
        else {
            return string.Empty;
        }
    }

    public static WINDOWINFO MyGetWindowInfo(IntPtr hWnd, out int retCode)
    {
        var wi = new WINDOWINFO();
        wi.cbSize = Marshal.SizeOf(wi);  // sizeof(WINDOWINFO);でもよいようだが sizeof()を使う場合は unsafe{}が必要
        retCode = NativeMethods.GetWindowInfo(hWnd, ref wi);
        return wi;
    }


}


[StructLayout(LayoutKind.Sequential)]
public struct POINT
{
    public int x;
    public int y;
}

[StructLayout(LayoutKind.Sequential)]
public struct WINDOWINFO
{
    public int   cbSize;
    public RECT  rcWindow;
    public RECT  rcClient;
    public int   dwStyle;
    public int   dwExStyle;
    public int   dwWindowStatus;
    public uint  cxWindowBorders;
    public uint  cyWindowBorders;
    public short atomWindowType;
    public short wCreatorVersion;
}

[StructLayout(LayoutKind.Sequential)]
public struct RECT
{
    public int left;
    public int top;
    public int right;  // external area. source:  https://docs.microsoft.com/en-us/previous-versions/dd162897(v%3Dvs.85)
    public int bottom; // 
    public int width{get{return right-left;}}
    public int height{get{return bottom-top;}}
}

public static class NativeMethods
{
    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowTextLength(IntPtr hWnd);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

    [DllImport("user32.dll",SetLastError = true)]
    public static extern int GetWindowInfo(IntPtr hwnd, ref WINDOWINFO pwi);


    [DllImport("user32.dll", SetLastError = true)]
    public static extern int GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);


    [DllImport("user32.dll",SetLastError = true)]
    public static extern IntPtr WindowFromPoint(POINT point);


    [DllImport("user32.dll",SetLastError = true)]
    public static extern IntPtr GetAncestor(IntPtr hWnd, uint gaFlags);
    public const uint GA_PARENT    = 1;
    public const uint GA_ROOT      = 2;
    public const uint GA_ROOTOWNER = 3; // 複数windowを持つ場合は、そのownerが返る


    public delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc,   IntPtr lparam);


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool EnumChildWindows(IntPtr handle, EnumWindowsDelegate enumProc, IntPtr lParam);


    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out POINT lpPoint);
}

public class TopLevelWindowInfo
{
    IntPtr _hWnd;
    string _className;
    string _windowText;
    WINDOWINFO _wi;
    int _pid;

    public IntPtr hWnd        {get{return _hWnd;}}
    public string className   {get{return _className;}}
    public string windowText  {get{return _windowText;}}
    public int    windowStatus{get{return _wi.dwWindowStatus;}}
    public int    left        {get{return _wi.rcWindow.left;}}
    public int    top         {get{return _wi.rcWindow.top;}}
    public int    width       {get{return _wi.rcWindow.width;}}
    public int    height      {get{return _wi.rcWindow.height;}}

    public int    clientLeft  {get{return _wi.rcClient.left;}}
    public int    clientTop   {get{return _wi.rcClient.top;}}
    public int    clientWidth {get{return _wi.rcClient.width;}}
    public int    clientHeight{get{return _wi.rcClient.height;}}

    public int    windowStyle {get{return _wi.dwStyle;}}
    public int    pid         {get{return _pid;}}

    public TopLevelWindowInfo(IntPtr hWnd)
    {
        int retCode;

        _hWnd = hWnd;
        _className = MyWin32Util.MyGetClassName(_hWnd, out retCode);
        _windowText = MyWin32Util.MyGetWindowText(_hWnd, out retCode);

        _wi = MyWin32Util.MyGetWindowInfo(_hWnd, out retCode);
        if ( retCode == 0 ) {
            Console.WriteLine("GetWindowInfo returns 0.");
        }

        NativeMethods.GetWindowThreadProcessId(_hWnd, out _pid);
    }


    const int WS_VISIBLE = 0x10000000;
    const int WS_ICONIC  = 0x20000000;

    public bool IsVisibleWindow()
    {
        if ((_wi.dwStyle & WS_VISIBLE) == 0) return false;
        if ((_wi.dwStyle & WS_ICONIC) == WS_ICONIC) return false;
        if ( width <= 0 || height <= 0 ) return false;

        return true;
    }
}


public class SubForm:Form
{
    IntPtr _hWnd;

    SplitContainer spl;
    TreeView trv;
    ListView lsvProperty;
    CheckBox chkMethod;

    Stack<TreeNode> stackWork;

    public SubForm(IntPtr hWnd)
    {
        _hWnd = hWnd;
        Text = hWnd.ToString("X8");

        ClientSize = new System.Drawing.Size(600,500);

        chkMethod = new CheckBox();
        chkMethod.Appearance = Appearance.Button;
        chkMethod.Width = 150;
        chkMethod.Text = "use UIAutomation method";
        chkMethod.Click += (sender,e)=>{ChkMethod_Click();};
        Controls.Add(chkMethod);

        spl = new SplitContainer();
        spl.Top = chkMethod.Bottom;
        spl.Orientation = Orientation.Horizontal; // .Vertical;
        spl.SplitterDistance = 300;
        Controls.Add(spl);
        
        trv = new TreeView();
        trv.Dock = DockStyle.Fill;
        trv.AfterSelect += (sender,e)=>{Trv_AfterSelect();};
        spl.Panel1.Controls.Add(trv);

        lsvProperty = new ListView();
        lsvProperty.View = View.Details;
        lsvProperty.FullRowSelect = true;
        lsvProperty.GridLines = true;
        lsvProperty.Columns.Add("Item",  100, HorizontalAlignment.Left);
        lsvProperty.Columns.Add("Value", 400, HorizontalAlignment.Left);
        lsvProperty.Dock = DockStyle.Fill;
        spl.Panel2.Controls.Add(lsvProperty);


        CreateHWndTreeByWin32Api();

        Load      += (sender,e)=>{MyResize();}; 
        Resize    += (sender,e)=>{MyResize();};
        ResizeEnd += (sender,e)=>{MyResize();};
    }
    
    private void MyResize()
    {
        int h = ClientSize.Height - spl.Top;
        if ( h < 10 ) {
            h = 10;
        }
        spl.Size = new System.Drawing.Size(ClientSize.Width, h);
    }

    private void ChkMethod_Click()
    {
        if ( chkMethod.Checked ) {
            CreateHWndTreeByUIAutomation();
        }
        else {
            CreateHWndTreeByWin32Api();
        }
    }

    private void Trv_AfterSelect()
    {
        TreeNode node = trv.SelectedNode;
        int retCode;

        lsvProperty.BeginUpdate();
        try {
            lsvProperty.Items.Clear();
            if ( node.Tag is Automation.AutomationElement ) {
                var elem = (Automation.AutomationElement)node.Tag;
                var elemInfo = elem.Current;
                System.Windows.Rect rect = elemInfo.BoundingRectangle; // needs assembly "WindowsBase.dll"

                // https://docs.microsoft.com/ja-jp/dotnet/api/system.windows.automation.automationelement.automationelementinformation?view=netframework-4.8

                lsvProperty.Items.Add(new ListViewItem(new string[]{"Name",         elemInfo.Name}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"LocalizedControlType",  elemInfo.LocalizedControlType}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"AutomationId", elemInfo.AutomationId}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"IsOffscreen",  elemInfo.IsOffscreen.ToString()}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"X",       rect.X.ToString()}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"Y",       rect.Y.ToString()}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"Width",   rect.Width.ToString()}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"Height",  rect.Height.ToString()}));
            }
            else {
                IntPtr hWnd = (IntPtr)node.Tag;
                lsvProperty.Items.Add(new ListViewItem(new string[]{"HWND", "0x"+hWnd.ToString("X8")}));
                lsvProperty.Items.Add(new ListViewItem(new string[]{"WindowText", MyWin32Util.MyGetWindowText(hWnd, out retCode)}));
            }
        }
        finally {
            lsvProperty.EndUpdate();
        }
    }

    private static TreeNode MakeTreeNode(IntPtr hWnd)
    {
        string className;
        int retCode;

        className = MyWin32Util.MyGetClassName(hWnd, out retCode);

        TreeNode node = new TreeNode(className);
        node.Tag = hWnd;
        return node;
    }

    private static TreeNode MakeTreeNodeAutomation(Automation.AutomationElement elem)
    {
        var elemInfo = elem.Current; // AutomationElementInformation

        TreeNode node;
        if ( String.IsNullOrEmpty(elemInfo.ClassName) ) {
            node = new TreeNode("<<NO CLASSNAME>>");
        }
        else {
            node = new TreeNode(elemInfo.ClassName);
        }
        node.Tag = elem;
        return node;
    }
    
    private void CreateHWndTreeByWin32Api()
    {
        stackWork = new Stack<TreeNode>();
        trv.BeginUpdate();
        try {
            trv.Nodes.Clear();
            TreeNode rootNode = MakeTreeNode(_hWnd);
            trv.Nodes.Add(rootNode);
            stackWork.Push(rootNode);
            NativeMethods.EnumChildWindows(_hWnd, EnumChildWindowCallBack, IntPtr.Zero);
        }
        finally {
            trv.EndUpdate();
            stackWork = null;
        }
        trv.ExpandAll();
    }

    private bool EnumChildWindowCallBack(IntPtr hWnd, IntPtr lparam)
    {
        TreeNode node = stackWork.Peek();
        TreeNode childNode = MakeTreeNode(hWnd);
        node.Nodes.Add(childNode);

        stackWork.Push(childNode);
        NativeMethods.EnumChildWindows(hWnd, EnumChildWindowCallBack, IntPtr.Zero);
        stackWork.Pop();
        return true;        //すべてのウィンドウを列挙する
    }

    private void CreateHWndTreeByUIAutomation()
    {
        stackWork = new Stack<TreeNode>();
        trv.BeginUpdate();
        try {
            trv.Nodes.Clear();
//            TreeNode rootNode = MakeTreeNode(_hWnd);
            var elem = Automation.AutomationElement.FromHandle(_hWnd);
            TreeNode rootNode = MakeTreeNodeAutomation(elem);
            trv.Nodes.Add(rootNode);
            stackWork.Push(rootNode);

//            EnumChildByUIAutomation(Automation.AutomationElement.FromHandle(_hWnd));
            EnumChildByUIAutomation(elem);
        }
        finally {
            trv.EndUpdate();
            stackWork = null;
        }
        trv.ExpandAll();
    }

    private void EnumChildByUIAutomation(Automation.AutomationElement elem)
    {
        TreeNode node = stackWork.Peek();
        var childElements = elem.FindAll(Automation.TreeScope.Children, Automation.Condition.TrueCondition);

        foreach(Automation.AutomationElement childElem in childElements) {
            TreeNode childNode = MakeTreeNodeAutomation(childElem);
            node.Nodes.Add(childNode);

            stackWork.Push(childNode);
            EnumChildByUIAutomation(childElem);
            stackWork.Pop();
        }
    }
}


public class MainForm:Form
{
    public enum LsvSortAttr {
        ByNumeric,
        ByHex,
        ByString
    };

    System.Windows.Forms.Timer timer;
    Button   btnRefresh;
    Button   btnPeriodicRefresh;
    CheckBox chkShowInvisibleWindow;
    ListView lsv;
    List<TopLevelWindowInfo> wndInfos;
    List<LsvSortAttr> SortOrderIdent;


    MainForm()
    {
        wndInfos = new List<TopLevelWindowInfo>();

        Text = "Listup Window Handles";
        SortOrderIdent = new List<LsvSortAttr>();
        ClientSize = new System.Drawing.Size(700,530);

        Shown += (sender,e)=>{EnumWndUpdateList();};
        Resize    += (sender,e)=>{MyResizeHandler();};
        ResizeEnd += (sender,e)=>{MyResizeHandler();};

        btnRefresh = new Button();
        btnRefresh.Text = "Refresh";
        btnRefresh.Click += (sender,e)=>{EnumWndUpdateList();};
        Controls.Add(btnRefresh);

        btnPeriodicRefresh = new Button();
        btnPeriodicRefresh.Left = 100;
        btnPeriodicRefresh.Text = "Auto";
        btnPeriodicRefresh.Click += (sender,e)=>
        {
            if ( timer==null || !timer.Enabled ) {
                btnPeriodicRefresh.Text = "Stop";
                StartTimer();
            }
            else {
                btnPeriodicRefresh.Text = "Auto";
                StopTimer();
            }
        };
        Controls.Add(btnPeriodicRefresh);


        chkShowInvisibleWindow = new CheckBox();
        chkShowInvisibleWindow.Left = 200;
        chkShowInvisibleWindow.Text = "ShowInvisible";
        chkShowInvisibleWindow.Click += (sender,e)=>{UpdateListControl();}; // 取得済みデータをフィルタするのは面倒なので 再取得
        Controls.Add(chkShowInvisibleWindow);


        lsv = new ListView();
        lsv.Location = new Point(0,30);
        lsv.Size = new System.Drawing.Size(700,500);
        lsv.View = View.Details;
        lsv.FullRowSelect = true;
        lsv.GridLines = true;
        lsv.Columns.Add("No"         , 35, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("HWND"       , 70, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByHex);
        lsv.Columns.Add("PID"        , 50, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("ClassName"  ,150, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByString);
        lsv.Columns.Add("WindowText" ,150, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByString);
        lsv.Columns.Add("Left"       , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("Top"        , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("Width"      , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("Height"     , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("cLeft"      , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("cTop"       , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("cWidth"     , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("cHeight"    , 45, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByNumeric);
        lsv.Columns.Add("Style"      , 70, HorizontalAlignment.Left);SortOrderIdent.Add(LsvSortAttr.ByHex);
        lsv.ColumnClick += (sender,e)=>{Lsv_ColumnClick(e);};
        lsv.DoubleClick += (sender,e)=>{Lsv_DoubleClick(e);};
        Controls.Add(lsv);
    }

    void MyResizeHandler()
    {
        lsv.Size = new System.Drawing.Size(ClientSize.Width, ClientSize.Height - lsv.Top);
    }

    void Lsv_ColumnClick(ColumnClickEventArgs e)
    {
        if ( e.Column >= 0 && e.Column < lsv.Columns.Count ) {
            lsv.ListViewItemSorter = new ListViewItemComparer(e.Column, SortOrderIdent[e.Column]);
        }
        else {
            Console.WriteLine("Out of column index");
        }
    }

    void Lsv_DoubleClick(EventArgs e)
    {
        var indices = lsv.SelectedIndices;

        if ( indices.Count == 1 ) {
            var a = (TopLevelWindowInfo)(lsv.Items[indices[0]].Tag);
            Form subForm = new SubForm(a.hWnd);
            subForm.ShowDialog();
        }
    }

    public class ListViewItemComparer : IComparer
    {
        private int _column;
        private LsvSortAttr _sortAttr;

        public ListViewItemComparer(int col, LsvSortAttr sortAttr)
        {
            _column   = col;
            _sortAttr = sortAttr;
        }

        private int CompareInt64(Int64 a, Int64 b)
        {
            if(a<b){return -1;}
            if(a>b){return  1;}
            return 0;
        }

        public int Compare(object obj1, object obj2)
        {
            string s1 = ((ListViewItem)obj1).SubItems[_column].Text;
            string s2 = ((ListViewItem)obj2).SubItems[_column].Text;

            if ( _sortAttr == LsvSortAttr.ByNumeric ) {
                Int64 n1;
                Int64 n2;
                try {
                    n1 = Convert.ToInt64(s1);
                    n2 = Convert.ToInt64(s2);
                    return CompareInt64(n1, n2);
                }
                catch(Exception e){Console.WriteLine(e);} // catchしたら文字列比較へ
            }
            else if ( _sortAttr == LsvSortAttr.ByHex ) {
                // 未実装.. とりあえず文字列比較
            }
            return string.Compare(s1, s2);
        }
    }

    public ListViewItem ConvertToListViewItem(int index, TopLevelWindowInfo a)
    {
        var ss = new string[]
        {
            index.ToString(),
            a.hWnd.ToString("X8"),
            a.pid.ToString(),
            a.className,
            a.windowText,
            a.left.ToString(),
            a.top.ToString(),
            a.width.ToString(),
            a.height.ToString(),
            a.clientLeft.ToString(),
            a.clientTop.ToString(),
            a.clientWidth.ToString(),
            a.clientHeight.ToString(),
            a.windowStyle.ToString("X8")
        };

        var t = new ListViewItem(ss);
        t.Tag = a;
        return t;
    }

    void StartTimer()
    {
        timer = new System.Windows.Forms.Timer();
        timer.Interval = 2000;
        timer.Tick += (sender,e)=>{EnumWndUpdateList();};
        timer.Start();
    }

    void StopTimer()
    {
        if ( timer != null ) {
            timer.Stop();
        }
    }

    void EnumWndUpdateList()
    {
        wndInfos = new List<TopLevelWindowInfo>();

        //ウィンドウを列挙する
        NativeMethods.EnumWindows(EnumWindowCallBack, IntPtr.Zero);

        UpdateListControl();

        var p = new POINT();
        NativeMethods.GetCursorPos(out p);
        IntPtr hWnd = NativeMethods.WindowFromPoint( p );
        IntPtr hWndRoot = IntPtr.Zero;

        if ( hWnd != IntPtr.Zero ) {
            hWndRoot = NativeMethods.GetAncestor(hWnd, NativeMethods.GA_ROOT);
        }

        Console.Write("HWND on cursor = 0x");
        Console.WriteLine(((int)hWndRoot).ToString("X8"));
    }


    void UpdateListControl()
    {
        bool showInvisWnd = chkShowInvisibleWindow.Checked;

        lsv.Items.Clear();
        lsv.BeginUpdate();
        try {
            int itemNo=0;
            foreach ( var t in wndInfos ) {
                itemNo++;
                if ( showInvisWnd || t.IsVisibleWindow() ) {
                    lsv.Items.Add(ConvertToListViewItem(itemNo, t));
                }
            }
        }
        finally {
            lsv.EndUpdate();
        }
    }

    [STAThread]
    public static void Main(string[] args)
    {
        Application.Run(new MainForm());
    }

    private bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
    {
        wndInfos.Add(new TopLevelWindowInfo(hWnd));
        return true;        //すべてのウィンドウを列挙する
    }
}

