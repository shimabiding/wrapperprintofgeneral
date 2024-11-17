using System;
//using System.Threading.Tasks;
using System.Windows.Forms;
using Npgsql;
//using System.Data;
//using System.Collections.Generic;

namespace pspgf{
internal static class Program{
    [STAThread]
    static void Main(){
        Application.Run(new Form1());

        string conn_str = "Server=localhost;Port=5432;Database=postgres;User ID=postgres;Password=pass";

        using(NpgsqlConnection conn = new NpgsqlConnection(conn_str)){
            conn.Open();
            System.Console.WriteLine("hn");
        }
    }
}

public partial class Form1:Form{
    public Form1(){
        this.ClientSize = new System.Drawing.Size(300,500);
    }
}

}