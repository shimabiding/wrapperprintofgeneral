﻿using System;
using System.Windows.Forms;

namespace wgp{

public static class Program{
    [STAThread]
    static void Main(){
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.Run(new Form1());
    }
}

}